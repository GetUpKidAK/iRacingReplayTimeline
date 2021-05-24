using System;
using System.Diagnostics;

namespace ReplayTimeline
{
	[DebuggerDisplay("{ToString()}")]
	public class Driver : IEquatable<Driver>
	{
		public Driver()
		{
		}

		/// <summary>
		/// The identifier (CarIdx) of this driver (unique to this session)
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// The current position of the driver
		/// </summary>
		public int Position { get; set; }

		/// <summary>
		/// The name of the driver
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The customer ID (custid) of the driver
		/// </summary>
		public int CustomerId { get; set; }

		/// <summary>
		/// The team name (TeamName) for the driver
		/// </summary>
		public string TeamName { get; set; }

		/// <summary>
		/// The car number of this driver
		/// </summary>
		public string Number { get; set; }

		/// <summary>
		/// The stripped-back car number of this driver
		/// </summary>
		public int NumberRaw { get; set; }

		/// <summary>
		/// A unique identifier for the car class this driver is using
		/// </summary>
		public int ClassId { get; set; }

		/// <summary>
		/// The name of the car of this driver
		/// </summary>
		public string CarPath { get; set; }

		/// <summary>
		/// The relative speed of this class in a multiclass session
		/// </summary>
		public int CarClassRelSpeed { get; set; }

		/// <summary>
		/// Used to determine if a driver is in the pits, off or on track
		/// </summary>
		public TrackSurfaces TrackSurface { get; set; }

		/// <summary>
		/// Whether or not the driver is currently in or approaching the pit stall
		/// </summary>
		public bool IsInPits
		{
			get { return this.TrackSurface == TrackSurfaces.AproachingPits || this.TrackSurface == TrackSurfaces.InPitStall; }
		}

		/// <summary>
		/// The lap this driver is currently in
		/// </summary>
		public int Lap { get; set; }

		/// <summary>
		/// The distance along the current lap of this driver (in percentage)
		/// </summary>
		public float LapDistance { get; set; }

		/// <summary>
		/// The iRating of this driver
		/// </summary>
		public int Rating { get; set; }


		public override bool Equals(object obj) => this.Equals(obj as Driver);

		public bool Equals(Driver other)
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
			return (CustomerId == other.CustomerId);
		}

		public override int GetHashCode() => (CustomerId).GetHashCode();

		public static bool operator ==(Driver lhs, Driver rhs)
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

		public static bool operator !=(Driver lhs, Driver rhs) => !(lhs == rhs);

		public override string ToString()
		{
			return $"{Name} #{Number}";
		}
	}
}
