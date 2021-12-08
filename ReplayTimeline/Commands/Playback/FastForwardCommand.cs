using iRacingSimulator;
using System;
using System.Windows.Input;
using System.Linq;


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
				var index = ReplayDirectorVM.AvailableSpeeds.IndexOf(ReplayDirectorVM.PlaybackSpeed);
				if (index < ReplayDirectorVM.AvailableSpeeds.Count)
				{
					ReplayDirectorVM.PlaybackSpeed = ReplayDirectorVM.AvailableSpeeds[index + 1];
				}
			}
			else
			{
				Sim.Instance.Sdk.Replay.SetPlaybackSpeed(2);
			}
		}
	}
}
