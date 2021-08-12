using iRacingSimulator;
using System.ComponentModel;


namespace iRacingReplayDirector
{
	public class TimelineNode : INotifyPropertyChanged
	{
		private bool _enabled = true;
		public bool Enabled
		{
			get { return _enabled; }
			set { _enabled = value; OnPropertyChanged("Enabled"); }
		}

		private int _frame;
		public int Frame
		{
			get { return _frame; }
			set { _frame = value; OnPropertyChanged("Frame"); }
		}

		private Driver _driver;
		public Driver Driver
		{
			get { return _driver; }
			set { _driver = value; OnPropertyChanged("Driver"); }
		}

		private Camera _camera;
		public Camera Camera
		{
			get { return _camera; }
			set { _camera = value; OnPropertyChanged("Camera"); }
		}


		public void ApplyNode()
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


		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	}
}
