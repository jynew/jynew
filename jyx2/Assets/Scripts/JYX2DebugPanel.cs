using DG.Tweening;
using Jyx2;
using HSFrameWork.ConfigTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JYX2DebugPanel : MonoBehaviour
{
    public Dropdown m_ChangeScene;
    public Dropdown m_TransportDropdown;

    List<GameMap> m_ChangeSceneMaps = new List<GameMap>();
    bool _debugPanelSwitchOff = false;

    //打开和关闭面板
    public void DebugPanelSwitch()
    {
        if (_debugPanelSwitchOff)
        {
            transform.DOLocalMoveX(-1360f, 0.3f);
        }
        else
        {
            transform.DOLocalMoveX(-960f, 0.3f);
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
        foreach (var map in ConfigTable.GetAll<GameMap>())
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
    public void OnChangeScene(int value)
    {
        if (value == 0) return;

        var levelKey = m_ChangeSceneMaps[value - 1].Key;
        LevelLoader.LoadGameMap(levelKey);
    }

    public void OnTransport(int value)
    {
        if (value == 0) return;
        var transportName = m_TransportDropdown.options[value].text;
        LevelMaster.Instance.Transport(transportName);
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
