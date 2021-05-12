using System;
using System.Collections.Generic;

namespace UniRx
{
    // Pair is used for Observable.Pairwise
    [Serializable]
    public struct Pair<T> : IEquatable<Pair<T>>
    {
        readonly T previous;
        readonly T current;

        public T Previous
        {
            get { return previous; }
        }

        public T Current
        {
            get { return current; }
        }

        public Pair(T previous, T current)
        {
            this.previous = previous;
            this.current = current;
        }

        public override int GetHashCode()
        {
            var comparer = EqualityComparer<T>.Default;

            int h0;
            h0 = comparer.GetHashCode(previous);
            h0 = (h0 << 5) + h0 ^ comparer.GetHashCode(current);
            return h0;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Pair<T>)) return false;

            return Equals((Pair<T>)obj);
        }

        public bool Equals(Pair<T> other)
        {
            var comparer = EqualityComparer<T>.Default;

            return comparer.Equals(previous, other.Previous) &&
                comparer.Equals(current, other.Current);
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", previous, current);
        }
    }
}