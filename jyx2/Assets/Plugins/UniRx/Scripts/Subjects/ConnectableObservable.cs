using System;

namespace UniRx
{
    public interface IConnectableObservable<T> : IObservable<T>
    {
        IDisposable Connect();
    }

    public static partial class Observable
    {
        class ConnectableObservable<T> : IConnectableObservable<T>
        {
            readonly IObservable<T> source;
            readonly ISubject<T> subject;
            readonly object gate = new object();
            Connection connection;

            public ConnectableObservable(IObservable<T> source, ISubject<T> subject)
            {
                this.source = source.AsObservable();
                this.subject = subject;
            }

            public IDisposable Connect()
            {
                lock (gate)
                {
                    // don't subscribe twice
                    if (connection == null)
                    {
                        var subscription = source.Subscribe(subject);
                        connection = new Connection(this, subscription);
                    }

                    return connection;
                }
            }

            public IDisposable Subscribe(IObserver<T> observer)
            {
                return subject.Subscribe(observer);
            }

            class Connection : IDisposable
            {
                readonly ConnectableObservable<T> parent;
                IDisposable subscription;

                public Connection(ConnectableObservable<T> parent, IDisposable subscription)
                {
                    this.parent = parent;
                    this.subscription = subscription;
                }

                public void Dispose()
                {
                    lock (parent.gate)
                    {
                        if (subscription != null)
                        {
                            subscription.Dispose();
                            subscription = null;
                            parent.connection = null;
                        }
                    }
                }
            }
        }
    }
}