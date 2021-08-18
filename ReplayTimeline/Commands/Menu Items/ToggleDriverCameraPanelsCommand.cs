using System;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class ToggleDriverCameraPanelsCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public ToggleDriverCameraPanelsCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return ReplayDirectorVM.WindowWidth > ReplayDirectorVM.WidthToDisableSidePanels;
		}

		public void Execute(object parameter) { }
	}
}
