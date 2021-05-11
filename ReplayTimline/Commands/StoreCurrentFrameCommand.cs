using System;
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
			bool timelineNodeSelected = ReplayTimelineVM.CurrentTimelineNode != null;

			bool driverSelected = ReplayTimelineVM.CurrentDriver != null;
			bool cameraSelected = ReplayTimelineVM.CurrentCamera != null;

			if (timelineNodeSelected)
			{
				return ReplayTimelineVM.CurrentDriver == ReplayTimelineVM.CurrentTimelineNode.Driver
					&& ReplayTimelineVM.CurrentCamera == ReplayTimelineVM.CurrentTimelineNode.Camera;
			}
			else
			{
				return driverSelected && cameraSelected;
			}
		}

		public void Execute(object parameter)
		{
			ReplayTimelineVM.StoreCurrentFrame();
		}
	}
}
