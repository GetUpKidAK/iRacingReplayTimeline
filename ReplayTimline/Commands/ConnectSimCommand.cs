using System;
using System.Windows.Input;


namespace ReplayTimeline
{
	public class ConnectSimCommand : ICommand
	{
		public ReplayTimelineVM ReplayTimelineVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public ConnectSimCommand(ReplayTimelineVM vm)
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
