using System;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class ConnectSimCommand : ICommand
	{
		public ReplayDirectorVM ReplayTimelineVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public ConnectSimCommand(ReplayDirectorVM vm)
		{
			ReplayTimelineVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return false;
			//return !ReplayTimelineVM.IsConnected();
		}

		public void Execute(object parameter)
		{
			//ReplayTimelineVM.ConnectSim();
		}
	}
}
