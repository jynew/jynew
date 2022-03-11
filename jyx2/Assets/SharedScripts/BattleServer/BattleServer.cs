using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jyx2.SharedScripts.BattleServer
{
    /// <summary>
    /// 战斗房间的管理器
    /// </summary>
    public interface IBattleRoomHost
    {
        
    }

    /// <summary>
    /// 战场配置信息
    /// </summary>
    public struct BattleFieldSettings
    {
        public int BattleId;
    }
    
    /// <summary>
    /// 己方战斗队伍配置
    /// </summary>
    public struct BattleClientSetup
    {
        /// <summary>
        /// 队伍编号
        /// 从0开始
        /// </summary>
        public int Team;
        
        /// <summary>
        /// 参与战斗的单位
        /// </summary>
        public List<BattleUnit> Units;


        /// <summary>
        /// 战场数据回调
        /// </summary>
        public Action<BattleMsg> BattleMsgCallback;
        
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
        public int Id;
        public ErrorCode ErrCode;
    }

    /// <summary>
    /// 战场传送的数据
    /// </summary>
    public struct BattleMsg
    {
        
    }
    
    /// <summary>
    /// 战斗服务器
    /// </summary>
    public class BattleC2SImpl : IBattleC2S, IBattleRoomHost
    {

        /// <summary>
        /// 当前所有的战斗房间
        /// </summary>
        private readonly Dictionary<int, BattleRoom> _dictBattleRooms = new Dictionary<int, BattleRoom>();

        /// <summary>
        /// 房间id分配
        /// </summary>
        private int _roomIdIndex = 0;
        
        public async Task<RetInfo>  RequestForBattleRoom(BattleFieldSettings battleFieldSettings)
        {
            if (_dictBattleRooms.Count > BattleServerConfigs.MAX_BATTLE_ROOMS)
            {
                return new RetInfo() {Id = -1, ErrCode = ErrorCode.RoomIsFull};
            }

            var battleRoom = new BattleRoom(this, battleFieldSettings);
            int id = _roomIdIndex++;
            
            //创建战场房间
            _dictBattleRooms.Add(id, battleRoom);

            return new RetInfo() {Id = id};
        }

        public async Task<RetInfo> JoinBattle(int battleRoomId, BattleClientSetup battleClientSetup)
        {
            //检测房间是否已经被创建
            if (!_dictBattleRooms.ContainsKey(battleRoomId))
            {
                return new RetInfo() {Id = -1, ErrCode = ErrorCode.RoomIsNotExist};
            }

            var room = _dictBattleRooms[battleRoomId];
            
            
            
            throw new NotImplementedException();
        }
        
    }
}
