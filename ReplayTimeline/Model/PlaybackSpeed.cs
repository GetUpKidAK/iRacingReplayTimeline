using iRacingSimulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRacingReplayDirector
{
	public class PlaybackSpeed : IEquatable<PlaybackSpeed>
	{
		public string SpeedLabel { get; set; }
		public int SpeedValue { get; set; }
		public bool SlowMotion { get; set; }

		public PlaybackSpeed(string speedLabel, int speedValue, bool slowMo)
		{
			SpeedLabel = speedLabel;
			SpeedValue = speedValue;
			SlowMotion = slowMo;
		}


		public override bool Equals(object obj) => this.Equals(obj as PlaybackSpeed);

		public bool Equals(PlaybackSpeed other)
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
			return (SpeedLabel == other.SpeedLabel);
		}

		public override int GetHashCode() => (SpeedLabel).GetHashCode();

		public static bool operator ==(PlaybackSpeed lhs, PlaybackSpeed rhs)
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

		public static bool operator !=(PlaybackSpeed lhs, PlaybackSpeed rhs) => !(lhs == rhs);
	}
}
