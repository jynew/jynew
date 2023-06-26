using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using Jyx2.ResourceManagement;
using Rewired;

namespace Jyx2.InputCore.Data
{
    public class DefaultControllerSpriteData
    {
        public Jyx2PlayerAction ActionType { get; set; }

        public AxisRange AxisRange { get; set; }

        public string ButtonName { get; set; }

        public DefaultControllerSpriteData(Jyx2PlayerAction actionType, AxisRange axisRange, string buttonName)
        {
            ActionType = actionType;
            AxisRange = axisRange;
            ButtonName = buttonName;
        }
    }

    public static class DefaultControllerButtonName
    {
        private static readonly List<DefaultControllerSpriteData> m_DataList = new List<DefaultControllerSpriteData>()
        {
            new DefaultControllerSpriteData(Jyx2PlayerAction.MoveHorizontal, AxisRange.Positive, "Left Stick Right"),
            new DefaultControllerSpriteData(Jyx2PlayerAction.MoveHorizontal, AxisRange.Negative, "Left Stick Left"),
            new DefaultControllerSpriteData(Jyx2PlayerAction.MoveVertical, AxisRange.Positive, "Left Stick Up"),
            new DefaultControllerSpriteData(Jyx2PlayerAction.MoveVertical, AxisRange.Negative, "Left Stick Down"),
            new DefaultControllerSpriteData(Jyx2PlayerAction.Interact1, AxisRange.Positive, "Action Top Row 1"),
            new DefaultControllerSpriteData(Jyx2PlayerAction.Interact2, AxisRange.Positive, "Action Top Row 2"),
            new DefaultControllerSpriteData(Jyx2PlayerAction.UIHorizontal, AxisRange.Positive, "Left Stick Right"),
            new DefaultControllerSpriteData(Jyx2PlayerAction.UIHorizontal, AxisRange.Negative, "Left Stick Left"),
            new DefaultControllerSpriteData(Jyx2PlayerAction.UIVertical, AxisRange.Positive, "Left Stick Up"),
            new DefaultControllerSpriteData(Jyx2PlayerAction.UIVertical, AxisRange.Negative, "Left Stick Down"),

            new DefaultControllerSpriteData(Jyx2PlayerAction.UIConfirm, AxisRange.Positive, "Action Bottom Row 1"),
            new DefaultControllerSpriteData(Jyx2PlayerAction.UICancel, AxisRange.Positive, "Action Bottom Row 2"),
            new DefaultControllerSpriteData(Jyx2PlayerAction.UIClose, AxisRange.Positive, "Action Bottom Row 2"),
            new DefaultControllerSpriteData(Jyx2PlayerAction.UISwitchLeft, AxisRange.Positive, "Left Shoulder 1"),
            new DefaultControllerSpriteData(Jyx2PlayerAction.UISwitchRight, AxisRange.Positive, "Right Shoulder 1"),
            new DefaultControllerSpriteData(Jyx2PlayerAction.UI_Yes, AxisRange.Positive, "Action Top Row 1"),
            new DefaultControllerSpriteData(Jyx2PlayerAction.UI_No, AxisRange.Positive, "Action Top Row 2"),
            new DefaultControllerSpriteData(Jyx2PlayerAction.UI_SystemMenu, AxisRange.Positive, "Center 2"),
            new DefaultControllerSpriteData(Jyx2PlayerAction.UI_Xiake, AxisRange.Positive, "Left Shoulder 1"),
            new DefaultControllerSpriteData(Jyx2PlayerAction.UI_Bag, AxisRange.Positive, "Right Shoulder 1"),

            new DefaultControllerSpriteData(Jyx2PlayerAction.RotateLeft, AxisRange.Positive, "Left Shoulder 2"),
            new DefaultControllerSpriteData(Jyx2PlayerAction.RotateRight, AxisRange.Positive, "Right Shoulder 2" ),

            new DefaultControllerSpriteData(Jyx2PlayerAction.CameraMoveX, AxisRange.Positive, "Right Stick Right"),
            new DefaultControllerSpriteData(Jyx2PlayerAction.CameraMoveX, AxisRange.Negative, "Right Stick Left"),
            new DefaultControllerSpriteData(Jyx2PlayerAction.CameraMoveY, AxisRange.Positive, "Right Stick Up"),
            new DefaultControllerSpriteData(Jyx2PlayerAction.CameraMoveY, AxisRange.Negative, "Right Stick Down"),

        };

        public static string GetButtonNameRichText(Jyx2PlayerAction playerAction, AxisRange axisRange)
        {
            var result = m_DataList.Find(item => item.ActionType == playerAction && item.AxisRange == axisRange);
            if (result != null)
            {
                return $"<sprite=\"Default_Control\" name=\"{result.ButtonName}\">";
            }
            return "?";
        }
    }
}