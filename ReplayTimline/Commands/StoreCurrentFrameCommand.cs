﻿using System;
using System.Windows.Input;


namespace ReplayTimeline
{
	public class StoreCurrentFrameCommand : ICommand
	{
		public ReplayTimelineVM ReplayTimelineVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public StoreCurrentFrameCommand(ReplayTimelineVM vm)
		{
			ReplayTimelineVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			if (!ReplayTimelineVM.PlaybackEnabled)
			{
				bool driverSelected = ReplayTimelineVM.CurrentDriver != null;
				bool cameraSelected = ReplayTimelineVM.CurrentCamera != null;

				bool timelineNodeSelected = ReplayTimelineVM.CurrentTimelineNode != null;

				if (timelineNodeSelected)
				{
					return ReplayTimelineVM.CurrentDriver != ReplayTimelineVM.CurrentTimelineNode.Driver
						|| ReplayTimelineVM.CurrentCamera != ReplayTimelineVM.CurrentTimelineNode.Camera;
				}
				else
				{
					return driverSelected && cameraSelected;
				}
			}

			return false;
		}

		public void Execute(object parameter)
		{
			ReplayTimelineVM.StoreCurrentFrame();
		}
	}
}
