using System;
using System.Linq;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class StoreCurrentFrameCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public StoreCurrentFrameCommand(ReplayDirectorVM vm)
		{
			ReplayDirectorVM = vm;
		}

		public bool CanExecute(object parameter)
		{
			if (ReplayDirectorVM.SessionInfoLoaded)
			{
				if (!ReplayDirectorVM.PlaybackEnabled)
				{
					bool driverSelected = ReplayDirectorVM.CurrentDriver != null;
					bool cameraSelected = ReplayDirectorVM.CurrentCamera != null;

					bool timelineNodeSelected = ReplayDirectorVM.CurrentTimelineNode != null;

					if (timelineNodeSelected)
					{
						return ReplayDirectorVM.CurrentDriver != ReplayDirectorVM.CurrentTimelineNode.Driver
							|| ReplayDirectorVM.CurrentCamera != ReplayDirectorVM.CurrentTimelineNode.Camera;
					}
					else
					{
						return driverSelected && cameraSelected;
					}
				}
			}

			return false;
		}

		public void Execute(object parameter)
		{
			if (ReplayDirectorVM.CurrentTimelineNode != null)
			{
				ReplayDirectorVM.CurrentTimelineNode.Driver = ReplayDirectorVM.CurrentDriver;
				ReplayDirectorVM.CurrentTimelineNode.Camera = ReplayDirectorVM.CurrentCamera;

				ReplayDirectorVM.SaveProjectChanges();
			}
			else
			{
				var timelineFrames = ReplayDirectorVM.TimelineNodes.Select(n => n.Frame).ToList();
				TimelineNode storedNode = null;

				if (!timelineFrames.Contains(ReplayDirectorVM.CurrentFrame))
				{
					TimelineNode newNode = new TimelineNode();
					newNode.Frame = ReplayDirectorVM.CurrentFrame;
					newNode.Driver = ReplayDirectorVM.CurrentDriver;
					newNode.Camera = ReplayDirectorVM.CurrentCamera;

					ReplayDirectorVM.TimelineNodes.Add(newNode);
					storedNode = newNode;

					ReplayDirectorVM.SaveProjectChanges();
				}

				ReplayDirectorVM.CurrentTimelineNode = storedNode;
			}
		}
	}
}
