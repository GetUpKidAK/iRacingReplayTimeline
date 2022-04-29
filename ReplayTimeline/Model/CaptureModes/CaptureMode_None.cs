using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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

		public override bool ToggleRecording(bool enabled)
		{
			return false;
		}
	}
}
