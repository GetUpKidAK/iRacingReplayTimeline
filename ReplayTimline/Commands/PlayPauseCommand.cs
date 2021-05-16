using System;
using System.Windows.Input;


namespace ReplayTimeline
{
	public class PlayPauseCommand : ICommand
	{
		public ReplayTimelineVM ReplayTimelineVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public PlayPauseCommand(ReplayTimelineVM vm)
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
