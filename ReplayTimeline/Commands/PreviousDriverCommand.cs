using System;
using System.Linq;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class PreviousDriverCommand : ICommand
	{
		public ReplayTimelineVM ReplayTimelineVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public PreviousDriverCommand(ReplayTimelineVM vm)
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
				int previousDriverIndex = driverIndex + 1;
				if (previousDriverIndex > orderedDriverList.Count - 1) previousDriverIndex = 0;

				var prevDriver = orderedDriverList.ElementAt(previousDriverIndex);

				ReplayTimelineVM.CurrentDriver = prevDriver;
			}
		}
	}
}
