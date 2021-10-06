/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameRecorder
{
	string _replayInfoFilePath;
    string _replayTickFilePath;
    Framework.BinaryLogFile _replayTickFile;
    string _battleError;
    public GameRecorder()
    {
        _replayInfoFilePath = string.Format(GameReplayInfo.INFO_FILE_NAME, battleModel.replayId, battleModel.selfPlayerServerId, GameReplayInfo.ARCHIVE_FORMAT_VERSION);
        LogFile replayInfoFile = new LogFile(_replayInfoFilePath, 0, true);
        _replayTickFilePath = string.Format(GameReplayInfo.TICK_FILE_NAME, battleModel.replayId, battleModel.selfPlayerServerId);
        _replayTickFile = new Framework.BinaryLogFile(_replayTickFilePath, 0, true);

        GameReplayInfo info = new GameReplayInfo();
        info.infoFullPath = replayInfoFile.fileFullPath;
        info.tickFullPath = _replayTickFile.fileFullPath;
        replayInfoFile.Log(info.ToJsonStr(battleModel));
        replayInfoFile.OnDispose();
    }
    public void OnDispose()
    {
        if (_replayTickFile != null)
        {
            _replayTickFile.OnDispose();
            _replayTickFile = null;
        }
    }
    public void RecordFrame(BattleServerTick tick, uint clientFrame)
    {
        if (tick.size > BattleStatic.tempBytes.Length)//做个兼容，自动扩大数组容量
            BattleStatic.ExpandTempBytes(tick.size);
        int count = tick.ToReplayTickBytes(BattleStatic.tempBytes, clientFrame);
        _replayTickFile.LogLine(BattleStatic.tempBytes, (ushort)count);
    }
    public void RecordError(string battleError)
    {
        _battleError = battleError;
        //把错误写入存档中
        GameReplayInfo replayInfo = GameReplayInfo.ReadFromJsonArchive(_replayInfoFilePath);
        replayInfo.errorInfo = battleError;
        GameReplayInfo.WriteJsonArchive(replayInfo, _replayInfoFilePath);
    }
    public void Upload2Server()
    {
        //把网络延时统计写入回放存档中
        GameReplayInfo replayInfo = GameReplayInfo.ReadFromJsonArchive(_replayInfoFilePath);
        GameReplayInfo.WriteJsonArchive(replayInfo, _replayInfoFilePath);
        //结束回放帧数据的写入磁盘
        _replayTickFile.OnDispose();
        _replayTickFile = null;
    }
}