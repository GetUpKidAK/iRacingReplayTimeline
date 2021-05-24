using System;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class PreviousLapCommand : ICommand
	{
		public ReplayTimelineVM ReplayTimelineVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public PreviousLapCommand(ReplayTimelineVM vm)
		{
			ReplayTimelineVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return ReplayTimelineVM.SessionInfoLoaded;
		}

		public void Execute(object parameter)
		{
			ReplayTimelineVM.JumpToEvent(iRSDKSharp.ReplaySearchModeTypes.PreviousLap);
		}
	}
}
