using iRacingSimulator;
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


		private Node PreviousNode;

		public PreviousStoredFrameCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			if (!ReplayDirectorVM.Nodes.NodesListOccupied)
				return false;

			if (ReplayDirectorVM.SessionInfoLoaded)
			{
				if (!ReplayDirectorVM.PlaybackEnabled)
				{
					var currentNode = ReplayDirectorVM.CurrentNode;

					if (currentNode != null)
					{
						PreviousNode = currentNode.PreviousNode;
					}
					else
					{
						var orderedNodes = ReplayDirectorVM.TimelineNodesView.Cast<Node>().ToList();
						PreviousNode = orderedNodes.LastOrDefault(n => n.Frame < ReplayDirectorVM.CurrentFrame);
					}
				}
			}

			return PreviousNode != null;
		}

		public void Execute(object parameter)
		{
			ReplayDirectorVM.CurrentNode = PreviousNode;
		}
	}
}
