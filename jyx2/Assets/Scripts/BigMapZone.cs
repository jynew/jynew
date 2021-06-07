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
        Jyx2_UIManager.Instance.HideUI("InteractUIPanel");
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
        if(runtime.GetSceneEntranceCondition(mapKey) == 0)
        {
            return true;
        }

        return false;
        //if (string.IsNullOrEmpty(mapKey))
        //    return false;
        //if (mapKey.Contains("BigMap") || mapKey.Contains("heluokezhan")
        //    ||mapKey.Contains("Battle")||mapKey.Contains("xiaoxiamiju"))
        //    return true;
        //return HasGetNanxianjuPosition();
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
            
        Jyx2_UIManager.Instance.ShowUI("InteractUIPanel", showText, new Action(() =>
        {
            //记录当前地图
            LevelMaster.LastGameMap = LevelMaster.Instance.GetCurrentGameMap();

            //记录当前世界位置
            if (LevelMaster.Instance.GetCurrentGameMap().Tags.Contains("WORLDMAP"))
            {
                GameRuntimeData.Instance.WorldPosition = UnityTools.Vector3ToString(LevelMaster.Instance.GetPlayerPosition());
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
					mapKey += "&transport#" + curMap.Tags;
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

	// add transport Wei to other hotel when leave hotel after meet him
	// added by eaphone at 2021/6/5
	public static void TransportWei(){
		var weiPath="Dynamic/韦小宝";
		var triggerPath="Triggers/16";
		string[] targetHotel={"01_heluokezhan","03_youjiankezhan","40_yuelaikezhan","60_longmenkezhan","61_gaoshengkezhan"};
		var cur=LevelMaster.Instance.GetCurrentGameMap();
		var isWeiAtCurMap=GameObject.Find(weiPath);
		if(isWeiAtCurMap!=null && isWeiAtCurMap.activeSelf){
			LevelMasterBooster level = GameObject.FindObjectOfType<LevelMasterBooster>();
			level.ReplaceSceneObject(cur.Jyx2MapId.ToString(), weiPath, "0");
			//level.ReplaceSceneObject(cur.Jyx2MapId.ToString(),triggerPath,"0");
			var ran=new System.Random();
			var index=ran.Next(0,targetHotel.Length);
			while(cur.Key==targetHotel[index]){
				index=ran.Next(0,targetHotel.Length);
			}
			var target=ConfigTable.Get<GameMap>(targetHotel[index]);
			Debug.Log("transport Wei to "+target.GetShowName());
			level.ReplaceSceneObject(target.Jyx2MapId.ToString(), weiPath, "1");
			//level.ReplaceSceneObject(target.Jyx2MapId.ToString(),triggerPath,"1");
		}
	}
}
