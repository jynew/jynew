// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.Events
{
    /// <summary>A <see cref="GolfHitController"/> that uses Animation Events.</summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/events/golf">Golf Events</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.Events/GolfHitControllerAnimation
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Golf Events - Animation")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(Events) + "/" + nameof(GolfHitControllerAnimation))]
    public sealed class GolfHitControllerAnimation : GolfHitController
    {
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
        /// Calls <see cref="GolfHitController.HitBall"/>. The <see cref="AnimationClip"/> used with this script has an
        /// Animation Event with the Function Name "Event", which will cause it to execute this method.
        /// <para></para>
        /// Normally you would just have the event use "HitBall" as its Function Name directly, but the same animation
        /// is also being used for <see cref="GolfHitControllerAnimationSimple"/> which relies on the Function Name
        /// being "Event".
        /// </summary>
        private void Event()
        {
            HitBall();
        }

        /************************************************************************************************************************/

        /// <summary>Calls <see cref="EndEventReceiver.End"/>.</summary>
        /// <remarks>
        /// Called by Animation Events with the Function Name "End".
        /// <para></para>
        /// Note that Unity will allocate some garbage every time it triggers an Animation Event with an
        /// <see cref="AnimationEvent"/> parameter.
        /// </remarks>
        private void End(AnimationEvent animationEvent)
        {
            EndEventReceiver.End(_Animancer, animationEvent);
        }

        /************************************************************************************************************************/
    }
}
