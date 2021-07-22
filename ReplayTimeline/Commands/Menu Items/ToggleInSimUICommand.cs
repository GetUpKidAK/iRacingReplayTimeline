using System;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class ToggleInSimUICommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public ToggleInSimUICommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return !ReplayDirectorVM.IsCaptureActive();
		}

		public void Execute(object parameter) { }
	}
}
