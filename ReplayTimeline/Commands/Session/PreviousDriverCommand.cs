using System;
using System.Linq;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class PreviousDriverCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public PreviousDriverCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			if (ReplayDirectorVM.PlaybackEnabled)
				return false;

			if (ReplayDirectorVM.IsSessionReady())
			{
				return ReplayDirectorVM.CurrentDriver != null;
			}

			return false;
		}

		public void Execute(object parameter)
		{
			var currentDriver = ReplayDirectorVM.CurrentDriver;
			var orderedDriverList = ReplayDirectorVM.Drivers.OrderByDescending(d => d.LapDistance).ToList();

			int driverIndex = orderedDriverList.IndexOf(currentDriver);

			if (driverIndex > -1)
			{
				int previousDriverIndex = driverIndex + 1;
				if (previousDriverIndex > orderedDriverList.Count - 1) previousDriverIndex = 0;

				var prevDriver = orderedDriverList.ElementAt(previousDriverIndex);

				ReplayDirectorVM.CurrentDriver = prevDriver;
			}
		}
	}
}
