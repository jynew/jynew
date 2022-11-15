﻿using Sirenix.OdinInspector;
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
        
        [LabelText("交互")]
        Interact = 2,

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
        
    }
    
    public class Jyx2ActionConst
    {
        public const int MoveHorizontal = (int)Jyx2PlayerAction.MoveHorizontal;
        public const int MoveVertical = (int)Jyx2PlayerAction.MoveVertical;
        public const int Interact = (int)Jyx2PlayerAction.Interact;
        public const int UIHorizontal = (int)Jyx2PlayerAction.UIHorizontal;
        public const int UIVertical = (int)Jyx2PlayerAction.UIVertical;
        public const int UIConfirm = (int)Jyx2PlayerAction.UIConfirm;
        public const int UICancel = (int)Jyx2PlayerAction.UICancel;
        public const int UIClose = (int)Jyx2PlayerAction.UIClose;
        public const int UISwitchLeft = (int)Jyx2PlayerAction.UISwitchLeft;
        public const int UISwitchRight = (int)Jyx2PlayerAction.UISwitchRight;
    }
}
