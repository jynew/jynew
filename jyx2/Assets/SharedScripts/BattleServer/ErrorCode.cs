namespace Jyx2.SharedScripts.BattleServer
{
    /// <summary>
    /// 战斗服务器通信错误码
    /// </summary>
    public enum ErrorCode
    {
        /// <summary>
        /// 正常
        /// </summary>
        Success = 0,
        
        /// <summary>
        /// 房间已满
        /// </summary>
        RoomIsFull = 10001,
        
        /// <summary>
        /// 指定ID的房间不存在
        /// </summary>
        RoomIsNotExist = 10002,

        /// <summary>
        /// 加入房间失败，token冲突
        /// </summary>
        JoinBattleErrorDuplicatedToken = 10003,
        
        /// <summary>
        /// 加入房间失败，队伍位已经被占用了
        /// </summary>
        JoinBattleErrorDuplicatedTeamId = 10004,
        
        /// <summary>
        /// TEAM ID非法
        /// </summary>
        InvalidTeamId = 10005,
        
        /// <summary>
        /// 客户端的token无效
        /// </summary>
        InvalidClientToken = 10006,
        
        
        /// <summary>
        /// 战斗已经开始了，不再能新加入玩家
        /// </summary>
        PlayerJoinAfterBattleStarted = 10007,
    }
}