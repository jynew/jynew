using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jyx2.SharedScripts
{
    public interface IBattleServer
    {
        /// <summary>
        /// 申请战场房间
        /// </summary>
        /// <param name="battle">己方战场初始化信息</param>
        /// <returns>返回信息，id如果为-1，则申请失败，否则为房间编号</returns>
        Task<RetInfo> RequestForBattleRoom(BattleFieldSettings battleFieldSettings);


        /// <summary>
        /// 加入战场
        /// </summary>
        /// <param name="battleTeamConfig">队伍信息</param>
        /// <returns></returns>
        Task<RetInfo> JoinBattle(BattleTeamConfig battleTeamConfig);
    }

    /// <summary>
    /// 战场配置信息
    /// </summary>
    public struct BattleFieldSettings
    {
        
    }
    
    /// <summary>
    /// 己方战斗队伍配置
    /// </summary>
    public struct BattleTeamConfig
    {
        /// <summary>
        /// 参与战斗的单位
        /// </summary>
        public List<BattleUnit> Units;
        
        
        /// <summary>
        /// 战斗结果回调
        /// </summary>
        public Action<BattleResult> BattleResultCallback;
        
        /// <summary>
        /// 是否是AI
        /// </summary>
        public bool IsAI;
    }

    public enum BattleResult
    {
        Win, Lose, Draw
    }

    public struct RetInfo
    {
        public int id;
        public string errMsg;
    }
    
    
    public class BattleServer : IBattleServer
    {
        public Task<RetInfo> RequestForBattleRoom(BattleFieldSettings battleFieldSettings)
        {
            throw new NotImplementedException();
        }

        public Task<RetInfo> JoinBattle(BattleTeamConfig battleTeamConfig)
        {
            throw new NotImplementedException();
        }
    }
}
