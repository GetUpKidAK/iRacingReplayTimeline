﻿using iRacingSimulator;


namespace iRacingReplayDirector
{
	public class FrameSkipNode : Node
	{
		public FrameSkipNode(bool enabled, int frame)
		{
			Enabled = enabled;
			Frame = frame;

			UpdateLabel();
		}

		protected override void UpdateLabel()
		{
			if (NextNode == null)
			{
				Enabled = false;
				NodeLabel = $"Frame #{Frame} - Skip Frame - NEEDS A NODE TO SKIP TO";
			}
			else
			{
				Enabled = true;
				NodeLabel = $"Frame #{Frame} - Skip Frame - Jump to {NextNode.Frame}";
			}
		}

		public override void ApplyNode()
		{
			bool playbackEnabled = Sim.Instance.Telemetry.ReplayPlaySpeed.Value != 0;

			if (!playbackEnabled)
			{
				Sim.Instance.Sdk.Replay.SetPosition(Frame);
				return;
			}

			if (NextNode != null)
			{
				Sim.Instance.Sdk.Replay.SetPosition(NextNode.Frame);
				Sim.Instance.Sdk.Replay.SetPlaybackSpeed(1);
			}
		}
	}
}