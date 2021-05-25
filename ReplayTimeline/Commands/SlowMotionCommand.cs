using System;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class SlowMotionCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }


		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public SlowMotionCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			// Disable at max speed?
			return ReplayDirectorVM.SessionInfoLoaded;
		}

		public void Execute(object parameter)
		{
			// Slow motion will jump to half speed forward (or backwards if rewinding)
			// Unless already in slow motion when the speed is halved again.
			if (ReplayDirectorVM.SlowMotionEnabled)
			{
				// In-Sim speed jumps are 1, 3, 7, 11, 15X
				int newSpeed = ReplayDirectorVM.CurrentPlaybackSpeed;
				bool inReverse = ReplayDirectorVM.CurrentPlaybackSpeed < 0;

				// Get absolute version of current speed (ignore negative speeds)
				switch (Math.Abs(ReplayDirectorVM.CurrentPlaybackSpeed))
				{
					// For each case, use the negative version if in reverse
					case 1:
						newSpeed += (inReverse) ? -2 : 2;
						break;
					case 3:
						newSpeed += (inReverse) ? -4 : 4;
						break;
					case 7:
						newSpeed += (inReverse) ? -4 : 4;
						break;
					case 11:
						newSpeed += (inReverse) ? -4 : 4;
						break;
					default:
						break;
				}

				// Add new speed
				ReplayDirectorVM.SetPlaybackSpeed(newSpeed, true);
			}
			else
			{
				if (ReplayDirectorVM.CurrentPlaybackSpeed >= 0)
				{
					ReplayDirectorVM.SetPlaybackSpeed(1, true);
				}
				else
				{
					ReplayDirectorVM.SetPlaybackSpeed(-1, true);
				}
			}
		}
	}
}
