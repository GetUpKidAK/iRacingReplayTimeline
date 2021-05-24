using System.Collections.Generic;


namespace iRacingReplayDirector
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
