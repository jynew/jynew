// original code from rx.codeplex.com
// some modified.

/* ------------------ */

// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System;
using UniRx.InternalUtil;

#pragma warning disable 0659
#pragma warning disable 0661

namespace UniRx
{
    /// <summary>
    /// Provides a mechanism for receiving push-based notifications and returning a response.
    /// </summary>
    /// <typeparam name="TValue">
    /// The type of the elements received by the observer.
    /// This type parameter is contravariant. That is, you can use either the type you specified or any type that is less derived. For more information about covariance and contravariance, see Covariance and Contravariance in Generics.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The type of the result returned from the observer's notification handlers.
    /// This type parameter is covariant. That is, you can use either the type you specified or any type that is more derived. For more information about covariance and contravariance, see Covariance and Contravariance in Generics.
    /// </typeparam>
    public interface IObserver<TValue, TResult>
    {
        /// <summary>
        /// Notifies the observer of a new element in the sequence.
        /// </summary>
        /// <param name="value">The new element in the sequence.</param>
        /// <returns>Result returned upon observation of a new element.</returns>
        TResult OnNext(TValue value);

        /// <summary>
        /// Notifies the observer that an exception has occurred.
        /// </summary>
        /// <param name="exception">The exception that occurred.</param>
        /// <returns>Result returned upon observation of an error.</returns>
        TResult OnError(Exception exception);

        /// <summary>
        /// Notifies the observer of the end of the sequence.
        /// </summary>
        /// <returns>Result returned upon observation of the sequence completion.</returns>
        TResult OnCompleted();
    }

    /// <summary>
    /// Indicates the type of a notification.
    /// </summary>
    public enum NotificationKind
    {
        /// <summary>
        /// Represents an OnNext notification.
        /// </summary>
        OnNext,

        /// <summary>
        /// Represents an OnError notification.
        /// </summary>
        OnError,

        /// <summary>
        /// Represents an OnCompleted notification.
        /// </summary>
        OnCompleted
    }

    /// <summary>
    /// Represents a notification to an observer.
    /// </summary>
    /// <typeparam name="T">The type of the elements received by the observer.</typeparam>
    [Serializable]
    public abstract class Notification<T> : IEquatable<Notification<T>>
    {
        /// <summary>
        /// Default constructor used by derived types.
        /// </summary>
        protected internal Notification()
        {
        }

        /// <summary>
        /// Returns the value of an OnNext notification or throws an exception.
        /// </summary>
        public abstract T Value
        {
            get;
        }

        /// <summary>
        /// Returns a value that indicates whether the notification has a value.
        /// </summary>
        public abstract bool HasValue
        {
            get;
        }

        /// <summary>
        /// Returns the exception of an OnError notification or returns null.
        /// </summary>
        public abstract Exception Exception
        {
            get;
        }

        /// <summary>
        /// Gets the kind of notification that is represented.
        /// </summary>
        public abstract NotificationKind Kind
        {
            get;
        }

        /// <summary>
        /// Represents an OnNext notification to an observer.
        /// </summary>
        [DebuggerDisplay("OnNext({Value})")]
        [Serializable]
        internal sealed class OnNextNotification : Notification<T>
        {
            T value;

            /// <summary>
            /// Constructs a notification of a new value.
            /// </summary>
            public OnNextNotification(T value)
            {
                this.value = value;
            }

            /// <summary>
            /// Returns the value of an OnNext notification.
            /// </summary>
            public override T Value { get { return value; } }

            /// <summary>
            /// Returns null.
            /// </summary>
            public override Exception Exception { get { return null; } }

            /// <summary>
            /// Returns true.
            /// </summary>
            public override bool HasValue { get { return true; } }

            /// <summary>
            /// Returns NotificationKind.OnNext.
            /// </summary>
            public override NotificationKind Kind { get { return NotificationKind.OnNext; } }

            /// <summary>
            /// Returns the hash code for this instance.
            /// </summary>
            public override int GetHashCode()
            {
                return EqualityComparer<T>.Default.GetHashCode(Value);
            }

            /// <summary>
            /// Indicates whether this instance and a specified object are equal.
            /// </summary>
            public override bool Equals(Notification<T> other)
            {
                if (Object.ReferenceEquals(this, other))
                    return true;
                if (Object.ReferenceEquals(other, null))
                    return false;
                if (other.Kind != NotificationKind.OnNext)
                    return false;
                return EqualityComparer<T>.Default.Equals(Value, other.Value);
            }

            /// <summary>
            /// Returns a string representation of this instance.
            /// </summary>
            public override string ToString()
            {
                return String.Format(CultureInfo.CurrentCulture, "OnNext({0})", Value);
            }

            /// <summary>
            /// Invokes the observer's method corresponding to the notification.
            /// </summary>
            /// <param name="observer">Observer to invoke the notification on.</param>
            public override void Accept(IObserver<T> observer)
            {
                if (observer == null)
                    throw new ArgumentNullException("observer");

                observer.OnNext(Value);
            }

            /// <summary>
            /// Invokes the observer's method corresponding to the notification and returns the produced result.
            /// </summary>
            /// <param name="observer">Observer to invoke the notification on.</param>
            /// <returns>Result produced by the observation.</returns>
            public override TResult Accept<TResult>(IObserver<T, TResult> observer)
            {
                if (observer == null)
                    throw new ArgumentNullException("observer");

                return observer.OnNext(Value);
            }

            /// <summary>
            /// Invokes the delegate corresponding to the notification.
            /// </summary>
            /// <param name="onNext">Delegate to invoke for an OnNext notification.</param>
            /// <param name="onError">Delegate to invoke for an OnError notification.</param>
            /// <param name="onCompleted">Delegate to invoke for an OnCompleted notification.</param>
            public override void Accept(Action<T> onNext, Action<Exception> onError, Action onCompleted)
            {
                if (onNext == null)
                    throw new ArgumentNullException("onNext");
                if (onError == null)
                    throw new ArgumentNullException("onError");
                if (onCompleted == null)
                    throw new ArgumentNullException("onCompleted");

                onNext(Value);
            }

            /// <summary>
            /// Invokes the delegate corresponding to the notification and returns the produced result.
            /// </summary>
            /// <param name="onNext">Delegate to invoke for an OnNext notification.</param>
            /// <param name="onError">Delegate to invoke for an OnError notification.</param>
            /// <param name="onCompleted">Delegate to invoke for an OnCompleted notification.</param>
            /// <returns>Result produced by the observation.</returns>
            public override TResult Accept<TResult>(Func<T, TResult> onNext, Func<Exception, TResult> onError, Func<TResult> onCompleted)
            {
                if (onNext == null)
                    throw new ArgumentNullException("onNext");
                if (onError == null)
                    throw new ArgumentNullException("onError");
                if (onCompleted == null)
                    throw new ArgumentNullException("onCompleted");

                return onNext(Value);
            }
        }

        /// <summary>
        /// Represents an OnError notification to an observer.
        /// </summary>
#if !NO_DEBUGGER_ATTRIBUTES
        [DebuggerDisplay("OnError({Exception})")]
#endif
#if !NO_SERIALIZABLE
        [Serializable]
#endif
        internal sealed class OnErrorNotification : Notification<T>
        {
            Exception exception;

            /// <summary>
            /// Constructs a notification of an exception.
            /// </summary>
            public OnErrorNotification(Exception exception)
            {
                this.exception = exception;
            }

            /// <summary>
            /// Throws the exception.
            /// </summary>
            public override T Value { get { exception.Throw(); throw exception; } }

            /// <summary>
            /// Returns the exception.
            /// </summary>
            public override Exception Exception { get { return exception; } }

            /// <summary>
            /// Returns false.
            /// </summary>
            public override bool HasValue { get { return false; } }

            /// <summary>
            /// Returns NotificationKind.OnError.
            /// </summary>
            public override NotificationKind Kind { get { return NotificationKind.OnError; } }

            /// <summary>
            /// Returns the hash code for this instance.
            /// </summary>
            public override int GetHashCode()
            {
                return Exception.GetHashCode();
            }

            /// <summary>
            /// Indicates whether this instance and other are equal.
            /// </summary>
            public override bool Equals(Notification<T> other)
            {
                if (Object.ReferenceEquals(this, other))
                    return true;
                if (Object.ReferenceEquals(other, null))
                    return false;
                if (other.Kind != NotificationKind.OnError)
                    return false;
                return Object.Equals(Exception, other.Exception);
            }

            /// <summary>
            /// Returns a string representation of this instance.
            /// </summary>
            public override string ToString()
            {
                return String.Format(CultureInfo.CurrentCulture, "OnError({0})", Exception.GetType().FullName);
            }

            /// <summary>
            /// Invokes the observer's method corresponding to the notification.
            /// </summary>
            /// <param name="observer">Observer to invoke the notification on.</param>
            public override void Accept(IObserver<T> observer)
            {
                if (observer == null)
                    throw new ArgumentNullException("observer");

                observer.OnError(Exception);
            }

            /// <summary>
            /// Invokes the observer's method corresponding to the notification and returns the produced result.
            /// </summary>
            /// <param name="observer">Observer to invoke the notification on.</param>
            /// <returns>Result produced by the observation.</returns>
            public override TResult Accept<TResult>(IObserver<T, TResult> observer)
            {
                if (observer == null)
                    throw new ArgumentNullException("observer");

                return observer.OnError(Exception);
            }

            /// <summary>
            /// Invokes the delegate corresponding to the notification.
            /// </summary>
            /// <param name="onNext">Delegate to invoke for an OnNext notification.</param>
            /// <param name="onError">Delegate to invoke for an OnError notification.</param>
            /// <param name="onCompleted">Delegate to invoke for an OnCompleted notification.</param>
            public override void Accept(Action<T> onNext, Action<Exception> onError, Action onCompleted)
            {
                if (onNext == null)
                    throw new ArgumentNullException("onNext");
                if (onError == null)
                    throw new ArgumentNullException("onError");
                if (onCompleted == null)
                    throw new ArgumentNullException("onCompleted");

                onError(Exception);
            }

            /// <summary>
            /// Invokes the delegate corresponding to the notification and returns the produced result.
            /// </summary>
            /// <param name="onNext">Delegate to invoke for an OnNext notification.</param>
            /// <param name="onError">Delegate to invoke for an OnError notification.</param>
            /// <param name="onCompleted">Delegate to invoke for an OnCompleted notification.</param>
            /// <returns>Result produced by the observation.</returns>
            public override TResult Accept<TResult>(Func<T, TResult> onNext, Func<Exception, TResult> onError, Func<TResult> onCompleted)
            {
                if (onNext == null)
                    throw new ArgumentNullException("onNext");
                if (onError == null)
                    throw new ArgumentNullException("onError");
                if (onCompleted == null)
                    throw new ArgumentNullException("onCompleted");

                return onError(Exception);
            }
        }

        /// <summary>
        /// Represents an OnCompleted notification to an observer.
        /// </summary>
        [DebuggerDisplay("OnCompleted()")]
        [Serializable]
        internal sealed class OnCompletedNotification : Notification<T>
        {
            /// <summary>
            /// Constructs a notification of the end of a sequence.
            /// </summary>
            public OnCompletedNotification()
            {
            }

            /// <summary>
            /// Throws an InvalidOperationException.
            /// </summary>
            public override T Value { get { throw new InvalidOperationException("No Value"); } }

            /// <summary>
            /// Returns null.
            /// </summary>
            public override Exception Exception { get { return null; } }

            /// <summary>
            /// Returns false.
            /// </summary>
            public override bool HasValue { get { return false; } }

            /// <summary>
            /// Returns NotificationKind.OnCompleted.
            /// </summary>
            public override NotificationKind Kind { get { return NotificationKind.OnCompleted; } }

            /// <summary>
            /// Returns the hash code for this instance.
            /// </summary>
            public override int GetHashCode()
            {
                return typeof(T).GetHashCode() ^ 8510;
            }

            /// <summary>
            /// Indicates whether this instance and other are equal.
            /// </summary>
            public override bool Equals(Notification<T> other)
            {
                if (Object.ReferenceEquals(this, other))
                    return true;
                if (Object.ReferenceEquals(other, null))
                    return false;
                return other.Kind == NotificationKind.OnCompleted;
            }

            /// <summary>
            /// Returns a string representation of this instance.
            /// </summary>
            public override string ToString()
            {
                return "OnCompleted()";
            }

            /// <summary>
            /// Invokes the observer's method corresponding to the notification.
            /// </summary>
            /// <param name="observer">Observer to invoke the notification on.</param>
            public override void Accept(IObserver<T> observer)
            {
                if (observer == null)
                    throw new ArgumentNullException("observer");

                observer.OnCompleted();
            }

            /// <summary>
            /// Invokes the observer's method corresponding to the notification and returns the produced result.
            /// </summary>
            /// <param name="observer">Observer to invoke the notification on.</param>
            /// <returns>Result produced by the observation.</returns>
            public override TResult Accept<TResult>(IObserver<T, TResult> observer)
            {
                if (observer == null)
                    throw new ArgumentNullException("observer");

                return observer.OnCompleted();
            }

            /// <summary>
            /// Invokes the delegate corresponding to the notification.
            /// </summary>
            /// <param name="onNext">Delegate to invoke for an OnNext notification.</param>
            /// <param name="onError">Delegate to invoke for an OnError notification.</param>
            /// <param name="onCompleted">Delegate to invoke for an OnCompleted notification.</param>
            public override void Accept(Action<T> onNext, Action<Exception> onError, Action onCompleted)
            {
                if (onNext == null)
                    throw new ArgumentNullException("onNext");
                if (onError == null)
                    throw new ArgumentNullException("onError");
                if (onCompleted == null)
                    throw new ArgumentNullException("onCompleted");

                onCompleted();
            }

            /// <summary>
            /// Invokes the delegate corresponding to the notification and returns the produced result.
            /// </summary>
            /// <param name="onNext">Delegate to invoke for an OnNext notification.</param>
            /// <param name="onError">Delegate to invoke for an OnError notification.</param>
            /// <param name="onCompleted">Delegate to invoke for an OnCompleted notification.</param>
            /// <returns>Result produced by the observation.</returns>
            public override TResult Accept<TResult>(Func<T, TResult> onNext, Func<Exception, TResult> onError, Func<TResult> onCompleted)
            {
                if (onNext == null)
                    throw new ArgumentNullException("onNext");
                if (onError == null)
                    throw new ArgumentNullException("onError");
                if (onCompleted == null)
                    throw new ArgumentNullException("onCompleted");

                return onCompleted();
            }
        }

        /// <summary>
        /// Determines whether the current Notification&lt;T&gt; object has the same observer message payload as a specified Notification&lt;T&gt; value.
        /// </summary>
        /// <param name="other">An object to compare to the current Notification&lt;T&gt; object.</param>
        /// <returns>true if both Notification&lt;T&gt; objects have the same observer message payload; otherwise, false.</returns>
        /// <remarks>
        /// Equality of Notification&lt;T&gt; objects is based on the equality of the observer message payload they represent, including the notification Kind and the Value or Exception (if any).
        /// This means two Notification&lt;T&gt; objects can be equal even though they don't represent the same observer method call, but have the same Kind and have equal parameters passed to the observer method.
        /// In case one wants to determine whether two Notification&lt;T&gt; objects represent the same observer method call, use Object.ReferenceEquals identity equality instead.
        /// </remarks>
        public abstract bool Equals(Notification<T> other);

        /// <summary>
        /// Determines whether the two specified Notification&lt;T&gt; objects have the same observer message payload.
        /// </summary>
        /// <param name="left">The first Notification&lt;T&gt; to compare, or null.</param>
        /// <param name="right">The second Notification&lt;T&gt; to compare, or null.</param>
        /// <returns>true if the first Notification&lt;T&gt; value has the same observer message payload as the second Notification&lt;T&gt; value; otherwise, false.</returns>
        /// <remarks>
        /// Equality of Notification&lt;T&gt; objects is based on the equality of the observer message payload they represent, including the notification Kind and the Value or Exception (if any).
        /// This means two Notification&lt;T&gt; objects can be equal even though they don't represent the same observer method call, but have the same Kind and have equal parameters passed to the observer method.
        /// In case one wants to determine whether two Notification&lt;T&gt; objects represent the same observer method call, use Object.ReferenceEquals identity equality instead.
        /// </remarks>
        public static bool operator ==(Notification<T> left, Notification<T> right)
        {
            if (object.ReferenceEquals(left, right))
                return true;

            if ((object)left == null || (object)right == null)
                return false;

            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether the two specified Notification&lt;T&gt; objects have a different observer message payload.
        /// </summary>
        /// <param name="left">The first Notification&lt;T&gt; to compare, or null.</param>
        /// <param name="right">The second Notification&lt;T&gt; to compare, or null.</param>
        /// <returns>true if the first Notification&lt;T&gt; value has a different observer message payload as the second Notification&lt;T&gt; value; otherwise, false.</returns>
        /// <remarks>
        /// Equality of Notification&lt;T&gt; objects is based on the equality of the observer message payload they represent, including the notification Kind and the Value or Exception (if any).
        /// This means two Notification&lt;T&gt; objects can be equal even though they don't represent the same observer method call, but have the same Kind and have equal parameters passed to the observer method.
        /// In case one wants to determine whether two Notification&lt;T&gt; objects represent a different observer method call, use Object.ReferenceEquals identity equality instead.
        /// </remarks>
        public static bool operator !=(Notification<T> left, Notification<T> right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Determines whether the specified System.Object is equal to the current Notification&lt;T&gt;.
        /// </summary>
        /// <param name="obj">The System.Object to compare with the current Notification&lt;T&gt;.</param>
        /// <returns>true if the specified System.Object is equal to the current Notification&lt;T&gt;; otherwise, false.</returns>
        /// <remarks>
        /// Equality of Notification&lt;T&gt; objects is based on the equality of the observer message payload they represent, including the notification Kind and the Value or Exception (if any).
        /// This means two Notification&lt;T&gt; objects can be equal even though they don't represent the same observer method call, but have the same Kind and have equal parameters passed to the observer method.
        /// In case one wants to determine whether two Notification&lt;T&gt; objects represent the same observer method call, use Object.ReferenceEquals identity equality instead.
        /// </remarks>
        public override bool Equals(object obj)
        {
            return Equals(obj as Notification<T>);
        }

        /// <summary>
        /// Invokes the observer's method corresponding to the notification.
        /// </summary>
        /// <param name="observer">Observer to invoke the notification on.</param>
        public abstract void Accept(IObserver<T> observer);

        /// <summary>
        /// Invokes the observer's method corresponding to the notification and returns the produced result.
        /// </summary>
        /// <typeparam name="TResult">The type of the result returned from the observer's notification handlers.</typeparam>
        /// <param name="observer">Observer to invoke the notification on.</param>
        /// <returns>Result produced by the observation.</returns>
        public abstract TResult Accept<TResult>(IObserver<T, TResult> observer);

        /// <summary>
        /// Invokes the delegate corresponding to the notification.
        /// </summary>
        /// <param name="onNext">Delegate to invoke for an OnNext notification.</param>
        /// <param name="onError">Delegate to invoke for an OnError notification.</param>
        /// <param name="onCompleted">Delegate to invoke for an OnCompleted notification.</param>
        public abstract void Accept(Action<T> onNext, Action<Exception> onError, Action onCompleted);

        /// <summary>
        /// Invokes the delegate corresponding to the notification and returns the produced result.
        /// </summary>
        /// <typeparam name="TResult">The type of the result returned from the notification handler delegates.</typeparam>
        /// <param name="onNext">Delegate to invoke for an OnNext notification.</param>
        /// <param name="onError">Delegate to invoke for an OnError notification.</param>
        /// <param name="onCompleted">Delegate to invoke for an OnCompleted notification.</param>
        /// <returns>Result produced by the observation.</returns>
        public abstract TResult Accept<TResult>(Func<T, TResult> onNext, Func<Exception, TResult> onError, Func<TResult> onCompleted);

        /// <summary>
        /// Returns an observable sequence with a single notification, using the immediate scheduler.
        /// </summary>
        /// <returns>The observable sequence that surfaces the behavior of the notification upon subscription.</returns>
        public IObservable<T> ToObservable()
        {
            return this.ToObservable(Scheduler.Immediate);
        }

        /// <summary>
        /// Returns an observable sequence with a single notification.
        /// </summary>
        /// <param name="scheduler">Scheduler to send out the notification calls on.</param>
        /// <returns>The observable sequence that surfaces the behavior of the notification upon subscription.</returns>
        public IObservable<T> ToObservable(IScheduler scheduler)
        {
            if (scheduler == null)
                throw new ArgumentNullException("scheduler");

            return Observable.Create<T>(observer => scheduler.Schedule(() =>
            {
                this.Accept(observer);
                if (this.Kind == NotificationKind.OnNext)
                    observer.OnCompleted();
            }));
        }
    }

    /// <summary>
    /// Provides a set of static methods for constructing notifications.
    /// </summary>
    public static class Notification
    {
        /// <summary>
        /// Creates an object that represents an OnNext notification to an observer.
        /// </summary>
        /// <typeparam name="T">The type of the elements received by the observer. Upon dematerialization of the notifications into an observable sequence, this type is used as the element type for the sequence.</typeparam>
        /// <param name="value">The value contained in the notification.</param>
        /// <returns>The OnNext notification containing the value.</returns>
        public static Notification<T> CreateOnNext<T>(T value)
        {
            return new Notification<T>.OnNextNotification(value);
        }

        /// <summary>
        /// Creates an object that represents an OnError notification to an observer.
        /// </summary>
        /// <typeparam name="T">The type of the elements received by the observer. Upon dematerialization of the notifications into an observable sequence, this type is used as the element type for the sequence.</typeparam>
        /// <param name="error">The exception contained in the notification.</param>
        /// <returns>The OnError notification containing the exception.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="error"/> is null.</exception>
        public static Notification<T> CreateOnError<T>(Exception error)
        {
            if (error == null)
                throw new ArgumentNullException("error");

            return new Notification<T>.OnErrorNotification(error);
        }

        /// <summary>
        /// Creates an object that represents an OnCompleted notification to an observer.
        /// </summary>
        /// <typeparam name="T">The type of the elements received by the observer. Upon dematerialization of the notifications into an observable sequence, this type is used as the element type for the sequence.</typeparam>
        /// <returns>The OnCompleted notification.</returns>
        public static Notification<T> CreateOnCompleted<T>()
        {
            return new Notification<T>.OnCompletedNotification();
        }
    }
}

#pragma warning restore 0659
#pragma warning restore 0661