using System.Collections.Generic;
using Codice.Client.Common;

namespace Jyx2.SharedScripts.BattleServer
{
    /// <summary>
    /// 游戏客户端注册连接的标记
    /// </summary>
    public class BattleClientHandle
    {
        /// <summary>
        /// 客户端ID
        /// </summary>
        public int ClientId;
        
        
        public BattleClientSetup ClientConfig;
        
    }
    
    
    /// <summary>
    /// 战斗房间
    ///
    /// 每一场战斗为一个战斗房间，即便玩家与NPC的本地战斗，也是新开一个本地的战斗服务器+房间进行通讯和逻辑。
    /// 本质上玩家PVE和玩家之间的PVP是走完全一套代码逻辑，是本地计算还是走服务器计算，只由通讯链路决定。
    /// </summary>
    public class BattleRoom
    {
        //客户端token到配置的映射
        private Dictionary<string, BattleClientSetup> _clientTokenMapping = new Dictionary<string, BattleClientSetup>();
        
        //当前战场配置
        private BattleFieldSettings _battleFieldSettings;
        
        //保存房间管理器
        private IBattleRoomHost _parent;

        //房间ID
        private int _roomId;
        
        /// <summary>
        /// 初始化战斗房间
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="battleFieldSettings"></param>
        public BattleRoom(int roomId, IBattleRoomHost parent, BattleFieldSettings battleFieldSettings)
        {
            _battleFieldSettings = battleFieldSettings;
            _parent = parent;
        }

        /// <summary>
        /// 玩家连接战斗房间服务
        /// </summary>
        /// <param name="client"></param>
        /// <param name="playerToken"></param>
        /// <returns></returns>
        public ErrorCode PlayerConnect(BattleClientSetup client, out string playerToken)
        {
            playerToken = string.Empty;
            
            //是否该队伍已经有人加入过了
            foreach (var clientSetup in _clientTokenMapping)
            {
                if (clientSetup.Value.Team == client.Team)
                {
                    return ErrorCode.JoinBattleErrorDuplicatedTeamId;
                }
            }

            playerToken = _parent.RequestForPlayerToken(_roomId);
            
            //客户端token重复
            if (_clientTokenMapping.ContainsKey(playerToken))
            {
                return ErrorCode.JoinBattleErrorDuplicatedToken;
            }
            
            //否则添加到队伍
            _clientTokenMapping.Add(playerToken, client);
            return ErrorCode.Success;
        }
    }
}