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


		private Node NextNode;

		public NextStoredFrameCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			if (!ReplayDirectorVM.NodeCollection.NodesListOccupied)
				return false;

			if (ReplayDirectorVM.IsSessionReady())
			{
				if (!ReplayDirectorVM.PlaybackEnabled)
				{
					var currentNode = ReplayDirectorVM.CurrentNode;

					if (currentNode != null)
					{
						NextNode = currentNode.NextNode;
					}
					else
					{
						var orderedNodes = ReplayDirectorVM.TimelineNodesView.Cast<Node>().ToList();
						NextNode = orderedNodes.FirstOrDefault(n => n.Frame > ReplayDirectorVM.CurrentFrame);
					}
				}
				else
					return false;
			}

			return NextNode != null;
		}

		public void Execute(object parameter)
		{
			ReplayDirectorVM.CurrentNode = NextNode;
		}
	}
}
