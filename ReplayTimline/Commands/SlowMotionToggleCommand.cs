using System;
using System.Windows.Input;


namespace ReplayTimeline
{
	public class SlowMotionToggleCommand : ICommand
	{
		public ReplayTimelineVM ReplayTimelineVM { get; set; }


		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public SlowMotionToggleCommand(ReplayTimelineVM vm)
		{
			ReplayTimelineVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			ReplayTimelineVM.CurrentPlaybackSpeed = ReplayTimelineVM.CurrentPlaybackSpeed >= 0 ? 1 : -1;

			ReplayTimelineVM.ChangePlaybackSpeed();
		}
	}
}
