﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class ToggleRecordingOnFinalNodeCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public ToggleRecordingOnFinalNodeCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			if (ReplayDirectorVM.VideoCaptureActive)
			{
				// If capture is active disable the button if the final node is already gone
				IEnumerable<TimelineNode> orderedNodes = ReplayDirectorVM.TimelineNodesView.Cast<TimelineNode>();
				TimelineNode finalNode = orderedNodes.LastOrDefault(node => node.Frame > ReplayDirectorVM.CurrentFrame);

				return finalNode != null;
			}

			return true;
		}

		public void Execute(object parameter) { }
	}
}