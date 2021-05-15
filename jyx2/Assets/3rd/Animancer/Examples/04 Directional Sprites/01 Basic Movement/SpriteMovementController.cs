// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.DirectionalSprites
{
    /// <summary>
    /// Animates a character to either stand idle or walk using animations defined in
    /// <see cref="DirectionalAnimationSet"/>s.
    /// </summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/directional-sprites/basic-movement">Basic Movement</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.DirectionalSprites/SpriteMovementController
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Directional Sprites - Sprite Movement Controller")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(DirectionalSprites) + "/" + nameof(SpriteMovementController))]
    public sealed class SpriteMovementController : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField] private AnimancerComponent _Animancer;
        [SerializeField] private DirectionalAnimationSet _Idles;
        [SerializeField] private DirectionalAnimationSet _Walks;
        [SerializeField] private Vector2 _Facing = Vector2.down;

        /************************************************************************************************************************/

        private void Awake()
        {
            Play(_Idles);
        }

        /************************************************************************************************************************/

        private void Update()
        {
            // WASD Controls.
            var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (input != Vector2.zero)
            {
                _Facing = input;

                Play(_Walks);

                // Play could return the AnimancerState it gets from _Animancer.Play,
                // But we can also just access it using _Animancer.States.Current.

                var isRunning = Input.GetButton("Fire3");// Left Shift by default.
                _Animancer.States.Current.Speed = isRunning ? 2 : 1;
            }
            else
            {
                // When we are not moving, we still remember the direction we are facing
                // so we can continue using the correct idle animation for that direction.
                Play(_Idles);
            }
        }

        /************************************************************************************************************************/

        private void Play(DirectionalAnimationSet animations)
        {
            // Instead of only a single animation, we have a different one for each direction we can face.
            // So we get whichever is appropriate for that direction and play it.

            var clip = animations.GetClip(_Facing);
            _Animancer.Play(clip);

            // Or we could do that in one line:
            // _Animancer.Play(animations.GetClip(_Facing));
        }

        /************************************************************************************************************************/
#if UNITY_EDITOR
        /************************************************************************************************************************/

        /// <summary>[Editor-Only]
        /// Sets the character's starting sprite in Edit Mode so you can see it while working in the scene.
        /// </summary>
        /// <remarks>
        /// Called by the Unity Editor in Edit Mode whenever an instance of this script is loaded or a value is changed
        /// in the Inspector.
        /// </remarks>
        private void OnValidate()
        {
            if (_Idles == null)
                return;

            AnimancerUtilities.EditModePlay(_Animancer, _Idles.GetClip(_Facing), true);
        }

        /************************************************************************************************************************/
#endif
        /************************************************************************************************************************/
    }
}
