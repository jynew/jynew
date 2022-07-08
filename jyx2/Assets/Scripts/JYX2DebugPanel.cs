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
using DG.Tweening;
using Jyx2;

using System.Collections;
using System.Collections.Generic;
using Jyx2Configs;
using UnityEngine;
using UnityEngine.UI;

public class JYX2DebugPanel : MonoBehaviour
{
    public Dropdown m_ChangeScene;
    public Dropdown m_TransportDropdown;

    List<Jyx2ConfigMap> m_ChangeSceneMaps = new List<Jyx2ConfigMap>();
    bool _debugPanelSwitchOff = false;

    public bool IsDebugPanelSwitchOff()
    {
        return _debugPanelSwitchOff;
    }

    //打开和关闭面板
    public void DebugPanelSwitch()
    {
        transform.DOLocalMoveX(_debugPanelSwitchOff ? -1360f : -960f, 0.3f);
        
        LevelMaster lm = LevelMaster.Instance;
        if (lm != null)
        {
            lm.ForceSetEnable(!_debugPanelSwitchOff);
        }

        _debugPanelSwitchOff = !_debugPanelSwitchOff;
    }

    #region 地点跳转
    private void InitLocationDebugTools()
    {
        //场景快速跳转器
        m_ChangeScene.ClearOptions();
        List<string> activeMaps = new List<string>();
        activeMaps.Add("选择场景");
        foreach (var map in GameConfigDatabase.Instance.GetAll<Jyx2ConfigMap>())
        {
            if (map.Tags.Contains("BATTLE")) continue;
            activeMaps.Add(map.GetShowName());
            m_ChangeSceneMaps.Add(map);
        }
        m_ChangeScene.AddOptions(activeMaps);
        m_ChangeScene.onValueChanged.AddListener(OnChangeScene);

        //地点快速跳转器
        m_TransportDropdown.ClearOptions();
        var triggerObj = GameObject.Find("Level/Triggers");
        if (triggerObj != null)
        {
            List<string> opts = new List<string>();
            opts.Add("传送点");
            for (int i = 0; i < triggerObj.transform.childCount; ++i)
            {
                opts.Add(triggerObj.transform.GetChild(i).name);
            }

            m_TransportDropdown.AddOptions(opts);
            m_TransportDropdown.onValueChanged.AddListener(OnTransport);
        }
    }

    //切换场景
    public async void OnChangeScene(int value)
    {
        if (value == 0) return;

        var id = m_ChangeSceneMaps[value - 1].Id;

        var curMap = LevelMaster.GetCurrentGameMap();
        if (!curMap.Tags.Contains("WORLDMAP"))
        {
            string msg = "<color=red>警告：不在大地图上执行传送可能会导致某些剧情中断，强烈建议您退到大地图再执行。是否强行执行？</color>";
            List<string> selectionContent = new List<string>() { "是(Y)", "否(N)" };
            await Jyx2_UIManager.Instance.ShowUIAsync(nameof(ChatUIPanel), ChatType.Selection, "0", msg, selectionContent, new Action<int>((index) =>
            {
                if (index == 0)
                {
                    LevelLoader.LoadGameMap(GameConfigDatabase.Instance.Get<Jyx2ConfigMap>(id));
                }
            }));
        }
        else
        {
            LevelLoader.LoadGameMap(GameConfigDatabase.Instance.Get<Jyx2ConfigMap>(id));
        }
    }

    public async void OnTransport(int value)
    {
        if (value == 0) return;
        var transportName = m_TransportDropdown.options[value].text;

        var curMap = LevelMaster.GetCurrentGameMap();
        if (!curMap.Tags.Contains("WORLDMAP"))
        {
            string msg = "<color=red>警告：不在大地图上执行传送可能会导致某些剧情中断，强烈建议您退到大地图再执行。是否强行执行？</color>";
            List<string> selectionContent = new List<string>() { "是(Y)", "否(N)" };
            await Jyx2_UIManager.Instance.ShowUIAsync(nameof(ChatUIPanel), ChatType.Selection, "0", msg, selectionContent, new Action<int>((index) =>
            {
                if (index == 0)
                {
                    LevelMaster.Instance.Transport(transportName);
                }
            }));
        }
        else
        {
            LevelMaster.Instance.Transport(transportName);
        }
    }
    #endregion

    private void Start()
    {
        InitLocationDebugTools();

    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.BackQuote))
        {
            DebugPanelSwitch();
        }
    }
}
