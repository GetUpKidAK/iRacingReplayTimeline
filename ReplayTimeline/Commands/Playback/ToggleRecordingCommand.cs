using System;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class ToggleRecordingCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }


		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public ToggleRecordingCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			// Disabled if in-sim recording is disabled
			if (!ReplayDirectorVM.VideoCaptureSettingEnabled)
				return false;

			// Enabled while paused and as a stop recording button when recording. Needs extra property to get recording status
			var playbackEnabled = ReplayDirectorVM.PlaybackEnabled;
			var recordingInProgress = ReplayDirectorVM.VideoCaptureActive;

			if (playbackEnabled)
			{
				if (recordingInProgress) return true;
				else return false;
			}

			return true;
		}

		public void Execute(object parameter)
		{
			if (ReplayDirectorVM.VideoCaptureActive)
			{
				ReplayDirectorVM.StopRecording();
			}
			else
			{
				ReplayDirectorVM.StartRecording();
			}
		}
	}
}
