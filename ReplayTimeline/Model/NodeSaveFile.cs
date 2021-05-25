

namespace iRacingReplayDirector
{
	public class NodeSaveFile
	{
		public bool Enabled { get; set; }
		public int Frame { get; set; }
		public int PlaybackSpeed { get; set; } // Currently unused
		public int DriverNumber { get; set; }
		public string CameraName { get; set; }

		public NodeSaveFile(bool enabled, int frame, int playbackSpeed, int driverNumber, string cameraName)
		{
			Enabled = enabled;
			Frame = frame;
			PlaybackSpeed = playbackSpeed;
			DriverNumber = driverNumber;
			CameraName = cameraName;
		}
	}
}
