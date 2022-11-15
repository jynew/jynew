// Copyright (c) 2018 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Rewired.Integration.UnityUI {
    using System.Text;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using Rewired.UI;

    /// <summary>
    /// Each touch event creates one of these containing all the relevant information.
    /// </summary>
    public class PlayerPointerEventData : PointerEventData {

        /// <summary>
        /// The Player id of the Player that generated this event.
        /// </summary>
        public int playerId { get; set; }

        /// <summary>
        /// The index of the mouse/touch input source owned by the Player that generated this event.
        /// </summary>
        public int inputSourceIndex { get; set; }

        /// <summary>
        /// The mouse that generated this event.
        /// </summary>
        public IMouseInputSource mouseSource { get; set; }

        /// <summary>
        /// The touch source that generated this event.
        /// </summary>
        public ITouchInputSource touchSource { get; set; }

        /// <summary>
        /// The input source type that generated this event.
        /// </summary>
        public PointerEventType sourceType { get; set; }

        /// <summary>
        /// The index of the button that generated this event.
        /// </summary>
        public int buttonIndex { get; set; }

        public PlayerPointerEventData(EventSystem eventSystem)
            : base(eventSystem) {
            playerId = -1;
            inputSourceIndex = -1;
            buttonIndex = -1;
        }

        public override string ToString() {
            var sb = new StringBuilder();
            sb.AppendLine("<b>Player Id</b>: " + playerId);
            sb.AppendLine("<b>Mouse Source</b>: " + mouseSource);
            sb.AppendLine("<b>Input Source Index</b>: " + inputSourceIndex);
            sb.AppendLine("<b>Touch Source/b>: " + touchSource);
            sb.AppendLine("<b>Source Type</b>: " + sourceType);
            sb.AppendLine("<b>Button Index</b>: " + buttonIndex);
            sb.Append(base.ToString());
            return sb.ToString();
        }
    }
}