using System;
using System.Windows;
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
			if (ReplayDirectorVM.IsSessionReady())
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
			if (!ReplayDirectorVM.FrameSkipInfoShown)
			{
				MessageBoxResult confirmationPopUp = MessageBox.Show($"You may experience noticable pauses during longer frame skips if replay spooling is enabled in iRacing.\n\n" +
							$"You can find more information about this and how to potentially stop it happening in the More Info/Guides option under the Help menu.",
							"Information about Frame Skips", MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.OK);

				if (confirmationPopUp == MessageBoxResult.OK)
				{
					ReplayDirectorVM.FrameSkipInfoShown = true;
				}
			}

			FrameSkipNode newNode = new FrameSkipNode(true, ReplayDirectorVM.CurrentFrame);
			ReplayDirectorVM.NodeCollection.AddNode(newNode);
			ReplayDirectorVM.CurrentNode = newNode;
		}
	}
}
