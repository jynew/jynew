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
using Jyx2;
using HSFrameWork.ConfigTable;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class BigMapZone : MonoBehaviour
{
    public string EnterMapKey;
    public string Command;
    public string ButtonText;
    public GameObject[] m_EventTargets;

    private Button mapZoneEnterButton;

    async private void Start()
    {
        await BeforeSceneLoad.loadFinishTask;
        mapZoneEnterButton = GameObject.Find("LevelMaster/UI").transform.Find("EnterZoneButton").GetComponent<Button>();
    }

    void OnTriggerEnter(Collider other)
    {
        ShowEnterButton(EnterMapKey, Command, ButtonText);
        UnityTools.HighLightObjects(m_EventTargets, Color.red);
    }

    void OnTriggerExit(Collider other)
    {
        //HideEnterButton();
        Jyx2_UIManager.Instance.HideUI(nameof(InteractUIPanel));
        UnityTools.DisHighLightObjects(m_EventTargets);
    }

    bool hasGetNanxianjuPos = false;
    bool HasGetNanxianjuPosition() 
    {
        if (hasGetNanxianjuPos)
            return true;
        string eventStr = GameRuntimeData.Instance.GetModifiedEvent(1, 1);
        if (string.IsNullOrEmpty(eventStr))
        {
            return false;
        }
        string[] tmp = eventStr.Split('_');
        int m_InteractiveEventId = int.Parse(tmp[0]);
        if (m_InteractiveEventId == 667) 
        {
            hasGetNanxianjuPos = true;
            return true;
        }
        return false;
    }

    //檢測是否可以進入地圖
    bool CheckCanEnterMap(string mapKey) 
    {
#if JYX2_TEST
        return true;
#endif

        var runtime = GameRuntimeData.Instance;
		var key=runtime.GetSceneEntranceCondition(mapKey);
        if(key == 0)
        {
            return true;
        }else if(key==2)
		{
			foreach(var role in runtime.Team)
			{
				if(role.Qinggong>=75) return true;
			}
		}
		return false;
    }

    void ShowEnterButton(string mapKey, string command, string showText)
    {
        mapKey = mapKey.Replace("jyx2_BigMap", "0_BigMap"); //fix命名

        if (!CheckCanEnterMap(mapKey))
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
            //记录当前地图
            LevelMaster.LastGameMap = LevelMaster.Instance.GetCurrentGameMap();

            //记录当前世界位置
            if (LevelMaster.Instance.GetCurrentGameMap().Tags.Contains("WORLDMAP"))
            {
                //GameRuntimeData.Instance.WorldPosition = UnityTools.Vector3ToString(LevelMaster.Instance.GetPlayerPosition());
                Jyx2Player.GetPlayer().RecordWorldInfo();
            }

            //command 优先级高于mapKey
            if (!string.IsNullOrEmpty(command))
            {
				// handle transport from indoor to sub-scene
				// modify by eaphoneo at 2021/05/23
				if(command.StartsWith("IndoorTransport"))
                {
					var curMap=LevelMaster.Instance.GetCurrentGameMap();
					mapKey += "&transport#" + curMap.Tags.Split(',')[0];
					LevelLoader.LoadGameMap(mapKey);
				}else if(command.StartsWith("TransportWei")){
					// add transport Wei to other hotel when leave hotel after meet him
					// added by eaphone at 2021/6/5
					TransportWei();
					IndoorToBigMap(mapKey);
				}else{
					StoryEngine.Instance.ExecuteCommand(Command, null);
				}
            }
            else if (!string.IsNullOrEmpty(mapKey))
            {
                
                IndoorToBigMap(mapKey);
            }
        }));
    }
	
    void IndoorToBigMap(string mapKey){
		if (mapKey.StartsWith("0_BigMap") && !mapKey.Contains("transport"))
		{
			var currentMap = LevelMaster.Instance.GetCurrentGameMap();
			if (!string.IsNullOrEmpty(currentMap.BigMapTriggerName))
			{
				mapKey += "&transport#" + currentMap.BigMapTriggerName;
			}
		}

		LevelLoader.LoadGameMap(mapKey);
	}

	// transport Wei to other hotel when leave hotel if had talked to him
	// added by eaphone at 2021/6/5
	public static void TransportWei(){
		var weiPath="Dynamic/韦小宝";
		var triggerPath="Level/Triggers";
		var cur=LevelMaster.Instance.GetCurrentGameMap();
		var isWeiAtCurMap=GameObject.Find(weiPath);
		var isTalkedToWei=false;
		if(isWeiAtCurMap!=null && isWeiAtCurMap.activeSelf){
			var hotelList=new List<Jyx2Shop>(ConfigTable.GetAll<Jyx2Shop>());
			LevelMasterBooster level = GameObject.FindObjectOfType<LevelMasterBooster>();
			var ran=new System.Random();
			var index=ran.Next(0,hotelList.Count);
			while(cur.Jyx2MapId==hotelList[index].Id){
				index=ran.Next(0,hotelList.Count);
			}
			GameObject eventsParent = GameObject.Find("Level/Triggers");
			foreach(Transform t in eventsParent.transform)
			{
				var evt = t.GetComponent<GameEvent>();
				if (evt == null) continue;
				foreach (var obj in evt.m_EventTargets){
					if (obj == null) continue;
					var o = obj.GetComponent<InteractiveObj>();
					if(o != null && "韦小宝"==o.name){
						isTalkedToWei=evt.m_InteractiveEventId==938;
						
					}
				}
			}
			if(isTalkedToWei){	
				var curTriggerId = ConfigTable.Get<Jyx2Shop>(int.Parse(cur.Jyx2MapId)).Trigger;
				Debug.Log("transport Wei to "+hotelList[index].Id);
				level.ReplaceSceneObject(cur.Jyx2MapId, weiPath, "0");
				level.ReplaceSceneObject(hotelList[index].Id, weiPath, "1");
				GameRuntimeData.Instance.ModifyEvent(int.Parse(cur.Jyx2MapId), curTriggerId, -1, -1, -1);
				GameRuntimeData.Instance.ModifyEvent(int.Parse(hotelList[index].Id), hotelList[index].Trigger, 937, -1, -1);
                LevelMaster.Instance.RefreshGameEvents();
			}
		}
	}
}
