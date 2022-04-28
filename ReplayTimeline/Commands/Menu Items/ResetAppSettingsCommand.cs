using System;
using System.Threading.Tasks;
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

		public async void Execute(object parameter)
		{
			MessageBoxResult confirmationPopUp = MessageBox.Show($"This will reset all application settings and the window size to the defaults. Are you sure?",
						"Reset Application Settings", MessageBoxButton.OKCancel, MessageBoxImage.None, MessageBoxResult.OK);

			if (confirmationPopUp == MessageBoxResult.OK)
			{
				await ResetSettings();
			}
		}

		private async Task ResetSettings()
		{
			// Runs twice to avoid weird bug with height not resetting
			// TODO: Look into it? Maybe?
			Properties.Settings.Default.Reset();
			SaveLoadHelper.LoadSettings(ReplayDirectorVM);

			await Task.Delay(100);

			Properties.Settings.Default.Reset();
			SaveLoadHelper.LoadSettings(ReplayDirectorVM);
		}
	}
}
