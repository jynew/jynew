// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using System;
using UnityEngine;

namespace Animancer.Examples.Events
{
    /// <summary>
    /// Base class for various scripts that use different event systems to have a character hit a golf ball.
    /// </summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/events/golf">Golf Events</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.Events/GolfHitController
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Golf Events - Golf Hit Controller")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(Events) + "/" + nameof(GolfHitController))]
    public abstract class GolfHitController : MonoBehaviour
    {
        /************************************************************************************************************************/

        // Normally it would be good to make read-only properties to wrap fields you want other classes to access so
        // that those classes do not accidentally change any of the fields this script does not expect to change.
        // But for this example, it is easier to just let the inheriting classes access protected fields directly.

        [SerializeField] protected AnimancerComponent _Animancer;
        [SerializeField] protected ClipState.Transition _Ready;
        [SerializeField] protected ClipState.Transition _Swing;
        [SerializeField] protected ClipState.Transition _Idle;
        [SerializeField] private Rigidbody _Ball;
        [SerializeField] private Vector3 _HitVelocity;
        [SerializeField] private AudioSource _HitSound;

        /************************************************************************************************************************/

        public enum State { Ready, Swing, Idle, }

        public State CurrentState { get; private set; }

        private Vector3 _BallStartPosition;

        /************************************************************************************************************************/

        /// <summary>
        /// Stores the position of the ball on startup so that it can be teleported back there when necessary.
        /// <para></para>
        /// This method is <c>virtual</c> in case any inheriting scripts need to do anything else on startup.
        /// <para></para>
        /// Most of them register <see cref="EndSwing"/> to be called when the <see cref="_Swing"/> animation ends,
        /// but <see cref="GolfHitControllerAnimancer"/> assumes that the event was already set up in the Inspector.
        /// </summary>
        protected virtual void Awake()
        {
            _BallStartPosition = _Ball.position;

            // A "Kinematic" Rigidbody essentially means that it is not currently being controlled by physics.
            // So while the character is ready we make the ball Kinematic to prevent it from rolling away.
            // Then when they hit the ball we set isKinematic = false to let regular physics take over.
            _Ball.isKinematic = true;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// After <see cref="Awake"/>, we also want to enter the ready state on startup.
        /// <para></para>
        /// The difference is that this method is called every time the object is enabled instead of only the first
        /// time. It does not matter in the Golf Events example, but the Hybrid Mini Game example reuses this script and
        /// deactivates it while the Mini Game is not being played so we want to always enter the ready state when the
        /// Mini Game starts.
        /// </summary>
        /// <remarks>
        /// The contents of the <see cref="ReturnToReady"/> method could simply be here instead of needing a separate
        /// method, but when other methods (like <see cref="Update"/>) call it we would rather be clear that we
        /// specifically want to "return to the ready state" instead of some arbitrary "do what we did on startup".
        /// <para></para>
        /// Also note that this method is <c>protected</c> instead of <c>private</c>. Being <c>protected</c> allows
        /// inheriting classes to call it which we do not particularly want, but it also means that if such a class
        /// tries to declare its own <see cref="OnEnable"/> method the compiler will give them a warning that this
        /// method already exists so they can come and make this method <c>virtual</c> if necessary. Otherwise Unity
        /// would call the <see cref="OnEnable"/> method in the derived class but not this one, which would very likely
        /// lead to errors that can be annoying to track down.
        /// </remarks>
        protected void OnEnable()
        {
            ReturnToReady();
        }

        /************************************************************************************************************************/

        /// <summary>
        /// When the player clicks the mouse, go to the next <see cref="State"/>: Ready -> Swing -> Idle -> Ready.
        /// </summary>
        /// <remarks>
        /// This method is <c>protected</c> for the same reason as <see cref="OnEnable"/>.
        /// </remarks>
        protected void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                switch (CurrentState)
                {
                    case State.Ready: StartSwing(); break;
                    case State.Swing: TryCancelSwing(); break;
                    case State.Idle: ReturnToReady(); break;
                    default: throw new ArgumentException("Unsupported State: " + CurrentState);
                }
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Enter the swing state and play the appropriate animation.
        /// <para></para>
        /// This method is <c>virtual</c> so that <see cref="GolfHitControllerAnimationSimple"/> can <c>override</c> it
        /// to register the <see cref="HitBall"/> method to be called by the event.
        /// </summary>
        protected virtual void StartSwing()
        {
            CurrentState = State.Swing;
            _Animancer.Play(_Swing);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// If the ball has not been hit yet, the swing can be cancelled to immediately return to the ready state. But
        /// after the ball has been hit, the character must fully complete the swing animation.
        /// </summary>
        private void TryCancelSwing()
        {
            if (_Ball.isKinematic)
            {
                CurrentState = State.Ready;
                _Animancer.Play(_Ready);
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// When the player clicks the mouse again after entering the idle state, we return to the ready state,
        /// teleport the ball back to its starting position, and make it Kinematic again so it does not roll away.
        /// </summary>
        public void ReturnToReady()
        {
            CurrentState = State.Ready;
            _Animancer.Play(_Ready);

            _Ball.isKinematic = true;
            _Ball.position = _BallStartPosition;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Once the swing animation is started, each of the classes that inherit from this one use a different system
        /// for determining how this method gets called.
        /// </summary>
        public void HitBall()
        {
            _Ball.isKinematic = false;

            // In a real golf game you would probably calculate the hit velocity based on player input.
            _Ball.velocity = _HitVelocity;

            _HitSound.Play();
        }

        /************************************************************************************************************************/

        /// <summary>
        /// As with <see cref="HitBall"/>, this method is called in various different ways depending on which event
        /// system is being used.
        /// <para></para>
        /// Most of them register this method to be called when the <see cref="_Swing"/> animation ends, but
        /// <see cref="GolfHitControllerAnimancer"/> assumes that the event was already set up in the Inspector.
        /// </summary>
        public void EndSwing()
        {
            CurrentState = State.Idle;

            // Since the swing animation is ending early, we want it to calculate the fade duration to fade out over
            // the remainder of that animation instead of the value specified by the _Idle transition.
            var fadeDuration = EndEventReceiver.GetFadeOutDuration();
            _Animancer.Play(_Idle, fadeDuration);
        }

        /************************************************************************************************************************/
    }
}
