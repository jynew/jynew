// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Animancer.Examples.StateMachines.GameManager
{
    /// <summary>A game manager that acts as an enum-based state machine.</summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/fsm/game-manager">Game Manager</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.StateMachines.GameManager/GameManagerEnum
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Game Manager - Enum")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(StateMachines) + "." + nameof(GameManager) + "/" + nameof(GameManagerEnum))]
    public sealed class GameManagerEnum : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField] private Transform _Camera;
        [SerializeField] private float _IntroductionOrbitSpeed = 45;
        [SerializeField] private float _IntroductionOrbitRadius = 3;
        [SerializeField] private Vector3 _ReadyCameraPosition = new Vector3(0.25f, 1, -2);
        [SerializeField] private Vector3 _ReadyCameraRotation = Vector3.zero;
        [SerializeField] private float _CameraTurnSpeedFactor = 5;
        [SerializeField] private Text _Text;
        [SerializeField] private Image _FadeImage;
        [SerializeField] private float _FadeSpeed = 2;
        [SerializeField] private Events.GolfHitController _Golfer;
        [SerializeField] private Rigidbody _Ball;

        /************************************************************************************************************************/

        public enum State
        {
            /// <summary>Camera orbiting the player, waiting for the player to click.</summary>
            Introduction,

            /// <summary>Waiting for player input to hit the ball.</summary>
            Ready,

            /// <summary>Waiting the the character to hit the ball and the ball to stop.</summary>
            Action,

            /// <summary>Fading the screen to black.</summary>
            FadeOut,

            /// <summary>Fading the screen back in after resetting the ball.</summary>
            FadeIn,
        }

        /************************************************************************************************************************/

        private State _CurrentState;

        public State CurrentState
        {
            get => _CurrentState;
            set
            {
                _CurrentState = value;
                OnEnterState();
            }
        }

        /************************************************************************************************************************/

        private void Awake()
        {
            OnEnterState();
        }

        /************************************************************************************************************************/

        private void OnEnterState()
        {
            switch (_CurrentState)
            {
                case State.Introduction:
                    _Text.gameObject.SetActive(true);
                    _Text.text = "Welcome to the Game Manager example\nClick to start playing";
                    _FadeImage.gameObject.SetActive(false);
                    _Golfer.EndSwing();
                    break;

                case State.Ready:
                    _Camera.position = _ReadyCameraPosition;
                    _Camera.eulerAngles = _ReadyCameraRotation;
                    _Text.gameObject.SetActive(true);
                    _Text.text = "Click to hit the ball";
                    _FadeImage.gameObject.SetActive(false);
                    _Golfer.enabled = true;
                    break;

                case State.Action:
                    _Text.gameObject.SetActive(true);
                    _FadeImage.gameObject.SetActive(false);
                    _Golfer.enabled = false;
                    break;

                case State.FadeOut:
                    _Text.gameObject.SetActive(false);
                    _FadeImage.gameObject.SetActive(true);
                    _FadeImage.color = new Color(0, 0, 0, 0);
                    break;

                case State.FadeIn:
                    _Camera.position = _ReadyCameraPosition;
                    _Camera.eulerAngles = _ReadyCameraRotation;
                    _Text.gameObject.SetActive(false);
                    _FadeImage.gameObject.SetActive(true);
                    _FadeImage.color = new Color(0, 0, 0, 1);
                    _Golfer.ReturnToReady();
                    break;
            }
        }

        /************************************************************************************************************************/

        private void Update()
        {
            switch (_CurrentState)
            {
                case State.Introduction:
                    var euler = _Camera.eulerAngles;
                    euler.y += _IntroductionOrbitSpeed * Time.deltaTime;
                    _Camera.eulerAngles = euler;

                    var lookAt = _Golfer.transform.position;
                    lookAt.y += 1;
                    _Camera.position = lookAt - _Camera.forward * _IntroductionOrbitRadius;

                    if (Input.GetMouseButtonUp(0))
                        CurrentState = State.Ready;

                    break;

                case State.Ready:
                    // The GolfHitController handles its own input now that it's enabled.
                    // So we just wait for it to change states.
                    if (_Golfer.CurrentState != Events.GolfHitController.State.Ready)
                        CurrentState = State.Action;
                    break;

                case State.Action:
                    _Text.text = $"Wait for the ball to stop\nCurrent Speed: {_Ball.velocity.magnitude:0.00}m/s";

                    var targetRotation = Quaternion.LookRotation(_Ball.position - _Camera.position);
                    _Camera.rotation = Quaternion.Slerp(_Camera.rotation, targetRotation, _CameraTurnSpeedFactor * Time.deltaTime);

                    if (_Golfer.CurrentState == Events.GolfHitController.State.Idle &&
                        _Ball.IsSleeping())
                        CurrentState = State.FadeOut;
                    break;

                case State.FadeOut:
                    {
                        var color = _FadeImage.color;
                        color.a = Mathf.MoveTowards(color.a, 1, _FadeSpeed * Time.deltaTime);
                        _FadeImage.color = color;

                        if (color.a == 1)// When the fade ends.
                            CurrentState = State.FadeIn;

                        break;
                    }

                case State.FadeIn:
                    {
                        var color = _FadeImage.color;
                        color.a = Mathf.MoveTowards(color.a, 0, _FadeSpeed * Time.deltaTime);
                        _FadeImage.color = color;

                        if (color.a == 0)// When the fade ends.
                            CurrentState = State.Ready;

                        break;
                    }
            }
        }

        /************************************************************************************************************************/
    }
}
