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
        private readonly List<BattleClientSetup> _clients = new List<BattleClientSetup>();
        
        private BattleFieldSettings _battleFieldSettings;
        private IBattleRoomHost _parent;
        
        public BattleRoom(IBattleRoomHost parent, BattleFieldSettings battleFieldSettings)
        {
            _battleFieldSettings = battleFieldSettings;
            _parent = parent;
        }
        
        
        
    }
}