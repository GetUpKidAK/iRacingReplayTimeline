using iRacingSimulator;
using System;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class PlayPauseCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public PlayPauseCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return ReplayDirectorVM.SessionInfoLoaded && !ReplayDirectorVM.IsCaptureActive();
		}

		public void Execute(object parameter)
		{
			if (ReplayDirectorVM.PlaybackEnabled)
			{
				Sim.Instance.Sdk.Replay.SetPlaybackSpeed(0);

				if (ReplayDirectorVM.DisableSimUIOnPlayback)
				{
					ReplayDirectorVM.InSimUIEnabled = true;
				}
			}
			else
			{
				Sim.Instance.Sdk.Replay.SetPlaybackSpeed(1);
				ReplayDirectorVM.InSimUIEnabled = !ReplayDirectorVM.DisableSimUIOnPlayback;
			}
		}
	}
}
