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
