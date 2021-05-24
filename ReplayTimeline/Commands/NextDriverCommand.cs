using System;
using System.Linq;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class NextDriverCommand : ICommand
	{
		public ReplayDirectorVM ReplayTimelineVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public NextDriverCommand(ReplayDirectorVM vm)
		{
			ReplayTimelineVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			if (ReplayTimelineVM.SessionInfoLoaded)
			{
				return ReplayTimelineVM.CurrentDriver != null;
			}

			return false;
		}

		public void Execute(object parameter)
		{
			var currentDriver = ReplayTimelineVM.CurrentDriver;
			var orderedDriverList = ReplayTimelineVM.Drivers.OrderByDescending(d => d.LapDistance).ToList();

			int driverIndex = orderedDriverList.IndexOf(currentDriver);

			if (driverIndex > -1)
			{
				int nextDriverIndex = driverIndex - 1;
				if (nextDriverIndex < 0) nextDriverIndex = orderedDriverList.Count - 1;

				var nextDriver = orderedDriverList.ElementAt(nextDriverIndex);

				ReplayTimelineVM.CurrentDriver = nextDriver;
			}
		}
	}
}
