// Copyright (c) 2016 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
// Based on Unity StandaloneInputModule.cs
// https://bitbucket.org/Unity-Technologies/ui/src

#region Defines
#if UNITY_2020 || UNITY_2021 || UNITY_2022 || UNITY_2023 || UNITY_2024 || UNITY_2025
#define UNITY_2020_PLUS
#endif
#if UNITY_2019 || UNITY_2020_PLUS
#define UNITY_2019_PLUS
#endif
#if UNITY_2018 || UNITY_2019_PLUS
#define UNITY_2018_PLUS
#endif
#if UNITY_2017 || UNITY_2018_PLUS
#define UNITY_2017_PLUS
#endif
#if UNITY_5 || UNITY_2017_PLUS
#define UNITY_5_PLUS
#endif
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_1_PLUS
#endif
#if UNITY_5_2 || UNITY_5_3_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_2_PLUS
#endif
#if UNITY_5_3_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_3_PLUS
#endif
#if UNITY_5_4_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_4_PLUS
#endif
#if UNITY_5_5_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_5_PLUS
#endif
#if UNITY_5_6_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_6_PLUS
#endif
#if UNITY_5_7_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_7_PLUS
#endif
#if UNITY_5_8_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_8_PLUS
#endif
#if UNITY_5_9_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_9_PLUS
#endif
#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649
#endregion

namespace Rewired.Integration.UnityUI {
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.Serialization;
    using System.Collections.Generic;
    using Rewired.UI;

    [AddComponentMenu("Rewired/Rewired Standalone Input Module")]
    public sealed class RewiredStandaloneInputModule : RewiredPointerInputModule {

        #region Rewired Constants

        private const string DEFAULT_ACTION_MOVE_HORIZONTAL = "UIHorizontal";
        private const string DEFAULT_ACTION_MOVE_VERTICAL = "UIVertical";
        private const string DEFAULT_ACTION_SUBMIT = "UISubmit";
        private const string DEFAULT_ACTION_CANCEL = "UICancel";

        #endregion

        #region Rewired Inspector Variables

        [Tooltip("(Optional) Link the Rewired Input Manager here for easier access to Player ids, etc.")]
        [SerializeField]
        private InputManager_Base rewiredInputManager;

        /// <summary>
        /// Allow all Rewired game Players to control the UI. This does not include the System Player. If enabled, this setting overrides individual Player Ids set in Rewired Player Ids.
        /// </summary>
        [SerializeField]
        [Tooltip("Use all Rewired game Players to control the UI. This does not include the System Player. If enabled, this setting overrides individual Player Ids set in Rewired Player Ids.")]
        private bool useAllRewiredGamePlayers = false;

        /// <summary>
        /// Allow the Rewired System Player to control the UI.
        /// </summary>
        [SerializeField]
        [Tooltip("Allow the Rewired System Player to control the UI.")]
        private bool useRewiredSystemPlayer = false;

        /// <summary>
        /// A list of Player Ids that are allowed to control the UI. If Use All Rewired Game Players = True, this list will be ignored.
        /// </summary>
        [SerializeField]
        [Tooltip("A list of Player Ids that are allowed to control the UI. If Use All Rewired Game Players = True, this list will be ignored.")]
        private int[] rewiredPlayerIds = new int[1] { 0 };

        /// <summary>
        /// Allow only Players with Player.isPlaying = true to control the UI.
        /// </summary>
        [SerializeField] 
        [Tooltip("Allow only Players with Player.isPlaying = true to control the UI.")]
        private bool usePlayingPlayersOnly = false;

        /// <summary>
        /// Player Mice allowed to interact with the UI.
        /// </summary>
        [SerializeField]
        [Tooltip("Player Mice allowed to interact with the UI. Each Player that owns a Player Mouse must also be allowed to control the UI or the Player Mouse will not function.")]
        private List<Rewired.Components.PlayerMouse> playerMice = new List<Rewired.Components.PlayerMouse>();

        /// <summary>
        /// Makes an axis press always move only one UI selection. Enable if you do not want to allow scrolling through UI elements by holding an axis direction.
        /// </summary>
        [SerializeField]
        [Tooltip("Makes an axis press always move only one UI selection. Enable if you do not want to allow scrolling through UI elements by holding an axis direction.")]
        private bool moveOneElementPerAxisPress;

        /// <summary>
        /// If enabled, Action Ids will be used to set the Actions. If disabled, string names will be used to set the Actions.
        /// </summary>
        [SerializeField]
        [Tooltip("If enabled, Action Ids will be used to set the Actions. If disabled, string names will be used to set the Actions.")]
        private bool setActionsById = false;

        /// <summary>
        /// Name of the horizontal axis for movement (if axis events are used).
        /// </summary>
        [SerializeField]
        [Tooltip("Id of the horizontal Action for movement (if axis events are used).")]
        private int horizontalActionId = -1;

        /// <summary>
        /// Name of the vertical axis for movement (if axis events are used).
        /// </summary>
        [SerializeField]
        [Tooltip("Id of the vertical Action for movement (if axis events are used).")]
        private int verticalActionId = -1;

        /// <summary>
        /// Name of the action used to submit.
        /// </summary>
        [SerializeField]
        [Tooltip("Id of the Action used to submit.")]
        private int submitActionId = -1;

        /// <summary>
        /// Name of the action used to cancel.
        /// </summary>
        [SerializeField]
        [Tooltip("Id of the Action used to cancel.")]
        private int cancelActionId = -1;

        #endregion

        #region StandaloneInputModule Inspector Variables

        /// <summary>
        /// Name of the horizontal axis for movement (if axis events are used).
        /// </summary>
        [SerializeField]
        [Tooltip("Name of the horizontal axis for movement (if axis events are used).")]
        private string m_HorizontalAxis = DEFAULT_ACTION_MOVE_HORIZONTAL;

        /// <summary>
        /// Name of the vertical axis for movement (if axis events are used).
        /// </summary>
        [SerializeField]
        [Tooltip("Name of the vertical axis for movement (if axis events are used).")]
        private string m_VerticalAxis = DEFAULT_ACTION_MOVE_VERTICAL;

        /// <summary>
        /// Name of the action used to submit.
        /// </summary>
        [SerializeField]
        [Tooltip("Name of the action used to submit.")]
        private string m_SubmitButton = DEFAULT_ACTION_SUBMIT;

        /// <summary>
        /// Name of the action used to cancel.
        /// </summary>
        [SerializeField]
        [Tooltip("Name of the action used to cancel.")]
        private string m_CancelButton = DEFAULT_ACTION_CANCEL;

        /// <summary>
        /// Number of selection changes allowed per second when a movement button/axis is held in a direction.
        /// </summary>
        [SerializeField]
        [Tooltip("Number of selection changes allowed per second when a movement button/axis is held in a direction.")]
        private float m_InputActionsPerSecond = 10;

        /// <summary>
        /// Delay in seconds before vertical/horizontal movement starts repeating continouously when a movement direction is held.
        /// </summary>
        [SerializeField]
        [Tooltip("Delay in seconds before vertical/horizontal movement starts repeating continouously when a movement direction is held.")]
        private float m_RepeatDelay = 0.0f;

        /// <summary>
        /// Allows the mouse to be used to select elements.
        /// </summary>
        [SerializeField]
        [Tooltip("Allows the mouse to be used to select elements.")]
        private bool m_allowMouseInput = true;

        /// <summary>
        /// Allows the mouse to be used to select elements if the device also supports touch control.
        /// </summary>
        [SerializeField]
        [Tooltip("Allows the mouse to be used to select elements if the device also supports touch control.")]
        private bool m_allowMouseInputIfTouchSupported = true;

        /// <summary>
        /// Allows touch input to be used to select elements.
        /// </summary>
        [SerializeField]
        [Tooltip("Allows touch input to be used to select elements.")]
        private bool m_allowTouchInput = true;
        
        /// <summary>
        /// Deselects the current selection on mouse/touch click when the pointer is not over a selectable object.
        /// </summary>
        [SerializeField]
        [Tooltip("Deselects the current selection on mouse/touch click when the pointer is not over a selectable object.")]
        private bool m_deselectIfBackgroundClicked = true;

        /// <summary>
        /// Deselects the current selection on mouse/touch click before selecting the next object.
        /// </summary>
        [SerializeField]
        [Tooltip("Deselects the current selection on mouse/touch click before selecting the next object.")]
        private bool m_deselectBeforeSelecting = true;

        /// <summary>
        /// Forces the module to always be active.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("m_AllowActivationOnMobileDevice")]
        [Tooltip("Forces the module to always be active.")]
        private bool m_ForceModuleActive;

        #endregion

        #region Rewired Variables and Properties

        [NonSerialized]
        private int[] playerIds;
        private bool recompiling;
        [NonSerialized]
        private bool isTouchSupported;

        /// <summary>
        /// (Optional) Link the Rewired Input Manager here for easier access to Player ids, etc.
        /// </summary>
        public InputManager_Base RewiredInputManager {
            get { return rewiredInputManager; }
            set { rewiredInputManager = value; }
        }

        /// <summary>
        /// Allow all Rewired game Players to control the UI. This does not include the System Player. If enabled, this setting overrides individual Player Ids set in Rewired Player Ids.
        /// </summary>
        public bool UseAllRewiredGamePlayers {
            get { return useAllRewiredGamePlayers; }
            set {
                bool changed = value != useAllRewiredGamePlayers;
                useAllRewiredGamePlayers = value;
                if (changed) SetupRewiredVars();
            }
        }

        /// <summary>
        /// Allow the Rewired System Player to control the UI.
        /// </summary>
        public bool UseRewiredSystemPlayer {
            get { return useRewiredSystemPlayer; }
            set {
                bool changed = value != useRewiredSystemPlayer;
                useRewiredSystemPlayer = value;
                if (changed) SetupRewiredVars();
            }
        }
        /// <summary>
        /// A list of Player Ids that are allowed to control the UI. If Use All Rewired Game Players = True, this list will be ignored.
        /// Returns a clone of the array.
        /// </summary>
        public int[] RewiredPlayerIds {
            get { return (int[])rewiredPlayerIds.Clone(); }
            set {
                rewiredPlayerIds = (value != null ? (int[])value.Clone() : new int[0]);
                SetupRewiredVars();
            }
        }

        /// <summary>
        /// Allow only Players with Player.isPlaying = true to control the UI.
        /// </summary>
        public bool UsePlayingPlayersOnly {
            get { return usePlayingPlayersOnly; }
            set { usePlayingPlayersOnly = value; }
        }

        /// <summary>
        /// Player Mice allowed to interact with the UI.
        /// Each Player that owns a Player Mouse must also be allowed to control the UI or the Player Mouse will not function.
        /// </summary>
        public List<Rewired.Components.PlayerMouse> PlayerMice {
            get {
                return new List<Rewired.Components.PlayerMouse>(playerMice);
            }
            set {
                if(value == null) {
                    playerMice = new List<Rewired.Components.PlayerMouse>();
                    SetupRewiredVars();
                    return;
                }
                playerMice = new List<Rewired.Components.PlayerMouse>(value);
                SetupRewiredVars();
            }
        }

        /// <summary>
        /// Makes an axis press always move only one UI selection. Enable if you do not want to allow scrolling through UI elements by holding an axis direction.
        /// </summary>
        public bool MoveOneElementPerAxisPress {
            get { return moveOneElementPerAxisPress; }
            set { moveOneElementPerAxisPress = value; }
        }

        /// <summary>
        /// Allows the mouse to be used to select elements.
        /// </summary>
        public bool allowMouseInput {
            get { return m_allowMouseInput; }
            set { m_allowMouseInput = value; }
        }

        /// <summary>
        /// Allows the mouse to be used to select elements if the device also supports touch control.
        /// </summary>
        public bool allowMouseInputIfTouchSupported {
            get { return m_allowMouseInputIfTouchSupported; }
            set { m_allowMouseInputIfTouchSupported = value; }
        }

        /// <summary>
        /// Allows touch input to be used to select elements.
        /// </summary>
        public bool allowTouchInput {
            get { return m_allowTouchInput; }
            set { m_allowTouchInput = value; }
        }

        /// <summary>
        /// Deselects the current selection on mouse/touch click when the pointer is not over a selectable object.
        /// </summary>
        public bool deselectIfBackgroundClicked {
            get {
                return m_deselectIfBackgroundClicked;
            }
            set {
                m_deselectIfBackgroundClicked = value;
            }
        }

        /// <summary>
        /// Deselects the current selection on mouse/touch click before selecting the next object.
        /// </summary>
        private bool deselectBeforeSelecting {
            get {
                return m_deselectBeforeSelecting;
            }
            set {
                m_deselectBeforeSelecting = value;
            }
        }

        /// <summary>
        /// If enabled, Action Ids will be used to set the Actions. If disabled, string names will be used to set the Actions.
        /// </summary>
        public bool SetActionsById {
            get {
                return setActionsById;
            }
            set {
                if(setActionsById == value) return;
                setActionsById = value;
                SetupRewiredVars();
            }
        }

        /// <summary>
        /// Name of the horizontal axis for movement (if axis events are used).
        /// </summary>
        public int HorizontalActionId {
            get {
                return horizontalActionId;
            }
            set {
                if(value == horizontalActionId) return;
                horizontalActionId = value;
                if(Rewired.ReInput.isReady) {
                    m_HorizontalAxis = Rewired.ReInput.mapping.GetAction(value) != null ? Rewired.ReInput.mapping.GetAction(value).name : string.Empty;
                }
            }
        }

        /// <summary>
        /// Name of the vertical axis for movement (if axis events are used).
        /// </summary>
        public int VerticalActionId {
            get {
                return verticalActionId;
            }
            set {
                if(value == verticalActionId) return;
                verticalActionId = value;
                if(Rewired.ReInput.isReady) {
                    m_VerticalAxis = Rewired.ReInput.mapping.GetAction(value) != null ? Rewired.ReInput.mapping.GetAction(value).name : string.Empty;
                }
            }
        }

        /// <summary>
        /// Name of the action used to submit.
        /// </summary>
        public int SubmitActionId {
            get {
                return submitActionId;
            }
            set {
                if(value == submitActionId) return;
                submitActionId = value;
                if(Rewired.ReInput.isReady) {
                    m_SubmitButton = Rewired.ReInput.mapping.GetAction(value) != null ? Rewired.ReInput.mapping.GetAction(value).name : string.Empty;
                }
            }
        }

        /// <summary>
        /// Name of the action used to cancel.
        /// </summary>
        public int CancelActionId {
            get {
                return cancelActionId;
            }
            set {
                if(value == cancelActionId) return;
                cancelActionId = value;
                if(Rewired.ReInput.isReady) {
                    m_CancelButton = Rewired.ReInput.mapping.GetAction(value) != null ? Rewired.ReInput.mapping.GetAction(value).name : string.Empty;
                }
            }
        }

        protected override bool isMouseSupported {
            get {
                if(!base.isMouseSupported) return false;
                if (!m_allowMouseInput) return false;
                return isTouchSupported ? m_allowMouseInputIfTouchSupported : true;
            }
        }

        private bool isTouchAllowed {
            get {
                // if (!isTouchSupported) return false; // Removed because Unity Remote will return touches even on platforms that report touch not supported and returning on this will break it.
                return m_allowTouchInput;
            }
        }
        
        #endregion

        [NonSerialized]
        private double m_PrevActionTime;
        [NonSerialized]
        Vector2 m_LastMoveVector;
        [NonSerialized]
        int m_ConsecutiveMoveCount = 0;
        [NonSerialized]
        private bool m_HasFocus = true;

        /// <summary>
        /// Allows the module to control UI input on mobile devices..
        /// </summary>
        [Obsolete("allowActivationOnMobileDevice has been deprecated. Use forceModuleActive instead")]
        public bool allowActivationOnMobileDevice {
            get { return m_ForceModuleActive; }
            set { m_ForceModuleActive = value; }
        }

        /// <summary>
        /// Forces the module to always be active.
        /// </summary>
        public bool forceModuleActive {
            get { return m_ForceModuleActive; }
            set { m_ForceModuleActive = value; }
        }

        // <summary>
        /// Number of selection changes allowed per second when a movement button/axis is held in a direction.
        /// </summary>
        public float inputActionsPerSecond {
            get { return m_InputActionsPerSecond; }
            set { m_InputActionsPerSecond = value; }
        }

        /// <summary>
        /// Delay in seconds before vertical/horizontal movement starts repeating continouously when a movement direction is held.
        /// </summary>
        public float repeatDelay {
            get { return m_RepeatDelay; }
            set { m_RepeatDelay = value; }
        }

        /// <summary>
        /// Name of the horizontal axis for movement (if axis events are used).
        /// </summary>
        public string horizontalAxis {
            get { return m_HorizontalAxis; }
            set {
                if(m_HorizontalAxis == value) return;
                m_HorizontalAxis = value;
                if(Rewired.ReInput.isReady) {
                    horizontalActionId = Rewired.ReInput.mapping.GetActionId(value);
                }
            }
        }

        /// <summary>
        /// Name of the vertical axis for movement (if axis events are used).
        /// </summary>
        public string verticalAxis {
            get { return m_VerticalAxis; }
            set {
                if(m_VerticalAxis == value) return;
                m_VerticalAxis = value;
                if(Rewired.ReInput.isReady) {
                    verticalActionId = Rewired.ReInput.mapping.GetActionId(value);
                }
            }
        }

        /// <summary>
        /// Name of the action used to submit.
        /// </summary>
        public string submitButton {
            get { return m_SubmitButton; }
            set {
                if(m_SubmitButton == value) return;
                m_SubmitButton = value;
                if(Rewired.ReInput.isReady) {
                    submitActionId = Rewired.ReInput.mapping.GetActionId(value);
                }
            }
        }

        /// <summary>
        /// Name of the action used to cancel.
        /// </summary>
        public string cancelButton {
            get { return m_CancelButton; }
            set {
                if(m_CancelButton == value) return;
                m_CancelButton = value;
                if(Rewired.ReInput.isReady) {
                    cancelActionId = Rewired.ReInput.mapping.GetActionId(value);
                }
            }
        }

        // Constructor

        private RewiredStandaloneInputModule() { }

        // Methods

        protected override void Awake() {
            base.Awake();

            // Determine if touch is supported
            isTouchSupported = defaultTouchInputSource.touchSupported;

            // Deactivate the TouchInputModule because it has been deprecated in 5.3. Functionality was moved into here on all versions.
            TouchInputModule tim = GetComponent<TouchInputModule>();
            if (tim != null) {
                tim.enabled = false;
#if UNITY_EDITOR
                Debug.LogWarning("The TouchInputModule is no longer used as the functionality has been moved into the RewiredStandaloneInputModule. Please remove the TouchInputModule component.");
#endif
            }

            Rewired.ReInput.InitializedEvent += OnRewiredInitialized;

            // Initialize Rewired
            InitializeRewired();
        }

        public override void UpdateModule() {
            CheckEditorRecompile();
            if (recompiling) return;
            if (!Rewired.ReInput.isReady) return;

            if (!m_HasFocus && ShouldIgnoreEventsOnNoFocus()) return;
        }

        public override bool IsModuleSupported() {
            return true; // there is never any reason this module should not be supported now that TouchInputModule is deprecated, so always return true.
        }

        public override bool ShouldActivateModule() {
            if (!base.ShouldActivateModule()) return false;
            if (recompiling) return false;
            if (!Rewired.ReInput.isReady) return false;

            bool shouldActivate = m_ForceModuleActive;

            // Combine input for all players
            for (int i = 0; i < playerIds.Length; i++) {
                Rewired.Player player = Rewired.ReInput.players.GetPlayer(playerIds[i]);
                if (player == null) continue;
                if (usePlayingPlayersOnly && !player.isPlaying) continue;

                shouldActivate |= GetButtonDown(player, submitActionId);
                shouldActivate |= GetButtonDown(player, cancelActionId);
                if (moveOneElementPerAxisPress) { // axis press moves only to the next UI element with each press
                    shouldActivate |= GetButtonDown(player, horizontalActionId) || GetNegativeButtonDown(player, horizontalActionId);
                    shouldActivate |= GetButtonDown(player, verticalActionId) || GetNegativeButtonDown(player, verticalActionId);
                } else { // default behavior - axis press scrolls quickly through UI elements
                    shouldActivate |= !Mathf.Approximately(GetAxis(player, horizontalActionId), 0.0f);
                    shouldActivate |= !Mathf.Approximately(GetAxis(player, verticalActionId), 0.0f);
                }
            }

            // Mouse input
            if (isMouseSupported) {
                shouldActivate |= DidAnyMouseMove();
                shouldActivate |= GetMouseButtonDownOnAnyMouse(0);
            }

            // Touch input
            if (isTouchAllowed) {
                for(int i = 0; i < defaultTouchInputSource.touchCount; ++i) {
                    Touch touch = defaultTouchInputSource.GetTouch(i);
                    shouldActivate |= touch.phase == TouchPhase.Began
                        || touch.phase == TouchPhase.Moved
                        || touch.phase == TouchPhase.Stationary;
                }
            }

            return shouldActivate;
        }

        public override void ActivateModule() {
            if (!m_HasFocus && ShouldIgnoreEventsOnNoFocus()) return;

            base.ActivateModule();

            var toSelect = eventSystem.currentSelectedGameObject;
            if (toSelect == null)
                toSelect = eventSystem.firstSelectedGameObject;

            eventSystem.SetSelectedGameObject(toSelect, GetBaseEventData());
        }

        public override void DeactivateModule() {
            base.DeactivateModule();
            ClearSelection();
        }

        public override void Process() {
            if (!Rewired.ReInput.isReady) return;
            if (!m_HasFocus && ShouldIgnoreEventsOnNoFocus()) return;
            if (!enabled || !gameObject.activeInHierarchy) return;

            bool usedEvent = SendUpdateEventToSelectedObject();

            if (eventSystem.sendNavigationEvents) {
                if (!usedEvent)
                    usedEvent |= SendMoveEventToSelectedObject();

                if (!usedEvent)
                    SendSubmitEventToSelectedObject();
            }

            // touch needs to take precedence because of the mouse emulation layer
            if (!ProcessTouchEvents()) {
                if (isMouseSupported) ProcessMouseEvents();
            }
        }

        private bool ProcessTouchEvents() {
            if (!isTouchAllowed) return false;

            for(int i = 0; i < defaultTouchInputSource.touchCount; ++i) {
                Touch touch = defaultTouchInputSource.GetTouch(i);

#if UNITY_5_3_OR_NEWER
                if(touch.type == TouchType.Indirect)
                    continue;
#endif

                bool released;
                bool pressed;
                var pointer = GetTouchPointerEventData(0, 0, touch, out pressed, out released);

                ProcessTouchPress(pointer, pressed, released);

                if (!released) {
                    ProcessMove(pointer);
                    ProcessDrag(pointer);
                } else
                    RemovePointerData(pointer);
            }
            return defaultTouchInputSource.touchCount > 0;
        }

        private void ProcessTouchPress(PointerEventData pointerEvent, bool pressed, bool released) {
            var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;

            // PointerDown notification
            if (pressed) {
                pointerEvent.eligibleForClick = true;
                pointerEvent.delta = Vector2.zero;
                pointerEvent.dragging = false;
                pointerEvent.useDragThreshold = true;
                pointerEvent.pressPosition = pointerEvent.position;
                pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

                HandleMouseTouchDeselectionOnSelectionChanged(currentOverGo, pointerEvent);

                if (pointerEvent.pointerEnter != currentOverGo) {
                    // send a pointer enter to the touched element if it isn't the one to select...
                    HandlePointerExitAndEnter(pointerEvent, currentOverGo);
                    pointerEvent.pointerEnter = currentOverGo;
                }

                // search for the control that will receive the press
                // if we can't find a press handler set the press
                // handler to be what would receive a click.
                var newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);

                // didnt find a press handler... search for a click handler
                if (newPressed == null)
                    newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                // Debug.Log("Pressed: " + newPressed);

                double time = Rewired.ReInput.time.unscaledTime;

                if (newPressed == pointerEvent.lastPress) {
                    var diffTime = time - pointerEvent.clickTime;
                    if (diffTime < 0.3f)
                        ++pointerEvent.clickCount;
                    else
                        pointerEvent.clickCount = 1;

                    pointerEvent.clickTime = (float)time;
                } else {
                    pointerEvent.clickCount = 1;
                }

                pointerEvent.pointerPress = newPressed;
                pointerEvent.rawPointerPress = currentOverGo;

                pointerEvent.clickTime = (float)time;

                // Save the drag handler as well
                pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

                if (pointerEvent.pointerDrag != null)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
            }

            // PointerUp notification
            if (released) {
                // Debug.Log("Executing pressup on: " + pointer.pointerPress);
                ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

                // Debug.Log("KeyCode: " + pointer.eventData.keyCode);

                // see if we mouse up on the same element that we clicked on...
                var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                // PointerClick and Drop events
                if (pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick) {
                    ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
                } else if (pointerEvent.pointerDrag != null && pointerEvent.dragging) {
                    ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.dropHandler);
                }

                pointerEvent.eligibleForClick = false;
                pointerEvent.pointerPress = null;
                pointerEvent.rawPointerPress = null;

                if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

                pointerEvent.dragging = false;
                pointerEvent.pointerDrag = null;

                if (pointerEvent.pointerDrag != null)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

                pointerEvent.pointerDrag = null;

                // send exit events as we need to simulate this on touch up on touch device
                ExecuteEvents.ExecuteHierarchy(pointerEvent.pointerEnter, pointerEvent, ExecuteEvents.pointerExitHandler);
                pointerEvent.pointerEnter = null;
            }
        }

        /// <summary>
        /// Process submit keys.
        /// </summary>
        private bool SendSubmitEventToSelectedObject() {
            if (eventSystem.currentSelectedGameObject == null)
                return false;
            if (recompiling) return false;

            var data = GetBaseEventData();
            for (int i = 0; i < playerIds.Length; i++) {
                Rewired.Player player = Rewired.ReInput.players.GetPlayer(playerIds[i]);
                if (player == null) continue;
                if (usePlayingPlayersOnly && !player.isPlaying) continue;

                if (GetButtonDown(player, submitActionId)) {
                    ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.submitHandler);
                    break;
                }

                if (GetButtonDown(player, cancelActionId)) {
                    ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.cancelHandler);
                    break;
                }
            }
            return data.used;
        }

        private Vector2 GetRawMoveVector() {
            if (recompiling) return Vector2.zero;

            Vector2 move = Vector2.zero;
            bool horizButton = false;
            bool vertButton = false;

            // Combine inputs of all Players
            for (int i = 0; i < playerIds.Length; i++) {
                Rewired.Player player = Rewired.ReInput.players.GetPlayer(playerIds[i]);
                if (player == null) continue;
                if (usePlayingPlayersOnly && !player.isPlaying) continue;
                
                // Must double check against axis value because "Activate Action Buttons on Negative Value" may be enabled 
                // and will prevent negative axis values from working because they're cancelled out by positive values.
                float horizontal = GetAxis(player, horizontalActionId);
                float vertical = GetAxis(player, verticalActionId);
                
                if(Mathf.Approximately(horizontal, 0f)) horizontal = 0f;
                if(Mathf.Approximately(vertical, 0f)) vertical = 0f;

                if (moveOneElementPerAxisPress) { // axis press moves only to the next UI element with each press
                    if (GetButtonDown(player, horizontalActionId) && horizontal > 0f) move.x += 1.0f;
                    if (GetNegativeButtonDown(player, horizontalActionId) && horizontal < 0f) move.x -= 1.0f;
                    if (GetButtonDown(player, verticalActionId) && vertical > 0f) move.y += 1.0f;
                    if (GetNegativeButtonDown(player, verticalActionId) && vertical < 0f) move.y -= 1.0f;
                } else { // default behavior - axis press scrolls quickly through UI elements
                    // Use GetButton instead of GetAxis so the Input Behavior's Button Dead Zone is used for each Player
                    if(GetButton(player, horizontalActionId) && horizontal > 0f) move.x += 1.0f;
                    if(GetNegativeButton(player, horizontalActionId) && horizontal < 0f) move.x -= 1.0f;
                    if(GetButton(player, verticalActionId) && vertical > 0f) move.y += 1.0f;
                    if(GetNegativeButton(player, verticalActionId) && vertical < 0f) move.y -= 1.0f;
                }
            }
            return move;
        }

        /// <summary>
        /// Process keyboard events.
        /// </summary>
        private bool SendMoveEventToSelectedObject() {
            if (recompiling) return false; // never allow movement while recompiling

            double time = Rewired.ReInput.time.unscaledTime; // get the current time

            // Check for zero movement and clear
            Vector2 movement = GetRawMoveVector();
            if (Mathf.Approximately(movement.x, 0f) && Mathf.Approximately(movement.y, 0f)) {
                m_ConsecutiveMoveCount = 0;
                return false;
            }

            // Check if movement is in the same direction as previously
            bool similarDir = (Vector2.Dot(movement, m_LastMoveVector) > 0);

            // Check if a button/key/axis was just pressed this frame
            bool buttonDownHorizontal, buttonDownVertical;
            CheckButtonOrKeyMovement(out buttonDownHorizontal, out buttonDownVertical);

            AxisEventData axisEventData = null;

            // If user just pressed button/key/axis, always allow movement
            bool allow = buttonDownHorizontal || buttonDownVertical;
            if (allow) { // we had a button down event

                // Get the axis move event now so we can tell the direction it will be moving
                axisEventData = GetAxisEventData(movement.x, movement.y, 0f);

                // Make sure the button down event was in the direction we would be moving, otherwise don't allow it.
                // This filters out double movements when pressing somewhat diagonally and getting down events for both
                // horizontal and vertical at the same time but both ending up being resolved in the same direction.
                MoveDirection moveDir = axisEventData.moveDir;
                allow = ((moveDir == MoveDirection.Up || moveDir == MoveDirection.Down) && buttonDownVertical) ||
                    ((moveDir == MoveDirection.Left || moveDir == MoveDirection.Right) && buttonDownHorizontal);
            }

            if (!allow) {
                // Apply repeat delay and input actions per second limits
                if (m_RepeatDelay > 0.0f) { // apply repeat delay
                    // Otherwise, user held down key or axis.
                    // If direction didn't change at least 90 degrees, wait for delay before allowing consecutive event.
                    if (similarDir && m_ConsecutiveMoveCount == 1) { // this is the 2nd tick after the initial that activated the movement in this direction
                        allow = (time > m_PrevActionTime + m_RepeatDelay);
                        // If direction changed at least 90 degree, or we already had the delay, repeat at repeat rate.
                    } else {
                        allow = (time > m_PrevActionTime + 1f / m_InputActionsPerSecond); // apply input actions per second limit
                    }
                } else { // not using a repeat delay
                    allow = (time > m_PrevActionTime + 1f / m_InputActionsPerSecond); // apply input actions per second limit
                }
            }
            if (!allow) return false; // movement not allowed, done

            // Get the axis move event
            if (axisEventData == null) {
                axisEventData = GetAxisEventData(movement.x, movement.y, 0f);
            }

            if (axisEventData.moveDir != MoveDirection.None) {
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
                if (!similarDir)
                    m_ConsecutiveMoveCount = 0;
                if (m_ConsecutiveMoveCount == 0 || !(buttonDownHorizontal || buttonDownVertical))
                    m_ConsecutiveMoveCount++;
                m_PrevActionTime = time;
                m_LastMoveVector = movement;
            } else {
                m_ConsecutiveMoveCount = 0;
            }

            return axisEventData.used;
        }

        private void CheckButtonOrKeyMovement(out bool downHorizontal, out bool downVertical) {
            downHorizontal = false;
            downVertical = false;

            for (int i = 0; i < playerIds.Length; i++) {
                Rewired.Player player = Rewired.ReInput.players.GetPlayer(playerIds[i]);
                if (player == null) continue;
                if (usePlayingPlayersOnly && !player.isPlaying) continue;

                downHorizontal |= GetButtonDown(player, horizontalActionId) || GetNegativeButtonDown(player, horizontalActionId);
                downVertical |= GetButtonDown(player, verticalActionId) || GetNegativeButtonDown(player, verticalActionId);
            }
        }

        private void ProcessMouseEvents() {
            for(int i = 0; i < playerIds.Length; i++) {
                Rewired.Player player = Rewired.ReInput.players.GetPlayer(playerIds[i]);
                if(player == null) continue;
                if(usePlayingPlayersOnly && !player.isPlaying) continue;
                int pointerCount = GetMouseInputSourceCount(playerIds[i]);
                for(int j = 0; j < pointerCount; j++) {
                    ProcessMouseEvent(playerIds[i], j);
                }
            }
        }

        /// <summary>
        /// Process all mouse events.
        /// </summary>
        private void ProcessMouseEvent(int playerId, int pointerIndex) {

            var mouseData = GetMousePointerEventData(playerId, pointerIndex);
            if(mouseData == null) return;

            var leftButtonData = mouseData.GetButtonState(0).eventData;

            // Process the first mouse button fully
            ProcessMousePress(leftButtonData);
            ProcessMove(leftButtonData.buttonData);
            ProcessDrag(leftButtonData.buttonData);

            // Now process right / middle clicks
            ProcessMousePress(mouseData.GetButtonState(1).eventData);
            ProcessDrag(mouseData.GetButtonState(1).eventData.buttonData);
            ProcessMousePress(mouseData.GetButtonState(2).eventData);
            ProcessDrag(mouseData.GetButtonState(2).eventData.buttonData);
            IMouseInputSource mouseInputSource = GetMouseInputSource(playerId, pointerIndex);
            if (mouseInputSource == null) return; // in case mouse source removed by user in event callback
            for(int i = 3; i < mouseInputSource.buttonCount; i++) {
                ProcessMousePress(mouseData.GetButtonState(i).eventData);
                ProcessDrag(mouseData.GetButtonState(i).eventData.buttonData);
            }

            if (!Mathf.Approximately(leftButtonData.buttonData.scrollDelta.sqrMagnitude, 0.0f)) {
                var scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(leftButtonData.buttonData.pointerCurrentRaycast.gameObject);
                ExecuteEvents.ExecuteHierarchy(scrollHandler, leftButtonData.buttonData, ExecuteEvents.scrollHandler);
            }
        }

        private bool SendUpdateEventToSelectedObject() {
            if (eventSystem.currentSelectedGameObject == null)
                return false;

            var data = GetBaseEventData();
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
            return data.used;
        }

        /// <summary>
        /// Process the current mouse press.
        /// </summary>
        private void ProcessMousePress(MouseButtonEventData data) {
            var pointerEvent = data.buttonData;
            if (GetMouseInputSource(pointerEvent.playerId, pointerEvent.inputSourceIndex) == null) return; // invalid mouse source
            var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;

            // PointerDown notification
            if (data.PressedThisFrame()) {
                pointerEvent.eligibleForClick = true;
                pointerEvent.delta = Vector2.zero;
                pointerEvent.dragging = false;
                pointerEvent.useDragThreshold = true;
                pointerEvent.pressPosition = pointerEvent.position;
                pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

                HandleMouseTouchDeselectionOnSelectionChanged(currentOverGo, pointerEvent);

                // search for the control that will receive the press
                // if we can't find a press handler set the press
                // handler to be what would receive a click.
                var newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);

                // didnt find a press handler... search for a click handler
                if (newPressed == null)
                    newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                // Debug.Log("Pressed: " + newPressed);

                double time = Rewired.ReInput.time.unscaledTime;

                if (newPressed == pointerEvent.lastPress) {
                    var diffTime = time - pointerEvent.clickTime;
                    if (diffTime < 0.3f)
                        ++pointerEvent.clickCount;
                    else
                        pointerEvent.clickCount = 1;

                    pointerEvent.clickTime = (float)time;
                } else {
                    pointerEvent.clickCount = 1;
                }

                pointerEvent.pointerPress = newPressed;
                pointerEvent.rawPointerPress = currentOverGo;

                pointerEvent.clickTime = (float)time;

                // Save the drag handler as well
                pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

                if (pointerEvent.pointerDrag != null)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
            }

            // PointerUp notification
            if (data.ReleasedThisFrame()) {
                // Debug.Log("Executing pressup on: " + pointer.pointerPress);
                ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

                // Debug.Log("KeyCode: " + pointer.eventData.keyCode);

                // see if we mouse up on the same element that we clicked on...
                var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                // PointerClick and Drop events
                if (pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick) {
                    ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
                } else if (pointerEvent.pointerDrag != null && pointerEvent.dragging) {
                    ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.dropHandler);
                }

                pointerEvent.eligibleForClick = false;
                pointerEvent.pointerPress = null;
                pointerEvent.rawPointerPress = null;

                if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

                pointerEvent.dragging = false;
                pointerEvent.pointerDrag = null;

                // redo pointer enter / exit to refresh state
                // so that if we moused over somethign that ignored it before
                // due to having pressed on something else
                // it now gets it.
                if (currentOverGo != pointerEvent.pointerEnter) {
                    HandlePointerExitAndEnter(pointerEvent, null);
                    HandlePointerExitAndEnter(pointerEvent, currentOverGo);
                }
            }
        }
        
        private void HandleMouseTouchDeselectionOnSelectionChanged(GameObject currentOverGo, BaseEventData pointerEvent) {
            if (m_deselectIfBackgroundClicked && m_deselectBeforeSelecting) {
                DeselectIfSelectionChanged(currentOverGo, pointerEvent);
                return;
            }
            var selectHandlerGO = ExecuteEvents.GetEventHandler<ISelectHandler>(currentOverGo);
            if (m_deselectIfBackgroundClicked) {
                // Deselect only if no new object will be selected
                if (selectHandlerGO != eventSystem.currentSelectedGameObject && selectHandlerGO != null) {
                    eventSystem.SetSelectedGameObject(null, pointerEvent);
                }
            } else if(m_deselectBeforeSelecting) {
                // Deselect only if there is a new selection
                if (selectHandlerGO != null && selectHandlerGO != eventSystem.currentSelectedGameObject) {
                    eventSystem.SetSelectedGameObject(null, pointerEvent);
                }
            }
        }

        private void OnApplicationFocus(bool hasFocus) {
            m_HasFocus = hasFocus;
        }

        private bool ShouldIgnoreEventsOnNoFocus() {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_WSA || UNITY_WINRT
#if UNITY_EDITOR && UNITY_5_PLUS
            if(UnityEditor.EditorApplication.isRemoteConnected) return false;
#endif
            if(!Rewired.ReInput.isReady) return true;
#else
            if (!Rewired.ReInput.isReady) return false;
#endif
            return Rewired.ReInput.configuration.ignoreInputWhenAppNotInFocus;
        }

#if UNITY_EDITOR

        protected override void OnValidate() {
            base.OnValidate();
            SetupRewiredVars();
        }

#endif

        protected override void OnDestroy() {
            base.OnDestroy();
            Rewired.ReInput.InitializedEvent -= OnRewiredInitialized;
            Rewired.ReInput.ShutDownEvent -= OnRewiredShutDown;
            Rewired.ReInput.EditorRecompileEvent -= OnEditorRecompile;
        }

        #region Rewired Methods

        protected override bool IsDefaultPlayer(int playerId) {
            if(playerIds == null) return false;
            if(!Rewired.ReInput.isReady) return false;
            
            for(int tries = 0; tries < 3; tries++) {

                // Try 0: Find the first Player that has the mouse assigned and is playing
                // Try 1: Find the first Player that has the mouse assigned
                // Try 2: Find the first Player

                for(int i = 0; i < playerIds.Length; i++) {
                    Rewired.Player player = Rewired.ReInput.players.GetPlayer(playerIds[i]);
                    if(player == null) continue;
                    if(tries < 1 && usePlayingPlayersOnly && !player.isPlaying) continue;
                    if(tries < 2 && !player.controllers.hasMouse) continue;
                    return playerIds[i] == playerId;
                }
            }

            return false;
        }

        private void InitializeRewired() {
            if (!Rewired.ReInput.isReady) {
                Debug.LogError("Rewired is not initialized! Are you missing a Rewired Input Manager in your scene?");
                return;
            }
            Rewired.ReInput.ShutDownEvent -= OnRewiredShutDown;
            Rewired.ReInput.ShutDownEvent += OnRewiredShutDown;
            Rewired.ReInput.EditorRecompileEvent -= OnEditorRecompile;
            Rewired.ReInput.EditorRecompileEvent += OnEditorRecompile;
            SetupRewiredVars();
        }

        private void SetupRewiredVars() {
            if(!Rewired.ReInput.isReady) return;

            // Set up Rewired vars

            // Set up actions
            SetUpRewiredActions();

            // Set up Rewired Players
            if (useAllRewiredGamePlayers) {
                IList<Rewired.Player> rwPlayers = useRewiredSystemPlayer ? Rewired.ReInput.players.AllPlayers : Rewired.ReInput.players.Players;
                this.playerIds = new int[rwPlayers.Count];
                for (int i = 0; i < rwPlayers.Count; i++) {
                    this.playerIds[i] = rwPlayers[i].id;
                }
            } else {
                bool foundSystem = false;
                List<int> playerIds = new List<int>(rewiredPlayerIds.Length + 1);
                for (int i = 0; i < rewiredPlayerIds.Length; i++) {
                    Rewired.Player player = Rewired.ReInput.players.GetPlayer(rewiredPlayerIds[i]);
                    if(player == null) continue;
                    if(playerIds.Contains(player.id)) continue; // already in list
                    playerIds.Add(player.id);
                    if(player.id == Consts.systemPlayerId) foundSystem = true;
                }
                if(useRewiredSystemPlayer && !foundSystem) playerIds.Insert(0, Rewired.ReInput.players.GetSystemPlayer().id);
                this.playerIds = playerIds.ToArray();
            }

            SetUpRewiredPlayerMice();
        }

        private void SetUpRewiredPlayerMice() {
            if(!Rewired.ReInput.isReady) return;
            ClearMouseInputSources();
            for(int i = 0; i < playerMice.Count; i++) {
                var mouse = playerMice[i];
                if(Utils.UnityTools.IsNullOrDestroyed(mouse)) continue;
                AddMouseInputSource(mouse);
            }
        }

        private void SetUpRewiredActions() {
            if(!Rewired.ReInput.isReady) return;
            if(!setActionsById) {
                horizontalActionId = Rewired.ReInput.mapping.GetActionId(m_HorizontalAxis);
                verticalActionId = Rewired.ReInput.mapping.GetActionId(m_VerticalAxis);
                submitActionId = Rewired.ReInput.mapping.GetActionId(m_SubmitButton);
                cancelActionId = Rewired.ReInput.mapping.GetActionId(m_CancelButton);
            } else {
                InputAction action;
                action = Rewired.ReInput.mapping.GetAction(horizontalActionId);
                m_HorizontalAxis = action != null ? action.name : string.Empty;
                if(action == null) horizontalActionId = -1;
                action = Rewired.ReInput.mapping.GetAction(verticalActionId);
                m_VerticalAxis = action != null ? action.name : string.Empty;
                if(action == null) verticalActionId = -1;
                action = Rewired.ReInput.mapping.GetAction(submitActionId);
                m_SubmitButton = action != null ? action.name : string.Empty;
                if(action == null) submitActionId = -1;
                action = Rewired.ReInput.mapping.GetAction(cancelActionId);
                m_CancelButton = action != null ? action.name : string.Empty;
                if(action == null) cancelActionId = -1;
            }
        }

        private bool GetButton(Rewired.Player player, int actionId) {
            if(actionId < 0) return false; // silence warnings
            return player.GetButton(actionId);
        }

        private bool GetButtonDown(Rewired.Player player, int actionId) {
            if(actionId < 0) return false; // silence warnings
            return player.GetButtonDown(actionId);
        }

        private bool GetNegativeButton(Rewired.Player player, int actionId) {
            if(actionId < 0) return false; // silence warnings
            return player.GetNegativeButton(actionId);
        }

        private bool GetNegativeButtonDown(Rewired.Player player, int actionId) {
            if(actionId < 0) return false; // silence warnings
            return player.GetNegativeButtonDown(actionId);
        }

        private float GetAxis(Rewired.Player player, int actionId) {
            if(actionId < 0) return 0f; // silence warnings
            return player.GetAxis(actionId);
        }

        private void CheckEditorRecompile() {
            if (!recompiling) return;
            if (!Rewired.ReInput.isReady) return;
            recompiling = false;
            InitializeRewired();
        }

        private void OnEditorRecompile() {
            recompiling = true;
            ClearRewiredVars();
        }

        private void ClearRewiredVars() {
            Array.Clear(playerIds, 0, playerIds.Length);
            ClearMouseInputSources();
        }

        private bool DidAnyMouseMove() {
            for(int i = 0; i < playerIds.Length; i++) {
                int playerId = playerIds[i];
                Rewired.Player player = Rewired.ReInput.players.GetPlayer(playerId);
                if(player == null) continue;
                if(usePlayingPlayersOnly && !player.isPlaying) continue;
                int mouseCount = GetMouseInputSourceCount(playerId);
                for(int j = 0; j < mouseCount; j++) {
                    IMouseInputSource source = GetMouseInputSource(playerId, j);
                    if(source == null) continue;
                    if(source.screenPositionDelta.sqrMagnitude > 0f) return true;
                }
            }
            return false;
        }

        private bool GetMouseButtonDownOnAnyMouse(int buttonIndex) {
            for(int i = 0; i < playerIds.Length; i++) {
                int playerId = playerIds[i];
                Rewired.Player player = Rewired.ReInput.players.GetPlayer(playerId);
                if(player == null) continue;
                if(usePlayingPlayersOnly && !player.isPlaying) continue;
                int mouseCount = GetMouseInputSourceCount(playerId);
                for(int j = 0; j < mouseCount; j++) {
                    IMouseInputSource source = GetMouseInputSource(playerId, j);
                    if(source == null) continue;
                    if(source.GetButtonDown(buttonIndex)) return true;
                }
            }
            return false;
        }

        private void OnRewiredInitialized() {
            InitializeRewired();
        }

        private void OnRewiredShutDown() {
            ClearRewiredVars();
        }

        #endregion

        [Serializable]
        public class PlayerSetting {

            public int playerId;
            public List<Rewired.Components.PlayerMouse> playerMice = new List<Components.PlayerMouse>();

            public PlayerSetting() {
            }
            private PlayerSetting(PlayerSetting other) {
                if(other == null) throw new ArgumentNullException("other");
                this.playerId = other.playerId;
                this.playerMice = new List<Components.PlayerMouse>();
                if(other.playerMice != null) {
                    foreach(var m in other.playerMice) {
                        this.playerMice.Add(m);
                    }
                }
            }

            public PlayerSetting Clone() {
                return new PlayerSetting(this);
            }
        }
    }
}