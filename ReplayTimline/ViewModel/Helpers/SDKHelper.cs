using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iRacingSdkWrapper;


namespace ReplayTimeline
{
	public class SDKHelper
	{
		private ReplayTimelineVM _timelineVM;
		private SdkWrapper m_Wrapper;

		//public TelemetryInfo TelemetryCache { get; private set; }


		public SDKHelper(ReplayTimelineVM timelimeVM)
		{
			_timelineVM = timelimeVM;

			m_Wrapper = new SdkWrapper();

			m_Wrapper.TelemetryUpdated += TelemetryUpdated;
			m_Wrapper.SessionInfoUpdated += SessionInfoUpdated;

			m_Wrapper.Start();
		}

		public void Stop()
		{
			m_Wrapper.TelemetryUpdated -= TelemetryUpdated;
			m_Wrapper.SessionInfoUpdated -= SessionInfoUpdated;

			m_Wrapper.Stop();
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

		public void SetCamera(Camera camera)
		{
			m_Wrapper.Camera.SwitchGroup(camera.GroupNum);
		}

		public void SetDriver(Driver driver)
		{
			m_Wrapper.Camera.SwitchToCar(driver.Id);
		}

		public void SetDriver(Driver driver, Camera camera)
		{
			m_Wrapper.Camera.SwitchToCar(driver.Id, camera.GroupNum);
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
