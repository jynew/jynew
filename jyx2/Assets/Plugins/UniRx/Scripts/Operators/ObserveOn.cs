using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
    internal class ObserveOnObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly IScheduler scheduler;

        public ObserveOnObservable(IObservable<T> source, IScheduler scheduler)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            var queueing = scheduler as ISchedulerQueueing;
            if (queueing == null)
            {
                return new ObserveOn(this, observer, cancel).Run();
            }
            else
            {
                return new ObserveOn_(this, queueing, observer, cancel).Run();
            }
        }

        class ObserveOn : OperatorObserverBase<T, T>
        {
            class SchedulableAction : IDisposable
            {
                public Notification<T> data;
                public LinkedListNode<SchedulableAction> node;
                public IDisposable schedule;

                public void Dispose()
                {
                    if (schedule != null)
                        schedule.Dispose();
                    schedule = null;

                    if (node.List != null)
                    {
                        node.List.Remove(node);
                    }
                }

                public bool IsScheduled { get { return schedule != null; } }
            }

            readonly ObserveOnObservable<T> parent;
            readonly LinkedList<SchedulableAction> actions = new LinkedList<SchedulableAction>();
            bool isDisposed;

            public ObserveOn(ObserveOnObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                isDisposed = false;

                var sourceDisposable = parent.source.Subscribe(this);

                return StableCompositeDisposable.Create(sourceDisposable, Disposable.Create(() =>
                {
                    lock (actions)
                    {
                        isDisposed = true;

                        while (actions.Count > 0)
						{
							// Dispose will both cancel the action (if not already running)
							// and remove it from 'actions'
                            actions.First.Value.Dispose();
						}
                    }
                }));
            }

            public override void OnNext(T value)
            {
                QueueAction(new Notification<T>.OnNextNotification(value));
            }

            public override void OnError(Exception error)
            {
                QueueAction(new Notification<T>.OnErrorNotification(error));
            }

            public override void OnCompleted()
            {
                QueueAction(new Notification<T>.OnCompletedNotification());
            }

            private void QueueAction(Notification<T> data)
            {
                var action = new SchedulableAction { data = data };
                lock (actions)
                {
                    if (isDisposed) return;

                    action.node = actions.AddLast(action);
                    ProcessNext();
                }
            }

            private void ProcessNext()
            {
                lock (actions)
                {
                    if (actions.Count == 0 || isDisposed)
                        return;

                    var action = actions.First.Value;

                    if (action.IsScheduled)
                        return;

                    action.schedule = parent.scheduler.Schedule(() =>
                    {
                        try
                        {
                            switch (action.data.Kind)
                            {
                                case NotificationKind.OnNext:
                                    observer.OnNext(action.data.Value);
                                    break;
                                case NotificationKind.OnError:
                                    observer.OnError(action.data.Exception);
                                    break;
                                case NotificationKind.OnCompleted:
                                    observer.OnCompleted();
                                    break;
                            }
                        }
                        finally
                        {
                            lock (actions)
                            {
                                action.Dispose();
                            }

                            if (action.data.Kind == NotificationKind.OnNext)
                                ProcessNext();
                            else
                                Dispose();
                        }
                    });
                }
            }
        }

        class ObserveOn_ : OperatorObserverBase<T, T>
        {
            readonly ObserveOnObservable<T> parent;
            readonly ISchedulerQueueing scheduler;
            readonly BooleanDisposable isDisposed;
            readonly Action<T> onNext;

            public ObserveOn_(ObserveOnObservable<T> parent, ISchedulerQueueing scheduler, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
                this.scheduler = scheduler;
                this.isDisposed = new BooleanDisposable();
                this.onNext = new Action<T>(OnNext_); // cache delegate
            }

            public IDisposable Run()
            {
                var sourceDisposable = parent.source.Subscribe(this);
                return StableCompositeDisposable.Create(sourceDisposable, isDisposed);
            }

            void OnNext_(T value)
            {
                base.observer.OnNext(value);
            }

            void OnError_(Exception error)
            {
                try { observer.OnError(error); } finally { Dispose(); };
            }

            void OnCompleted_(Unit _)
            {
                try { observer.OnCompleted(); } finally { Dispose(); };
            }

            public override void OnNext(T value)
            {
                scheduler.ScheduleQueueing(isDisposed, value, onNext);
            }

            public override void OnError(Exception error)
            {
                scheduler.ScheduleQueueing(isDisposed, error, OnError_);
            }

            public override void OnCompleted()
            {
                scheduler.ScheduleQueueing(isDisposed, Unit.Default, OnCompleted_);
            }
        }
    }
}