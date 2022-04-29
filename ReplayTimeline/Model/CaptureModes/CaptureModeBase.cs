using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace iRacingReplayDirector
{
	public abstract class CaptureModeBase
	{
		public string Name { get; set; }
		public string ProcessName { get; set; }

		public abstract bool IsAvailable();
		public abstract bool IsReadyToRecord();
		public abstract bool ToggleRecording(bool enabled);


		public CaptureModeBase() { }
	}
}
