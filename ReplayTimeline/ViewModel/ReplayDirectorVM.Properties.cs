using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using iRacingSdkWrapper;


namespace iRacingReplayDirector
{
	public partial class ReplayDirectorVM
	{
		private SDKHelper m_SDKHelper;
		private bool m_LiveSessionPopupVisible;

		private const string m_ApplicationTitle = "iRacing Sequence Director";
		private const string m_VersionNumber = "1.2";

		#region Properties
		public string WindowTitle { get { return $"{m_ApplicationTitle} (v{m_VersionNumber})"; } }
		public bool SessionInfoLoaded { get; private set; } = false;
		public SessionInfo SessionInfo { get; private set; }
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

		private TimelineNode _lastAppliedNode;

		private Driver _currentDriver;
		public Driver CurrentDriver
		{
			get { return _currentDriver; }
			set { _currentDriver = value;
				OnPropertyChanged("CurrentDriver");
				DriverChanged(); }
		}

		private Camera _currentCamera;
		public Camera CurrentCamera
		{
			get { return _currentCamera; }
			set { _currentCamera = value;
				OnPropertyChanged("CurrentCamera");
				CameraChanged(); }
		}

		private int _currentFrame;
		public int CurrentFrame
		{
			get { return _currentFrame; }
			set
			{
				_currentFrame = value;
				OnPropertyChanged("CurrentFrame");
			}
		}

		private double _sessionTime;
		public double SessionTime
		{
			get { return _sessionTime; }
			set { _sessionTime = value;
				OnPropertyChanged("SessionTime"); }
		}

		private int _finalFrame;
		public int FinalFrame
		{
			get { return _finalFrame; }
			set { _finalFrame = value;
				OnPropertyChanged("FinalFrame"); }
		}

		private bool _slowMotionEnabled;
		public bool SlowMotionEnabled
		{
			get { return _slowMotionEnabled; }
			set { _slowMotionEnabled = value;
				OnPropertyChanged("SlowMotionEnabled"); }
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

		private string _playbackSpeedText;
		public string PlaybackSpeedText
		{
			get { return _playbackSpeedText; }
			set { _playbackSpeedText = value;
				OnPropertyChanged("PlaybackSpeedText"); }
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
			private set { _inSimCaptureSettingEnabled = value;
				OnPropertyChanged("InSimCaptureSettingEnabled"); }
		}

		private bool _inSimCaptureActive;
		public bool InSimCaptureActive
		{
			get { return _inSimCaptureActive; }
			private set { _inSimCaptureActive = value;
				OnPropertyChanged("InSimCaptureActive"); }
		}

		private bool _InSimUIEnabled;
		public bool InSimUIEnabled
		{
			get { return _InSimUIEnabled; }
			set
			{
				_InSimUIEnabled = value;
				m_SDKHelper.ToggleUI(_InSimUIEnabled);
				OnPropertyChanged("InSimUIEnabled");
			}
		}

		#endregion

		#region Label Properties

		private string _storeFrameBtnText;
		public string StoreFrameBtnText
		{
			get { return _storeFrameBtnText; }
			set { _storeFrameBtnText = value;
				OnPropertyChanged("StoreFrameBtnText"); }
		}

		private string _playPauseBtnText;
		public string PlayPauseBtnText
		{
			get { return _playPauseBtnText; }
			set { _playPauseBtnText = value;
				OnPropertyChanged("PlayPauseBtnText"); }
		}

		private string _recordBtnText;
		public string RecordBtnText
		{
			get { return _recordBtnText; }
			set { _recordBtnText = value;
				OnPropertyChanged("RecordBtnText"); }
		}

		private string _captureModeText;
		public string CaptureModeText
		{
			get { return _captureModeText; }
			set { _captureModeText = value; OnPropertyChanged("CaptureModeText"); }
		}

		private bool _showVisualTimeline;
		public bool ShowVisualTimeline
		{
			get { return _showVisualTimeline; }
			set { _showVisualTimeline = value;
				OnPropertyChanged("ShowVisualTimeline"); }
		}

		private bool _disableSimUIOnPlayback;
		public bool DisableSimUIOnPlayback
		{
			get { return _disableSimUIOnPlayback; }
			set { _disableSimUIOnPlayback = value;
				OnPropertyChanged("DisableSimUIOnPlayback"); }
		}

		private bool _disableUIWhenRecording;
		public bool DisableUIWhenRecording
		{
			get { return _disableUIWhenRecording; }
			set { _disableUIWhenRecording = value;
				OnPropertyChanged("DisableUIWhenRecording"); }
		}

		private bool _stopRecordingOnFinalNode;
		public bool StopRecordingOnFinalNode
		{
			get { return _stopRecordingOnFinalNode; }
			set { _stopRecordingOnFinalNode = value;
				OnPropertyChanged("StopRecordingOnFinalNode"); }
		}

		private bool _useInSimCapture;
		public bool UseInSimCapture
		{
			get { return _useInSimCapture; }
			set { _useInSimCapture = value; OnPropertyChanged("UseInSimCapture"); }
		}

		private bool _useOBSCapture;
		public bool UseOBSCapture
		{
			get { return _useOBSCapture; }
			set { _useOBSCapture = value; OnPropertyChanged("UseOBSCapture"); }
		}

		private bool _externalCaptureActive;
		public bool ExternalCaptureActive
		{
			get { return _externalCaptureActive; }
			set { _externalCaptureActive = value; OnPropertyChanged("ExternalCaptureActive"); }
		}

		private bool _showRecordingButtons;
		public bool ShowRecordingButtons
		{
			get { return _showRecordingButtons; }
			set
			{
				_showRecordingButtons = value;
				OnPropertyChanged("ShowRecordingButtons");
			}
		}

		private bool _showSessionLapSkipButtons;
		public bool ShowSessionLapSkipButtons
		{
			get { return _showSessionLapSkipButtons; }
			set { _showSessionLapSkipButtons = value;
				OnPropertyChanged("ShowSessionLapSkipButtons"); }
		}

		private bool _showDriverCameraPanels;
		public bool ShowDriverCameraPanels
		{
			get { return _showDriverCameraPanels; }
			set { _showDriverCameraPanels = value;
				OnPropertyChanged("ShowDriverCameraPanels"); }
		}

		private bool _showTimelineNodeList;
		public bool ShowTimelineNodeList
		{
			get { return _showTimelineNodeList; }
			set { _showTimelineNodeList = value;
				OnPropertyChanged("ShowTimelineNodeList"); }
		}

		private bool _minimizedMode;
		public bool MinimizedMode
		{
			get { return _minimizedMode; }
			set
			{
				_minimizedMode = value;

				ShowVisualTimeline = !_minimizedMode;
				ShowRecordingButtons = !_minimizedMode;
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

		#endregion

		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		

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
		public ToggleRecordingCommand ToggleRecordingCommand { get; set; }

		public ApplicationQuitCommand ApplicationQuitCommand { get; set; }
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
