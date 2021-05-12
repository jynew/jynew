/* TILE WORLD CREATOR
 * Copyright (c) 2015 doorfortyfour OG
 * 
 * Create awesome tile worlds in seconds.
 *
 *
 * Documentation: http://tileworldcreator.doofortyfour.com
 * Like us on Facebook: http://www.facebook.com/doorfortyfour2013
 * Web: http://www.doorfortyfour.com
 * Contact: mail@doorfortyfour.com Contact us for help, bugs or 
 * share your awesome work you've made with TileWorldCreator
*/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TileWorld
{
	[Serializable]
	public class TileWorldUndoStack<T> : LinkedList<T>
	{
		
		private const int _size = 10;
		
		public T Peek()
		{
			return this.Last.Value;
		}
		
		public T Pop()
		{
			LinkedListNode<T> node = this.Last;
			
			if (node != null)
			{
				this.RemoveLast();
				return node.Value;
			}
			else
			{
				return default(T);
			}
		}
		
		public void Push(T value)
		{
			LinkedListNode<T> node = new LinkedListNode<T>(value);
			
			this.AddLast(node);
			
			if (this.Count > _size)
			{
				this.RemoveFirst();
			}
		}
	}
}