/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */

using UnityEngine;
using UnityEngine.UI;
using Jyx2;

public class RandomPropertyComponent : MonoBehaviour
{
    public Text m_titleText;
    public Transform m_propertyRoot;
    public Transform m_proItem;

    private static readonly int[] m_proIds = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

    private void Awake()
    {
        for (int i = 0; i < m_proIds.Length; i++)
        {
            Transform trans = GameObject.Instantiate(m_proItem);
            trans.name = m_proIds[i].ToString();
            trans.SetParent(m_propertyRoot);
            trans.localScale = Vector3.one;
            trans.gameObject.SetActive(true);
        }
    }

    public void ShowComponent( )
    {
        m_titleText.text = string.Format("{0}  这样的属性你满意吗?", GameRuntimeData.Instance.Player.Name);
        RefreshProperty();
    }

    Color GetNameColor(ref bool showBg,PropertyItem item,int realValue) 
    {
        Color col;
        string color = ColorStringDefine.Red;
        if (realValue >= item.DefaulMax) 
        {
            showBg = true;
            color = ColorStringDefine.Yellow;
        }else
            showBg = false;

        ColorUtility.TryParseHtmlString(color, out col);
        return col;
    }

    public void RefreshProperty() 
    {
        RoleInstance role = GameRuntimeData.Instance.Player;
        int count = m_propertyRoot.childCount;
        for (int i = 0; i < count; i++)
        {
            Transform trans = m_propertyRoot.GetChild(i);
            Text label = trans.Find("Text").GetComponent<Text>();
            Image BG = trans.Find("Bg").GetComponent<Image>();

            if (!GameConst.ProItemDic.ContainsKey(trans.name))
                continue;
            PropertyItem item = GameConst.ProItemDic[trans.name];

            var proValue = (int)role.GetType().GetField(item.PropertyName).GetValue(role);
            string text = string.Format("{0}：{1}", item.Name, proValue);
            label.text = text;

            bool showBg = false;
            Color color = GetNameColor(ref showBg, item, proValue);
            label.color = color;
            BG.gameObject.SetActive(showBg);
        }
        role.Recover();
    }    
}
