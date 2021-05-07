using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ReplayTimeline
{
	public class TimelineProject
	{
		public int SessionID { get; set; }
		public List<TimelineNode> TimelineNodes { get; set; }


		public TimelineProject()
		{

		}
	}
}
