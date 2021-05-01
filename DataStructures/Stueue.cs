using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.EventSystems;

namespace Bloops.GridFramework.DataStructures
{
	//A Stueue is a Stack-Queue hybrid. 
	//Thanks Kasey for the naming suggestion.
	//and by 'thanks' i mean, this is your fault.
	/// <summary>
	/// A Wrapper for a list that both Stack (pop, peek) and Queue (enqueue,dequeue) functions.
	/// </summary>
	public class Stueue<T> : List<T>
	{
		int _maxSize = -1;
		public void SetMaxSize(int max)
		{
			_maxSize = max;
		}
		
		public void Push(T t)
		{
			if (Count >= _maxSize && _maxSize != -1)
			{
				base.RemoveAt(0);
			} 
			base.Add(t);
		}

		public void Enqueue(T t)
		{
			Push(t);
		}

		public T Dequeue()
		{
			T t = base[0];
			RemoveAt(0);
			return t;
		}

		public T Pop()
		{
			T t = base[Count-1];
			RemoveAt((Count-1));
			return t;
		}

		public T PeekTop()
		{
			if (Count > 0)
			{
				return base[Count - 1];
			}
			else
			{
				return default(T);
			}
		}
		public T PeekBottom()
		{
			if (Count > 0)
			{
				return base[0];
			}
			else
			{
				return default(T);
			}
		}
	}
}