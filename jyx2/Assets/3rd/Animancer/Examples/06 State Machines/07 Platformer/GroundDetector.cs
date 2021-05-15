// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.StateMachines.Platformer
{
    /// <summary>Keeps track of whether or not an object is touching the ground.</summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/fsm/platformer">Platformer</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.StateMachines.Platformer/GroundDetector
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Platformer - Ground Detector")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(StateMachines) + "." + nameof(Platformer) + "/" + nameof(GroundDetector))]
    public sealed class GroundDetector : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField] private Collider2D _Collider;
        [SerializeField] private float _GripAngle = 45;

        /************************************************************************************************************************/

        public bool IsGrounded { get; private set; }

        /************************************************************************************************************************/

        private static ContactPoint2D[] _Contacts = new ContactPoint2D[8];

        private void FixedUpdate()
        {
            // A label for the goto statement to jump to.
            GetContacts:

            // Get the contacts.
            var contactCount = _Collider.GetContacts(_Contacts);

            // If the array is full, double its size, log a message, and go back to get the contacts again.
            if (contactCount >= _Contacts.Length)
            {
                _Contacts = new ContactPoint2D[_Contacts.Length * 2];

                // If you see this message while testing, you should increase the starting size.
                Debug.LogWarning("_Contacts array is full. Increased size to " + _Contacts.Length, this);

                goto GetContacts;
            }

            for (int i = 0; i < contactCount; i++)
            {
                // As long as at least one contact point has a normal close to straight up, we are grounded.
                var contact = _Contacts[i];
                if (Vector2.Angle(contact.normal, Vector2.up) < _GripAngle)
                {
                    IsGrounded = true;
                    return;
                }
            }

            IsGrounded = false;
        }

        /************************************************************************************************************************/
    }
}
