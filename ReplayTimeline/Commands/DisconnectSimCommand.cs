using System;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class DisconnectSimCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public DisconnectSimCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return false;
			//return ReplayTimelineVM.IsConnected();
		}

		public void Execute(object parameter)
		{
			//ReplayTimelineVM.DisconnectSim();
		}
	}
}
