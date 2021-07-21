using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using iRacingSdkWrapper;


namespace iRacingReplayDirector
{
	public partial class ReplayDirectorVM : INotifyPropertyChanged
	{
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
			RecordBtnText = "Record";
			DisableUIWhenRecording = true;

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
			ToggleRecordingCommand = new ToggleRecordingCommand(this);

			ToggleInSimUICommand = new ToggleInSimUICommand(this);
			ToggleSimUIOnPlaybackCommand = new ToggleSimUIOnPlaybackCommand(this);
			ToggleSimUIOnRecordingCommand = new ToggleSimUIOnRecordingCommand(this);
			ToggleRecordingOnFinalNodeCommand = new ToggleRecordingOnFinalNodeCommand(this);

			ApplicationQuitCommand = new ApplicationQuitCommand(this);
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
			StatusBarSessionID = "No Session Loaded.";
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
			NormalPlaybackSpeedEnabled = CurrentPlaybackSpeed == 1 && !SlowMotionEnabled;
			VideoCaptureSettingEnabled = m_SDKHelper.VideoCaptureEnabled.Value;
			VideoCaptureActive = m_SDKHelper.VideoCaptureActive.Value;

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

			// Update status bar session info
			var sessionType = SessionInfoHelper.GetCurrentSessionType(SessionInfo, telemetryInfo.SessionNum.Value);
			var sessionLaps = SessionInfoHelper.GetSessionLapCount(SessionInfo, telemetryInfo.SessionNum.Value);
			var lapInfo = (CurrentDriver.Lap > -1) ? $"(Lap {CurrentDriver.Lap}/{sessionLaps})" : ""; // Only show lap info if one has started
			StatusBarCurrentSessionInfo = $"Current Session: {sessionType} {lapInfo}";

			var currentNode = TimelineNodes.LastOrDefault(node => node.Frame == CurrentFrame);
			if (currentNode == null && !PlaybackEnabled)
				CurrentTimelineNode = currentNode;

			PlaybackCameraSwitching();
		}

		private void PlaybackCameraSwitching()
		{
			if (!NormalPlaybackSpeedEnabled)
				return;

			IEnumerable<TimelineNode> orderedNodes = TimelineNodesView.Cast<TimelineNode>();
			TimelineNode nodeToApply = orderedNodes.LastOrDefault(node => node.Frame <= CurrentFrame);

			if (nodeToApply == null || nodeToApply == _lastAppliedNode)
				return;

			if (StopRecordingOnFinalNode && VideoCaptureActive)
			{
				// Check index of node, if last one,....
				var orderedNodeList = orderedNodes.ToList();
				var nodeIndex = orderedNodeList.IndexOf(nodeToApply);

				if (nodeIndex == orderedNodeList.Count - 1)
				{
					StopRecording();
					return;
				}
			}

			_lastAppliedNode = nodeToApply;
			CurrentTimelineNode = nodeToApply;
			JumpToNode(CurrentTimelineNode);
		}

		public void SessionInfoUpdated(SessionInfo sessionInfo)
		{
			SessionInfo = sessionInfo;

			UpdateDriversAndCameras(sessionInfo);

			SessionID = SessionInfoHelper.GetSessionID(sessionInfo);
			StatusBarSessionID = $"Session ID: {SessionID}";

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

					InSimUIEnabled = true;
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
					var foundDriver = Drivers.FirstOrDefault(d => d.NumberRaw == node.DriverNumber);
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

		private void TimelineNodeChanged()
		{
			StoreFrameBtnText = CurrentTimelineNode == null ? "Store Node" : "Update Node";

			if (CurrentTimelineNode != null)
				JumpToNode(CurrentTimelineNode);
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

		public void GoToFrame(int frame)
		{
			m_SDKHelper.GoToFrame(frame);
		}

		public void SetPlaybackSpeed(int speed, bool slowMo = false)
		{
			// Shouldn't be possible, but lock speeds at 16X for safety
			if (speed > 16) speed = 16; else if (speed < -16) speed = -16;

			if (speed == 1 && !slowMo)
				if (DisableSimUIOnPlayback)
					InSimUIEnabled = false;

			if (speed == 0 && DisableSimUIOnPlayback)
				InSimUIEnabled = true;

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

		public void SaveProjectChanges()
		{
			SaveLoadHelper.SaveProject(TimelineNodes.ToList(), SessionID);
		}

		public void StartRecording()
		{
			InSimUIEnabled = !DisableUIWhenRecording;
			SetPlaybackSpeed(1);

			m_SDKHelper.EnableVideoCapture();
		}

		public void StopRecording()
		{
			InSimUIEnabled = true;
			SetPlaybackSpeed(0);
			m_SDKHelper.DisableVideoCapture();
		}
	}
}
