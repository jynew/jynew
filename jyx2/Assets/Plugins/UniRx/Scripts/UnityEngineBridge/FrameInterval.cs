using System;
using System.Collections.Generic;
using System.Globalization;

namespace UniRx
{
    /// <summary>
    /// Represents a value associated with time interval information.
    /// The time interval can represent the time it took to produce the value, the interval relative to a previous value, the value's delivery time relative to a base, etc.
    /// </summary>
    /// <typeparam name="T">The type of the value being annotated with time interval information.</typeparam>
    [Serializable]
    public struct FrameInterval<T> : IEquatable<FrameInterval<T>>
    {
        private readonly int _interval;
        private readonly T _value;

        /// <summary>
        /// Constructs a time interval value.
        /// </summary>
        /// <param name="value">The value to be annotated with a time interval.</param>
        /// <param name="interval">Time interval associated with the value.</param>
        public FrameInterval(T value, int interval)
        {
            _interval = interval;
            _value = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public T Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Gets the interval.
        /// </summary>
        public int Interval
        {
            get { return _interval; }
        }

        /// <summary>
        /// Determines whether the current FrameInterval&lt;T&gt; value has the same Value and Interval as a specified FrameInterval&lt;T&gt; value.
        /// </summary>
        /// <param name="other">An object to compare to the current FrameInterval&lt;T&gt; value.</param>
        /// <returns>true if both FrameInterval&lt;T&gt; values have the same Value and Interval; otherwise, false.</returns>
        public bool Equals(FrameInterval<T> other)
        {
            return other.Interval.Equals(Interval) && EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        /// <summary>
        /// Determines whether the two specified FrameInterval&lt;T&gt; values have the same Value and Interval.
        /// </summary>
        /// <param name="first">The first FrameInterval&lt;T&gt; value to compare.</param>
        /// <param name="second">The second FrameInterval&lt;T&gt; value to compare.</param>
        /// <returns>true if the first FrameInterval&lt;T&gt; value has the same Value and Interval as the second FrameInterval&lt;T&gt; value; otherwise, false.</returns>
        public static bool operator ==(FrameInterval<T> first, FrameInterval<T> second)
        {
            return first.Equals(second);
        }

        /// <summary>
        /// Determines whether the two specified FrameInterval&lt;T&gt; values don't have the same Value and Interval.
        /// </summary>
        /// <param name="first">The first FrameInterval&lt;T&gt; value to compare.</param>
        /// <param name="second">The second FrameInterval&lt;T&gt; value to compare.</param>
        /// <returns>true if the first FrameInterval&lt;T&gt; value has a different Value or Interval as the second FrameInterval&lt;T&gt; value; otherwise, false.</returns>
        public static bool operator !=(FrameInterval<T> first, FrameInterval<T> second)
        {
            return !first.Equals(second);
        }

        /// <summary>
        /// Determines whether the specified System.Object is equal to the current FrameInterval&lt;T&gt;.
        /// </summary>
        /// <param name="obj">The System.Object to compare with the current FrameInterval&lt;T&gt;.</param>
        /// <returns>true if the specified System.Object is equal to the current FrameInterval&lt;T&gt;; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is FrameInterval<T>))
                return false;

            var other = (FrameInterval<T>)obj;
            return this.Equals(other);
        }

        /// <summary>
        /// Returns the hash code for the current FrameInterval&lt;T&gt; value.
        /// </summary>
        /// <returns>A hash code for the current FrameInterval&lt;T&gt; value.</returns>
        public override int GetHashCode()
        {
            var valueHashCode = Value == null ? 1963 : Value.GetHashCode();

            return Interval.GetHashCode() ^ valueHashCode;
        }

        /// <summary>
        /// Returns a string representation of the current FrameInterval&lt;T&gt; value.
        /// </summary>
        /// <returns>String representation of the current FrameInterval&lt;T&gt; value.</returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.CurrentCulture, "{0}@{1}", Value, Interval);
        }
    }
}
