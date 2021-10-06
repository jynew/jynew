using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using LitJson;

[Serializable]
public class GameReplayInfo
{
    public const string INFO_FILE_NAME = "replays/{0}_{1}_{2}_info.txt";
    public const string TICK_FILE_NAME = "replays/{0}_{1}.txt";
    //存档格式，同时也是战斗关键帧包的版本号，此值要跟battle_case.lua中的NUM_TICK_VERSION同步修改
    public const int ARCHIVE_FORMAT_VERSION = 9;

    public long battleId;//战斗id
    public int mapId;//战斗地图id
    public string replayId;//回放惟一id
    public long uploadReplayPid;//战斗回放记录者的pid
    public string playerName;//战斗回放记录者的名字
    public int date;//生成此回放数据时的时间
    public int clientVersion;//生成回放数据时的客户端版本
    public string serverName;//服务器名字
    public int archiveFormatVersion;//回放数据存档格式版本，如果存档版本跟此版本不符，则废弃存档
    public string errorInfo;//回放过程若有产生过报错，则记录报错堆栈
    public bool isReport;//是否已经上报给服务器
    public string infoFullPath;//此回放摘要文档的完整路径
    public string tickFullPath;//此回放帧数据文档的完整路径
    [NonSerialized]
    public string infoName;
    [NonSerialized]
    public string tickName;
    //有些战斗可能使用了加速功能改变了战斗逻辑，所以为了保证加速过的战斗，回放也是加速过的，回放文件记录这些信息
    //战斗过程中做加速的情况无法支持！
    public uint killMapTime = 0;
    public int occupyTimeScale = 1;
    public int attackDefenseTimeScale = 1;
    public int bountyBecomeBossTimeScale = 1;
    public int bountyCountDownScale = 1;
    public bool isUseStrongestAI;//是否使用等级最高的ai
    public bool isAngerRecoverFast;//是否战斗中英雄大招怒气快速恢复
    /// <summary>
    /// 回放文件格式是否合法，不合法则忽略不放进回放列表
    /// </summary>
    /// <returns></returns>
    public bool isValidFormat() 
    {
        return true;
    }

    static private void LoadPlayersSprotoFromJsonString(GameReplayInfo replayInfo, string jsonString)
    {
        JsonData jsonData = JsonMapper.ToObject(jsonString);
        // 复写
        if (jsonData.Keys.Contains("players_sproto"))
        {
            JsonData playersData = jsonData["players_sproto"];

            if (replayInfo.playersSproto == null)
                replayInfo.playersSproto = new List<SprotoType.Player>();

            replayInfo.playersSproto.Clear();

            for (int i = 0; i < playersData.Count; i++)
            {
                replayInfo.playersSproto.Add(new SprotoType.Player(playersData[i]));
            }
        }
    }

    static public GameReplayInfo ReadFromJsonArchive(string archiveSubPath)
    {
        string archiveFullPath = PathUtil.GetArchiveFullPath(archiveSubPath);

        if (!File.Exists(archiveFullPath))
            return new GameReplayInfo();

        GameReplayInfo replayInfo = JsonArchive.Read<GameReplayInfo>(archiveSubPath);

        using (FileStream fileStream = new FileStream(archiveFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string jsonContent = reader.ReadToEnd();

                try
                {
                    if (!string.IsNullOrEmpty(jsonContent))//兼容为空的情况
                    {
                        LoadPlayersSprotoFromJsonString(replayInfo, jsonContent);
                    }
                }
                catch (System.Exception e)
                {
                    DebugUtil.LogError($"解析存档文件={archiveFullPath} 出错={jsonContent} error={e.Message}");
                }
            }
        }

        return replayInfo;
    }

    static public GameReplayInfo ReadFromJsonString(string jsonString)
    {
        GameReplayInfo replayInfo = JsonUtility.FromJson<GameReplayInfo>(jsonString);
        LoadPlayersSprotoFromJsonString(replayInfo, jsonString);
        return replayInfo;
    }

    static public void WriteJsonArchive(GameReplayInfo replayInfo, string archiveSubPath)
    {
        DebugUtil.Assert(replayInfo.playersSproto != null && replayInfo.playersSproto.Count > 0, $"没有角色快照数据");

        JsonArchive.Write(replayInfo, archiveSubPath);

        string archiveFullPath = PathUtil.GetArchiveFullPath(archiveSubPath);

        JsonData jsonData = null;

        using (FileStream fileStream = new FileStream(archiveFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string jsonContent = reader.ReadToEnd();

                try
                {
                    jsonData = JsonMapper.ToObject(jsonContent);

                    jsonData["players_sproto"] = new JsonData();
                    jsonData["players_sproto"].SetJsonType(JsonType.Array);

                    for (int i = 0; i < replayInfo.playersSproto.Count; i++)
                    {
                        jsonData["players_sproto"].Add(replayInfo.playersSproto[i].ToJsonData());
                    }
                }
                catch (System.Exception e)
                {
                    DebugUtil.LogError($"解析存档文件={archiveFullPath} 出错={jsonContent} error={e.Message}");
                }
            }
        }

        System.IO.File.WriteAllText(archiveFullPath, jsonData.ToJson());
    }

    public string ToJsonStr(BattleModel model)
    {
        battleId = model.battleId;
        replayId = model.replayId;
        mapId = model.mapId;
        monsterLevel = model.monsterLevel;
        uploadReplayPid = model.selfPlayerServerId;
        playerName = model.playerName;
        players = new List<PlayerInfo>();
        isReport = false;
        VersionUtil.GetClientSvnVersion(out clientVersion);
        archiveFormatVersion = ARCHIVE_FORMAT_VERSION;
        date = DatetimeUtil.ConvertDateTime2Int(DateTime.Now);
        serverName = Config.Inst.serverUsingTable.GetRow("using").Server.ShowName;
        for (int i = 0; i < model.players.Count; i++)
        {
            //PlayerInfo playerInfo = CreateBattleInfo.ConvertPlayer(model.players[i]);
            //players.Add(playerInfo);
        }
        //有些战斗可能使用了加速功能改变了战斗逻辑，所以为了保证加速过的战斗，回放也是加速过的，回放文件记录这些信息
        //战斗过程中做加速的情况无法支持！
        killMapTime = GlobalConfig.Inst.killMapTime;
        occupyTimeScale = GlobalConfig.Inst.occupyTimeScale;
        attackDefenseTimeScale = GlobalConfig.Inst.attackDefenseTimeScale;
        bountyBecomeBossTimeScale = GlobalConfig.Inst.bountyBecomeBossTimeScale;
        bountyCountDownScale = GlobalConfig.Inst.bountyCountDownScale;
        isAngerRecoverFast = GlobalConfig.Inst.isAngerRecoverFast;
        isUseStrongestAI = GlobalConfig.Inst.isUseStrongestAI;

        // 先走反射序列化到json
        JsonData jsonData = JsonMapper.ToObject(JsonUtility.ToJson(this));

        jsonData["players_sproto"] = new JsonData();
        jsonData["players_sproto"].SetJsonType(JsonType.Array);

        DebugUtil.Assert(model.players.Count > 0, $"序列化回访数据之前，看起来没有players数据");

        for (int i = 0; i < model.players.Count; i++)
        {
            jsonData["players_sproto"].Add(model.players[i].ToJsonData());
        }

        string jsonStr = jsonData.ToJson();

        return jsonStr;
    }
    //将本地序列化结构转换成sproto结构供战斗使用
    public List<SprotoType.Player> GetPlayers()
    {
        /*List<SprotoType.Player> sprotoPlayers = new List<SprotoType.Player>();
        SprotoType.Player player = null;

        players[0].heroGroups[0].partner = players[1].heroGroups[0].partner;
        //players[1].heroGroups[0].master = players[0].heroGroups[0].master;

        if (players.Count > 2)
        {
            players[2].heroGroups[0].partner = players[3].heroGroups[0].partner;
            //players[3].heroGroups[0].master = players[2].heroGroups[0].master;
        }

        for (int i = 0; i < players.Count; i++)
        {
            player = CreateBattleInfo.ConvertPlayer(players[i]);
            player.state = (long)EnumPlayerServerState.PK;//本地回放默认玩家都是pk状态
            sprotoPlayers.Add(player);
        }
        return sprotoPlayers;*/

        DebugUtil.Assert(playersSproto != null && playersSproto.Count > 0, "回放没有读取到单位数据");

        return playersSproto;
    }
    public string GetUploadPlayerHeroName()
    {
        for (int i = 0; i < players.Count; i++)
            if (players[i].pid == uploadReplayPid)
                return Config.Inst.heroTable.GetRow(players[i].heroGroups[0].master.id).Name;
        return "";
    }
}
