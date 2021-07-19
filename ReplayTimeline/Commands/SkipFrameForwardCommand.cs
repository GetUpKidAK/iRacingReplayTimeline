using System;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class SkipFrameForwardCommand : ICommand
	{
		public ReplayDirectorVM ReplayTimelineVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public SkipFrameForwardCommand(ReplayDirectorVM vm)
		{
			ReplayTimelineVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return ReplayTimelineVM.SessionInfoLoaded &&
				ReplayTimelineVM.CurrentFrame != ReplayTimelineVM.FinalFrame - 1;
		}

		public void Execute(object parameter)
		{
			ReplayTimelineVM.GoToFrame(ReplayTimelineVM.CurrentFrame + 1);
		}
	}
}
