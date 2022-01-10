using iRacingSimulator;
using System;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class ManualFrameEntryCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		private int _frameAsInt;

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public ManualFrameEntryCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			if (ReplayDirectorVM.IsSessionReady())
			{
				if (ReplayDirectorVM.PlaybackEnabled)
					return false;

				if (int.TryParse(ReplayDirectorVM.ManualFrameEntryText, out _frameAsInt))
				{
					if (_frameAsInt < ReplayDirectorVM.FinalFrame && _frameAsInt >= 0)
					{
						return true;
					}
				}
			}

			return false;
		}

		public void Execute(object parameter)
		{
			Sim.Instance.Sdk.Replay.SetPosition(_frameAsInt);
		}
	}
}
