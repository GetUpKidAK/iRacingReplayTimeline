using System;
using System.Linq;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class PreviousStoredFrameCommand : ICommand
	{
		public ReplayTimelineVM ReplayTimelineVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public PreviousStoredFrameCommand(ReplayTimelineVM vm)
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

						return nodeIndex > 0;
					}
					else
					{
						var nearestNode = ReplayTimelineVM.TimelineNodes.LastOrDefault(n => n.Frame < ReplayTimelineVM.CurrentFrame);

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
			var targetNode = ReplayTimelineVM.TimelineNodes.LastOrDefault(n => n.Frame < ReplayTimelineVM.CurrentFrame);

			if (targetNode != null)
			{
				ReplayTimelineVM.CurrentTimelineNode = targetNode;
				ReplayTimelineVM.GoToFrame(targetNode.Frame);
			}
		}
	}
}
