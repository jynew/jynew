using UnityEngine;

namespace MeshCombineStudio
{
    public class FastIndexList<T> : FastList<T>, IFastIndexList where T : IFastIndex
    {
        public FastIndexList()
        {
            items = new T[defaultCapacity];
        }

        public FastIndexList(int capacity)
        {
            items = new T[capacity];
        }

        new public void Clear()
        {
            for (int i = 0; i < _count; i++)
            {
                items[i].ListIndex = -1;
                items[i].List = null;
                items[i] = default(T);
            }
            Count = _count = 0;
        }

        public void SetItem(int index, T item)
        {
            if (item.List != null) { Debug.LogError("Is already in another list!"); return; }
            if (index >= items.Length) SetCapacity(index * 2);
            else if (index >= _count) _count = Count = index + 1;

            items[index] = item;
            item.ListIndex = index;
            item.List = this;
        } 

        new public int Add(T item)
        {
            var list = item.List;
            if (list == this) { Debug.LogError("Item is already in this list"); return item.ListIndex; }
            if (list != null) { Debug.LogError("Is already in another list!"); return -1; }
            if (item.ListIndex != -1) { Debug.Log("Item already added"); return -1; }

            if (_count == items.Length) DoubleCapacity();

            items[_count] = item;
            item.ListIndex = _count++;
            item.List = this;
            Count = _count;
            return _count - 1;
        }

        new public void AddRange(T[] newItems)
        {
            int newCount = _count + newItems.Length;
            if (newCount >= items.Length) SetCapacity(newCount * 2);

            for (int i = 0; i < newItems.Length; i++)
            {
                if (newItems[i].List != null) { Debug.LogError("Is already in another list!"); continue; }
                if (newItems[i].ListIndex != -1) { Debug.Log("Item already added"); continue; }

                items[_count] = newItems[i];
                newItems[i].ListIndex = _count++;
                newItems[i].List = this;
            }
            Count = _count;
        }

        new public bool RemoveAt(int index)
        {
            if (index >= _count) { Debug.LogError("Index " + index + " is out of range. List count is " + _count); return false; }

            T item = items[index];

            if (item.ListIndex == -1) { Debug.Log("Item already removed"); return false; }

            items[index] = items[--_count];
            items[index].ListIndex = index;
            items[_count] = default(T);
            item.ListIndex = -1;
            item.List = null;
            Count = _count;
            return true;
        }

        public override T Dequeue()
        {
            if (_count == 0) return default(T);

            T item = items[--_count];

            items[_count] = default(T);
            item.ListIndex = -1;
            item.List = null;
            Count = _count;
            return item;
        }

        public bool Remove(IFastIndex item)
        {
            if (item == null || item.List != this) return false;// Debug.LogError("Item is not an element of this list"); return; }

            int listIndex = item.ListIndex;
            if (listIndex == -1) { Debug.Log("Item already removed"); return false; }

            items[listIndex] = items[--_count];
            items[listIndex].ListIndex = listIndex;
            items[_count] = default(T);
            item.ListIndex = -1;
            item.List = null;
            Count = _count;

            return true;
        }

        //new public void RemoveRange(int startIndex, int length, int threadId = 0)
        //{
        //    if (startIndex + length > _count) { Debug.LogError("StartIndex " + startIndex + " Length "+length+" is out of range. List count is " + _count); return; }

        //    int minIndex = startIndex + length;
        //    int copyIndex = _count - length;

        //    if (copyIndex < minIndex) copyIndex = minIndex;

        //    int length2 = _count - copyIndex;
        //    int index = startIndex;

        //    // Debug.Log("CopyIndex " + copyIndex + " length2 " + length2 + " rest " + (length - length2));

        //    for (int i = 0; i < length2; i++)
        //    {
        //        if (items[index] == null) { Debug.LogError("Thread " + threadId + " item list " + (index) + " is null! List count " + _count); }
        //        if (items[index].__listIndex != -1)
        //        {
        //            items[index].__listIndex = -1;
        //            items[index] = items[copyIndex + i];
        //            items[index].__listIndex = index++;
        //            items[copyIndex + i] = default(T);
        //            --_count;
        //        }
        //        else Debug.Log("Item already removed");
        //    }

        //    length -= length2;

        //    for (int i = 0; i < length; i++)
        //    {
        //        if (items[index].__listIndex != -1)
        //        {
        //            items[index].__listIndex = -1;
        //            items[index++] = default(T);
        //            --_count;
        //        }
        //        else Debug.Log("Item already removed");
        //    }

        //    Count = _count;
        //}
    }
}