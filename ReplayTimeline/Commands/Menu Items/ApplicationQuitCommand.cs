using System;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class ApplicationQuitCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public ApplicationQuitCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			App.Current.Shutdown();
			ReplayDirectorVM.SetPlaybackSpeed(0);
			if (ReplayDirectorVM.IsCaptureActive()) ReplayDirectorVM.StopRecording();

			SaveLoadHelper.SaveSettings(ReplayDirectorVM);

			Environment.Exit(0);
		}
	}
}
