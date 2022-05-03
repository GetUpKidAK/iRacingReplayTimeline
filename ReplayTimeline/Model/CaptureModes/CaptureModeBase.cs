using System.ComponentModel;


namespace iRacingReplayDirector
{
	public abstract class CaptureModeBase : INotifyPropertyChanged
	{
		public string Name { get; set; }
		public string ProcessName { get; set; }
		public string CaptureAvailabilityMessage { get; set; }

		private bool _captureModeAvailable;
		public bool CaptureModeAvailable
		{
			get { return _captureModeAvailable; }
			set { _captureModeAvailable = value; OnPropertyChanged("CaptureModeAvailable"); }
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


		public abstract bool IsAvailable();
		public abstract bool IsReadyToRecord();
		public abstract void StartRecording();
		public abstract void StopRecording();


		public CaptureModeBase() { }
	}
}
