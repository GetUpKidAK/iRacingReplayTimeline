

namespace iRacingReplayDirector
{
	public class CaptureMode_None : CaptureModeBase
	{
		public CaptureMode_None() : base()
		{
			Name = "None";
		}

		public override bool IsAvailable()
		{
			return true;
		}

		public override bool IsReadyToRecord()
		{
			return false;
		}

		public override void ToggleRecording(bool enabled)
		{
			return;
		}
	}
}
