// original code from rx.codeplex.com

/* ------------------ */

// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;

namespace UniRx
{
    /// <summary>
    /// Represents value with a timestamp on it.
    /// The timestamp typically represents the time the value was received, using an IScheduler's clock to obtain the current time.
    /// </summary>
    /// <typeparam name="T">The type of the value being timestamped.</typeparam>
    [Serializable]
    public struct Timestamped<T> : IEquatable<Timestamped<T>>
    {
        private readonly DateTimeOffset _timestamp;
        private readonly T _value;

        /// <summary>
        /// Constructs a timestamped value.
        /// </summary>
        /// <param name="value">The value to be annotated with a timestamp.</param>
        /// <param name="timestamp">Timestamp associated with the value.</param>
        public Timestamped(T value, DateTimeOffset timestamp)
        {
            _timestamp = timestamp;
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
        /// Gets the timestamp.
        /// </summary>
        public DateTimeOffset Timestamp
        {
            get { return _timestamp; }
        }

        /// <summary>
        /// Determines whether the current Timestamped&lt;T&gt; value has the same Value and Timestamp as a specified Timestamped&lt;T&gt; value.
        /// </summary>
        /// <param name="other">An object to compare to the current Timestamped&lt;T&gt; value.</param>
        /// <returns>true if both Timestamped&lt;T&gt; values have the same Value and Timestamp; otherwise, false.</returns>
        public bool Equals(Timestamped<T> other)
        {
            return other.Timestamp.Equals(Timestamp) && EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        /// <summary>
        /// Determines whether the two specified Timestamped&lt;T&gt; values have the same Value and Timestamp.
        /// </summary>
        /// <param name="first">The first Timestamped&lt;T&gt; value to compare.</param>
        /// <param name="second">The second Timestamped&lt;T&gt; value to compare.</param>
        /// <returns>true if the first Timestamped&lt;T&gt; value has the same Value and Timestamp as the second Timestamped&lt;T&gt; value; otherwise, false.</returns>
        public static bool operator ==(Timestamped<T> first, Timestamped<T> second)
        {
            return first.Equals(second);
        }

        /// <summary>
        /// Determines whether the two specified Timestamped&lt;T&gt; values don't have the same Value and Timestamp.
        /// </summary>
        /// <param name="first">The first Timestamped&lt;T&gt; value to compare.</param>
        /// <param name="second">The second Timestamped&lt;T&gt; value to compare.</param>
        /// <returns>true if the first Timestamped&lt;T&gt; value has a different Value or Timestamp as the second Timestamped&lt;T&gt; value; otherwise, false.</returns>
        public static bool operator !=(Timestamped<T> first, Timestamped<T> second)
        {
            return !first.Equals(second);
        }

        /// <summary>
        /// Determines whether the specified System.Object is equal to the current Timestamped&lt;T&gt;.
        /// </summary>
        /// <param name="obj">The System.Object to compare with the current Timestamped&lt;T&gt;.</param>
        /// <returns>true if the specified System.Object is equal to the current Timestamped&lt;T&gt;; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Timestamped<T>))
                return false;

            var other = (Timestamped<T>)obj;
            return this.Equals(other);
        }

        /// <summary>
        /// Returns the hash code for the current Timestamped&lt;T&gt; value.
        /// </summary>
        /// <returns>A hash code for the current Timestamped&lt;T&gt; value.</returns>
        public override int GetHashCode()
        {
            var valueHashCode = Value == null ? 1979 : Value.GetHashCode();

            return _timestamp.GetHashCode() ^ valueHashCode;
        }

        /// <summary>
        /// Returns a string representation of the current Timestamped&lt;T&gt; value.
        /// </summary>
        /// <returns>String representation of the current Timestamped&lt;T&gt; value.</returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.CurrentCulture, "{0}@{1}", Value, Timestamp);
        }
    }

    /// <summary>
    /// A helper class with a factory method for creating Timestamped&lt;T&gt; instances.
    /// </summary>
    public static class Timestamped
    {
        /// <summary>
        /// Creates an instance of a Timestamped&lt;T&gt;.  This is syntactic sugar that uses type inference
        /// to avoid specifying a type in a constructor call, which is very useful when using anonymous types.
        /// </summary>
        /// <param name="value">The value to be annotated with a timestamp.</param>
        /// <param name="timestamp">Timestamp associated with the value.</param>
        /// <returns>Creates a new timestamped value.</returns>
        public static Timestamped<T> Create<T>(T value, DateTimeOffset timestamp)
        {
            return new Timestamped<T>(value, timestamp);
        }
    }
}
