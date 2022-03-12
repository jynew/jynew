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
        /// <summary>
        /// 申请分配玩家连接token
        /// </summary>
        /// <returns></returns>
        string RequestForPlayerToken(int roomId);
    }

    /// <summary>
    /// 战场配置信息
    /// </summary>
    public class BattleFieldSettings
    {
        /// <summary>
        /// TODO：改为具体的战斗配置信息
        /// </summary>
        public int BattleId;

        /// <summary>
        /// 队伍数量
        /// </summary>
        public int TeamCount = 2;
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
        public List<RoleInstance> Roles;

        /// <summary>
        /// 是否是AI
        /// </summary>
        public bool IsAI;
        
        /// <summary>
        /// 战场数据回调
        /// </summary>
        public Action<BattleMsg> BattleMsgCallback;
        
        /// <summary>
        /// 战斗结果回调
        /// </summary>
        public Action<BattleResult> BattleResultCallback;
    }

    public enum BattleResult
    {
        Win, Lose, Draw
    }

    /// <summary>
    /// 通信返回标准数据结构
    /// </summary>
    public struct RetInfo
    {
        /// <summary>
        /// 存放返回值的int
        /// </summary>
        public int RetCode; 
        
        /// <summary>
        /// 存放返回值的string
        /// </summary>
        public string RetMsg; 
        
        /// <summary>
        /// 错误码
        /// </summary>
        public ErrorCode ErrCode; 
    }

    /// <summary>
    /// 战场传送的数据
    /// </summary>
    public struct BattleMsg
    {
        public string Msg;
    }
    
    /// <summary>
    /// 客户端虚拟的战斗服务器
    /// </summary>
    public class FakeBattleServer : IBattleC2S, IBattleRoomHost
    {

        /// <summary>
        /// 当前所有的战斗房间
        /// </summary>
        private readonly Dictionary<int, BattleRoom> _dictBattleRooms = new Dictionary<int, BattleRoom>();


        /// <summary>
        /// 客户端到房间的映射
        ///
        /// key为客户端token
        /// value为房间id
        /// </summary>
        private readonly Dictionary<string, int> _clientRoomMapping = new Dictionary<string, int>();
        
        /// <summary>
        /// 房间id分配
        /// </summary>
        private int _roomIdIndex = 0;

        public async Task<RetInfo> RequestForBattleRoom(BattleFieldSettings battleFieldSettings)
        {
            if (_dictBattleRooms.Count > BattleServerConfigs.MAX_BATTLE_ROOMS)
            {
                return new RetInfo() {RetCode = -1, ErrCode = ErrorCode.RoomIsFull};
            }
            
            int id = _roomIdIndex++;
            var battleRoom = new BattleRoom(id, this, battleFieldSettings);

            //创建战场房间
            _dictBattleRooms.Add(id, battleRoom);

            return new RetInfo() {RetCode = id};
        }

        public async Task<RetInfo> JoinBattleRoom(int battleRoomId, BattleClientSetup battleClientSetup)
        {
            //检测房间是否已经被创建
            if (!_dictBattleRooms.ContainsKey(battleRoomId))
            {
                return new RetInfo() {RetCode = -1, ErrCode = ErrorCode.RoomIsNotExist};
            }

            var room = _dictBattleRooms[battleRoomId];
            
            //连接到房间服务器，分配玩家的token
            var errCode = room.PlayerConnect(battleClientSetup,out var playerToken);
            
            if (errCode == ErrorCode.Success)
            {
                return new RetInfo() {RetMsg = playerToken, ErrCode = errCode};
            }
            else
            {
                return new RetInfo() {ErrCode = errCode};
            }
        }

        /// <summary>
        /// 房间服务器向网关服务器申请玩家连接房间的token
        /// </summary>
        /// <returns></returns>
        public string RequestForPlayerToken(int roomId)
        {
            if (_clientRoomMapping.Count > BattleServerConfigs.MAX_CONNECTED_PLAYERS)
            {
                return string.Empty;
            }
            
            string token = System.Guid.NewGuid().ToString();
            _clientRoomMapping.Add(token, roomId); //记录玩家连接到房间的token - 房间ID映射
            return token;
        }
    }
}
