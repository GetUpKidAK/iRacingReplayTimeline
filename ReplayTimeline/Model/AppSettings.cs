

namespace iRacingReplayDirector
{
	public class AppSettings
	{
		public bool ShowVisualTimeline { get; set; } = true;
		public bool ShowRecordingButtons { get; set; } = true;
		public bool ShowSessionLapSkipButtons { get; set; } = true;
		public bool ShowDriverCameraPanels { get; set; } = true;
		public bool ShowTimelineNodeList { get; set; } = true;

		public bool DisableSimUIOnPlayback { get; set; } = false;
		public bool DisableUIWhenRecording { get; set; } = true;
		public bool StopRecordingOnFinalNode { get; set; } = false;

		public bool UseInSimCapture { get; set; } = true;
		public bool UseOBSCapture { get; set; } = false;


		public AppSettings()
		{
			
		}
	}
}
