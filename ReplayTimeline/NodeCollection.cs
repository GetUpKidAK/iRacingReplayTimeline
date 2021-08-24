using System.Collections.ObjectModel;
using System.Linq;


namespace iRacingReplayDirector
{
	public class NodeCollection
	{
		public ObservableCollection<Node> NodeList { get; private set; }

		public NodeCollection()
		{
			NodeList = new ObservableCollection<Node>();
		}

		public void Add(Node newNode)
		{
			Node current = GetCurrent(newNode.Frame);

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

		public bool Remove(Node oldNode)
		{
			Node prev = oldNode.PreviousNode;

			if (prev != null)
			{
				prev.NextNode = oldNode.NextNode;
			}

			return NodeList.Remove(oldNode);
		}

		public Node GetCurrent(int frameNum)
		{
			return NodeList.LastOrDefault(e => e.Frame <= frameNum);
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
