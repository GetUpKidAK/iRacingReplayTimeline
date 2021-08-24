using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;


namespace iRacingReplayDirector
{
	public class NodeCollection
	{
		public ObservableCollection<Node> Nodes { get; private set; }

		public int SessionID { get; private set; }
		public bool NodesListOccupied { get { return Nodes.Count > 0; } }


		public NodeCollection()
		{
			Nodes = new ObservableCollection<Node>();

			Nodes.CollectionChanged += CollectionChanged;
		}

		private void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (INotifyPropertyChanged added in e.NewItems)
				{
					added.PropertyChanged += TimelineNodesPropertyChanged;
				}
			}

			if (e.OldItems != null)
			{
				foreach (INotifyPropertyChanged removed in e.OldItems)
				{
					removed.PropertyChanged -= TimelineNodesPropertyChanged;
				}
			}
		}

		private void TimelineNodesPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{
			if (propertyChangedEventArgs.PropertyName == "Enabled")
			{
				SaveNodeChanges();
			}
		}

		public void SetSessionId(int sessionId)
		{
			SessionID = sessionId;
		}


		public void AddNode(Node newNode)
		{
			Node previouslyActiveNode = GetCurrentActiveNode(newNode.Frame);

			if (previouslyActiveNode == null)
			{
				Prepend(newNode);
			}
			else
			{
				// Chain events
				newNode.NextNode = previouslyActiveNode.NextNode;
				newNode.PreviousNode = previouslyActiveNode;
				if (newNode.NextNode != null) newNode.NextNode.PreviousNode = newNode;

				previouslyActiveNode.NextNode = newNode;

				// Insert in collection
				int index = Nodes.IndexOf(previouslyActiveNode) + 1;
				Nodes.Insert(index, newNode);
			}

			SaveNodeChanges();
		}

		public void RemoveNode(Node nodeToRemove)
		{
			Node prev = nodeToRemove.PreviousNode;
			Node next = nodeToRemove.NextNode;

			if (prev != null)
			{
				prev.NextNode = nodeToRemove.NextNode;
			}

			if (next != null)
			{
				next.PreviousNode = nodeToRemove.PreviousNode;
			}

			Nodes.Remove(nodeToRemove);
			SaveNodeChanges();
		}

		public void RemoveAllNodes()
		{
			Nodes.Clear();

			SaveNodeChanges();
		}

		public Node GetNodeOnCurrentFrame(int currentFrame)
		{
			return Nodes.LastOrDefault(node => node.Frame == currentFrame);
		}

		public Node GetCurrentActiveNode(int currentFrame)
		{
			return Nodes.LastOrDefault(e => e.Frame <= currentFrame);
		}

		private void Prepend(Node transition)
		{
			Node first = Nodes.FirstOrDefault();
			Nodes.Insert(0, transition);

			if (first != null)
			{
				first.PreviousNode = transition;
				transition.NextNode = first;
			}
		}

		public void SaveNodeChanges()
		{
			SaveLoadHelper.SaveProject(Nodes.ToList(), SessionID);
		}
	}
}
