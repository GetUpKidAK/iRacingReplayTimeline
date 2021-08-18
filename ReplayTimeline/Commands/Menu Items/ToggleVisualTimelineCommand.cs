using System;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class ToggleVisualTimelineCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public ToggleVisualTimelineCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return ReplayDirectorVM.WindowHeight > ReplayDirectorVM.HeightToDisableOneControl;
		}

		public void Execute(object parameter) { }
	}
}
