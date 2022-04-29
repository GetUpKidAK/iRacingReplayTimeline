using iRacingSimulator;


namespace iRacingReplayDirector
{
	public class CaptureMode_Iracing : CaptureModeBase
	{
		public CaptureMode_Iracing() : base()
		{
			Name = "In-Sim Capture";
		}

		public override bool IsAvailable()
		{
			return Sim.Instance.Sdk.GetTelemetryValue<bool>("VidCapEnabled").Value;
		}

		public override bool IsReadyToRecord()
		{
			return IsAvailable();
		}

		public override bool ToggleRecording(bool enabled)
		{
			if (enabled)
			{
				Sim.Instance.Sdk.Sdk.BroadcastMessage(iRSDKSharp.BroadcastMessageTypes.VideoCapture, 1, 0);

				return true;
			}
			else
			{
				Sim.Instance.Sdk.Sdk.BroadcastMessage(iRSDKSharp.BroadcastMessageTypes.VideoCapture, 2, 0);

				return false;
			}
		}
	}
}
