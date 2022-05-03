using System;
using System.Windows;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class OpenURLCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public OpenURLCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			string url = (string)parameter;

			MessageBoxResult confirmationPopUp = MessageBox.Show($"This will open your default browser and take you to:\n\n{url}.",
						"Open URL", MessageBoxButton.OKCancel, MessageBoxImage.None, MessageBoxResult.OK);

			if (confirmationPopUp == MessageBoxResult.OK)
			{
				var sInfo = new System.Diagnostics.ProcessStartInfo(url)
				{
					UseShellExecute = true,
				};
				System.Diagnostics.Process.Start(sInfo);
			}
		}
	}
}
