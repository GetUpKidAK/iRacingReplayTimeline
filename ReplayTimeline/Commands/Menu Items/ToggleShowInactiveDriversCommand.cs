using System;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class ToggleShowInactiveDriversCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public ToggleShowInactiveDriversCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			ReplayDirectorVM.ToggleInactiveDriverDisplay();
		}
	}
}
