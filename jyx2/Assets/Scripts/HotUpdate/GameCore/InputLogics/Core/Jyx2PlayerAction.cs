using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jyx2.InputCore
{
    public enum Jyx2PlayerAction
    {
        [LabelText("X轴移动")]
        MoveHorizontal = 0,
        
        [LabelText("Y轴移动")]
        MoveVertical = 1,
        
        [LabelText("交互按钮1")]
        Interact1 = 2,

        [LabelText("交互按钮2")]
        Interact2 = 16,  //新增的所以id排到UI_Bag后面去了

        [LabelText("UI水平移动")]
        UIHorizontal = 3,

        [LabelText("UI垂直移动")]
        UIVertical = 4,

        [LabelText("UI确认")]
        UIConfirm = 5,

        [LabelText("UI取消")]
        UICancel = 6,

        [LabelText("UI关闭")]
        UIClose = 7,

        [LabelText("UI左选项切换")]
        UISwitchLeft = 8,

        [LabelText("UI右选项切换")]
        UISwitchRight = 9,

        [LabelText("特殊UI确认YES")]
        UI_Yes = 10,

        [LabelText("特殊UI否认NO")]
        UI_No = 11,

        [LabelText("打开系统菜单")]
        UI_SystemMenu = 13,

        [LabelText("打开侠客属性")]
        UI_Xiake = 14,

        [LabelText("打开背包")]
        UI_Bag = 15,

        [LabelText("向左旋转")]
        RotateLeft = 17,

        [LabelText("向右旋转")]
        RotateRight = 18,

        [LabelText("摄像机水平移动")]
        CameraMoveX = 19,


        [LabelText("摄像机垂直移动")]
        CameraMoveY = 20,
    }
    
    public class Jyx2ActionConst
    {
        public const int MoveHorizontal = (int)Jyx2PlayerAction.MoveHorizontal;
        public const int MoveVertical = (int)Jyx2PlayerAction.MoveVertical;
        public const int Interact1 = (int)Jyx2PlayerAction.Interact1;
        public const int Interact2 = (int)Jyx2PlayerAction.Interact2;
        public const int UIHorizontal = (int)Jyx2PlayerAction.UIHorizontal;
        public const int UIVertical = (int)Jyx2PlayerAction.UIVertical;
        public const int UIConfirm = (int)Jyx2PlayerAction.UIConfirm;
        public const int UICancel = (int)Jyx2PlayerAction.UICancel;
        public const int UIClose = (int)Jyx2PlayerAction.UIClose;
        public const int UISwitchLeft = (int)Jyx2PlayerAction.UISwitchLeft;
        public const int UISwitchRight = (int)Jyx2PlayerAction.UISwitchRight;
        public const int UI_Yes = (int)Jyx2PlayerAction.UI_Yes;
        public const int UI_No = (int)Jyx2PlayerAction.UI_No;
        public const int UI_SystemMenu = (int)Jyx2PlayerAction.UI_SystemMenu;
        public const int UI_Xiake = (int)Jyx2PlayerAction.UI_Xiake;
        public const int UI_Bag = (int)Jyx2PlayerAction.UI_Bag;
        public const int RotateLeft = (int)Jyx2PlayerAction.RotateLeft;
        public const int RotateRight = (int)Jyx2PlayerAction.RotateRight;
        public const int CameraMoveX = (int)Jyx2PlayerAction.CameraMoveX;
        public const int CameraMoveY = (int)Jyx2PlayerAction.CameraMoveY;
    }
}
