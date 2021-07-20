using System;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class StartRecordCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }


		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public StartRecordCommand(ReplayDirectorVM vm)
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
			// Change Playback enabled to new property for recording?
			if (ReplayDirectorVM.VideoCaptureActive)
			{
				ReplayDirectorVM.InSimUIEnabled = true;
				ReplayDirectorVM.SetPlaybackSpeed(0);
				ReplayDirectorVM.StopRecording();
			}
			else
			{
				ReplayDirectorVM.InSimUIEnabled = false;
				ReplayDirectorVM.SetPlaybackSpeed(1);
				ReplayDirectorVM.StartRecording();
			}
		}
	}
}
