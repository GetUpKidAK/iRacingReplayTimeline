using System;
using System.ComponentModel;
using System.Diagnostics;


namespace ReplayTimeline
{
	[DebuggerDisplay("{ToString()}")]
	public class Camera : INotifyPropertyChanged, IEquatable<Camera>
	{
		private int _groupNum;
		public int GroupNum
		{
			get { return _groupNum; }
			set { _groupNum = value; OnPropertyChanged("GroupNum"); }
		}

		private string _groupName;
		public string GroupName
		{
			get { return _groupName; }
			set { _groupName = value; OnPropertyChanged("GroupName"); }
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));



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
			return (GroupName == other.GroupName);
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

		public override string ToString()
		{
			return $"{GroupName} #{GroupNum}";
		}
	}
}
