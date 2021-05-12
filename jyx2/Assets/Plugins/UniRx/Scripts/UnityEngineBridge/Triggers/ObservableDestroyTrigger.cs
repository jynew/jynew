using System; // require keep for Windows Universal App
using UnityEngine;

namespace UniRx.Triggers
{
    [DisallowMultipleComponent]
    public class ObservableDestroyTrigger : MonoBehaviour
    {
        bool calledDestroy = false;
        Subject<Unit> onDestroy;
        CompositeDisposable disposablesOnDestroy;

        [Obsolete("Internal Use.")]
        internal bool IsMonitoredActivate { get; set; }

        public bool IsActivated { get; private set; }

        /// <summary>
        /// Check called OnDestroy.
        /// This property does not guarantees GameObject was destroyed,
        /// when gameObject is deactive, does not raise OnDestroy.
        /// </summary>
        public bool IsCalledOnDestroy { get { return calledDestroy; } }

        void Awake()
        {
            IsActivated = true;
        }

        /// <summary>This function is called when the MonoBehaviour will be destroyed.</summary>
        void OnDestroy()
        {
            if (!calledDestroy)
            {
                calledDestroy = true;
                if (disposablesOnDestroy != null) disposablesOnDestroy.Dispose();
                if (onDestroy != null) { onDestroy.OnNext(Unit.Default); onDestroy.OnCompleted(); }
            }
        }

        /// <summary>This function is called when the MonoBehaviour will be destroyed.</summary>
        public IObservable<Unit> OnDestroyAsObservable()
        {
            if (this == null) return Observable.Return(Unit.Default);
            if (calledDestroy) return Observable.Return(Unit.Default);
            return onDestroy ?? (onDestroy = new Subject<Unit>());
        }

        /// <summary>Invoke OnDestroy, this method is used on internal.</summary>
        public void ForceRaiseOnDestroy()
        {
            OnDestroy();
        }

        public void AddDisposableOnDestroy(IDisposable disposable)
        {
            if (calledDestroy)
            {
                disposable.Dispose();
                return;
            }

            if (disposablesOnDestroy == null) disposablesOnDestroy = new CompositeDisposable();
            disposablesOnDestroy.Add(disposable);
        }
    }
}