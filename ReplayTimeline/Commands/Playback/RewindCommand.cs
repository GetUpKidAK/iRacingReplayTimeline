using iRacingSimulator;
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
			return ReplayDirectorVM.IsSessionReady() &&
				!ReplayDirectorVM.IsCaptureActive() &&
				!(ReplayDirectorVM.CurrentPlaybackSpeed <= -16);
		}

		public void Execute(object parameter)
		{
			// Rewind will jump to faster speeds (up to 16x) once rewind is enabled.
			// If in any other state (slow motion, fast-forward, etc.) then it will just start playback in reverse at 1X speed
			if (ReplayDirectorVM.CurrentPlaybackSpeed < 0 && !ReplayDirectorVM.SlowMotionEnabled)
			{
				var index = ReplayDirectorVM.AvailableSpeeds.IndexOf(ReplayDirectorVM.PlaybackSpeed);
				if (index > 0)
				{
					ReplayDirectorVM.PlaybackSpeed = ReplayDirectorVM.AvailableSpeeds[index - 1];
				}
			}
			else
			{
				Sim.Instance.Sdk.Replay.SetPlaybackSpeed(-1);
			}
		}
	}
}
