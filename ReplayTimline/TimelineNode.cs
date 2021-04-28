using System;
using System.Collections.Generic;
using System.ComponentModel;


namespace ReplayTimline
{
	public class TimelineNode : INotifyPropertyChanged
	{
		public int Frame { get; set; }

		//private Driver _driver;
		//public Driver Driver
		//{
		//	get { return _driver; }
		//	set { _driver = value; OnPropertyChanged("Driver"); }
		//}

		//private Camera _camera;
		//public Camera Camera
		//{
		//	get { return _camera; }
		//	set { _camera = value; OnPropertyChanged("Camera"); }
		//}


		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	}
}
