using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshCombineStudio
{
    public class FastListBase
    {
        protected const int defaultCapacity = 4;
        public int Count;

        protected int _count;
        protected int arraySize;
    }

    public class FastListBase<T> : FastListBase
    {
        public T[] items;

        //public T this[int index]
        //{
        //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //    get { return items[index]; }
        //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //    set { items[index] = value; }
        //}

        protected void DoubleCapacity()
        {
            arraySize *= 2;
            T[] newItems = new T[arraySize];
            Array.Copy(items, newItems, _count);
            items = newItems;
        }
    }


    [Serializable]
    public class FastList<T> : FastListBase<T>
    {
        // static readonly T[] emptyArray = new T[0];

        // Constructors
        //=========================================================================================================================================*********
        //=========================================================================================================================================*********
        public FastList()
        {
            items = new T[defaultCapacity];
            arraySize = defaultCapacity;
        }

        public FastList(bool reserve, int reserved)
        {
            int capacity = Mathf.Max(reserved, defaultCapacity);
            items = new T[capacity];
            arraySize = capacity;
            Count = _count = reserved;
        }

        public FastList(int capacity)
        {
            if (capacity < 1) capacity = 1;
            items = new T[capacity];
            arraySize = capacity;
            // Debug.Log(items.Length);
        }

        public FastList(FastList<T> list)
        {
            if (list == null)
            {
                items = new T[defaultCapacity];
                arraySize = defaultCapacity;
                return;
            }

            items = new T[list.Count];
            Array.Copy(list.items, items, list.Count);
            arraySize = items.Length;
            Count = _count = items.Length;
        }

        public FastList(T[] items)
        {
            this.items = items;
            _count = Count = arraySize = items.Length;
        }
        //=========================================================================================================================================*********
        //=========================================================================================================================================*********

        protected void SetCapacity(int capacity)
        {
            arraySize = capacity;
            T[] newItems = new T[arraySize];
            if (_count > 0) Array.Copy(items, newItems, _count);
            items = newItems;
        }

        public void SetCount(int count)
        {
            if (count > arraySize) SetCapacity(count);
            Count = _count = count;
        }

        public void EnsureCount(int count)
        {
            if (count <= _count) return;

            if (count > arraySize) SetCapacity(count);
            Count = _count = count;
        }

        public virtual void SetArray(T[] items)
        {
            this.items = items;
            _count = Count = arraySize = items.Length;
        }

        public int AddUnique(T item)
        {
            if (!Contains(item)) return Add(item);
            return -1;
        }

        public bool Contains(T item)
        {
            return Array.IndexOf(items, item, 0, _count) != -1;
        }

        public int IndexOf(T item)
        {
            return Array.IndexOf(items, item, 0, _count);
        }

        public T GetIndex(T item)
        {
            int index = Array.IndexOf(items, item, 0, _count);
            if (index == -1) return default(T); else return items[index];
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual int Add(T item)
        {
            if (_count == arraySize) DoubleCapacity();

            items[_count] = item;
            Count = ++_count;
            return _count - 1;
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual int AddThreadSafe(T item)
        {
            lock (this)
            {
                if (_count == arraySize) DoubleCapacity();

                items[_count] = item;
                Count = ++_count;
                return _count - 1;
            }
        }

        public virtual void Add(T item, T item2)
        {
            if (_count + 1 >= arraySize) DoubleCapacity();

            items[_count++] = item;
            items[_count++] = item2;
            Count = _count;
        }

        public virtual void Add(T item, T item2, T item3)
        {
            if (_count + 2 >= arraySize) DoubleCapacity();

            items[_count++] = item;
            items[_count++] = item2;
            items[_count++] = item3;
            Count = _count;
        }

        public virtual void Add(T item, T item2, T item3, T item4)
        {
            if (_count + 3 >= arraySize) DoubleCapacity();

            items[_count++] = item;
            items[_count++] = item2;
            items[_count++] = item3;
            items[_count++] = item4;
            Count = _count;
        }

        public virtual void Add(T item, T item2, T item3, T item4, T item5)
        {
            if (_count + 4 >= arraySize) DoubleCapacity();

            items[_count++] = item;
            items[_count++] = item2;
            items[_count++] = item3;
            items[_count++] = item4;
            items[_count++] = item5;
            Count = _count;
        }

        public virtual void Insert(int index, T item)
        {
            if (index > _count) { Debug.LogError("Index " + index + " is out of range " + _count); }
            if (_count == arraySize) DoubleCapacity();
            if (index < _count) Array.Copy(items, index, items, index + 1, _count - index);

            items[index] = item;
            Count = ++_count;
        }

        public virtual void AddRange(T[] arrayItems)
        {
            if (arrayItems == null) return;

            int length = arrayItems.Length;
            int newCount = _count + length;
            if (newCount >= arraySize) SetCapacity(newCount * 2);

            Array.Copy(arrayItems, 0, items, _count, length);
            Count = _count = newCount;
        }

        public virtual void AddRange(T[] arrayItems, int startIndex, int length)
        {
            int newCount = _count + length;
            if (newCount >= arraySize) SetCapacity(newCount * 2);

            Array.Copy(arrayItems, startIndex, items, _count, length);
            Count = _count = newCount;
        }

        public virtual void AddRange(FastList<T> list)
        {
            if (list.Count == 0) return;

            int newCount = _count + list.Count;
            if (newCount >= arraySize) SetCapacity(newCount * 2);

            Array.Copy(list.items, 0, items, _count, list.Count);
            Count = _count = newCount;
        }

        public virtual int GrabListThreadSafe(FastList<T> threadList, bool fastClear = false)
        {
            lock (threadList)
            {
                int count = _count;
                AddRange(threadList);
                if (fastClear) threadList.FastClear(); else threadList.Clear();
                return count;
            }
        }

        //public void AddRange(T item, int amount)
        //{
        //    int newCount = _count + amount;
        //    if (newCount >= arraySize) SetCapacity(newCount * 2);

        //    for (int i = 0; i < amount; i++) items[_count++] = item;

        //    Count = _count;
        //}

        public virtual void ChangeRange(int startIndex, T[] arrayItems)
        {
            for (int i = 0; i < arrayItems.Length; i++)
            {
                items[startIndex + i] = arrayItems[i];
            }
        }

        // Slow
        public virtual bool Remove(T item, bool weakReference = false)
        {
            int index = Array.IndexOf(items, item, 0, _count);

            if (index >= 0)
            {
                items[index] = items[--_count];
                items[_count] = default(T);
                Count = _count;
                return true;
            }
            return false;
        }

        public virtual void RemoveAt(int index)
        {
            if (index >= _count) { Debug.LogError("Index " + index + " is out of range. List count is " + _count); return; }

            items[index] = items[--_count];
            items[_count] = default(T);
            Count = _count;
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void RemoveLast()
        {
            if (_count == 0) return;

            --_count;

            items[_count] = default(T);

            Count = _count;
        }

        //public void RemoveRange(int startIndex, int length)
        //{
        //    if (startIndex + length > _count) { Debug.LogError("StartIndex " + startIndex + " Length " + length + " is out of range. List count is " + _count); return; }

        //    int minIndex = startIndex + length;
        //    int copyIndex = _count - length;

        //    if (copyIndex < minIndex) copyIndex = minIndex;

        //    int length2 = _count - copyIndex;
        //    int index = startIndex;

        //    // Debug.Log("CopyIndex " + copyIndex + " length2 " + length2 + " rest " + (length - length2));

        //    for (int i = 0; i < length2; i++)
        //    {
        //        if (items[index] == null) { Debug.LogError(" item list " + (index) + " is null! List count " + _count); }
        //        items[index] = items[copyIndex + i];
        //        items[copyIndex + i] = default(T);
        //        --_count;
        //    }

        //    length -= length2;

        //    for (int i = 0; i < length; i++)
        //    {
        //        items[index++] = default(T);
        //        --_count;
        //    }

        //    Count = _count;
        //}

        public virtual void RemoveRange(int index, int length)
        {
            if (_count - index < length)
            {
                Debug.LogError("Invalid length!");
            }

            if (length > 0)
            {
                _count -= length;
                if (index < _count)
                {
                    Array.Copy(items, index + length, items, index, _count - index);
                }
                Array.Clear(items, _count, length);
                Count = _count;
            }
        }

        public virtual T Dequeue()
        {
            if (_count == 0)
            {
                Debug.LogError("List is empty!");
                return default(T);
            }

            T item = items[--_count];

            items[_count] = default(T);
            Count = _count;
            return item;
        }

        public virtual T Dequeue(int index)
        {
            T item = items[index];

            items[index] = items[--_count];
            items[_count] = default(T);
            Count = _count;
            return item;
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Clear()
        {
            Array.Clear(items, 0, _count);
            Count = _count = 0;
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void ClearThreadSafe()
        {
            lock (this)
            {
                Array.Clear(items, 0, _count);
                Count = _count = 0;
            }
        }

        public virtual void ClearRange(int startIndex)
        {
            Array.Clear(items, startIndex, _count - startIndex);
            Count = _count = startIndex;
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void FastClear()
        {
            Count = _count = 0;
        }

        public virtual void FastClear(int newCount)
        {
            if (newCount < Count)
            {
                Count = _count = newCount;
            }
        }

        public virtual T[] ToArray()
        {
            T[] array = new T[_count];
            Array.Copy(items, 0, array, 0, _count);
            return array;
        }
    }

    [Serializable]
    public class SortedFastList<T> : FastList<T>
    {
        new public void RemoveAt(int index)
        {
            if (index >= _count) { Debug.LogError("Index " + index + " is out of range " + _count); }

            _count--;
            if (index < _count) Array.Copy(items, index + 1, items, index, _count - index);
            items[_count] = default(T);
            Count = _count;
        }

        new public void RemoveRange(int index, int endIndex)
        {
            int length = (endIndex - index) + 1;

            if (index < 0)
            {
                Debug.LogError("Index needs to be bigger than 0 -> " + index);
                return;
            }

            if (length < 0)
            {
                Debug.LogError("Length needs to be bigger than 0 -> " + length);
                return;
            }

            if (_count - index < length)
            {

                return;
            }

            _count -= length;
            if (index < _count)
            {
                Array.Copy(items, index + length, items, index, _count - index);
            }
            Array.Clear(items, _count, length);
        }
    }

}