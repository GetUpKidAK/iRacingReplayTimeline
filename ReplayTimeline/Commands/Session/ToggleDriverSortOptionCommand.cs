using iRacingSdkWrapper;
using iRacingSimulator;
using System;
using System.ComponentModel;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class ToggleDriverSortOptionCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public ToggleDriverSortOptionCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			// Should only be enabled during race sessions
			if (!ReplayDirectorVM.IsSessionReady())
				return false;

			YamlQuery sessionInfoQuery = Sim.Instance.SessionInfo["SessionInfo"]["Sessions"]["SessionNum", Sim.Instance.Telemetry.SessionNum.Value];
			var sessionType = sessionInfoQuery["SessionType"].GetValue("");

			return sessionType.Contains("Race");
		}

		public void Execute(object parameter)
		{
			if (ReplayDirectorVM.SortDriversById)
			{
				ReplayDirectorVM.DriversView.SortDescriptions.Clear();
				SortDescription lapSort = new SortDescription("Lap", ListSortDirection.Descending);
				ReplayDirectorVM.DriversView.SortDescriptions.Add(lapSort);
				SortDescription lapDistanceSort = new SortDescription("LapDistance", ListSortDirection.Descending);
				ReplayDirectorVM.DriversView.SortDescriptions.Add(lapDistanceSort);

				ReplayDirectorVM.SortDriversById = false;
			}
			else
			{
				ReplayDirectorVM.DriversView.SortDescriptions.Clear();
				SortDescription idSort = new SortDescription("NumberRaw", ListSortDirection.Ascending);
				ReplayDirectorVM.DriversView.SortDescriptions.Add(idSort);

				ReplayDirectorVM.SortDriversById = true;
			}
		}
	}
}
