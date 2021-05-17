using System.ComponentModel;


namespace ReplayTimeline
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


		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	}
}
