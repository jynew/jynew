/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;


using Jyx2;
using Jyx2Configs;
using UnityEngine;
using UnityEngine.UI;

public class Jyx2ItemUI : MonoBehaviour
{
    public Image m_Image;
    public Text m_NameText;
    public Text m_CountText;

    private const string ITEM_UI_PREFAB = "Jyx2ItemUI";
    
    public static Jyx2ItemUI Create(int id,int count)
    {
        var prefab = Jyx2ResourceHelper.GetCachedPrefab(ITEM_UI_PREFAB);
        var obj = Instantiate(prefab); 
        var itemUI = obj.GetComponent<Jyx2ItemUI>();
        itemUI.Show(id, count).Forget();
        return itemUI;
    }

    private int _id;

    public Jyx2ConfigItem GetItem()
    {
        return GameConfigDatabase.Instance.Get<Jyx2ConfigItem>(_id);
    }

    private async UniTaskVoid Show(int id,int count)
    {
        _id = id;
        var item = GetItem();//0-阴性内力，1-阳性内力，2-中性内力
        var color =
            (int)item.ItemType == 2
                ? (int)item.NeedMPType == 2 ? ColorStringDefine.Default :
                (int)item.NeedMPType == 1 ? ColorStringDefine.Mp_type1 : ColorStringDefine.Mp_type0
                : ColorStringDefine.Default;
        
        m_NameText.text = $"<color={color}>{item.Name}</color>";
        m_CountText.text = (count > 1 ? count.ToString() : "");

        m_Image.LoadAsyncForget(item.GetPic());
        
    }

    public void Select(bool active) 
    {
        Transform select = transform.Find("Select");
        select.gameObject.SetActive(active);
    }

}
