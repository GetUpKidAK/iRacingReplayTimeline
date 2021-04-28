using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using iRacingSdkWrapper;


namespace ReplayTimeline
{
	public class ReplayTimelineVM : INotifyPropertyChanged
	{
		private SdkWrapper m_Wrapper;

		public ObservableCollection<TimelineNode> TimelineNodes { get; set; }
		public ObservableCollection<Driver> Drivers { get; set; }
		public ObservableCollection<Camera> Cameras { get; set; }


		private TimelineNode _currentTimelineNode;
		public TimelineNode CurrentTimelineNode
		{
			get { return _currentTimelineNode; }
			set { _currentTimelineNode = value; OnPropertyChanged("CurrentTimelineNode"); }
		}

		private Driver _currentDriver;
		public Driver CurrentDriver
		{
			get { return _currentDriver; }
			set { _currentDriver = value; OnPropertyChanged("CurrentDriver"); DriverChanged(); }
		}

		private Camera _currentCamera;
		public Camera CurrentCamera
		{
			get { return _currentCamera; }
			set { _currentCamera = value; OnPropertyChanged("CurrentCamera"); CameraChanged(); }
		}

		private int _currentFrame;
		public int CurrentFrame
		{
			get { return _currentFrame; }
			set { _currentFrame = value; OnPropertyChanged("CurrentFrame"); }
		}

		private int m_CurrentPlaybackSpeed;
		private int CurrentPlaybackSpeed
		{
			get => m_CurrentPlaybackSpeed;
			set
			{
				m_CurrentPlaybackSpeed = value;
				if (m_CurrentPlaybackSpeed > 16) m_CurrentPlaybackSpeed = 16;
				else if (m_CurrentPlaybackSpeed < -16) m_CurrentPlaybackSpeed = -16;
				UpdatePlaybackButtonText();
			}
		}

		private string _playPauseBtnText;
		public string PlayPauseBtnText
		{
			get { return _playPauseBtnText; }
			set { _playPauseBtnText = value; OnPropertyChanged("PlayPauseBtnText"); }
		}

		private string _fastForwardBtnText;
		public string FastForwardBtnText
		{
			get { return _fastForwardBtnText; }
			set { _fastForwardBtnText = value; OnPropertyChanged("FastForwardBtnText"); }
		}

		private string _rewindBtnText;
		public string RewindBtnText
		{
			get { return _rewindBtnText; }
			set { _rewindBtnText = value; OnPropertyChanged("RewindBtnText"); }
		}

		public StoreCurrentFrameCommand StoreCurrentFrameCommand { get; set; }
		public PreviousStoredFrameCommand PreviousStoredFrameCommand { get; set; }
		public NextStoredFrameCommand NextStoredFrameCommand { get; set; }
		public PlayPauseCommand PlayPauseCommand { get; set; }
		public RewindCommand RewindCommand { get; set; }
		public FastForwardCommand FastForwardCommand { get; set; }

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

			PlayPauseCommand = new PlayPauseCommand(this);
			RewindCommand = new RewindCommand(this);
			FastForwardCommand = new FastForwardCommand(this);

			TestCommand = new TestCommand(this);
		}

		private void TelemetryUpdated(object sender, SdkWrapper.TelemetryUpdatedEventArgs e)
		{
			m_TelemetryCache = e.TelemetryInfo;

			CurrentFrame = e.TelemetryInfo.ReplayFrameNum.Value;
			CurrentPlaybackSpeed = e.TelemetryInfo.ReplayPlaySpeed.Value;
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

		public void PlayPauseToggle()
		{
			if (CurrentPlaybackSpeed == 0)
			{
				CurrentPlaybackSpeed = 1;

				ChangePlaybackSpeed();
			}
			else
			{
				CurrentPlaybackSpeed = 0;

				ChangePlaybackSpeed();
			}
		}

		public void RewindPlayback()
		{
			if (CurrentPlaybackSpeed < 0)
			{
				CurrentPlaybackSpeed *= 2;
			}
			else
			{
				CurrentPlaybackSpeed = -1;
			}

			ChangePlaybackSpeed();
		}

		public void FastForwardPlayback()
		{
			if (CurrentPlaybackSpeed > 0)
			{
				CurrentPlaybackSpeed *= 2;
			}
			else
			{
				CurrentPlaybackSpeed = 2;
			}

			ChangePlaybackSpeed();
		}

		private void ChangePlaybackSpeed()
		{
			m_Wrapper.Replay.SetPlaybackSpeed(CurrentPlaybackSpeed);
		}

		private void UpdatePlaybackButtonText()
		{
			PlayPauseBtnText = CurrentPlaybackSpeed != 0 ? "Pause" : "Play";

			if (CurrentPlaybackSpeed > 0)
			{
				RewindBtnText = "<<";
				FastForwardBtnText = ">>";

				if (CurrentPlaybackSpeed > 1)
				{
					FastForwardBtnText = $"{CurrentPlaybackSpeed}x";
				}
			}
			else
			{
				RewindBtnText = "<<";
				FastForwardBtnText = ">>";

				if (CurrentPlaybackSpeed < -1)
				{
					RewindBtnText = $"{-CurrentPlaybackSpeed}x";
				}
			}
		}

		private void DriverChanged()
		{
			if (CurrentDriver == null)
				return;

			m_Wrapper.Camera.SwitchToCar(CurrentDriver.Id);
		}

		private void CameraChanged()
		{
			if (CurrentCamera == null)
				return;

			if (CurrentDriver == null)
				m_Wrapper.Camera.SwitchGroup(CurrentCamera.GroupNum);
			else
			{
				m_Wrapper.Camera.SwitchToCar(CurrentDriver.Id, CurrentCamera.GroupNum);
			}
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

				CurrentTimelineNode = newNode;
			}

			// Simple version of sorting for now.
			var sortedTimeline = TimelineNodes.OrderBy(n => n.Frame).ToList();
			var cachedNode = CurrentTimelineNode;

			TimelineNodes.Clear();

			foreach (var node in sortedTimeline)
			{
				TimelineNodes.Add(node);
			}

			CurrentTimelineNode = cachedNode;
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
					CurrentTimelineNode = targetNode;
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
					CurrentTimelineNode = targetNode;
					m_Wrapper.Replay.SetPosition(targetNode.Frame);
				}
			}
		}
	}
}
