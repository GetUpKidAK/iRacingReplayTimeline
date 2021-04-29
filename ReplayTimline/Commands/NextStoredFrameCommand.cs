using System;
using System.Windows.Input;


namespace ReplayTimeline
{
	public class NextStoredFrameCommand : ICommand
	{
		public ReplayTimelineVM ReplayTimelineVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public NextStoredFrameCommand(ReplayTimelineVM vm)
		{
			ReplayTimelineVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return ReplayTimelineVM.TimelineNodes.Count > 0;
		}

		public void Execute(object parameter)
		{
			ReplayTimelineVM.GoToNextStoredFrame();
		}
	}
}
