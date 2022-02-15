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

public class RoleUIItem : MonoBehaviour
{
	public static RoleUIItem Create()
	{
		var obj = Jyx2ResourceHelper.CreatePrefabInstance("RoleItem");
		var roleItem = obj.GetComponent<RoleUIItem>();
		roleItem.InitTrans();
		return roleItem;
	}

	Transform m_select;
	Transform m_over;
	Image m_roleHead;
	Text m_roleName;
	Text m_roleInfo;
	Transform m_actionButton;
	RoleInstance m_role;
	List<int> m_showPropertyIds = new List<int>() { 14, 13, 15 };//要显示的属性

	void InitTrans()
	{
		m_select = transform.Find("Select");
		m_over = transform.Find("Over");
		m_roleHead = transform.Find("RoleHead").GetComponent<Image>();
		m_roleName = transform.Find("Name").GetComponent<Text>();
		m_roleInfo = transform.Find("Info").GetComponent<Text>();
		m_actionButton = transform.Find("ActionButton");
	}

	public void ShowRole(RoleInstance role, List<int> pros = null)
	{
		m_role = role;
		if (pros != null)
			m_showPropertyIds = pros;

		string nameText = role.Name + " Lv." + role.Level;
		m_roleName.text = nameText;

		ShowProperty();

		m_roleHead.LoadAsyncForget(role.Data.GetPic());
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

	bool isOver = false;

	public void SetState(bool? selected, bool? over)
	{
		if (selected.HasValue)
		{
			if (m_select != null)
				m_select.gameObject.SetActive(selected.Value);
		}

		if (over.HasValue)
		{
			isOver = over.Value;
			var allowPerformingOver = selected != null ? !selected.Value : true;

			//turn off over if selected
			if (m_over != null)
				m_over.gameObject.SetActive(isOver && allowPerformingOver);
			//always show
			if (m_actionButton != null)
				m_actionButton.gameObject.SetActive(isOver);
		}
	}

	public RoleInstance GetShowRole()
	{
		return m_role;
	}

	bool gamepadConnected = false;

	private void Update()
	{
		if (gamepadConnected != GamepadHelper.GamepadConnected)
		{
			gamepadConnected = GamepadHelper.GamepadConnected;

			if (gamepadConnected)
			{
				m_actionButton.gameObject.SetActive(isOver);
				m_over.gameObject.SetActive(isOver);
			}
			else if (!gamepadConnected)
			{
				m_actionButton.gameObject.SetActive(false);
				m_over.gameObject.SetActive(false);
			}
		}
	}
}
