using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codice.Client.GameUI.Update;

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
        private readonly Dictionary<string, BattleClientSetup> _clientTokenMapping = new Dictionary<string, BattleClientSetup>();
        
        //当前战场配置
        private BattleFieldSettings _battleFieldSettings;
        
        //保存房间管理器
        private IBattleRoomHost _parent;

        //房间ID
        private int _roomId;

        //战场单位
        private readonly List<BattleUnit> _units = new List<BattleUnit>();
        
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

            if (client.Team >= _battleFieldSettings.TeamCount)
                return ErrorCode.InValidTeamId;
            
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
            
            //如果所有人都已经进齐了，就开始战斗
            if (_clientTokenMapping.Count == _battleFieldSettings.TeamCount)
            {
                StartBattle().Start();
            }
            
            return ErrorCode.Success;
        }

        
        
        
        private async Task StartBattle()
        {
            //初始化战斗配置
            InitBattle();

            //战斗主循环
            await BattleMainLoop();
        }
        
        //战场主循环
        private async Task BattleMainLoop()
        {
            while (!IsBattleFinished())
            {
                foreach (var unit in _units)
                {
                    //判断是否存活
                    if (!unit.IsAlive()) continue;
                    
                    //询问每一个单位要如何行动
                    var result = await unit.BattleUnitAction();
                    
                    //实际在战场中执行该行动结果
                    BattleDoAction(unit, result);
                }
                await Task.Delay(10);
            }
        }

        private bool IsBattleFinished()
        {
            //判断所有存活的人是否是同一个队伍的
            var aliveRoles = _units.Where(p => p.IsAlive());

            int team = -1;
            foreach (var role in aliveRoles)
            {
                if (team == -1)
                {
                    team = role.Team;
                }else if (role.Team != team)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 战场执行结果
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="result"></param>
        private void BattleDoAction(BattleUnit unit, BattleUnitActionResult result)
        {
            //TODO：临时逻辑为，下发给所有客户端
            foreach (var client in _clientTokenMapping.Values)
            {
                BattleMsg msg = new BattleMsg() {Msg = result.Result};
                
                //回调给所有客户端，待改为异步
                client.BattleMsgCallback(msg);
            }
        }

        private void InitBattle()
        {
            //初始化所有的战斗单位
            foreach (var kv in _clientTokenMapping)
            {
                var config = kv.Value;
                foreach (var role in config.Roles)
                {
                    //TODO：根据战场配置，放到战场中合适的位置上
                    _units.Add(new BattleUnit() {m_Role = role, Team = config.Team});
                }
            }
        }
    }
}