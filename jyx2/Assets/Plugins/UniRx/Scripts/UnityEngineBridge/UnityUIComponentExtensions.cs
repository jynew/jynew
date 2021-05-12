// for uGUI(from 4.6)
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

using System;
using UnityEngine;
using UnityEngine.UI;

namespace UniRx
{
    public static partial class UnityUIComponentExtensions
    {
        public static IDisposable SubscribeToText(this IObservable<string> source, Text text)
        {
            return source.SubscribeWithState(text, (x, t) => t.text = x);
        }

        public static IDisposable SubscribeToText<T>(this IObservable<T> source, Text text)
        {
            return source.SubscribeWithState(text, (x, t) => t.text = x.ToString());
        }

        public static IDisposable SubscribeToText<T>(this IObservable<T> source, Text text, Func<T, string> selector)
        {
            return source.SubscribeWithState2(text, selector, (x, t, s) => t.text = s(x));
        }

        public static IDisposable SubscribeToInteractable(this IObservable<bool> source, Selectable selectable)
        {
            return source.SubscribeWithState(selectable, (x, s) => s.interactable = x);
        }

        /// <summary>Observe onClick event.</summary>
        public static IObservable<Unit> OnClickAsObservable(this Button button)
        {
            return button.onClick.AsObservable();
        }

        /// <summary>Observe onValueChanged with current `isOn` value on subscribe.</summary>
        public static IObservable<bool> OnValueChangedAsObservable(this Toggle toggle)
        {
            // Optimized Defer + StartWith
            return Observable.CreateWithState<bool, Toggle>(toggle, (t, observer) =>
            {
                observer.OnNext(t.isOn);
                return t.onValueChanged.AsObservable().Subscribe(observer);
            });
        }

        /// <summary>Observe onValueChanged with current `value` on subscribe.</summary>
        public static IObservable<float> OnValueChangedAsObservable(this Scrollbar scrollbar)
        {
            return Observable.CreateWithState<float, Scrollbar>(scrollbar, (s, observer) =>
            {
                observer.OnNext(s.value);
                return s.onValueChanged.AsObservable().Subscribe(observer);
            });
        }

        /// <summary>Observe onValueChanged with current `normalizedPosition` value on subscribe.</summary>
        public static IObservable<Vector2> OnValueChangedAsObservable(this ScrollRect scrollRect)
        {
            return Observable.CreateWithState<Vector2, ScrollRect>(scrollRect, (s, observer) =>
            {
                observer.OnNext(s.normalizedPosition);
                return s.onValueChanged.AsObservable().Subscribe(observer);
            });
        }

        /// <summary>Observe onValueChanged with current `value` on subscribe.</summary>
        public static IObservable<float> OnValueChangedAsObservable(this Slider slider)
        {
            return Observable.CreateWithState<float, Slider>(slider, (s, observer) =>
            {
                observer.OnNext(s.value);
                return s.onValueChanged.AsObservable().Subscribe(observer);
            });
        }

        /// <summary>Observe onEndEdit(Submit) event.</summary>
        public static IObservable<string> OnEndEditAsObservable(this InputField inputField)
        {
            return inputField.onEndEdit.AsObservable();
        }

#if (UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
        /// <summary>Observe onValueChange with current `text` value on subscribe.</summary>
        public static IObservable<string> OnValueChangeAsObservable(this InputField inputField)
        {
            return Observable.CreateWithState<string, InputField>(inputField, (i, observer) =>
            {
                observer.OnNext(i.text);
                return i.onValueChange.AsObservable().Subscribe(observer);
            });
        }
#else
        /// <summary>Observe onValueChanged with current `text` value on subscribe.</summary>
        public static IObservable<string> OnValueChangedAsObservable(this InputField inputField)
        {
            return Observable.CreateWithState<string, InputField>(inputField, (i, observer) =>
            {
                observer.OnNext(i.text);
                return i.onValueChanged.AsObservable().Subscribe(observer);
            });
        }
#endif

#if UNITY_5_3_OR_NEWER

        /// <summary>Observe onValueChanged with current `value` on subscribe.</summary>
        public static IObservable<int> OnValueChangedAsObservable(this Dropdown dropdown)
        {
            return Observable.CreateWithState<int, Dropdown>(dropdown, (d, observer) =>
            {
                observer.OnNext(d.value);
                return d.onValueChanged.AsObservable().Subscribe(observer);
            });
        }

#endif
    }
}

#endif