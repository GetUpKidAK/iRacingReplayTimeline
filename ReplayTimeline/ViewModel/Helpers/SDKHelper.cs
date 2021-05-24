using iRacingSdkWrapper;


namespace iRacingReplayDirector
{
	public class SDKHelper
	{
		private ReplayDirectorVM _timelineVM;
		private SdkWrapper m_Wrapper;


		public SDKHelper(ReplayDirectorVM timelimeVM)
		{
			_timelineVM = timelimeVM;

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
			_timelineVM.SdkConnected();
		}

		private void SdkDisconnected(object sender, System.EventArgs e)
		{
			_timelineVM.SdkDisconnected();
		}

		private void TelemetryUpdated(object sender, SdkWrapper.TelemetryUpdatedEventArgs e)
		{
			_timelineVM.TelemetryUpdated(e.TelemetryInfo);
		}

		private void SessionInfoUpdated(object sender, SdkWrapper.SessionInfoUpdatedEventArgs e)
		{
			_timelineVM.SessionInfoUpdated(e.SessionInfo);
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
	}
}
