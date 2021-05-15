// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

using System.Collections.Generic;

namespace Animancer
{
    /// <summary>A simple stack implementation that tracks an active index without actually adding or removing objects.</summary>
    /// https://kybernetik.com.au/animancer/api/Animancer/LazyStack_1
    /// 
    public sealed class LazyStack<T> where T : new()
    {
        /************************************************************************************************************************/

        /// <summary>The underlying collection of objects.</summary>
        /// <remarks>
        /// This is not a <see cref="Stack{T}"/> because that class comes from a different assembly that might not
        /// otherwise need to be included in builds, so using a <see cref="List{T}"/> can slightly reduce build size.
        /// </remarks>
        private readonly List<T> Stack = new List<T>();

        /// <summary>The index of the <see cref="Current"/> object in the <see cref="Stack"/>.</summary>
        private int _CurrentIndex = -1;

        /// <summary>The object currently on the top of the stack.</summary>
        public T Current { get; private set; }

        /************************************************************************************************************************/

        /// <summary>Moves to the next object in the stack.</summary>
        public T Increment()
        {
            _CurrentIndex++;
            if (_CurrentIndex == Stack.Count)
            {
                Current = new T();
                Stack.Add(Current);
            }
            else
            {
                Current = Stack[_CurrentIndex];
            }

            return Current;
        }

        /************************************************************************************************************************/

        /// <summary>Moves to the previous object in the stack.</summary>
        public void Decrement()
        {
            _CurrentIndex--;
            if (_CurrentIndex >= 0)
                Current = Stack[_CurrentIndex];
            else
                Current = default(T);
        }

        /************************************************************************************************************************/
    }
}

