using iRacingSimulator;


namespace iRacingReplayDirector
{
	public class CamChangeNode : Node
	{
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

		public CamChangeNode(int frame, Driver driver, Camera camera)
		{
			Frame = frame;
			Driver = driver;
			Camera = camera;

			UpdateLabel();
		}

		protected override void UpdateLabel()
		{
			if (Driver == null || Camera == null) return;

			NodeLabel = $"Frame #{Frame} - Cut to {Driver.TeamName} ({Camera.GroupName})";
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
