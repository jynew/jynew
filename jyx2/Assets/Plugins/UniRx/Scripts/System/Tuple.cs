// defined from .NET Framework 4.0 and NETFX_CORE
// This code is basaed from mono/mcs, but some performance modified
// 1. class to struct
// 2. implements IEquatable<T1, T2,...>

// note, we need to create ValueTuple or UniRxTuple...
#if !(NETFX_CORE || NET_4_6 || NET_STANDARD_2_0 || UNITY_WSA_10_0)

using System;
using System.Collections;
using System.Collections.Generic;

namespace UniRx
{
    public interface IStructuralEquatable
    {
        bool Equals(object other, IEqualityComparer comparer);

        int GetHashCode(IEqualityComparer comparer);
    }

    public interface IStructuralComparable
    {
        int CompareTo(object other, IComparer comparer);
    }

    interface ITuple
    {
        string ToString();
    }

    public static class Tuple
    {
        public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>> Create<T1, T2, T3, T4, T5, T6, T7, T8>
            (
             T1 item1,
             T2 item2,
             T3 item3,
             T4 item4,
             T5 item5,
             T6 item6,
             T7 item7,
             T8 item8)
        {
            return new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8>(item8));
        }

        public static Tuple<T1, T2, T3, T4, T5, T6, T7> Create<T1, T2, T3, T4, T5, T6, T7>
            (
             T1 item1,
             T2 item2,
             T3 item3,
             T4 item4,
             T5 item5,
             T6 item6,
             T7 item7)
        {
            return new Tuple<T1, T2, T3, T4, T5, T6, T7>(item1, item2, item3, item4, item5, item6, item7);
        }

        public static Tuple<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>
            (
             T1 item1,
             T2 item2,
             T3 item3,
             T4 item4,
             T5 item5,
             T6 item6)
        {
            return new Tuple<T1, T2, T3, T4, T5, T6>(item1, item2, item3, item4, item5, item6);
        }

        public static Tuple<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>
            (
             T1 item1,
             T2 item2,
             T3 item3,
             T4 item4,
             T5 item5)
        {
            return new Tuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5);
        }

        public static Tuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>
            (
             T1 item1,
             T2 item2,
             T3 item3,
             T4 item4)
        {
            return new Tuple<T1, T2, T3, T4>(item1, item2, item3, item4);
        }

        public static Tuple<T1, T2, T3> Create<T1, T2, T3>
            (
             T1 item1,
             T2 item2,
             T3 item3)
        {
            return new Tuple<T1, T2, T3>(item1, item2, item3);
        }

        public static Tuple<T1, T2> Create<T1, T2>
            (
             T1 item1,
             T2 item2)
        {
            return new Tuple<T1, T2>(item1, item2);
        }

        public static Tuple<T1> Create<T1>
            (
             T1 item1)
        {
            return new Tuple<T1>(item1);
        }
    }

    public partial class Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>
    {
        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, TRest rest)
        {
            this.item1 = item1;
            this.item2 = item2;
            this.item3 = item3;
            this.item4 = item4;
            this.item5 = item5;
            this.item6 = item6;
            this.item7 = item7;
            this.rest = rest;

            if (!(rest is ITuple))
                throw new ArgumentException("rest", "The last element of an eight element tuple must be a Tuple.");
        }
    }

    [Serializable]
    public struct Tuple<T1> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple, IEquatable<Tuple<T1>>
    {
        T1 item1;

        public Tuple(T1 item1)
        {
            this.item1 = item1;
        }

        public T1 Item1
        {
            get { return item1; }
        }

        int IComparable.CompareTo(object obj)
        {
            return ((IStructuralComparable)this).CompareTo(obj, Comparer<object>.Default);
        }

        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            if (other == null) return 1;
            if (!(other is Tuple<T1>))
            {
                throw new ArgumentException("other");
            }
            var t = (Tuple<T1>)other;
            return comparer.Compare(item1, t.item1);
        }

        public override bool Equals(object obj)
        {
            return ((IStructuralEquatable)this).Equals(obj, EqualityComparer<object>.Default);
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            if (!(other is Tuple<T1>))
                return false;

            var t = (Tuple<T1>)other;
            return comparer.Equals(item1, t.item1);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T1>.Default.GetHashCode(item1);
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            return comparer.GetHashCode(item1);
        }

        string ITuple.ToString()
        {
            return String.Format("{0}", item1);
        }

        public override string ToString()
        {
            return "(" + ((ITuple)this).ToString() + ")";
        }

        public bool Equals(Tuple<T1> other)
        {
            return EqualityComparer<T1>.Default.Equals(item1, other.item1);
        }
    }

    [Serializable]
    public struct Tuple<T1, T2> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple, IEquatable<Tuple<T1, T2>>
    {
        T1 item1;
        T2 item2;

        public Tuple(T1 item1, T2 item2)
        {
            this.item1 = item1;
            this.item2 = item2;
        }

        public T1 Item1
        {
            get { return item1; }
        }

        public T2 Item2
        {
            get { return item2; }
        }

        int IComparable.CompareTo(object obj)
        {
            return ((IStructuralComparable)this).CompareTo(obj, Comparer<object>.Default);
        }

        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            if (other == null) return 1;
            if (!(other is Tuple<T1, T2>))
            {
                throw new ArgumentException("other");
            }
            var t = (Tuple<T1, T2>)other;

            int res = comparer.Compare(item1, t.item1);
            if (res != 0) return res;
            return comparer.Compare(item2, t.item2);
        }

        public override bool Equals(object obj)
        {
            return ((IStructuralEquatable)this).Equals(obj, EqualityComparer<object>.Default);
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            if (!(other is Tuple<T1, T2>))
                return false;

            var t = (Tuple<T1, T2>)other;
            return comparer.Equals(item1, t.item1) &&
                comparer.Equals(item2, t.item2);
        }

        public override int GetHashCode()
        {
            var comparer1 = EqualityComparer<T1>.Default;
            var comparer2 = EqualityComparer<T2>.Default;

            int h0;
            h0 = comparer1.GetHashCode(item1);
            h0 = (h0 << 5) + h0 ^ comparer2.GetHashCode(item2);
            return h0;
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            int h0;
            h0 = comparer.GetHashCode(item1);
            h0 = (h0 << 5) + h0 ^ comparer.GetHashCode(item2);
            return h0;
        }

        string ITuple.ToString()
        {
            return String.Format("{0}, {1}", item1, item2);
        }

        public override string ToString()
        {
            return "(" + ((ITuple)this).ToString() + ")";
        }

        public bool Equals(Tuple<T1, T2> other)
        {
            var comparer1 = EqualityComparer<T1>.Default;
            var comparer2 = EqualityComparer<T2>.Default;

            return comparer1.Equals(item1, other.item1) &&
                comparer2.Equals(item2, other.item2);
        }
    }

    [Serializable]
    public struct Tuple<T1, T2, T3> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple, IEquatable<Tuple<T1, T2, T3>>
    {
        T1 item1;
        T2 item2;
        T3 item3;

        public Tuple(T1 item1, T2 item2, T3 item3)
        {
            this.item1 = item1;
            this.item2 = item2;
            this.item3 = item3;
        }

        public T1 Item1
        {
            get { return item1; }
        }

        public T2 Item2
        {
            get { return item2; }
        }

        public T3 Item3
        {
            get { return item3; }
        }

        int IComparable.CompareTo(object obj)
        {
            return ((IStructuralComparable)this).CompareTo(obj, Comparer<object>.Default);
        }

        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            if (other == null) return 1;
            if (!(other is Tuple<T1, T2, T3>))
            {
                throw new ArgumentException("other");
            }
            var t = (Tuple<T1, T2, T3>)other;

            int res = comparer.Compare(item1, t.item1);
            if (res != 0) return res;
            res = comparer.Compare(item2, t.item2);
            if (res != 0) return res;
            return comparer.Compare(item3, t.item3);
        }

        public override bool Equals(object obj)
        {
            return ((IStructuralEquatable)this).Equals(obj, EqualityComparer<object>.Default);
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            if (!(other is Tuple<T1, T2, T3>))
                return false;

            var t = (Tuple<T1, T2, T3>)other;
            return comparer.Equals(item1, t.item1) &&
                comparer.Equals(item2, t.item2) &&
                comparer.Equals(item3, t.item3);
        }

        public override int GetHashCode()
        {
            var comparer1 = EqualityComparer<T1>.Default;
            var comparer2 = EqualityComparer<T2>.Default;
            var comparer3 = EqualityComparer<T3>.Default;

            int h0;
            h0 = comparer1.GetHashCode(item1);
            h0 = (h0 << 5) + h0 ^ comparer2.GetHashCode(item2);
            h0 = (h0 << 5) + h0 ^ comparer3.GetHashCode(item3);
            return h0;
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            int h0;
            h0 = comparer.GetHashCode(item1);
            h0 = (h0 << 5) + h0 ^ comparer.GetHashCode(item2);
            h0 = (h0 << 5) + h0 ^ comparer.GetHashCode(item3);
            return h0;
        }

        string ITuple.ToString()
        {
            return String.Format("{0}, {1}, {2}", item1, item2, item3);
        }

        public override string ToString()
        {
            return "(" + ((ITuple)this).ToString() + ")";
        }

        public bool Equals(Tuple<T1, T2, T3> other)
        {
            var comparer1 = EqualityComparer<T1>.Default;
            var comparer2 = EqualityComparer<T2>.Default;
            var comparer3 = EqualityComparer<T3>.Default;

            return comparer1.Equals(item1, other.item1) &&
                comparer2.Equals(item2, other.item2) &&
                comparer3.Equals(item3, other.item3);
        }
    }

    [Serializable]
    public struct Tuple<T1, T2, T3, T4> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple, IEquatable<Tuple<T1, T2, T3, T4>>
    {
        T1 item1;
        T2 item2;
        T3 item3;
        T4 item4;

        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            this.item1 = item1;
            this.item2 = item2;
            this.item3 = item3;
            this.item4 = item4;
        }

        public T1 Item1
        {
            get { return item1; }
        }

        public T2 Item2
        {
            get { return item2; }
        }

        public T3 Item3
        {
            get { return item3; }
        }

        public T4 Item4
        {
            get { return item4; }
        }

        int IComparable.CompareTo(object obj)
        {
            return ((IStructuralComparable)this).CompareTo(obj, Comparer<object>.Default);
        }

        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            if (other == null) return 1;
            if (!(other is Tuple<T1, T2, T3, T4>))
            {
                throw new ArgumentException("other");
            }
            var t = (Tuple<T1, T2, T3, T4>)other;

            int res = comparer.Compare(item1, t.item1);
            if (res != 0) return res;
            res = comparer.Compare(item2, t.item2);
            if (res != 0) return res;
            res = comparer.Compare(item3, t.item3);
            if (res != 0) return res;
            return comparer.Compare(item4, t.item4);
        }

        public override bool Equals(object obj)
        {
            return ((IStructuralEquatable)this).Equals(obj, EqualityComparer<object>.Default);
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            if (!(other is Tuple<T1, T2, T3, T4>))
                return false;
            var t = (Tuple<T1, T2, T3, T4>)other;

            return comparer.Equals(item1, t.item1) &&
                comparer.Equals(item2, t.item2) &&
                comparer.Equals(item3, t.item3) &&
                comparer.Equals(item4, t.item4);
        }

        public override int GetHashCode()
        {
            var comparer1 = EqualityComparer<T1>.Default;
            var comparer2 = EqualityComparer<T2>.Default;
            var comparer3 = EqualityComparer<T3>.Default;
            var comparer4 = EqualityComparer<T4>.Default;

            int h0, h1;
            h0 = comparer1.GetHashCode(item1);
            h0 = (h0 << 5) + h0 ^ comparer2.GetHashCode(item2);
            h1 = comparer3.GetHashCode(item3);
            h1 = (h1 << 5) + h1 ^ comparer4.GetHashCode(item4);
            h0 = (h0 << 5) + h0 ^ h1;
            return h0;
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            int h0, h1;
            h0 = comparer.GetHashCode(item1);
            h0 = (h0 << 5) + h0 ^ comparer.GetHashCode(item2);
            h1 = comparer.GetHashCode(item3);
            h1 = (h1 << 5) + h1 ^ comparer.GetHashCode(item4);
            h0 = (h0 << 5) + h0 ^ h1;
            return h0;
        }

        string ITuple.ToString()
        {
            return String.Format("{0}, {1}, {2}, {3}", item1, item2, item3, item4);
        }

        public override string ToString()
        {
            return "(" + ((ITuple)this).ToString() + ")";
        }

        public bool Equals(Tuple<T1, T2, T3, T4> other)
        {
            var comparer1 = EqualityComparer<T1>.Default;
            var comparer2 = EqualityComparer<T2>.Default;
            var comparer3 = EqualityComparer<T3>.Default;
            var comparer4 = EqualityComparer<T4>.Default;

            return comparer1.Equals(item1, other.item1) &&
                comparer2.Equals(item2, other.item2) &&
                comparer3.Equals(item3, other.item3) &&
                comparer4.Equals(item4, other.item4);
        }
    }

    [Serializable]
    public struct Tuple<T1, T2, T3, T4, T5> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple, IEquatable<Tuple<T1, T2, T3, T4, T5>>
    {
        T1 item1;
        T2 item2;
        T3 item3;
        T4 item4;
        T5 item5;

        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
        {
            this.item1 = item1;
            this.item2 = item2;
            this.item3 = item3;
            this.item4 = item4;
            this.item5 = item5;
        }

        public T1 Item1
        {
            get { return item1; }
        }

        public T2 Item2
        {
            get { return item2; }
        }

        public T3 Item3
        {
            get { return item3; }
        }

        public T4 Item4
        {
            get { return item4; }
        }

        public T5 Item5
        {
            get { return item5; }
        }

        int IComparable.CompareTo(object obj)
        {
            return ((IStructuralComparable)this).CompareTo(obj, Comparer<object>.Default);
        }

        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            if (other == null) return 1;
            if (!(other is Tuple<T1, T2, T3, T4, T5>))
            {
                throw new ArgumentException("other");
            }
            var t = (Tuple<T1, T2, T3, T4, T5>)other;

            int res = comparer.Compare(item1, t.item1);
            if (res != 0) return res;
            res = comparer.Compare(item2, t.item2);
            if (res != 0) return res;
            res = comparer.Compare(item3, t.item3);
            if (res != 0) return res;
            res = comparer.Compare(item4, t.item4);
            if (res != 0) return res;
            return comparer.Compare(item5, t.item5);
        }

        public override bool Equals(object obj)
        {
            return ((IStructuralEquatable)this).Equals(obj, EqualityComparer<object>.Default);
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            if (!(other is Tuple<T1, T2, T3, T4, T5>))
                return false;
            var t = (Tuple<T1, T2, T3, T4, T5>)other;

            return comparer.Equals(item1, t.item1) &&
                comparer.Equals(item2, t.item2) &&
                comparer.Equals(item3, t.item3) &&
                comparer.Equals(item4, t.item4) &&
                comparer.Equals(item5, t.item5);
        }

        public override int GetHashCode()
        {
            var comparer1 = EqualityComparer<T1>.Default;
            var comparer2 = EqualityComparer<T2>.Default;
            var comparer3 = EqualityComparer<T3>.Default;
            var comparer4 = EqualityComparer<T4>.Default;
            var comparer5 = EqualityComparer<T5>.Default;

            int h0, h1;
            h0 = comparer1.GetHashCode(item1);
            h0 = (h0 << 5) + h0 ^ comparer2.GetHashCode(item2);
            h1 = comparer3.GetHashCode(item3);
            h1 = (h1 << 5) + h1 ^ comparer4.GetHashCode(item4);
            h0 = (h0 << 5) + h0 ^ h1;
            h0 = (h0 << 5) + h0 ^ comparer5.GetHashCode(item5);
            return h0;
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            int h0, h1;
            h0 = comparer.GetHashCode(item1);
            h0 = (h0 << 5) + h0 ^ comparer.GetHashCode(item2);
            h1 = comparer.GetHashCode(item3);
            h1 = (h1 << 5) + h1 ^ comparer.GetHashCode(item4);
            h0 = (h0 << 5) + h0 ^ h1;
            h0 = (h0 << 5) + h0 ^ comparer.GetHashCode(item5);
            return h0;
        }

        string ITuple.ToString()
        {
            return String.Format("{0}, {1}, {2}, {3}, {4}", item1, item2, item3, item4, item5);
        }

        public override string ToString()
        {
            return "(" + ((ITuple)this).ToString() + ")";
        }

        public bool Equals(Tuple<T1, T2, T3, T4, T5> other)
        {
            var comparer1 = EqualityComparer<T1>.Default;
            var comparer2 = EqualityComparer<T2>.Default;
            var comparer3 = EqualityComparer<T3>.Default;
            var comparer4 = EqualityComparer<T4>.Default;
            var comparer5 = EqualityComparer<T5>.Default;

            return comparer1.Equals(item1, other.Item1) &&
                comparer2.Equals(item2, other.Item2) &&
                comparer3.Equals(item3, other.Item3) &&
                comparer4.Equals(item4, other.Item4) &&
                comparer5.Equals(item5, other.Item5);
        }
    }

    [Serializable]
    public struct Tuple<T1, T2, T3, T4, T5, T6> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple, IEquatable<Tuple<T1, T2, T3, T4, T5, T6>>
    {
        T1 item1;
        T2 item2;
        T3 item3;
        T4 item4;
        T5 item5;
        T6 item6;

        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
        {
            this.item1 = item1;
            this.item2 = item2;
            this.item3 = item3;
            this.item4 = item4;
            this.item5 = item5;
            this.item6 = item6;
        }

        public T1 Item1
        {
            get { return item1; }
        }

        public T2 Item2
        {
            get { return item2; }
        }

        public T3 Item3
        {
            get { return item3; }
        }

        public T4 Item4
        {
            get { return item4; }
        }

        public T5 Item5
        {
            get { return item5; }
        }

        public T6 Item6
        {
            get { return item6; }
        }

        int IComparable.CompareTo(object obj)
        {
            return ((IStructuralComparable)this).CompareTo(obj, Comparer<object>.Default);
        }

        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            if (other == null) return 1;
            if (!(other is Tuple<T1, T2, T3, T4, T5, T6>))
            {
                throw new ArgumentException("other");
            }
            var t = (Tuple<T1, T2, T3, T4, T5, T6>)other;

            int res = comparer.Compare(item1, t.item1);
            if (res != 0) return res;
            res = comparer.Compare(item2, t.item2);
            if (res != 0) return res;
            res = comparer.Compare(item3, t.item3);
            if (res != 0) return res;
            res = comparer.Compare(item4, t.item4);
            if (res != 0) return res;
            res = comparer.Compare(item5, t.item5);
            if (res != 0) return res;
            return comparer.Compare(item6, t.item6);
        }

        public override bool Equals(object obj)
        {
            return ((IStructuralEquatable)this).Equals(obj, EqualityComparer<object>.Default);
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            if (!(other is Tuple<T1, T2, T3, T4, T5, T6>))
                return false;
            var t = (Tuple<T1, T2, T3, T4, T5, T6>)other;

            return comparer.Equals(item1, t.item1) &&
                comparer.Equals(item2, t.item2) &&
                comparer.Equals(item3, t.item3) &&
                comparer.Equals(item4, t.item4) &&
                comparer.Equals(item5, t.item5) &&
                comparer.Equals(item6, t.item6);
        }

        public override int GetHashCode()
        {
            var comparer1 = EqualityComparer<T1>.Default;
            var comparer2 = EqualityComparer<T2>.Default;
            var comparer3 = EqualityComparer<T3>.Default;
            var comparer4 = EqualityComparer<T4>.Default;
            var comparer5 = EqualityComparer<T5>.Default;
            var comparer6 = EqualityComparer<T6>.Default;

            int h0, h1;
            h0 = comparer1.GetHashCode(item1);
            h0 = (h0 << 5) + h0 ^ comparer2.GetHashCode(item2);
            h1 = comparer3.GetHashCode(item3);
            h1 = (h1 << 5) + h1 ^ comparer4.GetHashCode(item4);
            h0 = (h0 << 5) + h0 ^ h1;
            h1 = comparer5.GetHashCode(item5);
            h1 = (h1 << 5) + h1 ^ comparer6.GetHashCode(item6);
            h0 = (h0 << 5) + h0 ^ h1;
            return h0;
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            int h0, h1;
            h0 = comparer.GetHashCode(item1);
            h0 = (h0 << 5) + h0 ^ comparer.GetHashCode(item2);
            h1 = comparer.GetHashCode(item3);
            h1 = (h1 << 5) + h1 ^ comparer.GetHashCode(item4);
            h0 = (h0 << 5) + h0 ^ h1;
            h1 = comparer.GetHashCode(item5);
            h1 = (h1 << 5) + h1 ^ comparer.GetHashCode(item6);
            h0 = (h0 << 5) + h0 ^ h1;
            return h0;
        }

        string ITuple.ToString()
        {
            return String.Format("{0}, {1}, {2}, {3}, {4}, {5}", item1, item2, item3, item4, item5, item6);
        }

        public override string ToString()
        {
            return "(" + ((ITuple)this).ToString() + ")";
        }

        public bool Equals(Tuple<T1, T2, T3, T4, T5, T6> other)
        {
            var comparer1 = EqualityComparer<T1>.Default;
            var comparer2 = EqualityComparer<T2>.Default;
            var comparer3 = EqualityComparer<T3>.Default;
            var comparer4 = EqualityComparer<T4>.Default;
            var comparer5 = EqualityComparer<T5>.Default;
            var comparer6 = EqualityComparer<T6>.Default;

            return comparer1.Equals(item1, other.Item1) &&
                comparer2.Equals(item2, other.Item2) &&
                comparer3.Equals(item3, other.Item3) &&
                comparer4.Equals(item4, other.Item4) &&
                comparer5.Equals(item5, other.Item5) &&
                comparer6.Equals(item6, other.Item6);
        }
    }

    [Serializable]
    public struct Tuple<T1, T2, T3, T4, T5, T6, T7> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple, IEquatable<Tuple<T1, T2, T3, T4, T5, T6, T7>>
    {
        T1 item1;
        T2 item2;
        T3 item3;
        T4 item4;
        T5 item5;
        T6 item6;
        T7 item7;

        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
        {
            this.item1 = item1;
            this.item2 = item2;
            this.item3 = item3;
            this.item4 = item4;
            this.item5 = item5;
            this.item6 = item6;
            this.item7 = item7;
        }

        public T1 Item1
        {
            get { return item1; }
        }

        public T2 Item2
        {
            get { return item2; }
        }

        public T3 Item3
        {
            get { return item3; }
        }

        public T4 Item4
        {
            get { return item4; }
        }

        public T5 Item5
        {
            get { return item5; }
        }

        public T6 Item6
        {
            get { return item6; }
        }

        public T7 Item7
        {
            get { return item7; }
        }

        int IComparable.CompareTo(object obj)
        {
            return ((IStructuralComparable)this).CompareTo(obj, Comparer<object>.Default);
        }

        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            if (other == null) return 1;
            if (!(other is Tuple<T1, T2, T3, T4, T5, T6, T7>))
            {
                throw new ArgumentException("other");
            }
            var t = (Tuple<T1, T2, T3, T4, T5, T6, T7>)other;

            int res = comparer.Compare(item1, t.item1);
            if (res != 0) return res;
            res = comparer.Compare(item2, t.item2);
            if (res != 0) return res;
            res = comparer.Compare(item3, t.item3);
            if (res != 0) return res;
            res = comparer.Compare(item4, t.item4);
            if (res != 0) return res;
            res = comparer.Compare(item5, t.item5);
            if (res != 0) return res;
            res = comparer.Compare(item6, t.item6);
            if (res != 0) return res;
            return comparer.Compare(item7, t.item7);
        }

        public override bool Equals(object obj)
        {
            return ((IStructuralEquatable)this).Equals(obj, EqualityComparer<object>.Default);
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            if (!(other is Tuple<T1, T2, T3, T4, T5, T6, T7>))
                return false;
            var t = (Tuple<T1, T2, T3, T4, T5, T6, T7>)other;

            return comparer.Equals(item1, t.item1) &&
                comparer.Equals(item2, t.item2) &&
                comparer.Equals(item3, t.item3) &&
                comparer.Equals(item4, t.item4) &&
                comparer.Equals(item5, t.item5) &&
                comparer.Equals(item6, t.item6) &&
                comparer.Equals(item7, t.item7);
        }

        public override int GetHashCode()
        {
            var comparer1 = EqualityComparer<T1>.Default;
            var comparer2 = EqualityComparer<T2>.Default;
            var comparer3 = EqualityComparer<T3>.Default;
            var comparer4 = EqualityComparer<T4>.Default;
            var comparer5 = EqualityComparer<T5>.Default;
            var comparer6 = EqualityComparer<T6>.Default;
            var comparer7 = EqualityComparer<T7>.Default;

            int h0, h1;
            h0 = comparer1.GetHashCode(item1);
            h0 = (h0 << 5) + h0 ^ comparer2.GetHashCode(item2);
            h1 = comparer3.GetHashCode(item3);
            h1 = (h1 << 5) + h1 ^ comparer4.GetHashCode(item4);
            h0 = (h0 << 5) + h0 ^ h1;
            h1 = comparer5.GetHashCode(item5);
            h1 = (h1 << 5) + h1 ^ comparer6.GetHashCode(item6);
            h1 = (h1 << 5) + h1 ^ comparer7.GetHashCode(item7);
            h0 = (h0 << 5) + h0 ^ h1;
            return h0;
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            int h0, h1;
            h0 = comparer.GetHashCode(item1);
            h0 = (h0 << 5) + h0 ^ comparer.GetHashCode(item2);
            h1 = comparer.GetHashCode(item3);
            h1 = (h1 << 5) + h1 ^ comparer.GetHashCode(item4);
            h0 = (h0 << 5) + h0 ^ h1;
            h1 = comparer.GetHashCode(item5);
            h1 = (h1 << 5) + h1 ^ comparer.GetHashCode(item6);
            h1 = (h1 << 5) + h1 ^ comparer.GetHashCode(item7);
            h0 = (h0 << 5) + h0 ^ h1;
            return h0;
        }

        string ITuple.ToString()
        {
            return String.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}", item1, item2, item3, item4, item5, item6, item7);
        }

        public override string ToString()
        {
            return "(" + ((ITuple)this).ToString() + ")";
        }

        public bool Equals(Tuple<T1, T2, T3, T4, T5, T6, T7> other)
        {
            var comparer1 = EqualityComparer<T1>.Default;
            var comparer2 = EqualityComparer<T2>.Default;
            var comparer3 = EqualityComparer<T3>.Default;
            var comparer4 = EqualityComparer<T4>.Default;
            var comparer5 = EqualityComparer<T5>.Default;
            var comparer6 = EqualityComparer<T6>.Default;
            var comparer7 = EqualityComparer<T7>.Default;

            return comparer1.Equals(item1, other.Item1) &&
                comparer2.Equals(item2, other.Item2) &&
                comparer3.Equals(item3, other.Item3) &&
                comparer4.Equals(item4, other.Item4) &&
                comparer5.Equals(item5, other.Item5) &&
                comparer6.Equals(item6, other.Item6) &&
                comparer7.Equals(item7, other.Item7);
        }
    }

    [Serializable]
    public partial class Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple, IEquatable<Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>>
    {
        T1 item1;
        T2 item2;
        T3 item3;
        T4 item4;
        T5 item5;
        T6 item6;
        T7 item7;
        TRest rest;

        public T1 Item1
        {
            get { return item1; }
        }

        public T2 Item2
        {
            get { return item2; }
        }

        public T3 Item3
        {
            get { return item3; }
        }

        public T4 Item4
        {
            get { return item4; }
        }

        public T5 Item5
        {
            get { return item5; }
        }

        public T6 Item6
        {
            get { return item6; }
        }

        public T7 Item7
        {
            get { return item7; }
        }

        public TRest Rest
        {
            get { return rest; }
        }

        int IComparable.CompareTo(object obj)
        {
            return ((IStructuralComparable)this).CompareTo(obj, Comparer<object>.Default);
        }

        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            if (other == null) return 1;
            if (!(other is Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>))
            {
                throw new ArgumentException("other");
            }
            var t = (Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>)other;

            int res = comparer.Compare(item1, t.item1);
            if (res != 0) return res;
            res = comparer.Compare(item2, t.item2);
            if (res != 0) return res;
            res = comparer.Compare(item3, t.item3);
            if (res != 0) return res;
            res = comparer.Compare(item4, t.item4);
            if (res != 0) return res;
            res = comparer.Compare(item5, t.item5);
            if (res != 0) return res;
            res = comparer.Compare(item6, t.item6);
            if (res != 0) return res;
            res = comparer.Compare(item7, t.item7);
            if (res != 0) return res;
            return comparer.Compare(rest, t.rest);
        }

        public override bool Equals(object obj)
        {
            return ((IStructuralEquatable)this).Equals(obj, EqualityComparer<object>.Default);
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            if (!(other is Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>))
                return false;
            var t = (Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>)other;

            return comparer.Equals(item1, t.item1) &&
                comparer.Equals(item2, t.item2) &&
                comparer.Equals(item3, t.item3) &&
                comparer.Equals(item4, t.item4) &&
                comparer.Equals(item5, t.item5) &&
                comparer.Equals(item6, t.item6) &&
                comparer.Equals(item7, t.item7) &&
                comparer.Equals(rest, t.rest);
        }

        public override int GetHashCode()
        {
            var comparer1 = EqualityComparer<T1>.Default;
            var comparer2 = EqualityComparer<T2>.Default;
            var comparer3 = EqualityComparer<T3>.Default;
            var comparer4 = EqualityComparer<T4>.Default;
            var comparer5 = EqualityComparer<T5>.Default;
            var comparer6 = EqualityComparer<T6>.Default;
            var comparer7 = EqualityComparer<T7>.Default;
            var comparer8 = EqualityComparer<TRest>.Default;

            int h0, h1, h2;
            h0 = comparer1.GetHashCode(item1);
            h0 = (h0 << 5) + h0 ^ comparer2.GetHashCode(item2);
            h1 = comparer3.GetHashCode(item3);
            h1 = (h1 << 5) + h1 ^ comparer4.GetHashCode(item4);
            h0 = (h0 << 5) + h0 ^ h1;
            h1 = comparer5.GetHashCode(item5);
            h1 = (h1 << 5) + h1 ^ comparer6.GetHashCode(item6);
            h2 = comparer7.GetHashCode(item7);
            h2 = (h2 << 5) + h2 ^ comparer8.GetHashCode(rest);
            h1 = (h1 << 5) + h1 ^ h2;
            h0 = (h0 << 5) + h0 ^ h1;
            return h0;
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            int h0, h1, h2;
            h0 = comparer.GetHashCode(item1);
            h0 = (h0 << 5) + h0 ^ comparer.GetHashCode(item2);
            h1 = comparer.GetHashCode(item3);
            h1 = (h1 << 5) + h1 ^ comparer.GetHashCode(item4);
            h0 = (h0 << 5) + h0 ^ h1;
            h1 = comparer.GetHashCode(item5);
            h1 = (h1 << 5) + h1 ^ comparer.GetHashCode(item6);
            h2 = comparer.GetHashCode(item7);
            h2 = (h2 << 5) + h2 ^ comparer.GetHashCode(rest);
            h1 = (h1 << 5) + h1 ^ h2;
            h0 = (h0 << 5) + h0 ^ h1;
            return h0;
        }

        string ITuple.ToString()
        {
            return String.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", item1, item2, item3, item4, item5, item6, item7, ((ITuple)rest).ToString());
        }

        public override string ToString()
        {
            return "(" + ((ITuple)this).ToString() + ")";
        }

        public bool Equals(Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> other)
        {
            var comparer1 = EqualityComparer<T1>.Default;
            var comparer2 = EqualityComparer<T2>.Default;
            var comparer3 = EqualityComparer<T3>.Default;
            var comparer4 = EqualityComparer<T4>.Default;
            var comparer5 = EqualityComparer<T5>.Default;
            var comparer6 = EqualityComparer<T6>.Default;
            var comparer7 = EqualityComparer<T7>.Default;
            var comparer8 = EqualityComparer<TRest>.Default;

            return comparer1.Equals(item1, other.Item1) &&
                comparer2.Equals(item2, other.Item2) &&
                comparer3.Equals(item3, other.Item3) &&
                comparer4.Equals(item4, other.Item4) &&
                comparer5.Equals(item5, other.Item5) &&
                comparer6.Equals(item6, other.Item6) &&
                comparer7.Equals(item7, other.Item7) &&
                comparer8.Equals(rest, other.rest);
        }
    }
}

#endif