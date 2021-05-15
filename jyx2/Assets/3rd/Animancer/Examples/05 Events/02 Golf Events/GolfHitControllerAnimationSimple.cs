// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.Events
{
    /// <summary>
    /// An <see cref="GolfHitController"/> that uses a <see cref="SimpleEventReceiver"/> to recenve an Animation Event
    /// with the function name <c>"Event"</c>.
    /// </summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/events/golf">Golf Events</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.Events/GolfHitControllerAnimationSimple
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Golf Events - Animation Simple")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(Events) + "/" + nameof(GolfHitControllerAnimationSimple))]
    public sealed class GolfHitControllerAnimationSimple : GolfHitController
    {
        /************************************************************************************************************************/

        [SerializeField] private SimpleEventReceiver _EventReceiver;

        /************************************************************************************************************************/

        /// <summary>
        /// Calls the base <see cref="GolfHitController.Awake"/> method and register
        /// <see cref="GolfHitController.EndSwing"/> to be called whenever the swing animation ends.
        /// <para></para>
        /// Normally Animancer could call the registered method at the End Time defined in the transition, but in this
        /// case the <see cref="AnimationClip"/> used with this script has an Animation Event with the Function Name
        /// "End", which will execute the registered method when that event time passes.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            _Swing.Events.OnEnd = EndSwing;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// After starting the swing animation, we also need to give the <see cref="SimpleEventReceiver"/> the callback
        /// that we want it to trigger when it receives an Animation Event with the Function Name "Event".
        /// <para></para>
        /// In this case since there is only one animation with that event we could register it in <see cref="Awake"/>
        /// without tieing it to a specific state, but normally the point of a <see cref="SimpleEventReceiver"/> is to
        /// allow multiple scripts to register their own callback for whatever animation they are playing.
        /// </summary>
        protected override void StartSwing()
        {
            base.StartSwing();

            var state = _Animancer.States.Current;

            // When the Animation Event with the function name "Event" occurs:
            // If the swing animation doesn't have an event with that function name, this will log a warning.
            _EventReceiver.OnEvent.Set(state, (animationEvent) => HitBall());
        }

        /************************************************************************************************************************/
    }
}
