/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jyx2.Battle
{
    public static class BattleUtil
    {
        public static bool CellInRange(int posX,int posY,int otherX,int otherY,int range)
        {
            int distance = Mathf.Abs(posX - otherX) + Mathf.Abs(posY - otherY);
            return range >= distance;
        }
    }
}
