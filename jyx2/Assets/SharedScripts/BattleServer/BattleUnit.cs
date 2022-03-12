using System.Threading.Tasks;

namespace Jyx2.SharedScripts.BattleServer
{
    /// <summary>
    /// 战场单位
    /// </summary>
    public class BattleUnit
    {
        public int Team;
        public RoleInstance m_Role;

        //是否存活
        public bool IsAlive()
        {
            return m_Role.Hp > 0;
        }

        /// <summary>
        /// 战斗单位行动，给出行动结果
        /// </summary>
        /// <returns></returns>
        public async Task<BattleUnitActionResult> BattleUnitAction()
        {
            //TODO：调整为正式逻辑，这只是个测试逻辑
            if(m_Role.Hp > 0)
                m_Role.Hp--;
            
            BattleUnitActionResult rst = 
                new BattleUnitActionResult() {Result = $"Hello , my team = {Team}, my hp = {m_Role.Hp}"};
            
            return rst;
        }
    }

    public struct BattleUnitActionResult
    {
        public string Result;
    }


    public class RoleInstance
    {
        public int Hp;
    }
}
