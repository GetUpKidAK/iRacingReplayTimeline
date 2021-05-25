﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using iRacingSdkWrapper;


namespace iRacingReplayDirector
{
	public class ReplayDirectorVM : INotifyPropertyChanged
	{
		private SDKHelper m_SDKHelper;
		private int m_TargetFrame = -1;
		private bool m_LiveSessionPopupVisible;

		private const string m_ApplicationTitle = "iRacing Replay Director";
		private const string m_VersionNumber = "1.0";

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

		private double _sessionTime;
		public double SessionTime
		{
			get { return _sessionTime; }
			set { _sessionTime = value; OnPropertyChanged("SessionTime"); }
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
				UpdatePlaybackButtonText();
				OnPropertyChanged("CurrentPlaybackSpeed");
				CommandManager.InvalidateRequerySuggested();
			}
		}

		private string m_PlaybackSpeedText;
		public string PlaybackSpeedText
		{
			get { return m_PlaybackSpeedText; }
			set { m_PlaybackSpeedText = value; OnPropertyChanged("PlaybackSpeedText"); }
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
		public SlowMotionCommand SlowMotionCommand { get; set; }
		public NextLapCommand NextLapCommand { get; set; }
		public PreviousLapCommand PreviousLapCommand { get; set; }
		public NextSessionCommand NextSessionCommand { get; set; }
		public PreviousSessionCommand PreviousSessionCommand { get; set; }
		public NextDriverCommand NextDriverCommand { get; set; }
		public PreviousDriverCommand PreviousDriverCommand { get; set; }
		public ApplicationQuitCommand ApplicationQuitCommand { get; set; }
		public ConnectSimCommand ConnectSimCommand { get; set; }
		public DisconnectSimCommand DisconnectSimCommand { get; set; }
		public MoreInfoCommand MoreInfoCommand { get; set;}
		public AboutCommand AboutCommand { get; set; }
		#endregion


		public ReplayDirectorVM()
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
			SlowMotionCommand = new SlowMotionCommand(this);
			SkipFrameBackCommand = new SkipFrameBackCommand(this);
			SkipFrameForwardCommand = new SkipFrameForwardCommand(this);
			NextLapCommand = new NextLapCommand(this);
			PreviousLapCommand = new PreviousLapCommand(this);
			NextSessionCommand = new NextSessionCommand(this);
			PreviousSessionCommand = new PreviousSessionCommand(this);
			NextDriverCommand = new NextDriverCommand(this);
			PreviousDriverCommand = new PreviousDriverCommand(this);
			ApplicationQuitCommand = new ApplicationQuitCommand(this);
			ConnectSimCommand = new ConnectSimCommand(this);
			DisconnectSimCommand = new DisconnectSimCommand(this);
			MoreInfoCommand = new MoreInfoCommand(this);
			AboutCommand = new AboutCommand(this);

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
			StatusBarText = "iRacing Connected.";
		}

		public void SdkDisconnected()
		{
			StatusBarText = "iRacing Disconnected.";

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
			SessionTime = telemetryInfo.SessionTime.Value;
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
				if (!m_LiveSessionPopupVisible)
				{
					if (SessionInfoHelper.IsLiveSession(sessionInfo))
					{
						m_LiveSessionPopupVisible = true;

						var sessionWarningResult = MessageBox.Show("You seem to be running a live session.\n\nThis tool is designed for replays. Some stuff may work, but it's not supported.",
							"Viewing a Live session", MessageBoxButton.OK, MessageBoxImage.Warning);

						if (sessionWarningResult == MessageBoxResult.OK || sessionWarningResult == MessageBoxResult.Cancel)
						{
							m_LiveSessionPopupVisible = false;

							StatusBarText = "iRacing Connected. Running a live session, not currently supported.";
						}
					}

					LoadExistingProjectFile();

					SessionInfoLoaded = true;
				}
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
			if (loaddedProject.Nodes.Count > 0)
			{
				TimelineNodes.Clear();

				foreach (var node in loaddedProject.Nodes)
				{
					var foundDriver = Drivers.First(d => d.NumberRaw == node.DriverNumber);
					var foundCamera = Cameras.FirstOrDefault(c => c.GroupName == node.CameraName);

					if (foundDriver == null)
					{
						Console.WriteLine($"ERROR: Couldn't find driver with number {node.DriverNumber}. Node ignored.");
					}
					else
					{
						if (foundCamera == null)
							foundCamera = new Camera() { GroupName = node.CameraName };

						TimelineNode newTimelineNode = new TimelineNode()
						{
							Enabled = node.Enabled,
							Frame = node.Frame,
							Driver = foundDriver,
							Camera = foundCamera
						};

						TimelineNodes.Add(newTimelineNode);
					}
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

		public void SetPlaybackSpeed(int speed, bool slowMo = false)
		{
			// Shouldn't be possible, but lock speeds at 16X for safety
			if (speed > 16) speed = 16; else if (speed < -16) speed = -16;

			if (!slowMo) m_SDKHelper.SetPlaybackSpeed(speed);
			else m_SDKHelper.SetSlowMotionPlaybackSpeed(speed);
		}

		private void UpdatePlaybackButtonText()
		{
			PlayPauseBtnText = PlaybackEnabled ? "Pause" : "Play";
			
			if (CurrentPlaybackSpeed > 1)
				PlaybackSpeedText = SlowMotionEnabled ? $"FF 1/{CurrentPlaybackSpeed + 1}x" : $"FF {CurrentPlaybackSpeed}x";
			else if (CurrentPlaybackSpeed < -1)
				PlaybackSpeedText = SlowMotionEnabled ? $"RW 1/{-CurrentPlaybackSpeed + 1}x" : $"RW {-CurrentPlaybackSpeed}x";
			else
			{
				if (CurrentPlaybackSpeed > 0) PlaybackSpeedText = SlowMotionEnabled ? "FF 1/2x" : $"{CurrentPlaybackSpeed}x";
				else
				{
					if (PlaybackEnabled) PlaybackSpeedText = SlowMotionEnabled ? $"RW 1/2x" : $"RW";
					else PlaybackSpeedText = "Paused";
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

		public void GoToFrame(int frame)
		{
			m_TargetFrame = frame;
			MovingToFrame = true;

			m_SDKHelper.GoToFrame(frame);
		}

		public void SaveProjectChanges()
		{
			SaveLoadHelper.SaveProject(TimelineNodes.ToList(), SessionID);
		}
	}
}
