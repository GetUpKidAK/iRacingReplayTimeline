using System;
using System.Windows;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class AboutCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public AboutCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			MessageBoxResult confirmationPopUp = MessageBox.Show($"Developed by Ash Kendall.\n\nContact: ash.kendall@gmail.com.",
						"About", MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.No);
		}
	}
}
