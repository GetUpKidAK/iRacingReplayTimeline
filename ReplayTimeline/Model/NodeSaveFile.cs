

using Newtonsoft.Json;

namespace iRacingReplayDirector
{
	public class NodeSaveFile
	{
		public bool Enabled { get; set; }
		public int Frame { get; set; }
		public NodeType NodeType { get; set; }
		public int PlaybackSpeed { get; set; } // Currently unused
		public int DriverNumber { get; set; }
		public string CameraName { get; set; }


		public NodeSaveFile(bool enabled, int frame, NodeType nodeType)
		{
			Enabled = enabled;
			Frame = frame;
			NodeType = nodeType;
		}

		[JsonConstructor]
		public NodeSaveFile(bool enabled, int frame, NodeType nodeType, int playbackSpeed, int driverNumber, string cameraName) : this(enabled, frame, nodeType)
		{
			PlaybackSpeed = playbackSpeed;
			DriverNumber = driverNumber;
			CameraName = cameraName;
		}
	}

	public enum NodeType
	{
		CamChange,
		FrameSkip
	}
}
