using System;
using System.Windows;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class ToggleUseInSimCaptureCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public ToggleUseInSimCaptureCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return !ReplayDirectorVM.IsCaptureActive();
		}

		public void Execute(object parameter)
		{
			if (ReplayDirectorVM.InSimCaptureSettingEnabled)
			{
				ReplayDirectorVM.UseInSimCapture = true;
				ReplayDirectorVM.UseOBSCapture = false;
			}
			else
			{
				ReplayDirectorVM.UseInSimCapture = false;

				MessageBox.Show("The In-Sim capture setting is not enabled.\n\nPlease enable this under Options - Misc and restart the Sim.", "Error");
			}
		}
	}
}
