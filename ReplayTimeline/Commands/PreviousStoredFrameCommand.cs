using System;
using System.Linq;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class PreviousStoredFrameCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public PreviousStoredFrameCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			if (!(ReplayDirectorVM.TimelineNodes.Count > 0))
				return false;

			if (ReplayDirectorVM.SessionInfoLoaded)
			{
				if (!ReplayDirectorVM.PlaybackEnabled)
				{
					var currentNode = ReplayDirectorVM.CurrentTimelineNode;

					if (currentNode != null)
					{
						var nodeIndex = ReplayDirectorVM.TimelineNodes.IndexOf(currentNode);

						return nodeIndex > 0;
					}
					else
					{
						var nearestNode = ReplayDirectorVM.TimelineNodes.LastOrDefault(n => n.Frame < ReplayDirectorVM.CurrentFrame);

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
			var targetNode = ReplayDirectorVM.TimelineNodes.LastOrDefault(n => n.Frame < ReplayDirectorVM.CurrentFrame);

			if (targetNode != null)
			{
				ReplayDirectorVM.CurrentTimelineNode = targetNode;
				ReplayDirectorVM.GoToFrame(targetNode.Frame);
			}
		}
	}
}
