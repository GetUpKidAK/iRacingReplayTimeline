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
