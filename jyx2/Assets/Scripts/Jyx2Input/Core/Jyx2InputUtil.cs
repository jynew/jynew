using Rewired;
using System;
using System.Linq;
using UnityEngine;

namespace Jyx2.InputCore.Util
{
    public static class InputUtil
    {
        public static Pole AxisRange2Pole(AxisRange axisRange)
        {
            Pole pole = Pole.Positive;
            if (axisRange == AxisRange.Negative)
            {
                pole = Pole.Negative;
            }
            return pole;
        }

        public static ControllerMap GetKeyboardMap(this Player player, int categoryId = 0, int layoutId = 0)
        {
            var keyBoard = player.controllers.Keyboard;
            if (keyBoard != null)
            {
                return player.controllers.maps.GetMap(keyBoard, categoryId, layoutId);
            }
            return null;
        }

        public static ActionElementMap GetActionElementMap(this ControllerMap controllerMap, Jyx2PlayerAction actionID, AxisRange axisRange)
        {
            ActionElementMap res = null;
            if (controllerMap != null)
            {
                var elements = controllerMap.GetElementMapsWithAction((int)actionID);
                var pole = AxisRange2Pole(axisRange);
                res = elements.FirstOrDefault(item => item.axisContribution == pole);
            }
            return res;
        }


        public static string GetCurrentButtonName(this Player player, Jyx2PlayerAction actionID, AxisRange axisRange = AxisRange.Positive)
        {
            var lastController = player.controllers.GetLastActiveController();
            var controllerType = lastController?.type ?? ControllerType.Keyboard;
            var result = "?";
            if (controllerType == ControllerType.Keyboard)
            {
                var keyboadMap = GetKeyboardMap(player);
                var elementMap = keyboadMap?.GetActionElementMap(actionID, axisRange);
                if (elementMap != null)
                {
                    if (elementMap.keyCode == KeyCode.UpArrow)
                    {
                        result = "↑";
                    }
                    else if (elementMap.keyCode == KeyCode.DownArrow)
                    {
                        result = "↓";
                    }
                    else if (elementMap.keyCode == KeyCode.LeftArrow)
                    {
                        result = "←";
                    }
                    else if (elementMap.keyCode == KeyCode.RightArrow)
                    {
                        result = "→";
                    }
                    else
                    {
                        result = elementMap.keyCode.ToString();
                    }
                }
            }
            else if (controllerType == ControllerType.Joystick)
            {
                var controllerMap = GetControllerMap(player, lastController);
                var elementMap = controllerMap?.GetActionElementMap(actionID, axisRange);
                if (elementMap != null)
                {
                    //TODO 手柄按钮sprite显示
                    result = elementMap.elementIdentifierName;
                }
            }
            return result;
        }
        
        public static ControllerMap GetControllerMap(this Player player, Controller controller, int mapId = 0, int layout = 0)
        {
            return player.controllers.maps.GetMap(controller, mapId, layout);
        }
    }
}