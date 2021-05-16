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
			if (ReplayTimelineVM.SessionInfoLoaded)
			{
				if (!ReplayTimelineVM.PlaybackEnabled)
				{
					var currentNode = ReplayTimelineVM.CurrentTimelineNode;
					var nodeIndex = ReplayTimelineVM.TimelineNodes.IndexOf(currentNode);

					return ReplayTimelineVM.TimelineNodes.Count > 0 && nodeIndex != -1 && nodeIndex < ReplayTimelineVM.TimelineNodes.Count - 1;
				}
			}

			return false;
		}

		public void Execute(object parameter)
		{
			ReplayTimelineVM.GoToNextStoredFrame();
		}
	}
}
