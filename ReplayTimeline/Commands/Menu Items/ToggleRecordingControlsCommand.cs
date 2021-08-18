using System;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class ToggleRecordingControlsCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public ToggleRecordingControlsCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return ReplayDirectorVM.WindowHeight > ReplayDirectorVM.HeightToDisableAllControls;
		}

		public void Execute(object parameter) { }
	}
}
