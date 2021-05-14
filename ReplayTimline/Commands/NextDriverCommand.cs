using System;
using System.Linq;
using System.Windows.Input;


namespace ReplayTimeline
{
	public class NextDriverCommand : ICommand
	{
		public ReplayTimelineVM ReplayTimelineVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public NextDriverCommand(ReplayTimelineVM vm)
		{
			ReplayTimelineVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return ReplayTimelineVM.CurrentDriver != null;
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
