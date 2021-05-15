// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using Animancer.Examples.FineControl;
using UnityEngine;

namespace Animancer.Examples.Locomotion
{
    /// <summary>
    /// A <see cref="SpiderBot"/> with a <see cref="MixerState.Transition2D"/> and <see cref="Rigidbody"/> to allow the
    /// bot to move in any direction.
    /// </summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/locomotion/directional-blending">Directional Blending</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.Locomotion/SpiderBotAdvanced
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Locomotion - Spider Bot Advanced")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(Locomotion) + "/" + nameof(SpiderBotAdvanced))]
    public sealed class SpiderBotAdvanced : SpiderBot
    {
        /************************************************************************************************************************/

        [SerializeField] private Rigidbody _Body;
        [SerializeField] private float _TurnSpeed = 90;
        [SerializeField] private float _MovementSpeed = 1.5f;
        [SerializeField] private float _SprintMultiplier = 2;

        /************************************************************************************************************************/

        [SerializeField]
        private MixerState.Transition2D _Move;

        protected override ITransition MovementAnimation => _Move;

        /************************************************************************************************************************/

        private Vector3 _MovementDirection;

        protected override bool IsMoving => _MovementDirection != Vector3.zero;

        /************************************************************************************************************************/

        protected override void Awake()
        {
            base.Awake();

            // Create the movement state but don't play it yet.
            // This ensures that we can access _MovementAnimation.State in other methods before actually playing it.
            Animancer.States.GetOrCreate(_Move);
        }

        /************************************************************************************************************************/

        protected override void Update()
        {
            // Calculate the movement direction and call the base method to wake up or go to sleep if necessary.
            _MovementDirection = GetMovementDirection();
            base.Update();

            // If the movement state is playing and not fading out:
            if (_Move.State.IsActive)
            {
                // Rotate towards the same Y angle as the camera.
                var eulerAngles = transform.eulerAngles;
                var targetEulerY = Camera.main.transform.eulerAngles.y;
                eulerAngles.y = Mathf.MoveTowardsAngle(eulerAngles.y, targetEulerY, _TurnSpeed * Time.deltaTime);
                transform.eulerAngles = eulerAngles;

                // The movement direction is in world space, so we need to convert it to local space to be appropriate
                // for the current rotation by using dot-products to determine how much of that direction lies along
                // each axis. This would be unnecessary if we did not rotate at all.
                _Move.State.Parameter = new Vector2(
                    Vector3.Dot(transform.right, _MovementDirection),
                    Vector3.Dot(transform.forward, _MovementDirection));

                // Set its speed depending on whether you are sprinting or not.
                var isSprinting = Input.GetMouseButton(0);
                _Move.State.Speed = isSprinting ? _SprintMultiplier : 1;
            }
            else// Otherwise stop it entirely.
            {
                _Move.State.Parameter = Vector2.zero;
                _Move.State.Speed = 0;
            }
        }

        /************************************************************************************************************************/

        private Vector3 GetMovementDirection()
        {
            // Get a ray from the main camera in the direction of the mouse cursor.
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Do a raycast with it and stop trying to move it it does not hit anything.
            // Note that this object is set to the Ignore Raycast layer so that the raycast will not hit it.
            if (!Physics.Raycast(ray, out var raycastHit))// Note the exclamation mark !
                return Vector3.zero;

            // If the ray hit something, calculate the horizontal direction from this object to that point.
            var direction = raycastHit.point - transform.position;
            direction.y = 0;

            // If we are close to the destination, stop moving.
            // We could use an arbitrary small value like 0.1, but that would not work if the speed is too high.
            // Instead, we can calculate the distance it will actually move in a single frame at max speed to determine
            // if it would arrive or pass the destination next frame.
            var distance = direction.magnitude;
            if (distance < _MovementSpeed * _SprintMultiplier * Time.fixedDeltaTime)
            {
                return Vector3.zero;
            }
            else
            {
                // Otherwise normalize the direction so that we do not change speed based on distance.
                // Calling direction.Normalize() would do the same thing, but would calculate the magnitude again.
                return direction / distance;
            }
        }

        /************************************************************************************************************************/

        private void FixedUpdate()
        {
            // Move the rigidbody in the desired direction.
            _Body.velocity = _MovementDirection * _Move.State.Speed * _MovementSpeed;
        }

        /************************************************************************************************************************/
    }
}
