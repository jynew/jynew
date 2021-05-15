// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.DirectionalSprites
{
    /// <summary>
    /// A more complex version of the <see cref="SpriteMovementController"/> which adds running and pushing animations
    /// as well as the ability to actually move around.
    /// </summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/directional-sprites/character-controller">Character Controller</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.DirectionalSprites/SpriteCharacterController
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Directional Sprites - Sprite Character Controller")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(DirectionalSprites) + "/" + nameof(SpriteCharacterController))]
    public sealed class SpriteCharacterController : MonoBehaviour
    {
        /************************************************************************************************************************/

        [Header("Physics")]
        [SerializeField] private CapsuleCollider2D _Collider;
        [SerializeField] private Rigidbody2D _Rigidbody;
        [SerializeField] private float _WalkSpeed = 1;
        [SerializeField] private float _RunSpeed = 2;

        [Header("Animations")]
        [SerializeField] private AnimancerComponent _Animancer;
        [SerializeField] private DirectionalAnimationSet _Idle;
        [SerializeField] private DirectionalAnimationSet _Walk;
        [SerializeField] private DirectionalAnimationSet _Run;
        [SerializeField] private DirectionalAnimationSet _Push;
        [SerializeField] private Vector2 _Facing = Vector2.down;

        private Vector2 _Movement;
        private DirectionalAnimationSet _CurrentAnimationSet;
        private TimeSynchronisationGroup _MovementSynchronisation;

        /************************************************************************************************************************/

        private void Awake()
        {
            _MovementSynchronisation = new TimeSynchronisationGroup(_Animancer) { _Walk, _Run, _Push };
            Play(_Idle);
        }

        /************************************************************************************************************************/

        private void Update()
        {
            _Movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (_Movement != Vector2.zero)
            {
                _Facing = _Movement;
                UpdateMovementState();

                // Snap the movement to the exact directions we have animations for.
                // When using DirectionalAnimationSets this means the character will only move up/right/down/left.
                // But DirectionalAnimationSet8s will allow diagonal movement as well.
                _Movement = _CurrentAnimationSet.Snap(_Movement);
                _Movement = Vector2.ClampMagnitude(_Movement, 1);
            }
            else
            {
                Play(_Idle);
            }
        }

        /************************************************************************************************************************/

        private void Play(DirectionalAnimationSet animations)
        {
            // Store the current time.
            _MovementSynchronisation.StoreTime(_CurrentAnimationSet);

            _CurrentAnimationSet = animations;
            _Animancer.Play(animations.GetClip(_Facing));

            // If the new animation is in the synchronisation group, give it the same time the previous animation had.
            _MovementSynchronisation.SyncTime(_CurrentAnimationSet);
        }

        /************************************************************************************************************************/

        // Pre-allocate an array of contact points so Unity doesn't need to allocate a new one every time we call
        // _Collider.GetContacts. This example will never have more than 4 contact points, but you might consider a
        // higher number in a real game. Even a large number like 64 would be better than making new ones every time.
        private static readonly ContactPoint2D[] Contacts = new ContactPoint2D[4];

        private void UpdateMovementState()
        {
            var contactCount = _Collider.GetContacts(Contacts);
            for (int i = 0; i < contactCount; i++)
            {
                // If we are moving directly towards an object (or within 30 degrees of it), we are pushing it.
                if (Vector2.Angle(Contacts[i].normal, _Movement) > 180 - 30)
                {
                    Play(_Push);
                    return;
                }
            }

            var isRunning = Input.GetButton("Fire3");// Left Shift by default.
            Play(isRunning ? _Run : _Walk);
        }

        /************************************************************************************************************************/

        private void FixedUpdate()
        {
            // Determine the desired speed based on the current animation.
            var speed = _CurrentAnimationSet == _Run ? _RunSpeed : _WalkSpeed;
            _Rigidbody.velocity = _Movement * speed;
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
            if (_Idle == null)
                return;

            AnimancerUtilities.EditModePlay(_Animancer, _Idle.GetClip(_Facing), false);
        }

        /************************************************************************************************************************/
#endif
        /************************************************************************************************************************/
    }
}
