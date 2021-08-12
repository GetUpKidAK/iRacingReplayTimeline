﻿using System.Collections.ObjectModel;
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
		private const string m_VersionNumber = "1.21";

		/// <summary>
		/// PROPERTIES
		/// </summary>
		#region Properties
		public string WindowTitle { get { return $"{m_ApplicationTitle} (v{m_VersionNumber})"; } }
		public bool SessionInfoLoaded { get; private set; } = false;
		public int SessionID { get; private set; }
		#endregion

		/// <summary>
		/// BINDING PROPERTIES
		/// </summary>
		#region Binding Properties
		public ObservableCollection<TimelineNode> TimelineNodes { get; set; }
		public ICollectionView TimelineNodesView { get; private set; }
		public ObservableCollection<Driver> Drivers { get; set; }
		public ICollectionView DriversView { get; private set; }
		public ObservableCollection<Camera> Cameras { get; set; }


		private TimelineNode _currentTimelineNode;
		public TimelineNode CurrentTimelineNode
		{
			get { return _currentTimelineNode; }
			set
			{
				if (_currentTimelineNode != value && value != null)
				{
					_currentTimelineNode = value;
					_currentTimelineNode.ApplyNode();
					OnPropertyChanged("CurrentTimelineNode");
				}
			}
		}

		private TimelineNode _lastAppliedNode;

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

		private bool _windowAlwaysOnTop;
		public bool WindowAlwaysOnTop
		{
			get { return _windowAlwaysOnTop; }
			set { _windowAlwaysOnTop = value; OnPropertyChanged("WindowAlwaysOnTop"); }
		}

		#endregion

		/// <summary>
		/// LABEL PROPERTIES
		/// </summary>
		#region Label Properties

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
			set { _disableSimUIOnPlayback = value;OnPropertyChanged("DisableSimUIOnPlayback"); }
		}

		private bool _disableUIWhenRecording;
		public bool DisableUIWhenRecording
		{
			get { return _disableUIWhenRecording; }
			set { _disableUIWhenRecording = value; OnPropertyChanged("DisableUIWhenRecording"); }
		}

		private bool _stopRecordingOnFinalNode;
		public bool StopRecordingOnFinalNode
		{
			get { return _stopRecordingOnFinalNode; }
			set { _stopRecordingOnFinalNode = value; OnPropertyChanged("StopRecordingOnFinalNode"); }
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

		private bool _showVisualTimeline;
		public bool ShowVisualTimeline
		{
			get { return _showVisualTimeline; }
			set
			{
				_showVisualTimeline = value; MinimizedModeStatusCheck();
				OnPropertyChanged("ShowVisualTimeline");
			}
		}

		private bool _showRecordingButtons;
		public bool ShowRecordingButtons
		{
			get { return _showRecordingButtons; }
			set { _showRecordingButtons = value; MinimizedModeStatusCheck();
					OnPropertyChanged("ShowRecordingButtons");}
		}

		private bool _showSessionLapSkipButtons;
		public bool ShowSessionLapSkipButtons
		{
			get { return _showSessionLapSkipButtons; }
			set { _showSessionLapSkipButtons = value; MinimizedModeStatusCheck();
					OnPropertyChanged("ShowSessionLapSkipButtons"); }
		}

		private bool _showDriverCameraPanels;
		public bool ShowDriverCameraPanels
		{
			get { return _showDriverCameraPanels; }
			set { _showDriverCameraPanels = value; MinimizedModeStatusCheck();
				OnPropertyChanged("ShowDriverCameraPanels"); }
		}

		private bool _showTimelineNodeList;
		public bool ShowTimelineNodeList
		{
			get { return _showTimelineNodeList; }
			set { _showTimelineNodeList = value; MinimizedModeStatusCheck();
				OnPropertyChanged("ShowTimelineNodeList"); }
		}

		private bool _minimizedMode;
		public bool MinimizedMode
		{
			get { return _minimizedMode; }
			set
			{
				_minimizedMode = value;

				_showVisualTimeline = !_minimizedMode; OnPropertyChanged("ShowVisualTimeline");
				_showRecordingButtons = !_minimizedMode; OnPropertyChanged("ShowRecordingButtons");
				_showSessionLapSkipButtons = !_minimizedMode; OnPropertyChanged("ShowSessionLapSkipButtons");
				_showDriverCameraPanels = !_minimizedMode; OnPropertyChanged("ShowDriverCameraPanels");
				_showTimelineNodeList = !_minimizedMode; OnPropertyChanged("ShowTimelineNodeList");

				OnPropertyChanged("MinimizedMode");
			}
		}

		private void MinimizedModeStatusCheck()
		{
			if (ShowVisualTimeline || ShowRecordingButtons || ShowSessionLapSkipButtons ||
				ShowDriverCameraPanels || ShowTimelineNodeList)
			{
				_minimizedMode = false;
				OnPropertyChanged("MinimizedMode");
			}
			else
			{
				_minimizedMode = true;
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
		public ToggleDriverSortOptionCommand ToggleDriverSortOptionCommand { get; set; }
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
