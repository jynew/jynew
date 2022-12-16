// -----------------------------------------------------------------------
// <copyright file="SimpleObjPool.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace TapSDK.UI.AillieoTech
{
    using System;
    using System.Collections.Generic;

    public class SimpleObjPool<T>
    {
        private readonly Stack<T> stack;
        private readonly Func<T> ctor;
        private readonly Action<T> onRecycle;
        private int size;
        private int usedCount;

        public SimpleObjPool(int max = 7, Action<T> onRecycle = null, Func<T> ctor = null)
        {
            this.stack = new Stack<T>(max);
            this.size = max;
            this.onRecycle = onRecycle;
            this.ctor = ctor;
        }

        public T Get()
        {
            T item;
            if (this.stack.Count == 0)
            {
                if (this.ctor != null)
                {
                    item = this.ctor();
                }
                else
                {
                    item = Activator.CreateInstance<T>();
                }
            }
            else
            {
                item = this.stack.Pop();
            }

            this.usedCount++;
            return item;
        }

        public void Recycle(T item)
        {
            if (this.onRecycle != null)
            {
                this.onRecycle.Invoke(item);
            }

            if (this.stack.Count < this.size)
            {
                this.stack.Push(item);
            }

            this.usedCount--;
        }

        public void Purge()
        {
            // TODO
        }

        public override string ToString()
        {
            return $"SimpleObjPool: item=[{typeof(T)}], inUse=[{this.usedCount}], restInPool=[{this.stack.Count}/{this.size}] ";
        }
    }
}
