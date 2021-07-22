using System;
using System.Windows;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class ToggleUseOBSCaptureCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public ToggleUseOBSCaptureCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return !ReplayDirectorVM.IsCaptureActive();
		}

		public void Execute(object parameter)
		{
			var obsProcess = ExternalProcessHelper.GetExternalProcess();

			if (obsProcess != null)
			{
				ToggleOBSCaptureSetting(true);
			}
			else
			{
				ToggleOBSCaptureSetting(false);

				MessageBox.Show("Couldn't find OBS process, please ensure the application is running.", "Error");
			}
		}

		private void ToggleOBSCaptureSetting(bool enabled)
		{
			ReplayDirectorVM.UseOBSCapture = enabled;

			ReplayDirectorVM.UseInSimCapture = !ReplayDirectorVM.UseOBSCapture;
		}
	}
}
