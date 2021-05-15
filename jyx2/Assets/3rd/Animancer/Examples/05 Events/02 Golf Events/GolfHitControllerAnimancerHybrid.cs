// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.Events
{
    /// <summary>
    /// An <see cref="GolfHitController"/> that uses an Animancer Event which has its time set in the Inspector but its
    /// callback left blank so that it can be assigned by code (a "hybrid" between Inspector and Code based systems).
    /// </summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/events/golf">Golf Events</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.Events/GolfHitControllerAnimancerHybrid
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Golf Events - Animancer Hybrid")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(Events) + "/" + nameof(GolfHitControllerAnimancerHybrid))]
    [EventNames(HitEventName)]
    public sealed class GolfHitControllerAnimancerHybrid : GolfHitController
    {
        /************************************************************************************************************************/

        /// <summary>
        /// The <see href="https://kybernetik.com.au/animancer/api/Animancer/EventNamesAttribute">EventNamesAttribute</see>
        /// on this class will replace the text field usually used for the Event Name of any events on this component
        /// with a dropdown menu containing this value.
        /// </summary>
        public const string HitEventName = "Hit";

        /************************************************************************************************************************/

        /// <summary>
        /// Calls the base <see cref="GolfHitController.Awake"/> method and register
        /// <see cref="GolfHitController.EndSwing"/> to be called whenever the swing animation ends.
        /// <para></para>
        /// The <see cref="GolfHitController._Swing"/> transition has its End Time set so that it will execute the
        /// registered method at some point during the animation, but its End Callback was left blank so it can be
        /// assigned here.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            // Set the callback for the hit event using the constant from the base class which the [EventNames]
            // attribute will show in a dropdown menu in the Inspector when editing the event.
            // If no event exists with that name, this would throw an exception.
            _Swing.Events.SetCallback(HitEventName, HitBall);

            // Or we could just use a magic string, but that is less safe:
            // _Swing.Events.SetCallback("Hit", HitBall);

            // If we did not set the event's Name in the Inspector, we could just access it by index:
            // _Swing.Events.SetCallback(0, HitBall);
            // But that hard-codes the assumption that there will not be any other events before the one we want.

            // Or if we did not create the event in the Inspector, we could add it here:
            // _Swing.Events.Add(new AnimancerEvent(0.375f, OnHitBall));

            // Also set the end event's callback:
            _Swing.Events.OnEnd = EndSwing;
            // _Swing.Events.endEvent.callback = EndSwing;// Same thing, but slightly longer.
            // _Swing.Events.endEvent = new AnimancerEvent(0.7f, EndSwing);
        }

        /************************************************************************************************************************/
    }
}
