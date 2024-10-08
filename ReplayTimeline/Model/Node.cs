﻿using System.ComponentModel;


namespace iRacingReplayDirector
{
	[System.Diagnostics.DebuggerDisplay("Frame #{Frame} - {NodeDetails}")]
	public abstract class Node : INotifyPropertyChanged
	{
		private bool _enabled = true;
		public bool Enabled
		{
			get { return _enabled; }
			set { _enabled = value; OnPropertyChanged("Enabled"); }
		}

		private int _frame;
		public int Frame
		{
			get { return _frame; }
			set { _frame = value; OnPropertyChanged("Frame"); }
		}


		private Node _prevNode;
		public Node PreviousNode
		{
			get { return _prevNode; }
			set { _prevNode = value; UpdateLabel(); }
		}

		private Node _nextNode;
		public Node NextNode
		{
			get { return _nextNode; }
			set { _nextNode = value; UpdateLabel(); }
		}


		private string _nodeDetails;
		public string NodeDetails
		{
			get { return _nodeDetails; }
			set { _nodeDetails = value; OnPropertyChanged("NodeDetails"); }
		}

		private string _nodeDetailsAdditional;
		public string NodeDetailsAdditional
		{
			get { return _nodeDetailsAdditional; }
			set { _nodeDetailsAdditional = value; OnPropertyChanged("NodeDetailsAdditional"); }
		}


		public abstract string NodeType { get; }

		public abstract void ApplyNode();
		protected abstract void UpdateLabel();


		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
