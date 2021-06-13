
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jyx2
{
    public enum MapRoleBehavior
    {
        Npc, //NPC，包括原地不动等
        Guide, //引路
        Teammate, //队友
        NetworkPlayer, //联机角色
        LocalPlayer,
        Enemy,
    }

    public enum MapRoleStatus
    {
        Normal, //正常状态
        Battle, //战斗中
    }

    public enum MapRoleHealth
    {
        Normal, //正常状态
        Stun, //晕眩状态
        Death, //死亡状态
    }
}
