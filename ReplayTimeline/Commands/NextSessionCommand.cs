using System;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class NextSessionCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public NextSessionCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return ReplayDirectorVM.SessionInfoLoaded && !ReplayDirectorVM.PlaybackEnabled;
		}

		public void Execute(object parameter)
		{
			ReplayDirectorVM.JumpToEvent(iRSDKSharp.ReplaySearchModeTypes.NextSession);
		}
	}
}
