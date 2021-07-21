using System;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class ToggleSimUIOnPlaybackCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public ToggleSimUIOnPlaybackCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return !ReplayDirectorVM.PlaybackEnabled;
		}

		public void Execute(object parameter) { }
	}
}
