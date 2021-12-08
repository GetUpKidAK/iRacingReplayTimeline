using iRacingSimulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRacingReplayDirector
{
	public class PlaybackSpeed
	{
		public string SpeedLabel { get; set; }
		public int SpeedValue { get; set; }
		public bool SlowMotion { get; set; }

		public PlaybackSpeed(string speedLabel, int speedValue, bool slowMo)
		{
			SpeedLabel = speedLabel;
			SpeedValue = speedValue;
			SlowMotion = slowMo;
		}


		public void SetSpeed()
		{
			Sim.Instance.Sdk.Replay.SetPlaybackSpeed(SpeedValue, SlowMotion);
		}
	}
}
