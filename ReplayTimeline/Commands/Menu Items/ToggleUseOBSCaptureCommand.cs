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
			var obsProcess = ExternalProcessHelper.GetExternalProcess();

			if (obsProcess != null)
			{
				ReplayDirectorVM.UseOBSCapture = true;
				ReplayDirectorVM.UseInSimCapture = false;
			}
			else
			{
				ReplayDirectorVM.UseOBSCapture = false;

				MessageBox.Show("Couldn't find OBS process, please ensure the application is running.", "Error");
			}
		}
	}
}
