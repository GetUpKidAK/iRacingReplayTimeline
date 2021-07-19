using System;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class SkipFrameBackCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public SkipFrameBackCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return ReplayDirectorVM.SessionInfoLoaded &&
				!ReplayDirectorVM.RecordingInProgress &&
				ReplayDirectorVM.CurrentFrame > 0;
		}

		public void Execute(object parameter)
		{
			ReplayDirectorVM.GoToFrame(ReplayDirectorVM.CurrentFrame - 1);
		}
	}
}
