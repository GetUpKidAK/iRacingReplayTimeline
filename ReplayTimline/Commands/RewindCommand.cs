using System;
using System.Windows.Input;


namespace ReplayTimeline
{
	public class RewindCommand : ICommand
	{
		public ReplayTimelineVM ReplayTimelineVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public RewindCommand(ReplayTimelineVM vm)
		{
			ReplayTimelineVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			bool slowMoEnabled = ReplayTimelineVM.SlowMotionEnabled;

			if (ReplayTimelineVM.CurrentPlaybackSpeed < 0)
			{
				if (!slowMoEnabled)
					ReplayTimelineVM.CurrentPlaybackSpeed *= 2;
				else
					ReplayTimelineVM.CurrentPlaybackSpeed -= 2;
			}
			else
			{
				ReplayTimelineVM.CurrentPlaybackSpeed = -1;
			}

			ReplayTimelineVM.ChangePlaybackSpeed();
		}
	}
}
