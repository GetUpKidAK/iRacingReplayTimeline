using System;
using System.Windows;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class ClearAllNodesCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public ClearAllNodesCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return ReplayDirectorVM.NodeCollection.NodesListOccupied;
		}

		public void Execute(object parameter)
		{
			MessageBoxResult confirmationPopUp = MessageBox.Show($"Are you sure? This can't be undone.",
						"Delete all nodes?", MessageBoxButton.YesNo, MessageBoxImage.None, MessageBoxResult.No);

			if (confirmationPopUp == MessageBoxResult.Yes)
			{
				ReplayDirectorVM.NodeCollection.RemoveAllNodes();
				ReplayDirectorVM.CurrentNode = null;
			}
		}
	}
}
