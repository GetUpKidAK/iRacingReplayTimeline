using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using iRacingSdkWrapper;


namespace ReplayTimeline
{
	public class ReplayTimelineVM : INotifyPropertyChanged
	{
		private SDKHelper m_SDKHelper;
		private bool m_ReplayInitialised = false;
		private int m_TargetFrame = -1;

		#region Properties
		public int SessionID { get; private set; }
		#endregion

		#region Binding Properties
		public ObservableCollection<TimelineNode> TimelineNodes { get; set; }
		public ObservableCollection<Driver> Drivers { get; set; }
		public ObservableCollection<Camera> Cameras { get; set; }


		private TimelineNode _currentTimelineNode;
		public TimelineNode CurrentTimelineNode
		{
			get { return _currentTimelineNode; }
			set
			{
				_currentTimelineNode = value;
				OnPropertyChanged("CurrentTimelineNode");
				TimelineNodeChanged();
			}
		}

		private Driver _currentDriver;
		public Driver CurrentDriver
		{
			get { return _currentDriver; }
			set { _currentDriver = value; OnPropertyChanged("CurrentDriver"); DriverChanged(); }
		}

		private Camera _currentCamera;
		public Camera CurrentCamera
		{
			get { return _currentCamera; }
			set { _currentCamera = value; OnPropertyChanged("CurrentCamera"); CameraChanged(); }
		}

		private int _currentFrame;
		public int CurrentFrame
		{
			get { return _currentFrame; }
			set
			{
				var lastFrame = _currentFrame;
				_currentFrame = value;

				if (_currentFrame == m_TargetFrame || m_TargetFrame == -1)
					if (lastFrame != _currentFrame) CheckCurrentFrameForStoredNodes();

				if (_currentFrame == m_TargetFrame) m_TargetFrame = -1;
				OnPropertyChanged("CurrentFrame");
			}
		}

		private int m_CurrentPlaybackSpeed;
		public int CurrentPlaybackSpeed
		{
			get => m_CurrentPlaybackSpeed;
			private set
			{
				m_CurrentPlaybackSpeed = value;
				if (m_CurrentPlaybackSpeed > 16) m_CurrentPlaybackSpeed = 16;
				else if (m_CurrentPlaybackSpeed < -16) m_CurrentPlaybackSpeed = -16;
				UpdatePlaybackButtonText();

				OnPropertyChanged("CurrentPlaybackSpeed");
			}
		}

		private bool _playbackEnabled;
		public bool PlaybackEnabled
		{
			get { return _playbackEnabled; }
			private set
			{
				_playbackEnabled = value;
				OnPropertyChanged("PlaybackEnabled");
			}
		}

		private string _storeFrameBtnText;
		public string StoreFrameBtnText
		{
			get { return _storeFrameBtnText; }
			set { _storeFrameBtnText = value; OnPropertyChanged("StoreFrameBtnText"); }
		}

		private string _playPauseBtnText;
		public string PlayPauseBtnText
		{
			get { return _playPauseBtnText; }
			set { _playPauseBtnText = value; OnPropertyChanged("PlayPauseBtnText"); }
		}

		private string _fastForwardBtnText;
		public string FastForwardBtnText
		{
			get { return _fastForwardBtnText; }
			set { _fastForwardBtnText = value; OnPropertyChanged("FastForwardBtnText"); }
		}

		private string _rewindBtnText;
		public string RewindBtnText
		{
			get { return _rewindBtnText; }
			set { _rewindBtnText = value; OnPropertyChanged("RewindBtnText"); }
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		#endregion

		#region Commands
		public StoreCurrentFrameCommand StoreCurrentFrameCommand { get; set; }
		public PreviousStoredFrameCommand PreviousStoredFrameCommand { get; set; }
		public NextStoredFrameCommand NextStoredFrameCommand { get; set; }
		public DeleteStoredFrameCommand DeleteStoredFrameCommand { get; set; }
		public PlayPauseCommand PlayPauseCommand { get; set; }
		public RewindCommand RewindCommand { get; set; }
		public FastForwardCommand FastForwardCommand { get; set; }
		public NextLapCommand NextLapCommand { get; set; }
		public PreviousLapCommand PreviousLapCommand { get; set; }
		public NextSessionCommand NextSessionCommand { get; set; }
		public PreviousSessionCommand PreviousSessionCommand { get; set; }
		#endregion

		public TestCommand TestCommand { get; set; }


		public ReplayTimelineVM()
		{
			m_SDKHelper = new SDKHelper(this);

			TimelineNodes = new ObservableCollection<TimelineNode>();
			Drivers = new ObservableCollection<Driver>();
			Cameras = new ObservableCollection<Camera>();

			StoreFrameBtnText = "Store Node";

			StoreCurrentFrameCommand = new StoreCurrentFrameCommand(this);
			NextStoredFrameCommand = new NextStoredFrameCommand(this);
			PreviousStoredFrameCommand = new PreviousStoredFrameCommand(this);
			DeleteStoredFrameCommand = new DeleteStoredFrameCommand(this);
			PlayPauseCommand = new PlayPauseCommand(this);
			RewindCommand = new RewindCommand(this);
			FastForwardCommand = new FastForwardCommand(this);
			NextLapCommand = new NextLapCommand(this);
			PreviousLapCommand = new PreviousLapCommand(this);
			NextSessionCommand = new NextSessionCommand(this);
			PreviousSessionCommand = new PreviousSessionCommand(this);

			TestCommand = new TestCommand(this);
		}

		public void ApplicationClosing()
		{
			m_SDKHelper.Stop();

			App.Current.Shutdown();
			Environment.Exit(0);
		}

		public void TelemetryUpdated(TelemetryInfo telemetryInfo)
		{
			CurrentFrame = telemetryInfo.ReplayFrameNum.Value;
			CurrentPlaybackSpeed = telemetryInfo.ReplayPlaySpeed.Value;
		}

		public void SessionInfoUpdated(SessionInfo sessionInfo)
		{
			var sessionDrivers = SessionInfoHelper.GetSessionDrivers(sessionInfo, Drivers);
			Drivers.Clear();
			foreach (var newDriver in sessionDrivers)
			{
				Drivers.Add(newDriver);
			}

			var sessionCameras = SessionInfoHelper.GetSessionCameras(sessionInfo, Cameras);
			Cameras.Clear();
			foreach (var newCamera in sessionCameras)
			{
				Cameras.Add(newCamera);
			}

			SessionID = SessionInfoHelper.GetSessionID(sessionInfo);

			if (!m_ReplayInitialised)
			{
				var loaddedProject = SaveLoadHelper.LoadProject(SessionID);
				if (loaddedProject.TimelineNodes.Count > 0)
				{
					TimelineNodes.Clear();

					foreach (var node in loaddedProject.TimelineNodes)
					{
						TimelineNodes.Add(node);
					}
				}

				m_ReplayInitialised = true;
			}
		}
		
		private void CheckCurrentFrameForStoredNodes()
		{
			var foundNode = TimelineNodes.FirstOrDefault(n => n.Frame == CurrentFrame);

			if (CurrentTimelineNode != foundNode)
				CurrentTimelineNode = foundNode;
		}

		private void JumpToNode(TimelineNode node)
		{
			CurrentDriver = node.Driver;
			CurrentCamera = node.Camera;

			if (!PlaybackEnabled)
				GoToFrame(node.Frame);
		}

		public void PlayPauseToggle()
		{
			if (CurrentPlaybackSpeed == 0)
			{
				CurrentPlaybackSpeed = 1;
				ChangePlaybackSpeed();
			}
			else
			{
				CurrentPlaybackSpeed = 0;
				ChangePlaybackSpeed();
			}
		}

		public void RewindPlayback()
		{
			if (CurrentPlaybackSpeed < 0)
			{
				CurrentPlaybackSpeed *= 2;
			}
			else
			{
				CurrentPlaybackSpeed = -1;
			}
			ChangePlaybackSpeed();
		}

		public void FastForwardPlayback()
		{
			if (CurrentPlaybackSpeed > 0)
			{
				CurrentPlaybackSpeed *= 2;
			}
			else
			{
				CurrentPlaybackSpeed = 2;
			}
			ChangePlaybackSpeed();
		}

		private void ChangePlaybackSpeed()
		{
			m_SDKHelper.SetPlaybackSpeed(CurrentPlaybackSpeed);

			PlaybackEnabled = CurrentPlaybackSpeed != 0;
		}

		private void UpdatePlaybackButtonText()
		{
			PlayPauseBtnText = PlaybackEnabled ? "Pause" : "Play";

			if (CurrentPlaybackSpeed > 0)
			{
				RewindBtnText = "<<";
				FastForwardBtnText = ">>";

				if (CurrentPlaybackSpeed > 1)
					FastForwardBtnText = $"{CurrentPlaybackSpeed}x";
			}
			else
			{
				RewindBtnText = "<<";
				FastForwardBtnText = ">>";

				if (CurrentPlaybackSpeed < -1)
					RewindBtnText = $"{-CurrentPlaybackSpeed}x";
			}
		}

		public void JumpToEvent(iRSDKSharp.ReplaySearchModeTypes replayEvent)
		{
			m_SDKHelper.JumpToEvent(replayEvent);
		}

		private void DriverChanged()
		{
			if (CurrentDriver == null)
				return;

			m_SDKHelper.SetDriver(CurrentDriver);
		}

		private void CameraChanged()
		{
			if (CurrentCamera == null)
				return;

			if (CurrentDriver == null)
			{
				m_SDKHelper.SetCamera(CurrentCamera);
			}
			else
			{
				m_SDKHelper.SetDriver(CurrentDriver, CurrentCamera);
			}
		}

		private void TimelineNodeChanged()
		{
			StoreFrameBtnText = CurrentTimelineNode == null ? "Store Node" : "Update Node";
			if (CurrentTimelineNode != null) JumpToNode(CurrentTimelineNode);
		}

		public void StoreCurrentFrame()
		{
			if (CurrentTimelineNode != null)
			{
				CurrentTimelineNode.Driver = CurrentDriver;
				CurrentTimelineNode.Camera = CurrentCamera;

				SaveProjectChanges();
			}
			else
			{
				var timelineFrames = TimelineNodes.Select(n => n.Frame).ToList();
				TimelineNode storedNode = null;

				if (!timelineFrames.Contains(CurrentFrame))
				{
					TimelineNode newNode = new TimelineNode();
					newNode.Frame = CurrentFrame;
					newNode.Driver = CurrentDriver;
					newNode.Camera = CurrentCamera;

					TimelineNodes.Add(newNode);
					storedNode = newNode;

					SaveProjectChanges();
				}

				// Simple version of sorting for now.
				var sortedTimeline = TimelineNodes.OrderBy(n => n.Frame).ToList();

				TimelineNodes.Clear();

				foreach (var node in sortedTimeline)
				{
					TimelineNodes.Add(node);
				}

				CurrentTimelineNode = storedNode;
			}
		}

		public void GoToPreviousStoredFrame()
		{
			if (TimelineNodes.Count > 0)
			{
				TimelineNode targetNode = null;

				for (int i = 0; i < TimelineNodes.Count; i++)
				{
					if (TimelineNodes[i].Frame < CurrentFrame)
					{
						targetNode = TimelineNodes[i];
					}
					else if (TimelineNodes[i].Frame >= CurrentFrame)
					{
						break;
					}
				}

				if (targetNode != null)
				{
					CurrentTimelineNode = targetNode;
					GoToFrame(targetNode.Frame);
				}
			}
		}

		public void GoToNextStoredFrame()
		{
			if (TimelineNodes.Count > 0)
			{
				TimelineNode targetNode = null;

				for (int i = TimelineNodes.Count - 1; i >= 0; i--)
				{
					if (TimelineNodes[i].Frame > CurrentFrame)
					{
						targetNode = TimelineNodes[i];
					}
					else if (TimelineNodes[i].Frame <= CurrentFrame)
					{
						break;
					}
				}

				if (targetNode != null)
				{
					CurrentTimelineNode = targetNode;
					GoToFrame(targetNode.Frame);
				}
			}
		}

		public void DeleteStoredFrame()
		{
			if (CurrentTimelineNode != null)
			{
				TimelineNodes.Remove(CurrentTimelineNode);
				CurrentTimelineNode = null;
			}
		}

		private void GoToFrame(int frame)
		{
			m_TargetFrame = frame;

			m_SDKHelper.GoToFrame(frame);
		}

		private void SaveProjectChanges()
		{
			SaveLoadHelper.SaveProject(TimelineNodes.ToList(), SessionID);
		}
	}
}
