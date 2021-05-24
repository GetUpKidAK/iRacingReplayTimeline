using System;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class NextLapCommand : ICommand
	{
		public ReplayDirectorVM ReplayTimelineVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public NextLapCommand(ReplayDirectorVM vm)
		{
			ReplayTimelineVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return ReplayTimelineVM.SessionInfoLoaded;
		}

		public void Execute(object parameter)
		{
			ReplayTimelineVM.JumpToEvent(iRSDKSharp.ReplaySearchModeTypes.NextLap);
		}
	}
}
