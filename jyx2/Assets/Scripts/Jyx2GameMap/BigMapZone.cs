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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Jyx2;
using HSFrameWork.ConfigTable;
using Jyx2Configs;
using NUnit.Framework;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class BigMapZone : MonoBehaviour
{
	[InfoBox("注：默认ID 1000为大地图")]
	[LabelText("传送的地图ID")] 
	public int TransportMapId = GameConst.WORLD_MAP_ID;

	[InfoBox("对应指定场景的Level/Triggers下节点")]
	[LabelText("传送的位置名")] 
	public string TransportTriggerName;

	[LabelText("点击时额外触发指令")] 
	public string ExtraCommand;
	
	[LabelText("按钮文本")]
	public string ButtonText;
	
	[LabelText("高亮的物体")]
	public GameObject[] m_EventTargets;

	private async void Start()
	{
		await BeforeSceneLoad.loadFinishTask;

		await UniTask.Delay(TimeSpan.FromSeconds(1f)); //等1秒再生效
		triggerEnabled = true;
	}

	private bool triggerEnabled = false;

	void OnTriggerEnter(Collider other)
	{
		if (!triggerEnabled) return;
		ShowEnterButton(TransportMapId, TransportTriggerName, ButtonText);
		UnityTools.HighLightObjects(m_EventTargets, Color.red);
	}

	void OnTriggerExit(Collider other)
	{
		if (!triggerEnabled) return;
		//HideEnterButton();
		Jyx2_UIManager.Instance.HideUI(nameof(InteractUIPanel));
		UnityTools.DisHighLightObjects(m_EventTargets);
	}

	//檢測是否可以進入地圖
	bool CheckCanEnterMap(int mapId)
	{
#if UNITY_EDITOR
		return true;
#endif

		var runtime = GameRuntimeData.Instance;
		var key = runtime.GetSceneEntranceCondition(mapId);
		if (key == 0)
		{
			return true;
		}
		else if (key == 2)
		{
			foreach (var role in runtime.Team)
			{
				if (role.Qinggong >= 75) return true;
			}
		}

		return false;
	}

	void ShowEnterButton(int transportMapId, string transportTriggerName, string showText)
	{
		if (!CheckCanEnterMap(transportMapId))
		{
			GameUtil.DisplayPopinfo("目前还不能进入");
			return;
		}
		
		if (string.IsNullOrEmpty(showText))
		{
			showText = "进入";
		}

		Jyx2_UIManager.Instance.ShowUI(nameof(InteractUIPanel), showText, new Action(() =>
		{
			DoTransport();
		}));
	}

	public void DoTransport()
	{
		var curMap = LevelMaster.GetCurrentGameMap();
			
		Assert.IsNotNull(curMap);
			
		var nextMap = Jyx2ConfigMap.Get(TransportMapId);

		if (nextMap == null)
		{
			Debug.LogError($"错误，地图id={TransportMapId}未定义");
			return;
		}
			
		//记录当前地图
		LevelMaster.LastGameMap = curMap;
			
		//记录当前世界位置
		if (curMap.IsWorldMap())
		{
			Jyx2Player.GetPlayer().RecordWorldInfo();
		}

		LevelMaster.LevelLoadPara para = new LevelMaster.LevelLoadPara();
		para.loadType = LevelMaster.LevelLoadPara.LevelLoadType.StartAtTrigger;
		if (!string.IsNullOrEmpty(TransportTriggerName))
		{
			para.triggerName = TransportTriggerName;
		}else if (curMap.IsWorldMap())
		{
			para.triggerName = "Leave";
		}else if (nextMap.IsWorldMap())
		{
			para.triggerName = curMap.Name;
		}

		//额外触发指令
		if (!string.IsNullOrEmpty(ExtraCommand))
		{
			Jyx2Console.RunConsoleCommand(ExtraCommand);
		}
			
		//开始加载
		LevelLoader.LoadGameMap(nextMap, para);
	}
}
