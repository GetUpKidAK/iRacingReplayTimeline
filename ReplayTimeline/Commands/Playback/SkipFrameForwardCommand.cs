using System;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class SkipFrameForwardCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public SkipFrameForwardCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return ReplayDirectorVM.SessionInfoLoaded &&
				!ReplayDirectorVM.VideoCaptureActive &&
				ReplayDirectorVM.CurrentFrame != ReplayDirectorVM.FinalFrame - 1;
		}

		public void Execute(object parameter)
		{
			ReplayDirectorVM.GoToFrame(ReplayDirectorVM.CurrentFrame + 1);
		}
	}
}
