using System;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class SlowMotionToggleCommand : ICommand
	{
		public ReplayDirectorVM ReplayTimelineVM { get; set; }


		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public SlowMotionToggleCommand(ReplayDirectorVM vm)
		{
			ReplayTimelineVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return ReplayTimelineVM.SessionInfoLoaded;
		}

		public void Execute(object parameter)
		{
			ReplayTimelineVM.CurrentPlaybackSpeed = ReplayTimelineVM.CurrentPlaybackSpeed >= 0 ? 1 : -1;

			ReplayTimelineVM.ChangePlaybackSpeed();
		}
	}
}
