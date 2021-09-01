using System;
using System.Windows;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class ResetAppSettingsCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public ResetAppSettingsCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			MessageBoxResult confirmationPopUp = MessageBox.Show($"This will reset all application settings and the window size to the defaults. Are you sure?",
						"Reset Application Settings", MessageBoxButton.OKCancel, MessageBoxImage.None, MessageBoxResult.OK);

			if (confirmationPopUp == MessageBoxResult.OK)
			{
				ReplayDirectorVM.SetAppDefaults();
			}
		}
	}
}
