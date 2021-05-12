using System;
using System.Collections.Generic;
using System.Threading;

#if CSHARP_7_OR_LATER || (UNITY_2018_3_OR_NEWER && (NET_STANDARD_2_0 || NET_4_6))
using System.Threading.Tasks;
using UniRx.InternalUtil;
#endif
namespace UniRx
{
    public interface IReactiveCommand<T> : IObservable<T>
    {
        IReadOnlyReactiveProperty<bool> CanExecute { get; }
        bool Execute(T parameter);
    }

    public interface IAsyncReactiveCommand<T>
    {
        IReadOnlyReactiveProperty<bool> CanExecute { get; }
        IDisposable Execute(T parameter);
        IDisposable Subscribe(Func<T, IObservable<Unit>> asyncAction);
    }

    /// <summary>
    /// Represents ReactiveCommand&lt;Unit&gt;
    /// </summary>
    public class ReactiveCommand : ReactiveCommand<Unit>
    {
        /// <summary>
        /// CanExecute is always true.
        /// </summary>
        public ReactiveCommand()
            : base()
        { }

        /// <summary>
        /// CanExecute is changed from canExecute sequence.
        /// </summary>
        public ReactiveCommand(IObservable<bool> canExecuteSource, bool initialValue = true)
            : base(canExecuteSource, initialValue)
        {
        }

        /// <summary>Push null to subscribers.</summary>
        public bool Execute()
        {
            return Execute(Unit.Default);
        }

        /// <summary>Force push parameter to subscribers.</summary>
        public void ForceExecute()
        {
            ForceExecute(Unit.Default);
        }
    }

    public class ReactiveCommand<T> : IReactiveCommand<T>, IDisposable
    {
        readonly Subject<T> trigger = new Subject<T>();
        readonly IDisposable canExecuteSubscription;

        ReactiveProperty<bool> canExecute;
        public IReadOnlyReactiveProperty<bool> CanExecute
        {
            get
            {
                return canExecute;
            }
        }

        public bool IsDisposed { get; private set; }

        /// <summary>
        /// CanExecute is always true.
        /// </summary>
        public ReactiveCommand()
        {
            this.canExecute = new ReactiveProperty<bool>(true);
            this.canExecuteSubscription = Disposable.Empty;
        }

        /// <summary>
        /// CanExecute is changed from canExecute sequence.
        /// </summary>
        public ReactiveCommand(IObservable<bool> canExecuteSource, bool initialValue = true)
        {
            this.canExecute = new ReactiveProperty<bool>(initialValue);
            this.canExecuteSubscription = canExecuteSource
                .DistinctUntilChanged()
                .SubscribeWithState(canExecute, (b, c) => c.Value = b);
        }

        /// <summary>Push parameter to subscribers when CanExecute.</summary>
        public bool Execute(T parameter)
        {
            if (canExecute.Value)
            {
                trigger.OnNext(parameter);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>Force push parameter to subscribers.</summary>
        public void ForceExecute(T parameter)
        {
            trigger.OnNext(parameter);
        }

        /// <summary>Subscribe execute.</summary>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            return trigger.Subscribe(observer);
        }

        /// <summary>
        /// Stop all subscription and lock CanExecute is false.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed) return;

            IsDisposed = true;
            canExecute.Dispose();
            trigger.OnCompleted();
            trigger.Dispose();
            canExecuteSubscription.Dispose();
        }
    }

    /// <summary>
    /// Variation of ReactiveCommand, when executing command then CanExecute = false after CanExecute = true.
    /// </summary>
    public class AsyncReactiveCommand : AsyncReactiveCommand<Unit>
    {
        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// </summary>
        public AsyncReactiveCommand()
            : base()
        {

        }

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// </summary>
        public AsyncReactiveCommand(IObservable<bool> canExecuteSource)
            : base(canExecuteSource)
        {
        }

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// The source is shared between other AsyncReactiveCommand.
        /// </summary>
        public AsyncReactiveCommand(IReactiveProperty<bool> sharedCanExecute)
            : base(sharedCanExecute)
        {
        }

        public IDisposable Execute()
        {
            return base.Execute(Unit.Default);
        }
    }

    /// <summary>
    /// Variation of ReactiveCommand, canExecute is changed when executing command then CanExecute = false after CanExecute = true.
    /// </summary>
    public class AsyncReactiveCommand<T> : IAsyncReactiveCommand<T>
    {
        UniRx.InternalUtil.ImmutableList<Func<T, IObservable<Unit>>> asyncActions = UniRx.InternalUtil.ImmutableList<Func<T, IObservable<Unit>>>.Empty;

        readonly object gate = new object();
        readonly IReactiveProperty<bool> canExecuteSource;
        readonly IReadOnlyReactiveProperty<bool> canExecute;

        public IReadOnlyReactiveProperty<bool> CanExecute
        {
            get
            {
                return canExecute;
            }
        }

        public bool IsDisposed { get; private set; }

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// </summary>
        public AsyncReactiveCommand()
        {
            this.canExecuteSource = new ReactiveProperty<bool>(true);
            this.canExecute = canExecuteSource;
        }

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// </summary>
        public AsyncReactiveCommand(IObservable<bool> canExecuteSource)
        {
            this.canExecuteSource = new ReactiveProperty<bool>(true);
            this.canExecute = this.canExecuteSource.CombineLatest(canExecuteSource, (x, y) => x && y).ToReactiveProperty();
        }

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// The source is shared between other AsyncReactiveCommand.
        /// </summary>
        public AsyncReactiveCommand(IReactiveProperty<bool> sharedCanExecute)
        {
            this.canExecuteSource = sharedCanExecute;
            this.canExecute = sharedCanExecute;
        }

        /// <summary>Push parameter to subscribers when CanExecute.</summary>
        public IDisposable Execute(T parameter)
        {
            if (canExecute.Value)
            {
                canExecuteSource.Value = false;
                var a = asyncActions.Data;
                if (a.Length == 1)
                {
                    try
                    {
                        var asyncState = a[0].Invoke(parameter) ?? Observable.ReturnUnit();
                        return asyncState.Finally(() => canExecuteSource.Value = true).Subscribe();
                    }
                    catch
                    {
                        canExecuteSource.Value = true;
                        throw;
                    }
                }
                else
                {
                    var xs = new IObservable<Unit>[a.Length];
                    try
                    {
                        for (int i = 0; i < a.Length; i++)
                        {
                            xs[i] = a[i].Invoke(parameter) ?? Observable.ReturnUnit();
                        }
                    }
                    catch
                    {
                        canExecuteSource.Value = true;
                        throw;
                    }

                    return Observable.WhenAll(xs).Finally(() => canExecuteSource.Value = true).Subscribe();
                }
            }
            else
            {
                return Disposable.Empty;
            }
        }

        /// <summary>Subscribe execute.</summary>
        public IDisposable Subscribe(Func<T, IObservable<Unit>> asyncAction)
        {
            lock (gate)
            {
                asyncActions = asyncActions.Add(asyncAction);
            }

            return new Subscription(this, asyncAction);
        }

        /// <summary>
        /// Stop all subscription and lock CanExecute is false.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed) return;

            IsDisposed = true;
            asyncActions = UniRx.InternalUtil.ImmutableList<Func<T, IObservable<Unit>>>.Empty;
        }
        class Subscription : IDisposable
        {
            readonly AsyncReactiveCommand<T> parent;
            readonly Func<T, IObservable<Unit>> asyncAction;

            public Subscription(AsyncReactiveCommand<T> parent, Func<T, IObservable<Unit>> asyncAction)
            {
                this.parent = parent;
                this.asyncAction = asyncAction;
            }

            public void Dispose()
            {
                lock (parent.gate)
                {
                    parent.asyncActions = parent.asyncActions.Remove(asyncAction);
                }
            }
        }
    }

    public static class ReactiveCommandExtensions
    {
        /// <summary>
        /// Create non parameter commands. CanExecute is changed from canExecute sequence.
        /// </summary>
        public static ReactiveCommand ToReactiveCommand(this IObservable<bool> canExecuteSource, bool initialValue = true)
        {
            return new ReactiveCommand(canExecuteSource, initialValue);
        }

        /// <summary>
        /// Create parametered comamnds. CanExecute is changed from canExecute sequence.
        /// </summary>
        public static ReactiveCommand<T> ToReactiveCommand<T>(this IObservable<bool> canExecuteSource, bool initialValue = true)
        {
            return new ReactiveCommand<T>(canExecuteSource, initialValue);
        }

#if CSHARP_7_OR_LATER || (UNITY_2018_3_OR_NEWER && (NET_STANDARD_2_0 || NET_4_6))

        static readonly Action<object> Callback = CancelCallback;

        static void CancelCallback(object state)
        {
            var tuple = (Tuple<ICancellableTaskCompletionSource, IDisposable>)state;
            tuple.Item2.Dispose();
            tuple.Item1.TrySetCanceled();
        }

        public static Task<T> WaitUntilExecuteAsync<T>(this IReactiveCommand<T> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            var tcs = new CancellableTaskCompletionSource<T>();

            var disposable = new SingleAssignmentDisposable();
            disposable.Disposable = source.Subscribe(x =>
            {
                disposable.Dispose(); // finish subscription.
                tcs.TrySetResult(x);
            }, ex => tcs.TrySetException(ex), () => tcs.TrySetCanceled());

            cancellationToken.Register(Callback, Tuple.Create(tcs, disposable.Disposable), false);

            return tcs.Task;
        }

        public static System.Runtime.CompilerServices.TaskAwaiter<T> GetAwaiter<T>(this IReactiveCommand<T> command)
        {
            return command.WaitUntilExecuteAsync(CancellationToken.None).GetAwaiter();
        }

#endif

#if !UniRxLibrary

        // for uGUI(from 4.6)
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

        /// <summary>
        /// Bind ReactiveCommand to button's interactable and onClick.
        /// </summary>
        public static IDisposable BindTo(this IReactiveCommand<Unit> command, UnityEngine.UI.Button button)
        {
            var d1 = command.CanExecute.SubscribeToInteractable(button);
            var d2 = button.OnClickAsObservable().SubscribeWithState(command, (x, c) => c.Execute(x));
            return StableCompositeDisposable.Create(d1, d2);
        }

        /// <summary>
        /// Bind ReactiveCommand to button's interactable and onClick and register onClick action to command.
        /// </summary>
        public static IDisposable BindToOnClick(this IReactiveCommand<Unit> command, UnityEngine.UI.Button button, Action<Unit> onClick)
        {
            var d1 = command.CanExecute.SubscribeToInteractable(button);
            var d2 = button.OnClickAsObservable().SubscribeWithState(command, (x, c) => c.Execute(x));
            var d3 = command.Subscribe(onClick);

            return StableCompositeDisposable.Create(d1, d2, d3);
        }

        /// <summary>
        /// Bind canExecuteSource to button's interactable and onClick and register onClick action to command.
        /// </summary>
        public static IDisposable BindToButtonOnClick(this IObservable<bool> canExecuteSource, UnityEngine.UI.Button button, Action<Unit> onClick, bool initialValue = true)
        {
            return ToReactiveCommand(canExecuteSource, initialValue).BindToOnClick(button, onClick);
        }

#endif

#endif
    }

    public static class AsyncReactiveCommandExtensions
    {
        public static AsyncReactiveCommand ToAsyncReactiveCommand(this IReactiveProperty<bool> sharedCanExecuteSource)
        {
            return new AsyncReactiveCommand(sharedCanExecuteSource);
        }

        public static AsyncReactiveCommand<T> ToAsyncReactiveCommand<T>(this IReactiveProperty<bool> sharedCanExecuteSource)
        {
            return new AsyncReactiveCommand<T>(sharedCanExecuteSource);
        }

#if CSHARP_7_OR_LATER || (UNITY_2018_3_OR_NEWER && (NET_STANDARD_2_0 || NET_4_6))

        static readonly Action<object> Callback = CancelCallback;

        static void CancelCallback(object state)
        {
            var tuple = (Tuple<ICancellableTaskCompletionSource, IDisposable>)state;
            tuple.Item2.Dispose();
            tuple.Item1.TrySetCanceled();
        }

        public static Task<T> WaitUntilExecuteAsync<T>(this IAsyncReactiveCommand<T> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            var tcs = new CancellableTaskCompletionSource<T>();

            var subscription = source.Subscribe(x => { tcs.TrySetResult(x); return Observable.ReturnUnit(); });
            cancellationToken.Register(Callback, Tuple.Create(tcs, subscription), false);

            return tcs.Task;
        }

        public static System.Runtime.CompilerServices.TaskAwaiter<T> GetAwaiter<T>(this IAsyncReactiveCommand<T> command)
        {
            return command.WaitUntilExecuteAsync(CancellationToken.None).GetAwaiter();
        }

#endif


#if !UniRxLibrary

        // for uGUI(from 4.6)
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

        /// <summary>
        /// Bind AsyncRaectiveCommand to button's interactable and onClick.
        /// </summary>
        public static IDisposable BindTo(this IAsyncReactiveCommand<Unit> command, UnityEngine.UI.Button button)
        {
            var d1 = command.CanExecute.SubscribeToInteractable(button);
            var d2 = button.OnClickAsObservable().SubscribeWithState(command, (x, c) => c.Execute(x));

            return StableCompositeDisposable.Create(d1, d2);
        }

        /// <summary>
        /// Bind AsyncRaectiveCommand to button's interactable and onClick and register async action to command.
        /// </summary>
        public static IDisposable BindToOnClick(this IAsyncReactiveCommand<Unit> command, UnityEngine.UI.Button button, Func<Unit, IObservable<Unit>> asyncOnClick)
        {
            var d1 = command.CanExecute.SubscribeToInteractable(button);
            var d2 = button.OnClickAsObservable().SubscribeWithState(command, (x, c) => c.Execute(x));
            var d3 = command.Subscribe(asyncOnClick);

            return StableCompositeDisposable.Create(d1, d2, d3);
        }

        /// <summary>
        /// Create AsyncReactiveCommand and bind to button's interactable and onClick and register async action to command.
        /// </summary>
        public static IDisposable BindToOnClick(this UnityEngine.UI.Button button, Func<Unit, IObservable<Unit>> asyncOnClick)
        {
            return new AsyncReactiveCommand().BindToOnClick(button, asyncOnClick);
        }

        /// <summary>
        /// Create AsyncReactiveCommand and bind sharedCanExecuteSource source to button's interactable and onClick and register async action to command.
        /// </summary>
        public static IDisposable BindToOnClick(this UnityEngine.UI.Button button, IReactiveProperty<bool> sharedCanExecuteSource, Func<Unit, IObservable<Unit>> asyncOnClick)
        {
            return sharedCanExecuteSource.ToAsyncReactiveCommand().BindToOnClick(button, asyncOnClick);
        }
#endif

#endif
    }
}
