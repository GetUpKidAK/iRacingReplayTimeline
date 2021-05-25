using System;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class RewindCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public RewindCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return ReplayDirectorVM.SessionInfoLoaded && !(ReplayDirectorVM.CurrentPlaybackSpeed <= -16);
		}

		public void Execute(object parameter)
		{
			// Rewind will jump to faster speeds (up to 16x) once rewind is enabled.
			// If in any other state (slow motion, fast-forward, etc.) then it will just start playback in reverse at 1X speed
			if (ReplayDirectorVM.CurrentPlaybackSpeed < 0 && !ReplayDirectorVM.SlowMotionEnabled)
			{
				// In-Sim speed jumps are 1, 2, 4, 8, 12, 16X
				int newSpeed = ReplayDirectorVM.CurrentPlaybackSpeed;

				switch (ReplayDirectorVM.CurrentPlaybackSpeed)
				{
					case -1:
						newSpeed *= 2;
						break;
					case -2:
						newSpeed *= 2;
						break;
					case -4:
						newSpeed *= 2;
						break;
					case -8:
						newSpeed -= 4;
						break;
					case -12:
						newSpeed -= 4;
						break;
					default:
						break;
				}

				ReplayDirectorVM.SetPlaybackSpeed(newSpeed);
			}
			else
			{
				ReplayDirectorVM.SetPlaybackSpeed(-1);
			}
		}
	}
}
