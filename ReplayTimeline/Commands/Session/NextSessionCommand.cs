using iRacingSimulator;
using System;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class NextSessionCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public NextSessionCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return ReplayDirectorVM.IsSessionReady() && !ReplayDirectorVM.PlaybackEnabled;
		}

		public void Execute(object parameter)
		{
			Sim.Instance.Sdk.Replay.Jump(iRSDKSharp.ReplaySearchModeTypes.NextSession);
		}
	}
}
