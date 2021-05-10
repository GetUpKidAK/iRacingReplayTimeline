using System.Collections.Generic;


namespace ReplayTimeline
{
	public class TimelineProject
	{
		public int SessionID { get; set; }
		public List<TimelineNode> TimelineNodes { get; set; }


		public TimelineProject()
		{
			TimelineNodes = new List<TimelineNode>();
		}
	}
}
