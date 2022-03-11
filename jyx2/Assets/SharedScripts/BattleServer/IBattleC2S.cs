using System.Threading.Tasks;

namespace Jyx2.SharedScripts.BattleServer
{
    /// <summary>
    /// 客户端到服务器的接口
    /// </summary>
    public interface IBattleC2S
    {
        /// <summary>
        /// 申请战场房间
        /// </summary>
        /// <param name="battleFieldSettings">己方战场初始化信息</param>
        /// <returns>返回信息，id如果为-1，则申请失败，否则为房间编号</returns>
        Task<RetInfo> RequestForBattleRoom(BattleFieldSettings battleFieldSettings);


        /// <summary>
        /// 加入战场
        /// </summary>
        /// <param name="battleRoomId">房间ID</param>
        /// <param name="battleClientSetup">客户端配置</param>
        /// <returns>返回信息，id为服务器连接token，为-1则失败</returns>
        Task<RetInfo> JoinBattle(int battleRoomId, BattleClientSetup battleClientSetup);
    }
}