// for uGUI(from 4.6)
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

using System;
using UnityEngine;
using UnityEngine.UI;

namespace UniRx.Examples
{
    public class Sample12_ReactiveProperty : MonoBehaviour
    {
        // Open Sample12Scene. Set from canvas
        public Button MyButton;
        public Toggle MyToggle;
        public InputField MyInput;
        public Text MyText;
        public Slider MySlider;

        // You can monitor/modifie in inspector by SpecializedReactiveProperty
        public IntReactiveProperty IntRxProp = new IntReactiveProperty();

        Enemy enemy = new Enemy(1000);

        void Start()
        {
            // UnityEvent as Observable
            // (shortcut, MyButton.OnClickAsObservable())
            MyButton.onClick.AsObservable().Subscribe(_ => enemy.CurrentHp.Value -= 99);

            // Toggle, Input etc as Observable(OnValueChangedAsObservable is helper for provide isOn value on subscribe)
            // SubscribeToInteractable is UniRx.UI Extension Method, same as .interactable = x)
            MyToggle.OnValueChangedAsObservable().SubscribeToInteractable(MyButton);

            // input shows delay after 1 second
#if !(UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
            MyInput.OnValueChangedAsObservable()
#else
            MyInput.OnValueChangeAsObservable()
#endif
                .Where(x => x != null)
                .Delay(TimeSpan.FromSeconds(1))
                .SubscribeToText(MyText); // SubscribeToText is UniRx.UI Extension Method

            // converting for human visibility
            MySlider.OnValueChangedAsObservable()
                .SubscribeToText(MyText, x => Math.Round(x, 2).ToString());

            // from RxProp, CurrentHp changing(Button Click) is observable
            enemy.CurrentHp.SubscribeToText(MyText);
            enemy.IsDead.Where(isDead => isDead == true)
                .Subscribe(_ =>
                {
                    MyToggle.interactable = MyButton.interactable = false;
                });

            // initial text:)
            IntRxProp.SubscribeToText(MyText);
        }
    }

    // Reactive Notification Model
    public class Enemy
    {
        public IReactiveProperty<long> CurrentHp { get; private set; }

        public IReadOnlyReactiveProperty<bool> IsDead { get; private set; }

        public Enemy(int initialHp)
        {
            // Declarative Property
            CurrentHp = new ReactiveProperty<long>(initialHp);
            IsDead = CurrentHp.Select(x => x <= 0).ToReactiveProperty();
        }
    }
}

#endif