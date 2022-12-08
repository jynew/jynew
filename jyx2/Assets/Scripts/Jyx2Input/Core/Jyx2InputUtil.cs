using Jyx2.InputCore.Data;
using Jyx2.ResourceManagement;
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

        private static ControllerMap GetKeyboardMap(this Player player, int categoryId = 0, int layoutId = 0)
        {
            var keyBoard = player.controllers.Keyboard;
            if (keyBoard != null)
            {
                return player.controllers.maps.GetMap(keyBoard, categoryId, layoutId);
            }
            return null;
        }

        private static ActionElementMap GetActionElementMap(this ControllerMap controllerMap, Jyx2PlayerAction actionID, AxisRange axisRange)
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


        public static string GetPlayerButtonName(this Player player, Jyx2PlayerAction actionID, AxisRange axisRange = AxisRange.Positive)
        {
            var lastController = player.controllers.GetLastActiveController();
            var controllerType = lastController?.type ?? ControllerType.Keyboard;
            var result = "?";
            if (controllerType == ControllerType.Keyboard)
            {
                
                var keyboad = player.controllers.Keyboard;
                var keyBoardMaps = player.controllers.maps.GetMaps(keyboad);
                ActionElementMap elementMap = null;
                foreach (var map in keyBoardMaps)
                {
                    elementMap = map.GetActionElementMap(actionID, axisRange);
                    if (elementMap != null)
                    {
                        result = GetKeyCodeRichTextName(elementMap);
                        break;
                    }
                }
            }
            else if (controllerType == ControllerType.Joystick)
            {
                var controllerMaps = player.controllers.maps.GetMaps(lastController);
                ActionElementMap elementMap = null;
                foreach (var map in controllerMaps)
                {
                    elementMap = map.GetActionElementMap(actionID, axisRange);
                    if (elementMap != null)
                    {
                        result = GetJoyStickButtonRichText(lastController, elementMap);
                        break;
                    }
                }
            }
            return result;
        }

        private static string GetKeyCodeRichTextName(ActionElementMap elementMap)
        {
            string result = "?";
            if (elementMap == null)
                return result;
            
            result = elementMap.keyCode.ToString();
            var controllerAssetMap = ResLoader.LoadAssetSync<ControllerSpriteAsset>("Assets/BuildSource/Gamepad/ControllerSpriteAsset.asset");
            var spriteAsset = controllerAssetMap?.GetSpriteAsset("Keyboard");
            if (spriteAsset == null)
                return result;
            
            var keyCodeName = result;
            if (spriteAsset.GetSpriteIndexFromName(keyCodeName) == -1)
                return keyCodeName;
            
            result = $"<sprite=\"{spriteAsset.name}\" name=\"{keyCodeName}\">";
            return result;
        }

        private static string GetJoyStickButtonRichText(Controller controller, ActionElementMap elementMap)
        {
            string result = "?";
            if (controller == null || elementMap == null)
                return result;
            var controllerAssetMap = ResLoader.LoadAssetSync<ControllerSpriteAsset>("Assets/BuildSource/Gamepad/ControllerSpriteAsset.asset");
            var spriteAsset = controllerAssetMap?.GetSpriteAsset(controller.hardwareTypeGuid.ToString());
            if (spriteAsset != null)
            {
                result = $"<sprite=\"{spriteAsset.name}\" name=\"{elementMap.elementIdentifierName}\">";
            }
            return result;
        }

        private static ControllerMap GetControllerMap(this Player player, Controller controller, int mapId = 0, int layout = 0)
        {
            return player.controllers.maps.GetMap(controller, mapId, layout);
        }
    }
}