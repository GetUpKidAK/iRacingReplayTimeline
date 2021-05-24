using System;
using System.Windows;
using System.Windows.Input;

namespace iRacingReplayDirector
{
	public class DeleteStoredFrameCommand : ICommand
	{
		public ReplayDirectorVM ReplayTimelineVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public DeleteStoredFrameCommand(ReplayDirectorVM vm)
		{
			ReplayTimelineVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			if (ReplayTimelineVM.SessionInfoLoaded)
			{
				return ReplayTimelineVM.CurrentTimelineNode != null && !ReplayTimelineVM.PlaybackEnabled;
			}

			return false;
		}

		public void Execute(object parameter)
		{
			MessageBoxResult confirmationPopUp = MessageBox.Show($"Are you sure? This can't be undone.",
						"Delete stored frame?", MessageBoxButton.YesNo, MessageBoxImage.None, MessageBoxResult.No);

			if (confirmationPopUp == MessageBoxResult.Yes)
				ReplayTimelineVM.DeleteStoredFrame();
		}
	}
}
