using UnityEngine;
using System;


namespace MeshCombineStudio {
    public class ObjectHolder<T> : FastIndex
    {
        public T item;

        public ObjectHolder() { }

        public ObjectHolder(T item)
        {
            this.item = item;
        }
    }

    public abstract class Parent<T>
    {
        [NonSerialized] public T parent;
    }

    public abstract class ParentFastHashListIndex<T> : FastIndex
    {
        [NonSerialized] public T parent;
    }

    public abstract class ParentMono<T> : MonoBehaviour
    {
        [NonSerialized] public T parent;
    }

    public abstract class ParentMonoHash<T> : MonoBehaviourFastIndex
    {
        [NonSerialized] public T parent;
    }

    public interface IFastIndexList
    {
        bool RemoveAt(int index);
        bool Remove(IFastIndex item);
    }

    public interface IFastIndex
    {
        IFastIndexList List { get; set; }

        int ListIndex { get; set; }
    }

    public class FastIndex : IFastIndex
    {
        public IFastIndexList List { get; set; }

        public int ListIndex { get; set; }

        public FastIndex()
        {
            ListIndex = -1;
        }

        public void RemoveFromList()
        {
            if (List != null) List.Remove(this);
            // else Debug.LogError("Can't remove item because list is null!");
        }


    }

    public class MonoBehaviourFastIndex : MonoBehaviour, IFastIndex
    {
        public IFastIndexList List { get; set; }

        public int ListIndex { get; set; }

        public MonoBehaviourFastIndex()
        {
            ListIndex = -1;
        }

        public void RemoveFromList()
        {
            if (List != null) List.Remove(this);
        }
    } 
}