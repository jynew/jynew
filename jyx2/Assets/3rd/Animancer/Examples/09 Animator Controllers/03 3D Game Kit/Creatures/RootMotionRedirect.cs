// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.AnimatorControllers.GameKit
{
    /// <summary>
    /// Takes the root motion from the <see cref="Animator"/> attached to the same <see cref="GameObject"/> and applies
    /// it to a <see cref="Creature"/> on a different object.
    /// </summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/animator-controllers/3d-game-kit">3D Game Kit</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.AnimatorControllers.GameKit/RootMotionRedirect
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Game Kit - Root Motion Redirect")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(AnimatorControllers) + "." + nameof(GameKit) + "/" + nameof(RootMotionRedirect))]
    public sealed class RootMotionRedirect : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField]
        private Creature _Creature;

        /************************************************************************************************************************/

        private void OnAnimatorMove()
        {
            _Creature.OnAnimatorMove();
        }

        /************************************************************************************************************************/

        // Ignore these Animation Events because the attack animations will only start when we tell them to, so it
        // would be silly to use additional events for something we already directly caused. That sort of thing is only
        // necessary in Animator Controllers because they run their own logic to decide what they want to do.
        private void MeleeAttackStart(int throwing = 0) { }
        private void MeleeAttackEnd() { }

        /************************************************************************************************************************/
    }
}
