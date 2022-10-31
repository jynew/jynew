// Copyright (c) 2018 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
// Based on Unity StandaloneInputModule.cs
// https://bitbucket.org/Unity-Technologies/ui/src
// Heavily modified to add multiple pointer support, interchangeable touch and mouse input sources, and 128 buttons per mouse.

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
    using System.Text;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using System.Collections.Generic;
    using Rewired.UI;

    // Content added for Rewired
    public abstract class RewiredPointerInputModule : BaseInputModule {

        public const int kMouseLeftId = -1;
        public const int kMouseRightId = -2;
        public const int kMouseMiddleId = -3;

        public const int kFakeTouchesId = -4;
        private const int customButtonsStartingId = int.MinValue + 128;
        private const int customButtonsMaxCount = 128;
        private const int customButtonsLastId = customButtonsStartingId + customButtonsMaxCount;

        private readonly List<IMouseInputSource> m_MouseInputSourcesList = new List<IMouseInputSource>();
        private Dictionary<int, Dictionary<int, PlayerPointerEventData>[]> m_PlayerPointerData = new Dictionary<int, Dictionary<int, PlayerPointerEventData>[]>();

        private ITouchInputSource m_UserDefaultTouchInputSource;
        private UnityInputSource __m_DefaultInputSource;
        private UnityInputSource defaultInputSource {
            get {
                return __m_DefaultInputSource != null ? __m_DefaultInputSource : __m_DefaultInputSource = new UnityInputSource();
            }
        }

        private IMouseInputSource defaultMouseInputSource {
            get {
                return defaultInputSource;
            }
        }

        protected ITouchInputSource defaultTouchInputSource {
            get {
                return defaultInputSource;
            }
        }

        protected bool IsDefaultMouse(IMouseInputSource mouse) {
            return defaultMouseInputSource == mouse;
        }

        /// <summary>
        /// Gets or sets the input source used for mouse input.
        /// </summary>
        /// <param name="playerId">The Player Id that owns the mouse input source.</param>
        /// <param name="mouseIndex">The index of the mouse input source. If a Player owns more than one mouse input source, set this to the index. See <see cref="GetMouseInputSourceCount"/> to determine the number of mouse input sources that exist for a Player.</param>
        public IMouseInputSource GetMouseInputSource(int playerId, int mouseIndex) {
            if(mouseIndex < 0) throw new ArgumentOutOfRangeException("mouseIndex");
            if(m_MouseInputSourcesList.Count == 0 && IsDefaultPlayer(playerId)) return defaultMouseInputSource; // fall back to default if nothing set

            int count = m_MouseInputSourcesList.Count;
            int pointerCount = 0;
            for(int i = 0; i < count; i++) {
                IMouseInputSource source = m_MouseInputSourcesList[i];
                if(Utils.UnityTools.IsNullOrDestroyed(source)) continue;
                if(source.playerId != playerId) continue;
                if(mouseIndex == pointerCount) return source;
                pointerCount++;
            }
            return null;
        }

        /// <summary>
        /// Removes the mouse input source.
        /// </summary>
        /// <param name="source">The mouse input source.</param>
        public void RemoveMouseInputSource(IMouseInputSource source) {
            if(source == null) throw new ArgumentNullException("source"); // do not check if destroyed here so removal works on destroyed objects
            m_MouseInputSourcesList.Remove(source);
        }

        /// <summary>
        /// Adds the mouse input source.
        /// </summary>
        /// <param name="source">The mouse input source.</param>
        public void AddMouseInputSource(IMouseInputSource source) {
            if(Utils.UnityTools.IsNullOrDestroyed(source)) throw new ArgumentNullException("source");
            m_MouseInputSourcesList.Add(source);
        }

        /// <summary>
        /// Returns the number of possible pointers.
        /// Does not count the number actually assigned.
        /// </summary>
        public int GetMouseInputSourceCount(int playerId) {
            if(m_MouseInputSourcesList.Count == 0 && IsDefaultPlayer(playerId)) return 1; // fall back to default if nothing set
            int count = m_MouseInputSourcesList.Count;
            int pointerCount = 0;
            for(int i = 0; i < count; i++) {
                IMouseInputSource source = m_MouseInputSourcesList[i];
                if(Utils.UnityTools.IsNullOrDestroyed(source)) continue;
                if(source.playerId != playerId) continue;
                pointerCount++;
            }
            return pointerCount;
        }

        /// <summary>
        /// Gets or sets the input source used for touch input.
        /// </summary>
        /// <param name="playerId">The Player Id that owns the touch input source.</param>
        /// <param name="touchIndex">The index of the touch input source. If a Player owns more than one touch input source, set this to the index. See <see cref="GettouchInputSourceCount"/> to determine the number of touch input sources that exist for a Player. WARNING: Currently only one touch input source is supported.</param>
        public ITouchInputSource GetTouchInputSource(int playerId, int sourceIndex) {
            if(!Utils.UnityTools.IsNullOrDestroyed(m_UserDefaultTouchInputSource)) return m_UserDefaultTouchInputSource;
            return defaultTouchInputSource;
        }

        /// <summary>
        /// Removes the touch input source.
        /// </summary>
        /// <param name="source">The touch input source.</param>
        public void RemoveTouchInputSource(ITouchInputSource source) {
            if(source == null) throw new ArgumentNullException("source"); // do not check if destroyed here so removal works on destroyed objects
            if(m_UserDefaultTouchInputSource == source) m_UserDefaultTouchInputSource = null;
        }

        /// <summary>
        /// Adds the touch input source.
        /// </summary>
        /// <param name="source">The touch input source.</param>
        public void AddTouchInputSource(ITouchInputSource source) {
            if(Utils.UnityTools.IsNullOrDestroyed(source)) throw new ArgumentNullException("source");
            m_UserDefaultTouchInputSource = source;
        }

        /// <summary>
        /// Returns the number of possible pointers.
        /// Does not count the number actually assigned.
        /// </summary>
        public int GetTouchInputSourceCount(int playerId) {
            return IsDefaultPlayer(playerId) ? 1 : 0;
        }

        protected void ClearMouseInputSources() {
            m_MouseInputSourcesList.Clear();
        }

        protected virtual bool isMouseSupported {
            get {
                int count = m_MouseInputSourcesList.Count;
                if(count == 0) return defaultMouseInputSource.enabled; // fall back to default
                for(int i = 0; i < count; i++) {
                    if(m_MouseInputSourcesList[i].enabled) return true;
                }
                return false;
            }
        }

        protected abstract bool IsDefaultPlayer(int playerId);

        protected bool GetPointerData(int playerId, int pointerIndex, int pointerTypeId, out PlayerPointerEventData data, bool create, PointerEventType pointerEventType) {

            // Get or create the by Player dictionary
            Dictionary<int, PlayerPointerEventData>[] pointerDataByIndex;
            if(!m_PlayerPointerData.TryGetValue(playerId, out pointerDataByIndex)) {
                pointerDataByIndex = new Dictionary<int, PlayerPointerEventData>[pointerIndex + 1];
                for(int i = 0; i < pointerDataByIndex.Length; i++){
                    pointerDataByIndex[i] = new Dictionary<int,PlayerPointerEventData>();
                }
                m_PlayerPointerData.Add(playerId, pointerDataByIndex);
            }

            // Expand array if necessary
            if(pointerIndex >= pointerDataByIndex.Length) { // array is not large enough, expand it

                Dictionary<int, PlayerPointerEventData>[] newArray = new Dictionary<int, PlayerPointerEventData>[pointerIndex + 1];
                for(int i = 0; i < pointerDataByIndex.Length; i++) {
                    newArray[i] = pointerDataByIndex[i];
                }

                newArray[pointerIndex] = new Dictionary<int, PlayerPointerEventData>();
                pointerDataByIndex = newArray;
                m_PlayerPointerData[playerId] = pointerDataByIndex;
                
            }

            // Get or create the pointer event data

            Dictionary<int, PlayerPointerEventData> byMouseIndexDict = pointerDataByIndex[pointerIndex];
            if(!byMouseIndexDict.TryGetValue(pointerTypeId, out data)) {
                if (!create) return false;
                data = CreatePointerEventData(playerId, pointerIndex, pointerTypeId, pointerEventType); // create the event data
                byMouseIndexDict.Add(pointerTypeId, data);
                return true;
            }

            // Update the input sources
            data.mouseSource = pointerEventType == PointerEventType.Mouse ? GetMouseInputSource(playerId, pointerIndex) : null;
            data.touchSource = pointerEventType == PointerEventType.Touch ? GetTouchInputSource(playerId, pointerIndex) : null;

            return false;
        }

        private PlayerPointerEventData CreatePointerEventData(int playerId, int pointerIndex, int pointerTypeId, PointerEventType pointerEventType) {
            PlayerPointerEventData data = new PlayerPointerEventData(eventSystem) {
                playerId = playerId,
                inputSourceIndex = pointerIndex,
                pointerId = pointerTypeId,
                sourceType = pointerEventType
            };

            if(pointerEventType == PointerEventType.Mouse) data.mouseSource = GetMouseInputSource(playerId, pointerIndex); // this can change in the future but it will be updated on Get
            else if(pointerEventType == PointerEventType.Touch) data.touchSource = GetTouchInputSource(playerId, pointerIndex); // this can change in the future but it will be updated on Get

            // Get the button index from the pointerTypeId
            if(pointerTypeId == kMouseLeftId) data.buttonIndex = 0;
            else if(pointerTypeId == kMouseRightId) data.buttonIndex = 1;
            else if(pointerTypeId == kMouseMiddleId) data.buttonIndex = 2;
            else {
                // encoded custom buttons
                if(pointerTypeId >= customButtonsStartingId && pointerTypeId <= customButtonsLastId) {
                    data.buttonIndex = pointerTypeId - customButtonsStartingId;
                }
            }
            return data;
        }

        protected void RemovePointerData(PlayerPointerEventData data) {
            Dictionary<int, PlayerPointerEventData>[] pointerDataByIndex;
            if(m_PlayerPointerData.TryGetValue(data.playerId, out pointerDataByIndex)) {
                if((uint)data.inputSourceIndex < (uint)pointerDataByIndex.Length) {
                    pointerDataByIndex[data.inputSourceIndex].Remove(data.pointerId);
                }
            }
        }

        protected PlayerPointerEventData GetTouchPointerEventData(int playerId, int touchDeviceIndex, Touch input, out bool pressed, out bool released) {
            PlayerPointerEventData pointerData;
            var created = GetPointerData(playerId, touchDeviceIndex, input.fingerId, out pointerData, true, PointerEventType.Touch);

            pointerData.Reset();

            pressed = created || (input.phase == TouchPhase.Began);
            released = (input.phase == TouchPhase.Canceled) || (input.phase == TouchPhase.Ended);

            if(created)
                pointerData.position = input.position;

            if(pressed)
                pointerData.delta = Vector2.zero;
            else
                pointerData.delta = input.position - pointerData.position;

            pointerData.position = input.position;

            pointerData.button = PlayerPointerEventData.InputButton.Left;

            eventSystem.RaycastAll(pointerData, m_RaycastResultCache);

            var raycast = FindFirstRaycast(m_RaycastResultCache);
            pointerData.pointerCurrentRaycast = raycast;
            m_RaycastResultCache.Clear();
            return pointerData;
        }

        protected class MouseState {
            private List<ButtonState> m_TrackedButtons = new List<ButtonState>();

            public bool AnyPressesThisFrame() {
                for(int i = 0; i < m_TrackedButtons.Count; i++) {
                    if(m_TrackedButtons[i].eventData.PressedThisFrame())
                        return true;
                }
                return false;
            }

            public bool AnyReleasesThisFrame() {
                for(int i = 0; i < m_TrackedButtons.Count; i++) {
                    if(m_TrackedButtons[i].eventData.ReleasedThisFrame())
                        return true;
                }
                return false;
            }

            public ButtonState GetButtonState(int button) {
                ButtonState tracked = null;
                for(int i = 0; i < m_TrackedButtons.Count; i++) {
                    if(m_TrackedButtons[i].button == button) {
                        tracked = m_TrackedButtons[i];
                        break;
                    }
                }

                if(tracked == null) {
                    tracked = new ButtonState { button = button, eventData = new MouseButtonEventData() };
                    m_TrackedButtons.Add(tracked);
                }
                return tracked;
            }

            public void SetButtonState(int button, PointerEventData.FramePressState stateForMouseButton, PlayerPointerEventData data) {
                var toModify = GetButtonState(button);
                toModify.eventData.buttonState = stateForMouseButton;
                toModify.eventData.buttonData = data;
            }
        }

        public class MouseButtonEventData {
            public PlayerPointerEventData.FramePressState buttonState;
            public PlayerPointerEventData buttonData;

            public bool PressedThisFrame() {
                return buttonState == PlayerPointerEventData.FramePressState.Pressed || buttonState == PlayerPointerEventData.FramePressState.PressedAndReleased;
            }

            public bool ReleasedThisFrame() {
                return buttonState == PlayerPointerEventData.FramePressState.Released || buttonState == PlayerPointerEventData.FramePressState.PressedAndReleased;
            }
        }

        private readonly MouseState m_MouseState = new MouseState();

        protected virtual MouseState GetMousePointerEventData(int playerId, int mouseIndex) {

            IMouseInputSource mouseInputSource = GetMouseInputSource(playerId, mouseIndex);
            if(mouseInputSource == null) return null;

            // Populate the left button...
            PlayerPointerEventData leftData;
            var created = GetPointerData(playerId, mouseIndex, kMouseLeftId, out leftData, true, PointerEventType.Mouse);

            leftData.Reset();

            if(created)
                leftData.position = mouseInputSource.screenPosition;

            Vector2 pos = mouseInputSource.screenPosition;

            if(mouseInputSource.locked || !mouseInputSource.enabled) {
                // We don't want to do ANY cursor-based interaction when the mouse is locked
                leftData.position = new Vector2(-1.0f, -1.0f);
                leftData.delta = Vector2.zero;
            } else {
                leftData.delta = pos - leftData.position;
                leftData.position = pos;
            }

            leftData.scrollDelta = mouseInputSource.wheelDelta;
            leftData.button = PlayerPointerEventData.InputButton.Left;
            eventSystem.RaycastAll(leftData, m_RaycastResultCache);
            var raycast = FindFirstRaycast(m_RaycastResultCache);
            leftData.pointerCurrentRaycast = raycast;
            m_RaycastResultCache.Clear();

            // copy the apropriate data into right and middle slots
            PlayerPointerEventData rightData;
            GetPointerData(playerId, mouseIndex, kMouseRightId, out rightData, true, PointerEventType.Mouse);
            CopyFromTo(leftData, rightData);
            rightData.button = PlayerPointerEventData.InputButton.Right;

            PlayerPointerEventData middleData;
            GetPointerData(playerId, mouseIndex, kMouseMiddleId, out middleData, true, PointerEventType.Mouse);
            CopyFromTo(leftData, middleData);
            middleData.button = PlayerPointerEventData.InputButton.Middle;

            // Do remaining buttons
            for(int i = 3; i < mouseInputSource.buttonCount; i++) {
                PlayerPointerEventData data;
                GetPointerData(playerId, mouseIndex, customButtonsStartingId + i, out data, true, PointerEventType.Mouse);
                CopyFromTo(leftData, data);
                data.button = (PlayerPointerEventData.InputButton)(-1);
            }

            m_MouseState.SetButtonState(0, StateForMouseButton(playerId, mouseIndex, 0), leftData);
            m_MouseState.SetButtonState(1, StateForMouseButton(playerId, mouseIndex, 1), rightData);
            m_MouseState.SetButtonState(2, StateForMouseButton(playerId, mouseIndex, 2), middleData);
            // Do remaining buttons
            for(int i = 3; i < mouseInputSource.buttonCount; i++) {
                PlayerPointerEventData data;
                GetPointerData(playerId, mouseIndex, customButtonsStartingId + i, out data, false, PointerEventType.Mouse);
                m_MouseState.SetButtonState(i, StateForMouseButton(playerId, mouseIndex, i), data);
            }

            return m_MouseState;
        }

        protected PlayerPointerEventData GetLastPointerEventData(int playerId, int pointerIndex, int pointerTypeId, bool ignorePointerTypeId, PointerEventType pointerEventType) {
            PlayerPointerEventData data;
            if(!ignorePointerTypeId) {
                GetPointerData(playerId, pointerIndex, pointerTypeId, out data, false, pointerEventType);
                return data;
            }

            Dictionary<int, PlayerPointerEventData>[] pointerDataByIndex;
            if(!m_PlayerPointerData.TryGetValue(playerId, out pointerDataByIndex)) return null;
            if((uint)pointerIndex >= (uint)pointerDataByIndex.Length) return null;
            
            // Just get the first found regardless of the button id
            foreach(var kvp in pointerDataByIndex[pointerIndex]) return kvp.Value;

            return null;
        }

        private static bool ShouldStartDrag(Vector2 pressPos, Vector2 currentPos, float threshold, bool useDragThreshold) {
            if(!useDragThreshold)
                return true;

            return (pressPos - currentPos).sqrMagnitude >= threshold * threshold;
        }

        protected virtual void ProcessMove(PlayerPointerEventData pointerEvent) {
            GameObject targetGO;
            if(pointerEvent.sourceType == PointerEventType.Mouse) {
                IMouseInputSource source = GetMouseInputSource(pointerEvent.playerId, pointerEvent.inputSourceIndex);
                if(source != null) {
                    targetGO = !source.enabled || source.locked ? null : pointerEvent.pointerCurrentRaycast.gameObject;
                } else {
                    targetGO = null;
                }
            } else if(pointerEvent.sourceType == PointerEventType.Touch) {
                targetGO = pointerEvent.pointerCurrentRaycast.gameObject;
            } else throw new NotImplementedException();
            HandlePointerExitAndEnter(pointerEvent, targetGO);
        }

        protected virtual void ProcessDrag(PlayerPointerEventData pointerEvent) {
            if(!pointerEvent.IsPointerMoving() || pointerEvent.pointerDrag == null) return;
            if(pointerEvent.sourceType == PointerEventType.Mouse) {
                IMouseInputSource source = GetMouseInputSource(pointerEvent.playerId, pointerEvent.inputSourceIndex);
                if(source == null || source.locked || !source.enabled) return;
            }

            if(!pointerEvent.dragging
                && ShouldStartDrag(pointerEvent.pressPosition, pointerEvent.position, eventSystem.pixelDragThreshold, pointerEvent.useDragThreshold)) {
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.beginDragHandler);
                pointerEvent.dragging = true;
            }

            // Drag notification
            if(pointerEvent.dragging) {
                // Before doing drag we should cancel any pointer down state
                // And clear selection!
                if(pointerEvent.pointerPress != pointerEvent.pointerDrag) {
                    ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

                    pointerEvent.eligibleForClick = false;
                    pointerEvent.pointerPress = null;
                    pointerEvent.rawPointerPress = null;
                }
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.dragHandler);
            }
        }

        public override bool IsPointerOverGameObject(int pointerTypeId) {
            foreach(var perPlayer in m_PlayerPointerData) {
                foreach(var perIndex in perPlayer.Value) {
                    PlayerPointerEventData data;
                    if(!perIndex.TryGetValue(pointerTypeId, out data)) continue;
                    if(data.pointerEnter != null) return true;
                }
            }
            return false;
        }

        protected void ClearSelection() {
            var baseEventData = GetBaseEventData();

            foreach(var playerSetKVP in m_PlayerPointerData) {
                var byIndex = playerSetKVP.Value;
                for(int i = 0; i < byIndex.Length; i++) {
                    foreach(var buttonSetKVP in byIndex[i]) {
                        // clear all selection
                        HandlePointerExitAndEnter(buttonSetKVP.Value, null);
                    }
                    byIndex[i].Clear();
                }
            }
            eventSystem.SetSelectedGameObject(null, baseEventData);
        }

        public override string ToString() {
            var sb = new StringBuilder("<b>Pointer Input Module of type: </b>" + GetType());
            sb.AppendLine();
            foreach(var playerSetKVP in m_PlayerPointerData) {
                sb.AppendLine("<B>Player Id:</b> " + playerSetKVP.Key);
                var byIndex = playerSetKVP.Value;
                for(int i = 0; i < byIndex.Length; i++) {
                    sb.AppendLine("<B>Pointer Index:</b> " + i);
                    foreach(var buttonSetKVP in byIndex[i]) {
                        sb.AppendLine("<B>Button Id:</b> " + buttonSetKVP.Key);
                        sb.AppendLine(buttonSetKVP.Value.ToString());
                    }
                }
            }
            return sb.ToString();
        }

        protected void DeselectIfSelectionChanged(GameObject currentOverGo, BaseEventData pointerEvent) {
            // Selection tracking
            var selectHandlerGO = ExecuteEvents.GetEventHandler<ISelectHandler>(currentOverGo);
            // if we have clicked something new, deselect the old thing
            // leave 'selection handling' up to the press event though.
            if (selectHandlerGO != eventSystem.currentSelectedGameObject) {
                eventSystem.SetSelectedGameObject(null, pointerEvent);
            }
        }

        protected void CopyFromTo(PointerEventData @from, PointerEventData @to) {
            @to.position = @from.position;
            @to.delta = @from.delta;
            @to.scrollDelta = @from.scrollDelta;
            @to.pointerCurrentRaycast = @from.pointerCurrentRaycast;
            @to.pointerEnter = @from.pointerEnter;
        }

        protected PointerEventData.FramePressState StateForMouseButton(int playerId, int mouseIndex, int buttonId) {
            IMouseInputSource mouseInputSource = GetMouseInputSource(playerId, mouseIndex);
            if(mouseInputSource == null) return PointerEventData.FramePressState.NotChanged;
            var pressed = mouseInputSource.GetButtonDown(buttonId);
            var released = mouseInputSource.GetButtonUp(buttonId);
            if(pressed && released)
                return PointerEventData.FramePressState.PressedAndReleased;
            if(pressed)
                return PointerEventData.FramePressState.Pressed;
            if(released)
                return PointerEventData.FramePressState.Released;
            return PointerEventData.FramePressState.NotChanged;
        }

        protected class ButtonState {
            private int m_Button = 0;

            public MouseButtonEventData eventData {
                get { return m_EventData; }
                set { m_EventData = value; }
            }

            public int button {
                get { return m_Button; }
                set { m_Button = value; }
            }

            private MouseButtonEventData m_EventData;
        }

        private sealed class UnityInputSource : IMouseInputSource, ITouchInputSource {

            private Vector2 m_MousePosition;
            private Vector2 m_MousePositionPrev;
            private int m_LastUpdatedFrame = -1;

            int IMouseInputSource.playerId {
                get { TryUpdate(); return 0; }
            }

            int ITouchInputSource.playerId {
                get { TryUpdate(); return 0; }
            }

            bool IMouseInputSource.enabled {
                get {
                    TryUpdate();
                    return true;
                    // return Input.mousePresent; // REMOVED: Input.mousePresent is unreliable. Some platforms will return false when a mouse is present and working.
                }
            }

            bool IMouseInputSource.locked {
                get {
                    TryUpdate();
#if UNITY_5_PLUS
                    return Cursor.lockState == CursorLockMode.Locked;
#else
                    return false;
#endif
                }
            }

            int IMouseInputSource.buttonCount {
                get {
                    TryUpdate(); return 3;
                }
            }

            bool IMouseInputSource.GetButtonDown(int button) {
                TryUpdate(); return Input.GetMouseButtonDown(button);
            }

            bool IMouseInputSource.GetButtonUp(int button) {
                TryUpdate(); return Input.GetMouseButtonUp(button);
            }

            bool IMouseInputSource.GetButton(int button) {
                TryUpdate(); return Input.GetMouseButton(button);
            }

            Vector2 IMouseInputSource.screenPosition {
                get { TryUpdate(); return Input.mousePosition; }
            }

            Vector2 IMouseInputSource.screenPositionDelta {
                get { TryUpdate(); return m_MousePosition - m_MousePositionPrev; }
            }

            Vector2 IMouseInputSource.wheelDelta {
                get { TryUpdate(); return Input.mouseScrollDelta; }
            }

            bool ITouchInputSource.touchSupported {
                get { TryUpdate(); return Input.touchSupported; }
            }

            int ITouchInputSource.touchCount {
                get { TryUpdate(); return Input.touchCount; }
            }

            Touch ITouchInputSource.GetTouch(int index) {
                TryUpdate(); return Input.GetTouch(index);
            }

            private void TryUpdate() {
                if(Time.frameCount == m_LastUpdatedFrame) return;
                m_LastUpdatedFrame = Time.frameCount;
                m_MousePositionPrev = m_MousePosition;
                m_MousePosition = Input.mousePosition;
            }
        }
    }

    public enum PointerEventType {
        Mouse = 0,
        Touch = 1
    }
}