
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jyx2
{
    public enum SkillCoverType
    {
        /// <summary>
        /// 点攻击
        /// CoverSize>1为目标点区域施展（面攻击）
        /// </summary>
        POINT = 0,

        /// <summary>
        /// 直线攻击
        /// CastSize参数无效
        /// </summary>
        LINE = 1,

        /// <summary>
        /// 十字
        /// </summary>
        CROSS = 2,
        
        /// <summary>
        /// 面攻击
        /// </summary>
        FACE = 3,

        /// <summary>
        /// 无效
        /// </summary>
        INVALID = -1,
    }

    public enum MoveDirection
    {
        UP_RIGHT = 0,
        RIGHT = 1,
        DOWN_RIGHT = 2,
        DOWN_LEFT = 3,
        LEFT = 4,
        UP_LEFT = 5,
        UP = 6,
        DOWN = 7,
        ERROR = -1
    }
}
