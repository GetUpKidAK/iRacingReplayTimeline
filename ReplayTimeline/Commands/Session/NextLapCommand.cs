using System;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class NextLapCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public NextLapCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return ReplayDirectorVM.SessionInfoLoaded && !ReplayDirectorVM.VideoCaptureActive;
		}

		public void Execute(object parameter)
		{
			ReplayDirectorVM.JumpToEvent(iRSDKSharp.ReplaySearchModeTypes.NextLap);
		}
	}
}
