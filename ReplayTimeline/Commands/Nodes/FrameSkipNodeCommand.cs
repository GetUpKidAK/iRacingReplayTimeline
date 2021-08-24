using System;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class FrameSkipNodeCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public FrameSkipNodeCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			if (ReplayDirectorVM.SessionInfoLoaded)
			{
				if (!ReplayDirectorVM.PlaybackEnabled)
				{
					return ReplayDirectorVM.CurrentNode == null;
				}

				return false;
			}

			return false;
		}

		public void Execute(object parameter)
		{
			FrameSkipNode newNode = new FrameSkipNode(true, ReplayDirectorVM.CurrentFrame);

			ReplayDirectorVM.NodeCollection.AddNode(newNode);

			ReplayDirectorVM.CurrentNode = newNode;
		}
	}
}
