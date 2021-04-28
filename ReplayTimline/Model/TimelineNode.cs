using System;
using System.Collections.Generic;
using System.ComponentModel;


namespace ReplayTimeline
{
	public class TimelineNode : INotifyPropertyChanged
	{
		public int Frame { get; set; }
		public Driver Driver { get; set; }
		public Camera Camera { get; set; }



		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	}
}
