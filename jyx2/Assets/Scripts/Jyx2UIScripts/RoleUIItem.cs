using HanSquirrel.ResourceManager;
using Jyx2;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class RoleUIItem : MonoBehaviour
{
    public static RoleUIItem Create()
    {
        var obj = Jyx2ResourceHelper.CreatePrefabInstance(string.Format(GameConst.UI_PREFAB_PATH, "RoleItem"));
        var roleItem = obj.GetComponent<RoleUIItem>();
        roleItem.InitTrans();
        return roleItem;
    }

    Transform m_select;
    Image m_roleHead;
    Text m_roleName;
    Text m_roleInfo;
    RoleInstance m_role;
    List<int> m_showPropertyIds = new List<int>(){ 13, 15, 14 };//要显示的属性

    void InitTrans() 
    {
        m_select = transform.Find("Select");
        m_roleHead = transform.Find("RoleHead").GetComponent<Image>();
        m_roleName = transform.Find("Name").GetComponent<Text>();
        m_roleInfo = transform.Find("Info").GetComponent<Text>();
    }

    public void ShowRole(RoleInstance role, List<int> pros = null) 
    {
        m_role = role;
        if (pros != null)
            m_showPropertyIds = pros;

        string nameText = role.Name + " Lv." + role.Level;
        m_roleName.text = nameText;

        ShowProperty();

        Jyx2ResourceHelper.GetRoleHeadSprite(role, m_roleHead);
    }

    void ShowProperty() 
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < m_showPropertyIds.Count; i++)
        {
            string proId = m_showPropertyIds[i].ToString();
            if (!GameConst.ProItemDic.ContainsKey(proId))
                continue;
            var proItem = GameConst.ProItemDic[proId];
            if (proItem.PropertyName == "Hp")
            {
                sb.Append($"{proItem.Name}:{m_role.Hp}/{m_role.MaxHp}\n");
            }
            else if (proItem.PropertyName == "Tili")
            {
                sb.Append($"{proItem.Name}:{m_role.Tili}/{GameConst.MaxTili}\n");
            }
            else if (proItem.PropertyName == "Mp")
            {
                sb.Append($"{proItem.Name}:{m_role.Mp}/{m_role.MaxMp}\n");
            }
            else 
            {
                int value = (int)m_role.GetType().GetProperty(proItem.PropertyName).GetValue(m_role, null);
                sb.Append($"{proItem.Name}:{value}\n");
            }
        }
        m_roleInfo.text = sb.ToString();
    }

    public void SetSelect(bool active) 
    {
        m_select.gameObject.SetActive(active);
    }

    public RoleInstance GetShowRole() 
    {
        return m_role;
    }
}
