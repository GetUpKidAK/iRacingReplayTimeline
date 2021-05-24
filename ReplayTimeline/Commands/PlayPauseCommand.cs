using System;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class PlayPauseCommand : ICommand
	{
		public ReplayDirectorVM ReplayTimelineVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public PlayPauseCommand(ReplayDirectorVM vm)
		{
			ReplayTimelineVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return ReplayTimelineVM.SessionInfoLoaded;
		}

		public void Execute(object parameter)
		{
			bool slowMoEnabled = ReplayTimelineVM.SlowMotionEnabled;

			if (ReplayTimelineVM.CurrentPlaybackSpeed == 0)
			{
				ReplayTimelineVM.CurrentPlaybackSpeed = 1;
				ReplayTimelineVM.ChangePlaybackSpeed();
			}
			else
			{
				ReplayTimelineVM.CurrentPlaybackSpeed = 0;
				ReplayTimelineVM.ChangePlaybackSpeed();
			}
		}
	}
}
