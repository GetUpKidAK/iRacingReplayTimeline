using iRacingSdkWrapper;
using iRacingSdkWrapper.Bitfields;


namespace iRacingReplayDirector
{
	public class SDKHelper
	{
		private ReplayDirectorVM m_DirectorVM;
		private SdkWrapper m_Wrapper;

		// Is Video Capture mode enabled in user settings?
		public TelemetryValue<bool> VideoCaptureEnabled { get => m_Wrapper.GetTelemetryValue<bool>("VidCapEnabled"); }
		// Is video capture currently recording?
		public TelemetryValue<bool> VideoCaptureActive { get => m_Wrapper.GetTelemetryValue<bool>("VidCapActive"); }


		public SDKHelper(ReplayDirectorVM vm)
		{
			m_DirectorVM = vm;

			m_Wrapper = new SdkWrapper();

			m_Wrapper.Connected += SdkConnected;
			m_Wrapper.Disconnected += SdkDisconnected;
			m_Wrapper.TelemetryUpdated += TelemetryUpdated;
			m_Wrapper.SessionInfoUpdated += SessionInfoUpdated;

			m_Wrapper.Start();
		}

		public void Stop()
		{
			m_Wrapper.Connected -= SdkConnected;
			m_Wrapper.Disconnected -= SdkDisconnected;
			m_Wrapper.TelemetryUpdated -= TelemetryUpdated;
			m_Wrapper.SessionInfoUpdated -= SessionInfoUpdated;

			m_Wrapper.Stop();
		}

		private void SdkConnected(object sender, System.EventArgs e)
		{
			m_DirectorVM.SdkConnected();
		}

		private void SdkDisconnected(object sender, System.EventArgs e)
		{
			m_DirectorVM.SdkDisconnected();
		}

		private void TelemetryUpdated(object sender, SdkWrapper.TelemetryUpdatedEventArgs e)
		{
			m_DirectorVM.TelemetryUpdated(e.TelemetryInfo);
		}

		private void SessionInfoUpdated(object sender, SdkWrapper.SessionInfoUpdatedEventArgs e)
		{
			m_DirectorVM.SessionInfoUpdated(e.SessionInfo);
		}


		public void SetPlaybackSpeed(int playbackSpeed)
		{
			m_Wrapper.Replay.SetPlaybackSpeed(playbackSpeed);
		}

		public void SetSlowMotionPlaybackSpeed(int playbackSpeed)
		{
			m_Wrapper.Replay.SetSlowmotionPlaybackSpeed(playbackSpeed);
		}

		public void SetCamera(Camera camera)
		{
			m_Wrapper.Camera.SwitchGroup(camera.GroupNum);
		}

		public void SetDriver(Driver driver)
		{
			m_Wrapper.Camera.SwitchToCar(driver.NumberRaw);
		}

		public void SetDriver(Driver driver, Camera camera)
		{
			m_Wrapper.Camera.SwitchToCar(driver.NumberRaw, camera.GroupNum);
		}

		public void GoToFrame(int frameNumber)
		{
			m_Wrapper.Replay.SetPosition(frameNumber);
		}

		public void JumpToEvent(iRSDKSharp.ReplaySearchModeTypes replayEvent)
		{
			m_Wrapper.Replay.Jump(replayEvent);
		}

		public void EnableUI()
		{
			CameraState state = new CameraState();
			state.Remove(CameraStates.UIHidden);

			m_Wrapper.Camera.SetCameraState(state);
		}

		public void DisableUI()
		{
			CameraState state = new CameraState();
			state.Add(CameraStates.UIHidden);

			m_Wrapper.Camera.SetCameraState(state);
		}

		public void EnableVideoCapture()
		{
			// Pass '1' as parameter to start recording
			m_Wrapper.Sdk.BroadcastMessage(iRSDKSharp.BroadcastMessageTypes.VideoCapture, 1, 0);
		}

		public void DisableVideoCapture()
		{
			// Pass '2' as parameter to stop recording
			m_Wrapper.Sdk.BroadcastMessage(iRSDKSharp.BroadcastMessageTypes.VideoCapture, 2, 0);
		}

		public void ToggleVideoCapture()
		{
			// Pass '3' as parameter to toggle recording
			m_Wrapper.Sdk.BroadcastMessage(iRSDKSharp.BroadcastMessageTypes.VideoCapture, 3, 0);
		}
	}
}
