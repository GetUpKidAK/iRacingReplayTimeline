using System.ComponentModel;


namespace iRacingReplayDirector
{
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

		private string _nodeLabel;
		public string NodeLabel
		{
			get { return _nodeLabel; }
			set { _nodeLabel = value; OnPropertyChanged("NodeLabel"); }
		}

		public Node PreviousNode;
		public Node NextNode;

		//TODO: CONSTRUCTOR??

		public abstract void ApplyNode();


		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
