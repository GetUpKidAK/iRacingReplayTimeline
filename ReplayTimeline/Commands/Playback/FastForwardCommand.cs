using iRacingSimulator;
using System;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class FastForwardCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public FastForwardCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return ReplayDirectorVM.IsSessionReady() &&
				!ReplayDirectorVM.IsCaptureActive() && 
				!(ReplayDirectorVM.CurrentPlaybackSpeed >= 16);
		}

		public void Execute(object parameter)
		{
			// Fast-forward will jump to faster speeds (up to 16x) once playback is enabled.
			// If in any other state (slow motion, rewinding, etc.) then it will just start playback at normal speed
			if (ReplayDirectorVM.CurrentPlaybackSpeed > 0 && !ReplayDirectorVM.SlowMotionEnabled)
			{
				// In-Sim speed jumps are 1, 2, 4, 8, 12, 16X
				int newSpeed = ReplayDirectorVM.CurrentPlaybackSpeed;
				
				switch (ReplayDirectorVM.CurrentPlaybackSpeed)
				{
					case 1:
						newSpeed *= 2;
						break;
					case 2:
						newSpeed *= 2;
						break;
					case 4:
						newSpeed *= 2;
						break;
					case 8:
						newSpeed += 4;
						break;
					case 12:
						newSpeed += 4;
						break;
					default:
						break;
				}

				Sim.Instance.Sdk.Replay.SetPlaybackSpeed(newSpeed);
			}
			else
			{
				Sim.Instance.Sdk.Replay.SetPlaybackSpeed(2);
			}
		}
	}
}
