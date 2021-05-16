﻿using System;
using System.Windows.Input;


namespace ReplayTimeline
{
	public class SkipFrameForwardCommand : ICommand
	{
		public ReplayTimelineVM ReplayTimelineVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public SkipFrameForwardCommand(ReplayTimelineVM vm)
		{
			ReplayTimelineVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			return ReplayTimelineVM.CurrentFrame != ReplayTimelineVM.FinalFrame - 1;
		}

		public void Execute(object parameter)
		{
			if (!ReplayTimelineVM.MovingToFrame)
				ReplayTimelineVM.GoToFrame(ReplayTimelineVM.CurrentFrame + 1);
		}
	}
}