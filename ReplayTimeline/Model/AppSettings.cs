﻿

namespace iRacingReplayDirector
{
	public class AppSettings
	{
		public Window WindowSize { get; set; }
		public InterfaceOptions UIOptions { get; set; }
		public SimOptionsClass SimOptions { get; set; }

		public AppSettings() { }

		public bool SettingsAreValid()
		{
			return WindowSize != null && UIOptions != null && SimOptions != null;
		}


		public class Window
		{
			public int Width { get; set; }
			public int Height { get; set; }

			public Window(int width, int height)
			{
				Width = width;
				Height = height;
			}
		}

		public class InterfaceOptions
		{
			public bool WindowAlwaysOnTop { get; set; } = false;
			public bool ShowVisualTimeline { get; set; } = true;
			public bool ShowRecordingControls { get; set; } = true;
			public bool ShowSessionLapSkipControls { get; set; } = true;
			public bool FrameSkipInfoShown { get; set; } = false;


			public InterfaceOptions(bool windowAlwaysOnTop, bool showVisualTimeline,
				bool showRecordingControls, bool showSessionLapSkipControls, bool frameSkipInfoShown)
			{
				WindowAlwaysOnTop = windowAlwaysOnTop;
				ShowVisualTimeline = showVisualTimeline;
				ShowRecordingControls = showRecordingControls;
				ShowSessionLapSkipControls = showSessionLapSkipControls;
				FrameSkipInfoShown = frameSkipInfoShown;
			}
		}

		public class SimOptionsClass
		{
			public bool DisableSimUIOnPlayback { get; set; } = true;
			public bool DisableUIWhenRecording { get; set; } = true;
			public bool StopRecordingOnFinalNode { get; set; } = false;

			public bool UseInSimCapture { get; set; } = true;
			public bool UseOBSCapture { get; set; } = false;


			public SimOptionsClass(bool disableSimUIOnPlayback, bool disableUIWhenRecording,
				bool stopRecordingOnFinalNode, bool useInSimCapture, bool useOBSCapture)
			{
				DisableSimUIOnPlayback = disableSimUIOnPlayback;
				DisableUIWhenRecording = disableUIWhenRecording;
				StopRecordingOnFinalNode = stopRecordingOnFinalNode;
				UseInSimCapture = useInSimCapture;
				UseOBSCapture = useOBSCapture;
			}
		}
	}
}
