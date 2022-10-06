using System.Collections;
using System.Collections.Generic;
using i18n.TranslatorDef;
using Jyx2;
using UnityEngine;

public class BigMapLocationNameDrawer : MonoBehaviour
{
    public GameObject m_NameTextPrefab;
    
    
    // Start is called before the first frame update
    async void Start()
    {
        await RuntimeEnvSetup.Setup();
        var allLocs = FindObjectsOfType<MapTeleportor>();
        foreach (var loc in allLocs)
        {
            if (JudgeIfIgnoreLocationNameDisplay(loc)) continue;

            var nameObj = Instantiate(m_NameTextPrefab);
            nameObj.transform.position = loc.transform.position + Vector3.up * 6;
            nameObj.transform.localScale = Vector3.one * 3;
            if (loc.name == GlobalAssetConfig.Instance.defaultHomeName)
            {
                //---------------------------------------------------------------------------
                //var name = GameRuntimeData.Instance.Player.Name + "居";
                //---------------------------------------------------------------------------
                //特定位置的翻译【大地图主角居的名字显示】
                //---------------------------------------------------------------------------
                var name = GameRuntimeData.Instance.Player.Name + "居".GetContent(nameof(BigMapLocationNameDrawer));
                //---------------------------------------------------------------------------
                //---------------------------------------------------------------------------
                nameObj.GetComponent<TextMesh>().text = name;
            }
            else
            {
                nameObj.GetComponent<TextMesh>().text = loc.name;    
            }
        }
    }

    /// <summary>
    /// 判断是否要跳过名字显示
    ///
    /// 目前支持的方法是在lua中设置flag
    ///
    /// - 禁止显示所有的地点，只显示田伯光居：
    /// SetFlag("BanLocationName.All",1)
    /// SetFlag("ShowLocationName.田伯光居",1)
    ///
    /// - 显示所有地点，只禁用田伯光居：
    /// SetFlag("BanLocationName.All",0)
    /// SetFlag("BanLocationName.田伯光居",1)
    /// 
    /// </summary>
    /// <param name="loc"></param>
    /// <returns></returns>
    private static bool JudgeIfIgnoreLocationNameDisplay(MapTeleportor loc)
    {
        //全部禁止显示，只过滤设置了ShowLocationName的
        if (Jyx2LuaBridge.jyx2_GetFlagInt($"BanLocationName.All") == 1 &&
            Jyx2LuaBridge.jyx2_GetFlagInt($"ShowLocationName.{loc.name}") == 0)
        {
            return true;
        }

        //禁止地点名称
        if (Jyx2LuaBridge.jyx2_GetFlagInt($"BanLocationName.{loc.name}") == 1)
        {
            return true;
        }

        return false;
    }
}
