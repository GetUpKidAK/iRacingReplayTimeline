using System;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class PlayPauseCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public PlayPauseCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return ReplayDirectorVM.SessionInfoLoaded && !ReplayDirectorVM.RecordingInProgress;
		}

		public void Execute(object parameter)
		{
			if (ReplayDirectorVM.PlaybackEnabled)
			{
				ReplayDirectorVM.SetPlaybackSpeed(0);
			}
			else
			{
				ReplayDirectorVM.SetPlaybackSpeed(1);
			}
		}
	}
}
