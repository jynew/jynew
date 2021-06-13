using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Jyx2;
using HanSquirrel.ResourceManager;
using System;

public enum RolePanelState 
{
    CreateRole = 0,
    RoleInGame = 1,
}

public class RolePanel : MonoBehaviour
{
    public Transform m_BottomRight;
    public Transform m_MiddleBtns;
    public Text m_level;
    public Slider m_expSlider;
    public Text m_expText;
    public Slider m_hpSlider;
    public Text m_hpText;
    public Slider m_neiliSlider;
    public Text m_neiliText;
    public Slider m_tiliSlider;
    public Text m_tiliText;

    public Text m_attack;
    public Text m_defense;
    public Text m_qinggong;
    public Text m_heal;
    public Text m_depoison;
    public Text m_usePoison;

    public Text m_quanzhang;
    public Text m_yujian;
    public Text m_shuadao;
    public Text m_teshu;
    public Text m_anqi;

    public List<Text> m_wuxueNames;
    public List<Text> m_wuxueLevels;

    private RolePanelState m_currentState = RolePanelState.CreateRole;
    private Action m_onCloseCallback;
    public static void Create(Transform parent,RolePanelState state, Action onclose) 
    {
        var obj = Jyx2ResourceHelper.CreatePrefabInstance("Assets/Prefabs/Jyx2UI/RolePanel.prefab");
        obj.transform.SetParent(parent);

        var rt = obj.GetComponent<RectTransform>();
        rt.localPosition = Vector3.zero;
        rt.localScale = Vector3.one;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        var _rolePanel = obj.GetComponent<RolePanel>();
        _rolePanel.OnOpen(state, onclose);
    }
    public void OnOpen(RolePanelState state, Action onclose) 
    {
        this.m_currentState = state;
        this.m_onCloseCallback = onclose;
        SetPanelState();
        RefreshView();
    }

    void SetPanelState() 
    {
        m_BottomRight.gameObject.SetActive(m_currentState == RolePanelState.CreateRole);
        m_MiddleBtns.gameObject.SetActive(m_currentState == RolePanelState.RoleInGame);
    }
    void RefreshView()
    {
        RoleInstance role = GameRuntimeData.Instance.Player;
        m_level.text = role.Level.ToString();
        //经验
        m_expSlider.value = ((float)role.Exp) / role.GetLevelUpExp();
        m_expText.text = string.Format("{0}/{1}", role.Exp, role.GetLevelUpExp());
        //血量
        m_hpSlider.value = ((float)role.Hp) / role.MaxHp;
        m_hpText.text = string.Format("{0}/{1}", role.Hp, role.MaxHp);

        //m_neiliSlider.value = ((float)role.)//TODO 内力
        //体力
        m_tiliSlider.value = ((float)role.Tili) / GameConst.MAX_ROLE_TILI;
        m_tiliText.text = string.Format("{0}/{1}", role.Tili, GameConst.MAX_ROLE_TILI);
        RefreshProperty();
        RefreshWugong();
    }

    void RefreshProperty() 
    {
        RoleInstance role = GameRuntimeData.Instance.Player;
        m_attack.text = role.Attack.ToString();
        m_defense.text = role.Defence.ToString();
        m_qinggong.text = role.Qinggong.ToString();
        m_heal.text = role.Heal.ToString();
        m_depoison.text = role.DePoison.ToString();
        m_usePoison.text = role.UsePoison.ToString();

        m_quanzhang.text = role.Quanzhang.ToString();
        m_yujian.text = role.Yujian.ToString();
        m_shuadao.text = role.Shuadao.ToString();
        m_teshu.text = role.Qimen.ToString();
        m_anqi.text = role.Anqi.ToString();
    }

    void RefreshWugong()
    {
        RoleInstance role = GameRuntimeData.Instance.Player;
        Text tempName,tempValue;
        WugongInstance wugong;
        int wugongCount = role.Wugongs.Count;
        for (int i = 0; i < m_wuxueNames.Count; i++)
        {
            tempName = m_wuxueNames[i];
            tempValue = m_wuxueLevels[i];
            if (i < wugongCount)
            {
                wugong = role.Wugongs[i];
                tempName.text = wugong.Name;
                tempValue.text = wugong.Level.ToString();
            }
            else 
            {
                tempName.text = "-----------------";
                tempValue.text = "-----";
            }
        }
    }

    public void OnConfirmBtnClick() 
    {
        
    }

    public void OnRandomBtnClick() 
    {

    }

    public void OnHealBtnClick() 
    {

    }

    public void OnDepoisonBtnClick() 
    {

    }

    public void OnReturnBtnClick() 
    {
        Destroy(this.gameObject);
        m_onCloseCallback?.Invoke();
    }
}
