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
}
