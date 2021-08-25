using iRacingSimulator;
using System;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class SkipFrameForwardCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public SkipFrameForwardCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return ReplayDirectorVM.IsSessionReady() &&
				!ReplayDirectorVM.IsCaptureActive() &&
				ReplayDirectorVM.CurrentFrame != ReplayDirectorVM.FinalFrame - 1;
		}

		public void Execute(object parameter)
		{
			Sim.Instance.Sdk.Replay.SetPosition(ReplayDirectorVM.CurrentFrame + 1);
		}
	}
}
