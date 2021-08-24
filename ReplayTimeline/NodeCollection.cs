using System.Collections.ObjectModel;
using System.Linq;


namespace iRacingReplayDirector
{
	public class NodeCollection
	{
		public ObservableCollection<Node> NodeList { get; private set; }

		public bool NodesListOccupied { get { return NodeList.Count > 0; } }

		public NodeCollection()
		{
			NodeList = new ObservableCollection<Node>();
		}

		public void AddNode(Node newNode)
		{
			Node current = GetCurrentActiveNode(newNode.Frame);

			if (current == null)
			{
				Prepend(newNode);
				return;
			}

			// Chain events
			current.NextNode = newNode;
			newNode.PreviousNode = current;

			// Insert in collection
			int index = NodeList.IndexOf(current) + 1;
			NodeList.Insert(index, newNode);
		}

		public bool RemoveNode(Node nodeToRemove)
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

			return NodeList.Remove(nodeToRemove);
		}

		public void RemoveAllNodes()
		{
			NodeList.Clear();
		}

		public Node GetNodeOnCurrentFrame(int currentFrame)
		{
			return NodeList.LastOrDefault(node => node.Frame == currentFrame);
		}

		public Node GetCurrentActiveNode(int currentFrame)
		{
			return NodeList.LastOrDefault(e => e.Frame <= currentFrame);
		}

		private void Prepend(Node transition)
		{
			Node first = NodeList.FirstOrDefault();
			NodeList.Insert(0, transition);

			if (first != null)
			{
				first.PreviousNode = transition;
				transition.NextNode = first;
			}
		}
	}
}
