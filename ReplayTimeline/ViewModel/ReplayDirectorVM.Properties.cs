using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using iRacingSdkWrapper.Bitfields;
using iRacingSimulator;


namespace iRacingReplayDirector
{
	public partial class ReplayDirectorVM
	{
		private bool m_LiveSessionPopupVisible;

		private const string m_ApplicationTitle = "iRacing Sequence Director";
		private const string m_VersionNumber = "1.4";

		/// <summary>
		/// PROPERTIES
		/// </summary>
		#region Properties
		public string WindowTitle { get { return $"{m_ApplicationTitle} (v{m_VersionNumber})"; } }
		public bool SessionInfoLoaded { get; private set; } = false;
		public bool IsLiveSession { get; private set; }
		public int SessionID { get; private set; }
		private bool _frameSkipInfoShown;
		public bool FrameSkipInfoShown
		{
			get { return _frameSkipInfoShown; }
			set
			{
				_frameSkipInfoShown = value;
				Properties.Settings.Default.FrameSkipInfoShown = value;
			}
		}
		#endregion

		public ObservableCollection<CaptureModeBase> CaptureModes { get; set; }

		private CaptureModeBase _selectedCaptureMode;
		public CaptureModeBase SelectedCaptureMode
		{
			get { return _selectedCaptureMode; }
			set { _selectedCaptureMode = value; System.Console.WriteLine($"{_selectedCaptureMode.Name} selected"); OnPropertyChanged("SelectedCaptureMode"); }
		}


		/// <summary>
		/// BINDING PROPERTIES
		/// </summary>
		#region Binding Properties
		public NodeCollection NodeCollection { get; set; }
		public ICollectionView TimelineNodesView { get; private set; }
		public ObservableCollection<Driver> Drivers { get; set; }
		public ICollectionView DriversView { get; private set; }
		public ObservableCollection<Camera> Cameras { get; set; }

		private Node _currentNode;
		public Node CurrentNode
		{
			get { return _currentNode; }
			set
			{
				if (_currentNode != value && value != null)
				{
					_currentNode = value;
					_currentNode.ApplyNode();
					OnPropertyChanged("CurrentNode");
				}
			}
		}

		private Node _lastAppliedNode;

		private Driver _currentDriver;
		public Driver CurrentDriver
		{
			get { return _currentDriver; }
			set
			{
				if (_currentDriver != value && value != null)
				{
					_currentDriver = value;
					Sim.Instance.Sdk.Camera.SwitchToCar(_currentDriver.NumberRaw);
					OnPropertyChanged("CurrentDriver");
				}
			}
		}

		private Camera _currentCamera;
		public Camera CurrentCamera
		{
			get { return _currentCamera; }
			set
			{
				if (_currentCamera != value && value != null)
				{
					_currentCamera = value;
					Sim.Instance.Sdk.Camera.SwitchToCar(CurrentDriver.NumberRaw, _currentCamera.GroupNum);
					OnPropertyChanged("CurrentCamera");
				}
			}
		}

		private int _currentFrame;
		public int CurrentFrame
		{
			get { return _currentFrame; }
			set { _currentFrame = value; OnPropertyChanged("CurrentFrame"); }
		}

		private double _sessionTime;
		public double SessionTime
		{
			get { return _sessionTime; }
			set { _sessionTime = value; OnPropertyChanged("SessionTime"); }
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

		private int _currentPlaybackSpeed;
		public int CurrentPlaybackSpeed
		{
			get => _currentPlaybackSpeed;
			set
			{
				_currentPlaybackSpeed = value;
				UpdatePlaybackButtonText();
				OnPropertyChanged("CurrentPlaybackSpeed");
				CommandManager.InvalidateRequerySuggested();
			}
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

		private string _playbackSpeedText;
		public string PlaybackSpeedText
		{
			get { return _playbackSpeedText; }
			set { _playbackSpeedText = value; OnPropertyChanged("PlaybackSpeedText"); }
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

		private bool _normalPlaybackSpeedEnabled;
		public bool NormalPlaybackSpeedEnabled
		{
			get { return _normalPlaybackSpeedEnabled; }
			private set
			{
				_normalPlaybackSpeedEnabled = value;
				OnPropertyChanged("NormalPlaybackSpeedEnabled");
				CommandManager.InvalidateRequerySuggested();
			}
		}

		private bool _inSimCaptureSettingEnabled;
		public bool InSimCaptureSettingEnabled
		{
			get { return _inSimCaptureSettingEnabled; }
			private set { _inSimCaptureSettingEnabled = value; OnPropertyChanged("InSimCaptureSettingEnabled"); }
		}

		private bool _inSimCaptureActive;
		public bool InSimCaptureActive
		{
			get { return _inSimCaptureActive; }
			private set { _inSimCaptureActive = value; OnPropertyChanged("InSimCaptureActive"); }
		}

		private bool _InSimUIEnabled;
		public bool InSimUIEnabled
		{
			get { return _InSimUIEnabled; }
			set
			{
				_InSimUIEnabled = value;
				ToggleUI(_InSimUIEnabled);
				OnPropertyChanged("InSimUIEnabled");
			}
		}

		private void ToggleUI(bool enabled)
		{
			CameraState state = new CameraState();

			if (enabled) state.Remove(CameraStates.UIHidden);
			else state.Add(CameraStates.UIHidden);

			Sim.Instance.Sdk.Camera.SetCameraState(state);
		}

		private bool _sortDriversById;
		public bool SortDriversById
		{
			get { return _sortDriversById; }
			set
			{
				_sortDriversById = value;
				DriverSortButtonLabel = _sortDriversById ? "Sort By Pos" : "Sort By ID";
				OnPropertyChanged("SortDriversById");
			}
		}

		private int _windowWidth;
		public int WindowWidth
		{
			get { return _windowWidth; }
			set
			{
				_windowWidth = value;
				Properties.Settings.Default.WindowWidth = value;
				OnPropertyChanged("WindowWidth");
			}
		}

		private int _windowHeight;
		public int WindowHeight
		{
			get { return _windowHeight; }
			set
			{
				_windowHeight = value;
				Properties.Settings.Default.WindowHeight = value;
				OnPropertyChanged("WindowHeight");
			}
		}

		private bool _windowAlwaysOnTop;
		public bool WindowAlwaysOnTop
		{
			get { return _windowAlwaysOnTop; }
			set
			{
				_windowAlwaysOnTop = value;
				Properties.Settings.Default.WindowOnTop = value;
				OnPropertyChanged("WindowAlwaysOnTop");
			}
		}

		private string _manualFrameEntryText;
		public string ManualFrameEntryText
		{
			get { return _manualFrameEntryText; }
			set { _manualFrameEntryText = value; OnPropertyChanged("ManualFrameEntryText"); }
		}


		#endregion

		/// <summary>
		/// LABEL PROPERTIES
		/// </summary>
		#region Label Properties

		private string _camChangeBtnText;
		public string CamChangeBtnText
		{
			get { return _camChangeBtnText; }
			set { _camChangeBtnText = value; OnPropertyChanged("CamChangeBtnText"); }
		}

		private string _playPauseBtnText;
		public string PlayPauseBtnText
		{
			get { return _playPauseBtnText; }
			set { _playPauseBtnText = value; OnPropertyChanged("PlayPauseBtnText"); }
		}

		private string _recordBtnText;
		public string RecordBtnText
		{
			get { return _recordBtnText; }
			set { _recordBtnText = value; OnPropertyChanged("RecordBtnText"); }
		}

		private string _captureModeText;
		public string CaptureModeText
		{
			get { return _captureModeText; }
			set { _captureModeText = value; OnPropertyChanged("CaptureModeText"); }
		}

		private bool _disableSimUIOnPlayback;
		public bool DisableSimUIOnPlayback
		{
			get { return _disableSimUIOnPlayback; }
			set
			{
				_disableSimUIOnPlayback = value;
				Properties.Settings.Default.DisableSimUIOnPlayback = value;
				OnPropertyChanged("DisableSimUIOnPlayback");
			}
		}

		private bool _disableUIWhenRecording;
		public bool DisableUIWhenRecording
		{
			get { return _disableUIWhenRecording; }
			set
			{
				_disableUIWhenRecording = value;
				Properties.Settings.Default.DisableUIWhenRecording = value;
				OnPropertyChanged("DisableUIWhenRecording");
			}
		}

		private bool _stopRecordingOnFinalNode;
		public bool StopRecordingOnFinalNode
		{
			get { return _stopRecordingOnFinalNode; }
			set
			{
				_stopRecordingOnFinalNode = value;
				Properties.Settings.Default.StopRecordingOnFinalNode = value;
				OnPropertyChanged("StopRecordingOnFinalNode");
			}
		}

		private bool _useInSimCapture;
		public bool UseInSimCapture
		{
			get { return _useInSimCapture; }
			set
			{
				_useInSimCapture = value;
				Properties.Settings.Default.UseInSimCapture = value;
				OnPropertyChanged("UseInSimCapture");
			}
		}

		private bool _useOBSCapture;
		public bool UseOBSCapture
		{
			get { return _useOBSCapture; }
			set
			{
				_useOBSCapture = value;
				Properties.Settings.Default.UseOBSCapture = value;
				OnPropertyChanged("UseOBSCapture");
			}
		}

		private bool _externalCaptureActive;
		public bool ExternalCaptureActive
		{
			get { return _externalCaptureActive; }
			set { _externalCaptureActive = value; OnPropertyChanged("ExternalCaptureActive"); }
		}

		private bool _showVisualTimeline;
		public bool ShowVisualTimeline
		{
			get { return _showVisualTimeline; }
			set
			{
				_showVisualTimeline = value;
				Properties.Settings.Default.ShowVisualTimeline = value;
				OnPropertyChanged("ShowVisualTimeline");
			}
		}

		private bool _showRecordingControls;
		public bool ShowRecordingControls
		{
			get { return _showRecordingControls; }
			set
			{
				_showRecordingControls = value;
				Properties.Settings.Default.ShowRecordingControls = value;
				OnPropertyChanged("ShowRecordingControls");
			}
		}

		private bool _showSessionLapSkipControls;
		public bool ShowSessionLapSkipControls
		{
			get { return _showSessionLapSkipControls; }
			set
			{
				_showSessionLapSkipControls = value;
				Properties.Settings.Default.ShowSessionLapSkipControls = value;
				OnPropertyChanged("ShowSessionLapSkipControls");
			}
		}

		private bool _showDriverCameraPanels;
		public bool ShowDriverCameraPanels
		{
			get { return _showDriverCameraPanels; }
			set { _showDriverCameraPanels = value; 
				OnPropertyChanged("ShowDriverCameraPanels"); }
		}

		public int HeightToDisableControls { get { return 500; } }
		public int WidthToDisableSidePanels { get { return 900; } }


		private string _statusBarText;
		public string StatusBarText
		{
			get { return _statusBarText; }
			set { _statusBarText = value; OnPropertyChanged("StatusBarText"); }
		}

		private string _statusBarSessionID;
		public string StatusBarSessionID
		{
			get { return _statusBarSessionID; }
			set { _statusBarSessionID = value; OnPropertyChanged("StatusBarSessionID"); }
		}

		private string _statusBarCurrentSessionInfo;
		public string StatusBarCurrentSessionInfo
		{
			get { return _statusBarCurrentSessionInfo; }
			set { _statusBarCurrentSessionInfo = value; OnPropertyChanged("StatusBarCurrentSessionInfo"); }
		}

		private string _driverSortButtonLabel;
		public string DriverSortButtonLabel
		{
			get { return _driverSortButtonLabel; }
			set { _driverSortButtonLabel = value; OnPropertyChanged("DriverSortButtonLabel"); }
		}

		#endregion

		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		/// <summary>
		/// COMMANDS
		/// </summary>
		#region Commands
		public CamChangeNodeCommand CamChangeNodeCommand { get; set; }
		public FrameSkipNodeCommand FrameSkipNodeCommand { get; set; }
		public ClearAllNodesCommand ClearAllNodesCommand { get; set; }
		public PreviousStoredFrameCommand PreviousStoredFrameCommand { get; set; }
		public NextStoredFrameCommand NextStoredFrameCommand { get; set; }
		public DeleteStoredFrameCommand DeleteStoredFrameCommand { get; set; }

		public PlayPauseCommand PlayPauseCommand { get; set; }
		public RewindCommand RewindCommand { get; set; }
		public FastForwardCommand FastForwardCommand { get; set; }
		public SkipFrameBackCommand SkipFrameBackCommand { get; set; }
		public SkipFrameForwardCommand SkipFrameForwardCommand { get; set; }
		public SlowMotionCommand SlowMotionCommand { get; set; }

		public ManualFrameEntryCommand ManualFrameEntryCommand { get; set; }

		public NextLapCommand NextLapCommand { get; set; }
		public PreviousLapCommand PreviousLapCommand { get; set; }
		public NextSessionCommand NextSessionCommand { get; set; }
		public PreviousSessionCommand PreviousSessionCommand { get; set; }

		public NextDriverCommand NextDriverCommand { get; set; }
		public PreviousDriverCommand PreviousDriverCommand { get; set; }
		public ToggleDriverSortOptionCommand ToggleDriverSortOptionCommand { get; set; }

		public ToggleRecordingCommand ToggleRecordingCommand { get; set; }

		public ApplicationQuitCommand ApplicationQuitCommand { get; set; }
		public ResetAppSettingsCommand ResetAppSettingsCommand { get; set; }
		public MoreInfoCommand MoreInfoCommand { get; set; }
		public AboutCommand AboutCommand { get; set; }

		public ToggleInSimUICommand ToggleInSimUICommand { get; set; }
		public ToggleSimUIOnPlaybackCommand ToggleSimUIOnPlaybackCommand { get; set; }
		public ToggleSimUIOnRecordingCommand ToggleSimUIOnRecordingCommand { get; set; }
		public ToggleRecordingOnFinalNodeCommand ToggleRecordingOnFinalNodeCommand { get; set; }
		public ToggleUseInSimCaptureCommand ToggleUseInSimCaptureCommand { get; set; }
		public ToggleUseOBSCaptureCommand ToggleUseOBSCaptureCommand { get; set; }
		#endregion
	}
}
