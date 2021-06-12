
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jyx2
{
    //战斗行动指令
    public class BattleAction
    {
        //移动到的坐标
        public BattleBlockVector MoveTo;

        //使用技能
        public BattleZhaoshiInstance Skill;

        //技能释放点
        public BattleBlockVector SkillTo;
    }
}
