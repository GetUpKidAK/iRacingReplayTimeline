using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using iRacingSdkWrapper;


namespace ReplayTimeline
{
	public class ReplayTimelineVM : INotifyPropertyChanged
	{
		private SDKHelper m_SDKHelper;
		private int m_TargetFrame = -1;

		private const string m_ApplicationTitle = "iRacing Replay Timeline";
		private const float m_VersionNumber = 0.9f;

		#region Properties
		public string WindowTitle { get { return $"{m_ApplicationTitle} (v{m_VersionNumber})"; } }
		public bool SessionInfoLoaded { get; private set; } = false;
		public int SessionID { get; private set; }
		#endregion

		#region Binding Properties
		public ObservableCollection<TimelineNode> TimelineNodes { get; set; }
		public ICollectionView TimelineNodesView { get; private set; }
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

				if (_currentFrame == m_TargetFrame)
				{
					m_TargetFrame = -1;
					MovingToFrame = false;
				}
				OnPropertyChanged("CurrentFrame");
				//CommandManager.InvalidateRequerySuggested();
			}
		}

		private bool _movingToFrame;
		public bool MovingToFrame
		{
			get { return _movingToFrame; }
			private set { _movingToFrame = value; OnPropertyChanged("MovingToFrame"); }
		}

		private int _finalFrame;
		public int FinalFrame
		{
			get { return _finalFrame; }
			set { _finalFrame = value; OnPropertyChanged("FinalFrame"); }
		}

		private bool _slowMotionEnabled;
		public bool SlowMotionEnabled
		{
			get { return _slowMotionEnabled; }
			set { _slowMotionEnabled = value; OnPropertyChanged("SlowMotionEnabled"); }
		}

		private int m_CurrentPlaybackSpeed;
		public int CurrentPlaybackSpeed
		{
			get => m_CurrentPlaybackSpeed;
			set
			{
				m_CurrentPlaybackSpeed = value;
				if (m_CurrentPlaybackSpeed > 16) m_CurrentPlaybackSpeed = 16;
				else if (m_CurrentPlaybackSpeed < -16) m_CurrentPlaybackSpeed = -16;
				UpdatePlaybackButtonText();

				OnPropertyChanged("CurrentPlaybackSpeed");
				CommandManager.InvalidateRequerySuggested();
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
				CommandManager.InvalidateRequerySuggested();
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

		private bool _showReplayTimeline;
		public bool ShowReplayTimeline
		{
			get { return _showReplayTimeline; }
			set { _showReplayTimeline = value; OnPropertyChanged("ShowReplayTimeline"); }
		}

		private bool _showSessionLapSkipButtons;
		public bool ShowSessionLapSkipButtons
		{
			get { return _showSessionLapSkipButtons; }
			set { _showSessionLapSkipButtons = value; OnPropertyChanged("ShowSessionLapSkipButtons"); }
		}

		private bool _showDriverCameraPanels;
		public bool ShowDriverCameraPanels
		{
			get { return _showDriverCameraPanels; }
			set { _showDriverCameraPanels = value; OnPropertyChanged("ShowDriverCameraPanels"); }
		}

		private bool _showTimelineNodeList;
		public bool ShowTimelineNodeList
		{
			get { return _showTimelineNodeList; }
			set { _showTimelineNodeList = value; OnPropertyChanged("ShowTimelineNodeList"); }
		}

		private bool _minimizedMode;
		public bool MinimizedMode
		{
			get { return _minimizedMode; }
			set
			{
				_minimizedMode = value;

				ShowReplayTimeline = !_minimizedMode;
				ShowSessionLapSkipButtons = !_minimizedMode;
				ShowDriverCameraPanels = !_minimizedMode;
				ShowTimelineNodeList = !_minimizedMode;

				OnPropertyChanged("MinimizedMode");
			}
		}

		private string _statusBarText;
		public string StatusBarText
		{
			get { return _statusBarText; }
			set { _statusBarText = value; OnPropertyChanged("StatusBarText"); }
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
		public SkipFrameBackCommand SkipFrameBackCommand { get; set; }
		public SkipFrameForwardCommand SkipFrameForwardCommand { get; set; }
		public SlowMotionToggleCommand SlowMotionToggleCommand { get; set; }
		public NextLapCommand NextLapCommand { get; set; }
		public PreviousLapCommand PreviousLapCommand { get; set; }
		public NextSessionCommand NextSessionCommand { get; set; }
		public PreviousSessionCommand PreviousSessionCommand { get; set; }
		public NextDriverCommand NextDriverCommand { get; set; }
		public PreviousDriverCommand PreviousDriverCommand { get; set; }
		public ApplicationQuitCommand ApplicationQuitCommand { get; set; }
		#endregion

		public TestCommand TestCommand { get; set; }


		public ReplayTimelineVM()
		{
			m_SDKHelper = new SDKHelper(this);

			TimelineNodes = new ObservableCollection<TimelineNode>();
			TimelineNodesView = CollectionViewSource.GetDefaultView(TimelineNodes);
			Drivers = new ObservableCollection<Driver>();
			Cameras = new ObservableCollection<Camera>();

			TimelineNodesView.SortDescriptions.Clear();

			SortDescription frameSort = new SortDescription("Frame", ListSortDirection.Ascending);
			TimelineNodesView.SortDescriptions.Add(frameSort);

			StatusBarText = "iRacing Not Connected.";
			StoreFrameBtnText = "Store Node";
			ShowReplayTimeline = true;
			ShowSessionLapSkipButtons = true;
			ShowDriverCameraPanels = true;
			ShowTimelineNodeList = true;

			StoreCurrentFrameCommand = new StoreCurrentFrameCommand(this);
			NextStoredFrameCommand = new NextStoredFrameCommand(this);
			PreviousStoredFrameCommand = new PreviousStoredFrameCommand(this);
			DeleteStoredFrameCommand = new DeleteStoredFrameCommand(this);
			PlayPauseCommand = new PlayPauseCommand(this);
			RewindCommand = new RewindCommand(this);
			FastForwardCommand = new FastForwardCommand(this);
			SlowMotionToggleCommand = new SlowMotionToggleCommand(this);
			SkipFrameBackCommand = new SkipFrameBackCommand(this);
			SkipFrameForwardCommand = new SkipFrameForwardCommand(this);
			NextLapCommand = new NextLapCommand(this);
			PreviousLapCommand = new PreviousLapCommand(this);
			NextSessionCommand = new NextSessionCommand(this);
			PreviousSessionCommand = new PreviousSessionCommand(this);
			NextDriverCommand = new NextDriverCommand(this);
			PreviousDriverCommand = new PreviousDriverCommand(this);
			ApplicationQuitCommand = new ApplicationQuitCommand(this);

			TestCommand = new TestCommand(this);

			TimelineNodes.CollectionChanged += TimelineNodes_CollectionChanged;
		}

		public void ApplicationClosing()
		{
			m_SDKHelper.Stop();

			ApplicationQuitCommand.Execute(this);
		}

		private void TimelineNodes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (INotifyPropertyChanged added in e.NewItems)
				{
					added.PropertyChanged += TimelineNodesPropertyChanged;
				}
			}

			if (e.OldItems != null)
			{
				foreach (INotifyPropertyChanged removed in e.OldItems)
				{
					removed.PropertyChanged -= TimelineNodesPropertyChanged;
				}
			}
		}

		private void TimelineNodesPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{
			if (propertyChangedEventArgs.PropertyName == "Enabled")
			{
				SaveProjectChanges();
			}
		}

		public void SdkConnected()
		{
			StatusBarText = "iRacing Connected";
		}

		public void SdkDisconnected()
		{
			StatusBarText = "iRacing Disconnected";

			// Unload session info, clear information
			SessionInfoLoaded = false;
			Drivers.Clear();
			Cameras.Clear();
			TimelineNodes.Clear();
		}

		public void TelemetryUpdated(TelemetryInfo telemetryInfo)
		{
			// Leave now if the session info hasn't already been loaded
			if (!SessionInfoLoaded)
				return;

			CurrentFrame = telemetryInfo.ReplayFrameNum.Value;
			FinalFrame = CurrentFrame + telemetryInfo.ReplayFrameNumEnd.Value;
			CurrentPlaybackSpeed = telemetryInfo.ReplayPlaySpeed.Value;
			PlaybackEnabled = CurrentPlaybackSpeed != 0;
			SlowMotionEnabled = telemetryInfo.ReplayPlaySlowMotion.Value;

			// Get current car ID and current camera group from sim
			var currentCarId = telemetryInfo.CamCarIdx.Value;
			var currentCamGroup = telemetryInfo.CamGroupNumber.Value;

			if (CurrentDriver == null || CurrentCamera == null)
			{
				// Set the current driver and camera if they were null
				CurrentDriver = Drivers.FirstOrDefault(d => d.Id == currentCarId);
				CurrentCamera = Cameras.FirstOrDefault(c => c.GroupNum == currentCamGroup);
			}

			if (CurrentDriver != null && CurrentCamera != null)
			{
				// If the sim car/camera doesn't match the currently selected ones in the UI (i.e. it changed in sim), then update it
				// OnPropertyChanged is called and not the property directly so that there's no recursive loop of changing the driver again, causing stutters
				if (currentCarId != CurrentDriver.Id) _currentDriver = Drivers.FirstOrDefault(d => d.Id == currentCarId); OnPropertyChanged("CurrentDriver");
				if (currentCamGroup != CurrentCamera.GroupNum) _currentCamera = Cameras.FirstOrDefault(c => c.GroupNum == currentCamGroup); OnPropertyChanged("CurrentCamera");
			}

			// Update driver telemetry info
			SessionInfoHelper.UpdateDriverTelemetry(telemetryInfo, Drivers);
		}

		public void SessionInfoUpdated(SessionInfo sessionInfo)
		{
			UpdateDriversAndCameras(sessionInfo);

			SessionID = SessionInfoHelper.GetSessionID(sessionInfo);

			if (!SessionInfoLoaded)
			{
				LoadExistingProjectFile();

				SessionInfoLoaded = true;
			}
		}

		private void UpdateDriversAndCameras(SessionInfo sessionInfo)
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

			VerifyExistingNodeCameras();
		}

		private void VerifyExistingNodeCameras()
		{
			foreach (var node in TimelineNodes)
			{
				// Check for node camera in camera list, based on name
				var existingCameraInSession = Cameras.FirstOrDefault(c => c.GroupName == node.Camera.GroupName);

				if (existingCameraInSession == null)
				{
					// Disable node if it wasn't found
					node.Enabled = false;
				}
				else
				{
					// If a camera is found but the group number is different...
					if (node.Camera.GroupNum != existingCameraInSession.GroupNum)
					{
						// Set the camera group number to the one in the active camera liust
						node.Camera.GroupNum = existingCameraInSession.GroupNum;
					}
				}
			}
		}

		private void LoadExistingProjectFile()
		{
			var loaddedProject = SaveLoadHelper.LoadProject(SessionID);
			if (loaddedProject.TimelineNodes.Count > 0)
			{
				TimelineNodes.Clear();

				foreach (var node in loaddedProject.TimelineNodes)
				{
					TimelineNodes.Add(node);
				}

				VerifyExistingNodeCameras();
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
			// If replay is playing back AND node is disabled, skip it...
			if (PlaybackEnabled && !node.Enabled)
				return;

			// Otherwise, switch driver and camera
			CurrentDriver = node.Driver;
			CurrentCamera = node.Camera;

			// If playback is disabled, skip to the frame
			if (!PlaybackEnabled)
				GoToFrame(node.Frame);
		}

		public void ChangePlaybackSpeed()
		{
			if (SlowMotionEnabled) m_SDKHelper.SetSlowMotionPlaybackSpeed(CurrentPlaybackSpeed);
			else m_SDKHelper.SetPlaybackSpeed(CurrentPlaybackSpeed);
		}

		private void UpdatePlaybackButtonText()
		{
			PlayPauseBtnText = PlaybackEnabled ? "Pause" : "Play";

			if (CurrentPlaybackSpeed > 0)
			{
				RewindBtnText = "<<";
				FastForwardBtnText = ">>";

				if (CurrentPlaybackSpeed > 1)
				{
					FastForwardBtnText = SlowMotionEnabled ? $"1/{CurrentPlaybackSpeed + 1}x" : $"{CurrentPlaybackSpeed}x";
				}
			}
			else
			{
				RewindBtnText = "<<";
				FastForwardBtnText = ">>";

				if (CurrentPlaybackSpeed < -1)
				{
					RewindBtnText = SlowMotionEnabled ? $"1/{-CurrentPlaybackSpeed - 1}x" : $"{-CurrentPlaybackSpeed}x";
				}
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

				CurrentTimelineNode = storedNode;
			}
		}

		public void DeleteStoredFrame()
		{
			if (CurrentTimelineNode != null)
			{
				TimelineNodes.Remove(CurrentTimelineNode);
				CurrentTimelineNode = null;

				SaveProjectChanges();
			}
		}

		public void GoToFrame(int frame)
		{
			m_TargetFrame = frame;
			MovingToFrame = true;

			m_SDKHelper.GoToFrame(frame);
		}

		private void SaveProjectChanges()
		{
			SaveLoadHelper.SaveProject(TimelineNodes.ToList(), SessionID);
		}
	}
}
