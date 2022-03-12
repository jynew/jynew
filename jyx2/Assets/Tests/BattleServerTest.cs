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

        BattleFieldSettings battleFieldSettings = new BattleFieldSettings() {BattleId = 0};
        var ret = await battleServer.RequestForBattleRoom(battleFieldSettings);
        Assert.True(ret.ErrCode == ErrorCode.Success);

        var roomId = ret.RetCode;
        ret = await battleServer.JoinBattleRoom(roomId, new BattleClientSetup());
        
        Assert.True(ret.ErrCode == ErrorCode.Success);

        var token = ret.RetMsg;
        Debug.Log(token);
        Assert.AreNotEqual(token,string.Empty);
        
        ret = await battleServer.JoinBattleRoom(roomId, new BattleClientSetup());

        //重复加入
        Assert.True(ret.ErrCode == ErrorCode.JoinBattleErrorDuplicatedTeamId);
    }
}
