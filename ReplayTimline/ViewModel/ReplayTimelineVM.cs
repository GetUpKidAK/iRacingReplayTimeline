using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using iRacingSdkWrapper;


namespace ReplayTimline
{
	public class ReplayTimelineVM : INotifyPropertyChanged
	{
		private SdkWrapper m_Wrapper;

		public ObservableCollection<TimelineNode> TimelineNodes { get; set; }
		public ObservableCollection<Driver> Drivers { get; set; }
		public ObservableCollection<Camera> Cameras { get; set; }

		private Driver _currentDriver;
		public Driver CurrentDriver
		{
			get { return _currentDriver; }
			set { _currentDriver = value; OnPropertyChanged("CurrentDriver"); }
		}

		private Camera _currentCamera;
		public Camera CurrentCamera
		{
			get { return _currentCamera; }
			set { _currentCamera = value; OnPropertyChanged("CurrentCamera"); }
		}

		private int _currentFrame;
		public int CurrentFrame
		{
			get { return _currentFrame; }
			set { _currentFrame = value; OnPropertyChanged("CurrentFrame"); }
		}

		public StoreCurrentFrameCommand StoreCurrentFrameCommand { get; set; }
		public PreviousStoredFrameCommand PreviousStoredFrameCommand { get; set; }
		public NextStoredFrameCommand NextStoredFrameCommand { get; set; }

		public TestCommand TestCommand { get; set; }


		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


		private TelemetryInfo m_TelemetryCache;


		public ReplayTimelineVM()
		{
			m_Wrapper = new SdkWrapper();

			m_Wrapper.TelemetryUpdated += TelemetryUpdated;
			m_Wrapper.SessionInfoUpdated += SessionInfoUpdated;

			m_Wrapper.Start();

			TimelineNodes = new ObservableCollection<TimelineNode>();
			Drivers = new ObservableCollection<Driver>();
			Cameras = new ObservableCollection<Camera>();

			StoreCurrentFrameCommand = new StoreCurrentFrameCommand(this);
			NextStoredFrameCommand = new NextStoredFrameCommand(this);
			PreviousStoredFrameCommand = new PreviousStoredFrameCommand(this);

			TestCommand = new TestCommand(this);
		}

		private void TelemetryUpdated(object sender, SdkWrapper.TelemetryUpdatedEventArgs e)
		{
			m_TelemetryCache = e.TelemetryInfo;

			CurrentFrame = m_TelemetryCache.ReplayFrameNum.Value;
		}

		private void SessionInfoUpdated(object sender, SdkWrapper.SessionInfoUpdatedEventArgs e)
		{
			ParseDrivers(e.SessionInfo);
			ParseCameras(e.SessionInfo);
		}

		private void ParseDrivers(SessionInfo sessionInfo)
		{
			// Number of starters (Live only)
			YamlQuery weekendOptionsQuery = sessionInfo["WeekendInfo"]["WeekendOptions"];
			var starters = weekendOptionsQuery["NumStarters"].GetValue();
			int currentStarters = int.Parse(weekendOptionsQuery["NumStarters"].GetValue());

			var newDrivers = new List<Driver>();

			for (int i = 0; i < currentStarters; i++)
			{
				YamlQuery query = sessionInfo["DriverInfo"]["Drivers"]["CarIdx", i];
				Driver newDriver;

				string driverName = query["UserName"].GetValue("");

				if (!string.IsNullOrEmpty(driverName) && driverName != "Pace Car")
				{
					// Get driver if driver is in previous list
					newDriver = Drivers.FirstOrDefault(d => d.Name == driverName);

					// If not...
					if (newDriver == null)
					{
						// Populate driver info
						newDriver = new Driver();
						newDriver.Id = i;
						newDriver.Name = driverName;
						newDriver.CustomerId = int.Parse(query["UserID"].GetValue("0")); // default value 0
						newDriver.Number = query["CarNumber"].GetValue("").TrimStart('\"').TrimEnd('\"'); // trim the quotes
						newDriver.Rating = int.Parse(query["IRating"].GetValue("0"));
					}

					// Add to drivers list
					newDrivers.Add(newDriver);
				}
			}

			// Cache current driver, needed for live sessions
			//var cachedCurrentDriver = CurrentDriver;
			// Replace old list of drivers with new list of drivers and update the grid
			Drivers.Clear();
			foreach (var newDriver in newDrivers)
			{
				Drivers.Add(newDriver);
			}
			// Replace previous driver once list is rebuilt.
			//CurrentDriver = cachedCurrentDriver;
		}

		private void ParseCameras(SessionInfo sessionInfo)
		{
			int id = 1;
			Camera newCam;

			var newCameras = new List<Camera>();

			// Loop through cameras until none are found anymore
			do
			{
				newCam = null;
				YamlQuery query = sessionInfo["CameraInfo"]["Groups"]["GroupNum", id];

				// Get Camera Group name
				string groupName = query["GroupName"].GetValue("");

				if (!string.IsNullOrEmpty(groupName))
				{
					// Get Camera if it is in previous list
					newCam = Cameras.FirstOrDefault(c => c.GroupName == groupName);

					// Otherwise...
					if (newCam == null)
					{
						// If group name is found, create a new camera
						newCam = new Camera();
						newCam.GroupNum = id;
						newCam.GroupName = groupName;
					}
					newCameras.Add(newCam);

					id++;
				}
			}
			while (newCam != null);

			// Cache current camera, needed for live sessions
			//var cachedCurrentCamera = CurrentCamera;

			// Replace old list of drivers with new list of drivers and update the grid
			Cameras.Clear();
			foreach (var newCamera in newCameras)
			{
				Cameras.Add(newCamera);
			}

			// Replace previous camera once list is rebuilt.
			//CurrentCamera = cachedCurrentCamera;
		}

		public void StoreCurrentFrame()
		{
			if (m_TelemetryCache == null)
				return;

			var timelineFrames = TimelineNodes.Select(n => n.Frame).ToList();

			if (!timelineFrames.Contains(CurrentFrame))
			{
				TimelineNode newNode = new TimelineNode();
				newNode.Frame = CurrentFrame;
				newNode.Driver = CurrentDriver;
				newNode.Camera = CurrentCamera;

				TimelineNodes.Add(newNode);
			}

			// Simple version of sorting for now.
			var sortedTimeline = TimelineNodes.OrderBy(n => n.Frame).ToList();

			TimelineNodes.Clear();

			foreach (var node in sortedTimeline)
			{
				TimelineNodes.Add(node);
			}
		}

		public void GoToPreviousStoredFrame()
		{
			if (m_TelemetryCache == null)
				return;

			if (TimelineNodes.Count > 0)
			{
				TimelineNode targetNode = null;

				for (int i = 0; i < TimelineNodes.Count; i++)
				{
					if (TimelineNodes[i].Frame < CurrentFrame)
					{
						targetNode = TimelineNodes[i];
					}
					else if (TimelineNodes[i].Frame >= CurrentFrame)
					{
						break;
					}
				}

				if (targetNode != null)
				{
					m_Wrapper.Replay.SetPosition(targetNode.Frame);
				}
			}
		}

		public void GoToNextStoredFrame()
		{
			if (m_TelemetryCache == null)
				return;

			if (TimelineNodes.Count > 0)
			{
				TimelineNode targetNode = null;

				for (int i = TimelineNodes.Count - 1; i >= 0; i--)
				{
					if (TimelineNodes[i].Frame > CurrentFrame)
					{
						targetNode = TimelineNodes[i];
					}
					else if (TimelineNodes[i].Frame <= CurrentFrame)
					{
						break;
					}
				}

				if (targetNode != null)
				{
					m_Wrapper.Replay.SetPosition(targetNode.Frame);
				}
			}
		}
	}
}
