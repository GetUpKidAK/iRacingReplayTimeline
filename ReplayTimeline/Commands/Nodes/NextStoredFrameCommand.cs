using iRacingSimulator;
using System;
using System.Linq;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class NextStoredFrameCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public NextStoredFrameCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			if (!(ReplayDirectorVM.Nodes.NodeList.Count > 0))
				return false;

			if (ReplayDirectorVM.SessionInfoLoaded)
			{
				if (!ReplayDirectorVM.PlaybackEnabled)
				{
					var currentNode = ReplayDirectorVM.CurrentNode;
					var orderedNodes = ReplayDirectorVM.TimelineNodesView.Cast<Node>().ToList();

					if (currentNode != null)
					{
						var nodeIndex = orderedNodes.IndexOf(currentNode);

						return nodeIndex < orderedNodes.Count - 1;
					}
					else
					{
						var nearestNode = orderedNodes.FirstOrDefault(n => n.Frame > ReplayDirectorVM.CurrentFrame);

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
			var orderedNodes = ReplayDirectorVM.TimelineNodesView.Cast<Node>();
			var targetNode = orderedNodes.FirstOrDefault(n => n.Frame > ReplayDirectorVM.CurrentFrame);

			if (targetNode != null)
			{
				ReplayDirectorVM.CurrentNode = targetNode;
			}
		}
	}
}
