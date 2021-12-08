using iRacingSimulator;
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
			return ReplayDirectorVM.IsSessionReady() && !ReplayDirectorVM.IsCaptureActive();
		}

		public void Execute(object parameter)
		{
			if (ReplayDirectorVM.PlaybackEnabled)
			{
				ReplayDirectorVM.PlaybackSpeed = ReplayDirectorVM.PauseSpeed;

				if (ReplayDirectorVM.DisableSimUIOnPlayback)
				{
					ReplayDirectorVM.InSimUIEnabled = true;
				}
			}
			else
			{
				ReplayDirectorVM.PlaybackSpeed = ReplayDirectorVM.PlaySpeed;
				ReplayDirectorVM.InSimUIEnabled = !ReplayDirectorVM.DisableSimUIOnPlayback;
			}
		}
	}
}
