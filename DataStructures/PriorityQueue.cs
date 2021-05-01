using System;
using System.Collections.Generic;

namespace Bloops.GridFramework.DataStructures
{
	//PResumabely this commented out version, modified from here: http://xfleury.github.io/graphsearch.html is faster.
	
	// public class PriorityQueue<T> {
	// 	private List<T> items = new List<T>();
	// 	private Dictionary<T, int> costs = new Dictionary<T, int>();
	// 	public int Count { get { return items.Count; } }
	// 	public void Clear() {
	// 		items.Clear();
	// 		costs.Clear();
	// 	}
	// 	public void Enqueue(T item, int cost)
	// 	{
	// 		costs[item] = cost;
	// 		int i = items.Count;
	// 		items.Add(item);
	// 		while (i > 0 && costs[items[(i - 1) / 2]].CompareTo(costs[item]) > 0) {
	// 			items[i] = items[(i - 1) / 2];
	// 			i = (i - 1) / 2;
	// 		}
	// 		items[i] = item;
	// 	}
	// 	public T Peek() { return items[0]; }
	// 	public T Dequeue() {
	// 		T firstItem = items[0];
	// 		T tempItem = items[items.Count - 1];
	// 		items.RemoveAt(items.Count - 1);
	// 		if (items.Count > 0) {
	// 			int i = 0;
	// 			while (i < items.Count / 2) {
	// 				int j = (2 * i) + 1;
	// 				if ((j < items.Count - 1) && (costs[items[j]].CompareTo(costs[items[j + 1]]) > 0)) ++j;
	// 				if (costs[items[j]].CompareTo(costs[tempItem]) >= 0) break;
	// 				items[i] = items[j];
	// 				i = j;
	// 			}
	// 			items[i] = tempItem;
	// 		}
	// 		return firstItem;
	// 	}
	// }
	public class PriorityQueue<T>
	{
		// I'm using an unsorted array for this example, but ideally this
		// would be a binary heap. There's an open issue for adding a binary
		// heap to the standard C# library: https://github.com/dotnet/corefx/issues/574
		//
		private List<Tuple<T, double>> elements = new List<Tuple<T, double>>();

		public int Count
		{
			get { return elements.Count; }
		}
    
		public void Enqueue(T item, double priority)
		{
			elements.Add(Tuple.Create(item, priority));
		}

		public T Dequeue()
		{
			int bestIndex = 0;

			for (int i = 0; i < elements.Count; i++) {
				if (elements[i].Item2 < elements[bestIndex].Item2) {
					bestIndex = i;
				}
			}

			T bestItem = elements[bestIndex].Item1;
			elements.RemoveAt(bestIndex);
			return bestItem;
		}
	}
}