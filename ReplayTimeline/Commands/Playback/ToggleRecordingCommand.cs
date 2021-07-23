﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class ToggleRecordingCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }


		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public ToggleRecordingCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			// Disabled if in-sim recording is disabled
			if (!ReplayDirectorVM.IsCaptureAvailable())
				return false;

			if (ReplayDirectorVM.StopRecordingOnFinalNode)
			{
				if (ReplayDirectorVM.TimelineNodes.Count > 0)
				{
					var orderedNodes = ReplayDirectorVM.TimelineNodesView.Cast<TimelineNode>().ToList();
					var finalNode = orderedNodes[orderedNodes.Count - 1];

					if (ReplayDirectorVM.CurrentFrame >= finalNode.Frame)
					{
						return false;
					}
				}
			}

			// Enabled while paused and as a stop recording button when recording. Needs extra property to get recording status
			if (ReplayDirectorVM.PlaybackEnabled)
			{
				if (ReplayDirectorVM.IsCaptureActive()) return true;
				else return false;
			}

			return true;
		}

		public void Execute(object parameter)
		{
			if (ReplayDirectorVM.IsCaptureActive())
			{
				ReplayDirectorVM.StopRecording();
			}
			else
			{
				ReplayDirectorVM.StartRecording();
			}
		}
	}
}
