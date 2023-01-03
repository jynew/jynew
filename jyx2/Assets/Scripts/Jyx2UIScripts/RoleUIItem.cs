/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */

using Jyx2;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Jyx2.Util;
using Jyx2.UINavigation;
using System;
using UnityEngine.EventSystems;

public class RoleUIItem : Selectable, IPointerClickHandler, IDataContainer<RoleInstance>, INavigable
{
	public event Action<RoleUIItem, bool> OnSelectStateChange;

    protected override void Awake()
    {
		base.Awake();
		InitTrans();
    }

    protected override void OnDestroy()
    {
		base.OnDestroy();
		OnSelectStateChange = null;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
		RefreshMark();
    }

    Image m_roleHead;
	Text m_roleName;
	Text m_roleInfo;
	RoleInstance m_role;
	List<int> m_showPropertyIds = new List<int>() { 14, 13, 15 };//要显示的属性

    [SerializeField]
    private Graphic m_SelectMark;
	[SerializeField]
    private bool m_IsSelected = false;

    public bool IsSelected => m_IsSelected;

#if UNITY_EDITOR
	protected override void OnValidate()
	{
		base.OnValidate();
		RefreshMark();
	}
#endif


	void InitTrans()
	{
		m_roleHead = transform.Find("RoleHead").GetComponent<Image>();
		m_roleName = transform.Find("Name").GetComponent<Text>();
		m_roleInfo = transform.Find("Info").GetComponent<Text>();
	}

    public void SetData(RoleInstance data)
    {
        m_role = data;
        RefreshRole();
    }

    private void RefreshRole()
	{
		if (m_role == null)
			return;

		string nameText = m_role.Name + " Lv." + m_role.Level;
		m_roleName.text = nameText;

		ShowProperty();

		m_roleHead.LoadAsyncForget(m_role.GetPic());
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
				var color1 = m_role.GetHPColor1();
				var color2 = m_role.GetHPColor2();
				sb.Append($"{proItem.Name}:<color={color1}>{m_role.Hp}</color>/<color={color2}>{m_role.MaxHp}</color>\n");
			}
			else if (proItem.PropertyName == "Tili")
			{
				sb.Append($"{proItem.Name}:{m_role.Tili}/{GameConst.MAX_ROLE_TILI}\n");
			}
			else if (proItem.PropertyName == "Mp")
			{
				var color = m_role.GetMPColor();
				sb.Append($"{proItem.Name}:<color={color}>{m_role.Mp}/{m_role.MaxMp}</color>\n");
			}
			else
			{
				try
				{
					int value = (int) m_role.GetType().GetProperty(proItem.PropertyName).GetValue(m_role, null);
					sb.Append($"{proItem.Name}:{value}\n");
				}
				catch
				{
					
				}
			}
		}
		m_roleInfo.text = sb.ToString();
	}

	public void SetState(bool _isSelected, bool notifyEvent = true)
	{
		m_IsSelected = _isSelected;
		if(notifyEvent)
		{
			OnSelectStateChange?.Invoke(this, m_IsSelected);
		}
        RefreshMark();
    }

    private void RefreshMark(bool instant = true)
    {
        if (m_SelectMark != null)
        {
            if (!Application.isPlaying)
            {
                m_SelectMark.canvasRenderer.SetAlpha(IsSelected ? 1f : 0f);
            }
            else
            {
                m_SelectMark.CrossFadeAlpha(IsSelected ? 1f : 0f, instant ? 0f : 0.1f, true);
            }
        }
    }

    public RoleInstance GetShowRole()
	{
		return m_role;
	}

    public void Connect(INavigable up = null, INavigable down = null, INavigable left = null, INavigable right = null)
    {
        var selectable = GetSelectable();
		if (selectable == null)
			return;
        Navigation navigation = new Navigation();
        navigation.mode = Navigation.Mode.Explicit;
        navigation.selectOnUp = up?.GetSelectable();
        navigation.selectOnDown = down?.GetSelectable();
        navigation.selectOnLeft = left?.GetSelectable();
        navigation.selectOnRight = right?.GetSelectable();
        selectable.navigation = navigation;
    }

    public Selectable GetSelectable()
    {
		return this;
    }

    public void Select(bool notifyEvent)
    {
		SetState(true, notifyEvent);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
		if (eventData.button == PointerEventData.InputButton.Left)
			SetState(!IsSelected);
    }
}
