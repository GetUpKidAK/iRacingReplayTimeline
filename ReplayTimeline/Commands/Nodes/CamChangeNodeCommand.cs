using System;
using System.Linq;
using System.Windows.Input;


namespace iRacingReplayDirector
{
	public class CamChangeNodeCommand : ICommand
	{
		public ReplayDirectorVM ReplayDirectorVM { get; set; }

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


		public CamChangeNodeCommand(ReplayDirectorVM vm)
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

					bool timelineNodeSelected = ReplayDirectorVM.CurrentNode != null;

					if (timelineNodeSelected)
					{
						if (ReplayDirectorVM.CurrentNode is CamChangeNode)
						{
							var currentNode = ReplayDirectorVM.CurrentNode as CamChangeNode;

							return ReplayDirectorVM.CurrentDriver != currentNode.Driver
								|| ReplayDirectorVM.CurrentCamera != currentNode.Camera;
						}
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
			if (ReplayDirectorVM.CurrentNode != null)
			{
				if (ReplayDirectorVM.CurrentNode is CamChangeNode)
				{
					var currentNode = ReplayDirectorVM.CurrentNode as CamChangeNode;

					currentNode.Driver = ReplayDirectorVM.CurrentDriver;
					currentNode.Camera = ReplayDirectorVM.CurrentCamera;

					ReplayDirectorVM.SaveProjectChanges();
				}
			}
			else
			{
				// TODO: Check this logic. Checking if a node exists already? Seems pointless
				var timelineFrames = ReplayDirectorVM.Nodes.NodeList.Select(n => n.Frame).ToList();
				Node storedNode = null;

				if (!timelineFrames.Contains(ReplayDirectorVM.CurrentFrame))
				{
					CamChangeNode newNode = new CamChangeNode(ReplayDirectorVM.CurrentFrame, ReplayDirectorVM.CurrentDriver, ReplayDirectorVM.CurrentCamera);

					ReplayDirectorVM.Nodes.AddNode(newNode);
					storedNode = newNode;

					ReplayDirectorVM.SaveProjectChanges();
				}

				ReplayDirectorVM.CurrentNode = storedNode;
			}
		}
	}
}
