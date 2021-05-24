using System.Collections.Generic;


namespace ReplayTimeline
{
	public class TimelineProject
	{
		public int SessionID { get; set; }
		public List<NodeSaveFile> Nodes { get; set; }


		public TimelineProject()
		{
			Nodes = new List<NodeSaveFile>();
		}
	}
}
