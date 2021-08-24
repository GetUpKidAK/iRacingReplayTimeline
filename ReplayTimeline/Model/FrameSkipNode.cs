using iRacingSimulator;


namespace iRacingReplayDirector
{
	public class FrameSkipNode : Node
	{
		private int _targetFrame;
		public int TargetFrame
		{
			get { return _targetFrame; }
			set { _targetFrame = value; OnPropertyChanged("TargetFrame"); }
		}


		public override void ApplyNode()
		{
			bool playbackEnabled = Sim.Instance.Telemetry.ReplayPlaySpeed.Value != 0;

			// If replay is playing back AND node is disabled, skip it...
			if (playbackEnabled && !Enabled)
				return;

			// Otherwise, switch driver and camera
			Sim.Instance.Sdk.Replay.SetPosition(TargetFrame);
		}
	}
}
