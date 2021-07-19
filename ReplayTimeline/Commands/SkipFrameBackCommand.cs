using System;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class SkipFrameBackCommand : ICommand
	{
		public ReplayDirectorVM ReplayTimelineVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public SkipFrameBackCommand(ReplayDirectorVM vm)
		{
			ReplayTimelineVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return ReplayTimelineVM.SessionInfoLoaded &&
				ReplayTimelineVM.CurrentFrame > 0;
		}

		public void Execute(object parameter)
		{
			ReplayTimelineVM.GoToFrame(ReplayTimelineVM.CurrentFrame - 1);
		}
	}
}
