using System;
using System.Linq;
using System.Windows.Input;


namespace iRacingReplayDirector
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
			if (!(ReplayTimelineVM.TimelineNodes.Count > 0))
				return false;

			if (ReplayTimelineVM.SessionInfoLoaded)
			{
				if (!ReplayTimelineVM.PlaybackEnabled)
				{
					var currentNode = ReplayTimelineVM.CurrentTimelineNode;

					if (currentNode != null)
					{
						var nodeIndex = ReplayTimelineVM.TimelineNodes.IndexOf(currentNode);

						return nodeIndex < ReplayTimelineVM.TimelineNodes.Count - 1;
					}
					else
					{
						var nearestNode = ReplayTimelineVM.TimelineNodes.FirstOrDefault(n => n.Frame > ReplayTimelineVM.CurrentFrame);

						if (nearestNode != null)
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		public void Execute(object parameter)
		{
			var targetNode = ReplayTimelineVM.TimelineNodes.FirstOrDefault(n => n.Frame > ReplayTimelineVM.CurrentFrame);

			if (targetNode != null)
			{
				ReplayTimelineVM.CurrentTimelineNode = targetNode;
				ReplayTimelineVM.GoToFrame(targetNode.Frame);
			}
		}
	}
}
