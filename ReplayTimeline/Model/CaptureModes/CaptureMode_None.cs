

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
			CaptureModeAvailable = true;
			return true;
		}

		public override bool IsReadyToRecord()
		{
			CaptureModeAvailable = true;
			return false;
		}

		public override void StartRecording()
		{
			return;
		}

		public override void StopRecording()
		{
			return;
		}
	}
}
