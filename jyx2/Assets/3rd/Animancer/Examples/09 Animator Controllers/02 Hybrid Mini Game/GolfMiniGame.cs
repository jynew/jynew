// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using Animancer.Examples.StateMachines.Brains;
using Animancer.FSM;
using UnityEngine;

namespace Animancer.Examples.AnimatorControllers
{
    /// <summary>
    /// A <see cref="CreatureState"/> which allows the player to play golf using the
    /// <see cref="Events.GolfHitController"/> script.
    /// </summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/animator-controllers/mini-game">Hybrid Mini Game</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.AnimatorControllers/GolfMiniGame
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Hybrid - Golf Mini Game")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(AnimatorControllers) + "/" + nameof(GolfMiniGame))]
    public sealed class GolfMiniGame : CreatureBrain
    {
        /************************************************************************************************************************/

        [SerializeField] private Events.GolfHitController _GolfHitController;
        [SerializeField] private Transform _GolfClub;
        [SerializeField] private Transform _ExitPoint;
        [SerializeField] private GameObject _RegularControls;
        [SerializeField] private GameObject _GolfControls;

        private Vector3 _GolfClubStartPosition;
        private Quaternion _GolfClubStartRotation;
        private CreatureBrain _PreviousBrain;

        private enum State { Entering, Turning, Playing, Exiting, }
        private State _State;

        /************************************************************************************************************************/

        private void Awake()
        {
            _GolfClubStartPosition = _GolfClub.localPosition;
            _GolfClubStartRotation = _GolfClub.localRotation;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// When a <see cref="Creature"/> enters this trigger, try to make it enter this state.
        /// </summary>
        private void OnTriggerEnter(Collider collider)
        {
            if (enabled)
                return;

            var creature = collider.GetComponent<Creature>();
            if (creature == null ||
                !creature.Idle.TryEnterState())
                return;

            _State = State.Entering;
            _PreviousBrain = creature.Brain;
            Creature = creature;
        }

        /************************************************************************************************************************/

        private void FixedUpdate()
        {
            switch (_State)
            {
                case State.Entering:
                    if (MoveTowards(_GolfHitController.transform.position))
                        StartTurning();
                    break;

                case State.Turning:
                    if (Quaternion.Angle(Creature.Animancer.transform.rotation, _GolfHitController.transform.rotation) < 1)
                        StartPlaying();
                    break;

                case State.Playing:
                    break;

                case State.Exiting:
                    if (MoveTowards(_ExitPoint.position))
                        Creature.Brain = _PreviousBrain;
                    break;
            }
        }

        /************************************************************************************************************************/

        private bool MoveTowards(Vector3 destination)
        {
            var step = Creature.Stats.GetMoveSpeed(false) * Time.deltaTime;
            var direction = destination - Creature.Rigidbody.position;
            var distance = direction.magnitude;
            MovementDirection = direction / distance;// Normalize.
            return distance <= step;
        }

        /************************************************************************************************************************/

        private void StartTurning()
        {
            _State = State.Turning;
            MovementDirection = _GolfHitController.transform.forward;

            // Disable the Creature's movement and move them next to the golf ball.
            Creature.Rigidbody.velocity = Vector3.zero;
            Creature.Rigidbody.isKinematic = true;
            Creature.Rigidbody.position = _GolfHitController.transform.position;
        }

        /************************************************************************************************************************/

        private void StartPlaying()
        {
            _State = State.Playing;

            // Put the GolfClub in their hand, specifically as a child of the "RightHandHolder" object which is positioned
            // correctly for holding objects.
            const string HolderName = "RightHandHolder";
            var rightHand = Creature.Animancer.Animator.GetBoneTransform(HumanBodyBones.RightHand);
            rightHand = rightHand.Find(HolderName);
            Debug.Assert(rightHand != null, "Unable to find " + HolderName);

            _GolfClub.parent = rightHand;
            _GolfClub.localPosition = Vector3.zero;
            _GolfClub.localRotation = Quaternion.identity;

            // Activate the GolfHitController so it can now take control of the creature.
            _GolfHitController.gameObject.SetActive(true);

            // Swap the displayed controls.
            _RegularControls.SetActive(false);
            _GolfControls.SetActive(true);
        }

        /************************************************************************************************************************/

        public void Quit()
        {
            // Basically just undo everything StartTurning and StartPlaying did.

            _State = State.Exiting;

            _GolfHitController.gameObject.SetActive(false);
            _RegularControls.SetActive(true);
            _GolfControls.SetActive(false);

            _GolfClub.parent = transform;
            _GolfClub.localPosition = _GolfClubStartPosition;
            _GolfClub.localRotation = _GolfClubStartRotation;

            Creature.Rigidbody.isKinematic = false;

            // The creature will still be in the Idle state from when the mini game started.
            // But it only plays its animation when first entered so we force the creature to re-enter that state.
            Creature.Idle.TryReEnterState();
        }

        /************************************************************************************************************************/
    }
}
