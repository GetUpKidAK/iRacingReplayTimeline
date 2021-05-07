using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplayTimeline
{
	public class Camera : IEquatable<Camera>
	{
		public int GroupNum { get; set; }
		public string GroupName { get; set; }


		public override bool Equals(object obj) => this.Equals(obj as Camera);

		public bool Equals(Camera other)
		{
			if (other is null)
			{
				return false;
			}

			// Optimization for a common success case.
			if (Object.ReferenceEquals(this, other))
			{
				return true;
			}

			// If run-time types are not exactly the same, return false.
			if (this.GetType() != other.GetType())
			{
				return false;
			}

			// Return true if the fields match.
			// Note that the base class is not invoked because it is
			// System.Object, which defines Equals as reference equality.
			return (GroupName == other.GroupName && GroupNum == other.GroupNum);
		}

		public override int GetHashCode() => (GroupName).GetHashCode();

		public static bool operator ==(Camera lhs, Camera rhs)
		{
			if (lhs is null)
			{
				if (rhs is null)
				{
					return true;
				}

				// Only the left side is null.
				return false;
			}
			// Equals handles case of null on right side.
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Camera lhs, Camera rhs) => !(lhs == rhs);
	}
}
