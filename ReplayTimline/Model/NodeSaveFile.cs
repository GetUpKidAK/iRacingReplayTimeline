

namespace ReplayTimeline
{
	public class NodeSaveFile
	{
		public bool Enabled { get; set; }
		public int Frame { get; set; }
		public int DriverNumber { get; set; }
		public string CameraName { get; set; }

		public NodeSaveFile(bool enabled, int frame, int driverNumber, string cameraName)
		{
			Enabled = enabled;
			Frame = frame;
			DriverNumber = driverNumber;
			CameraName = cameraName;
		}
	}
}
