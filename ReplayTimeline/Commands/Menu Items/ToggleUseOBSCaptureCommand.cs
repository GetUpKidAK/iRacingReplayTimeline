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
			if (!ReplayDirectorVM.IsSessionReady())
				return false;

			return !ReplayDirectorVM.IsCaptureActive();
		}

		public void Execute(object parameter)
		{
			var obsProcess = ExternalProcessHelper.GetExternalProcess("obs64");

			// Disabling OBS capture, don't bother checking if application is running
			if (!ReplayDirectorVM.UseOBSCapture)
			{
				return;
			}

			if (obsProcess != null)
			{
				ReplayDirectorVM.UseInSimCapture = false;
			}
			else  // If OBS can't be found
			{
				ReplayDirectorVM.UseOBSCapture = false;

				MessageBox.Show("Couldn't find OBS process, please ensure the application is running.", "Error");
			}
		}
	}
}
