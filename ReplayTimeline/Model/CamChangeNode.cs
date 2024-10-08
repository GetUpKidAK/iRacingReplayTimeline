﻿using iRacingSimulator;


namespace iRacingReplayDirector
{
	public class CamChangeNode : Node
	{
		public override string NodeType { get => "Camera Change"; }

		private Driver _driver;
		public Driver Driver
		{
			get { return _driver; }
			set { _driver = value; UpdateLabel(); OnPropertyChanged("Driver"); }
		}

		private Camera _camera;
		public Camera Camera
		{
			get { return _camera; }
			set { _camera = value; UpdateLabel(); OnPropertyChanged("Camera"); }
		}

		public CamChangeNode(bool enabled, int frame, Driver driver, Camera camera)
		{
			Enabled = enabled;
			Frame = frame;
			Driver = driver;
			Camera = camera;

			UpdateLabel();
		}

		protected override void UpdateLabel()
		{
			if (Driver == null || Camera == null) return;

			NodeDetails = Driver.TeamName;
			NodeDetailsAdditional = Camera.GroupName;
		}

		public override void ApplyNode()
		{
			bool playbackEnabled = Sim.Instance.Telemetry.ReplayPlaySpeed.Value != 0;

			// If replay is playing back AND node is disabled, skip it...
			if (playbackEnabled && !Enabled)
				return;

			// Otherwise, switch driver and camera
			Sim.Instance.Sdk.Camera.SwitchToCar(Driver.NumberRaw, Camera.GroupNum);

			// If playback is disabled, skip to the frame
			if (!playbackEnabled)
				Sim.Instance.Sdk.Replay.SetPosition(Frame);
		}
	}
}
