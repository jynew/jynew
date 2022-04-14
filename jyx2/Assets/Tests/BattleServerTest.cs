using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jyx2.SharedScripts.BattleServer;
using NUnit.Framework;
using UnityEngine;

public class BattleServerTest
{
    [Test(Description="连接本地战斗服务器")]
    public async void TestConnectToFakeBattleServer()
    {
        var battleServer = BattleServerFactory.CreateServer();

        BattleFieldSettings battleFieldSettings = new BattleFieldSettings() {BattleId = 0};
        var ret = await battleServer.RequestForBattleRoom(battleFieldSettings);
        Assert.True(ret.ErrCode == ErrorCode.Success);
    }

    [Test(Description="正常连接流程")]
    public async void TestConnectToFakeBattleRoom()
    {
        var battleServer = BattleServerFactory.CreateServer();

        BattleFieldSettings battleFieldSettings = new BattleFieldSettings() {BattleId = 0, TeamCount = 2};
        var ret = await battleServer.RequestForBattleRoom(battleFieldSettings);
        Assert.True(ret.ErrCode == ErrorCode.Success);

        var roomId = ret.RetCode;
        ret = await battleServer.JoinBattleRoom(roomId, new BattleClientSetup()
        {
            Team = 0, 
            BattleMsgCallback = OnServerCallback, 
            BattleResultCallback = (rst) => { Debug.Log($"team=0, BattleResult={rst}"); },
            Roles = new List<RoleInstance>() { new RoleInstance(){Hp = 3}},
        });
        
        Assert.True(ret.ErrCode == ErrorCode.Success);

        var token = ret.RetMsg;
        Debug.Log(token);
        Assert.AreNotEqual(token,string.Empty);
        
        //重复加入
        ret = await battleServer.JoinBattleRoom(roomId, new BattleClientSetup());
        Assert.True(ret.ErrCode == ErrorCode.JoinBattleErrorDuplicatedTeamId);

        //team id非法
        ret = await battleServer.JoinBattleRoom(roomId, new BattleClientSetup() {Team = 1000});
        Assert.True(ret.ErrCode == ErrorCode.InvalidTeamId);
        
        
        //正常加入
        ret = await battleServer.JoinBattleRoom(roomId, new BattleClientSetup()
        {
            Team = 1,
            BattleMsgCallback = OnServerCallback,
            BattleResultCallback = (rst) => { Debug.Log($"team=1, BattleResult={rst}"); },
            Roles = new List<RoleInstance>() { new RoleInstance(){Hp = 2}},
        });
        
        Assert.True(ret.ErrCode == ErrorCode.Success);
        
        //下面应该开始战斗了。。

        while(true)
        {
            var rst = await battleServer.GetMyBattleStatus(ret.RetMsg);
            if (rst.RetCode == (int) BattleRoomStatus.Finished)
            {
                break;
            }
        }
        
        Debug.Log("test finished.");
    }

    void OnServerCallback(BattleMsg msg)
    {
        Debug.Log(msg.Msg);
    }
}
