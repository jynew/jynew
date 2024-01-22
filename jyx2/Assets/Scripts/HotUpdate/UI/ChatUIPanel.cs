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

using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Jyx2.ResourceManagement;
using Sirenix.Utilities;
using Jyx2.Util;
using Jyx2.UINavigation;

public enum ChatType
{
	None = -1,
	RoleId = 1,
	Selection = 2,
}
public partial class ChatUIPanel : Jyx2_UIBase
{
	[SerializeField]
	private string m_SelectionItemPrefabPath;

	public override UILayer Layer => UILayer.NormalUI;
	public override bool IsOnly => true;

	Action _onTalkEnd;
	ChatType _currentShowType = ChatType.None;
	string _currentText;//存一下要显示的文字 当文字要显示的时候 用一个指针显示当前显示到的索引 分多次显示，点击显示接下来的
	int _currentShowIndex = 0;

	public ChatType CurrentShowType => _currentShowType;

	private List<ChatUISelectionItem> m_SelectItems = new List<ChatUISelectionItem>();

	protected override void OnCreate()
	{
		InitTrans();
		MainBg_Button.onClick.AddListener(OnMainBgClick);
	}

	public void OnMainBgClick()
	{
		if (_currentShowType == ChatType.None)
			return;
		if (_currentShowType == ChatType.Selection)
			return;
		ShowText();
	}

	protected override void OnShowPanel(params object[] allParams)
	{
		base.OnShowPanel(allParams);
		ChatType _type = (ChatType)allParams[0];
		_currentShowType = _type;
		_currentShowIndex = 0;
		switch (_type)
		{
			case ChatType.RoleId:
				Show((int)allParams[1], (string)allParams[2], (int)allParams[3], (Action)allParams[4]);
				break;
			case ChatType.Selection:
				ShowSelection((string)allParams[1], (string)allParams[2], (List<string>)allParams[3], (Action<int>)allParams[4]);
				break;
		}
	}

	protected override void OnHidePanel()
	{
		base.OnHidePanel();
		selectionCallback = null;
	}


	private GameObject _interactivePanel = null;
	private Action<int> selectionCallback;
	private int selectionContentCount = 0;

	private async UniTask ShowCharacter(int headId)
	{
		ChangePosition(headId);
		
		var url = $"Assets/BuildSource/head/{headId}.png";
        
		RoleHeadImage_Image.gameObject.SetActive(false);
		RoleHeadImage_Image.sprite = await ResLoader.LoadAsset<Sprite>(url);
		RoleHeadImage_Image.gameObject.SetActive(true);
	}

	//根据对话框最大显示字符以及标点断句分段显示对话 by eaphone at 2021/6/12
	void ShowText()
	{
		if (_currentShowIndex >= _currentText.Length - 1)
		{
			Jyx2_UIManager.Instance.HideUI(nameof(ChatUIPanel));
			var talkEndCallback = _onTalkEnd;
			_onTalkEnd = null;

            talkEndCallback?.Invoke();
			return;
		}
		var finalS = _currentText;
		if (_currentText.Length > GameConst.MAX_CHAT_CHART_NUM)
		{
			int preIndex = _currentShowIndex;
			string[] sList = _currentText.Substring(preIndex, _currentText.Length - preIndex).Split(new char[] { '！', '？', '，', '　' }, StringSplitOptions.RemoveEmptyEntries);//暂时不对,'．'进行分割，不然对话中...都会被去除掉
			var tempIndex = 0;
			foreach (var i in sList)
			{
				var tempNum = i.Length + 1;//包含分隔符
				if (tempIndex + tempNum < GameConst.MAX_CHAT_CHART_NUM)
				{
					tempIndex += tempNum;
					_currentShowIndex += tempNum;
					continue;
				}
				break;
			}
			_currentShowIndex = Mathf.Clamp(_currentShowIndex, 0, _currentText.Length);
			finalS = _currentText.Substring(preIndex, _currentShowIndex - preIndex);
		}
		else
		{
			_currentShowIndex = _currentText.Length;
		}
		MainContent_Text.text = finalS;
        EventSystem.current.SetSelectedGameObject(MainBg_Button.gameObject);
    }

	public void Show(int headId, string msg, int type, Action callback)
	{
		_currentText = $"{msg}";
		_onTalkEnd = callback;
		SelectionPanel_RectTransform.gameObject.SetActive(false);

		HeadAvataPre_RectTransform.gameObject.SetActive(!(type == 2 || type == 3));

		//不显示人物
		if (type == 2 || type == 3)
		{
			ChangePosition(1, false);
			RoleHeadImage_Image.gameObject.SetActive(false);
		}
		else
		{
			ShowCharacter(headId).Forget();
		}
		ShowText();
	}

	string GetRoleName(int headId)
	{
		//主角名定制
		if (headId == 0 && GameRuntimeData.Instance.Player != null)
		{
			return GameRuntimeData.Instance.Player.Name;
		}

		//先找替换修正的
		if (RuntimeEnvSetup.CurrentModConfig.StoryIdNameFixes != null)
		{
			var find = RuntimeEnvSetup.CurrentModConfig.StoryIdNameFixes.SingleOrDefault(p => p.Id == headId);
			if (find != null)
			{
				return find.Name;
			}
		}

		//再从人物库找
		var roleName = LuaToCsBridge.CharacterTable[headId].Name;
		return roleName;
	}

	//根据角色ID修改左右位置
	public void ChangePosition(int headId, bool ShowName = true)
	{
		Name_RectTransform.gameObject.SetActive(ShowName);
		kuang_RectTransform.gameObject.SetActive(ShowName);
		if (ShowName)
		{
			NameTxt_Text.text = GetRoleName(headId);
		}

		Content_RectTransform.anchoredPosition = headId == 0 || !ShowName ? Vector3.zero : new Vector3(450, 0, 0);

		Content_RectTransform.sizeDelta = ShowName ? new Vector2(-450, 280) : new Vector2(0, 280);


		HeadAvataPre_RectTransform.anchorMax = headId == 0 ? Vector2.right : Vector2.zero;
		HeadAvataPre_RectTransform.anchorMin = headId == 0 ? Vector2.right : Vector2.zero;
		HeadAvataPre_RectTransform.pivot = headId == 0 ? Vector2.right : Vector2.zero;
		HeadAvataPre_RectTransform.anchoredPosition = Vector3.zero;

		kuang_RectTransform.anchorMax = headId == 0 ? Vector2.right : Vector2.zero;
		kuang_RectTransform.anchorMin = headId == 0 ? Vector2.right : Vector2.zero;
		kuang_RectTransform.pivot = headId == 0 ? Vector2.right : Vector2.zero;
		kuang_RectTransform.anchoredPosition = Vector3.zero;

		Name_RectTransform.anchorMax = headId == 0 ? Vector2.right : Vector2.zero;
		Name_RectTransform.anchorMin = headId == 0 ? Vector2.right : Vector2.zero;
		Name_RectTransform.pivot = headId == 0 ? Vector2.right : Vector2.zero;
		Name_RectTransform.anchoredPosition = new Vector2(headId == 0 ? -450 : 450, 280);
	}

	private void InvokeSelection(int iSelection)
	{
        Action<int> tmpSelectionCalback = selectionCallback;
		selectionCallback = null;
        Jyx2_UIManager.Instance.HideUI(nameof(ChatUIPanel));
        tmpSelectionCalback?.Invoke(iSelection);
    }


	public void ShowSelection(string roleId, string msg, List<string> selectionContent, Action<int> OnChooseSelection)
	{
		ShowCharacter(int.Parse(roleId)).Forget();
		MainContent_Text.text = $"{msg}";

		selectionCallback = OnChooseSelection;
		selectionContentCount = selectionContent.Count;

		Action<int, ChatUISelectionItem, string> onSelectionItemCreate = (idx, item, data) =>
		{
			item.SetIndex(idx);
			item.SetClickCallBack(InvokeSelection);
		};

		MonoUtil.GenerateMonoElementsWithCacheList(m_SelectionItemPrefabPath, selectionContent, m_SelectItems, Container_RectTransform, onSelectionItemCreate);
		NavigateUtil.SetUpNavigation(m_SelectItems, selectionContentCount, 1);
		ConnectMainBgNavigation();
        SelectionPanel_RectTransform.gameObject.SetActive(true);
	}

	private void ConnectMainBgNavigation()
	{
		var firstItem = m_SelectItems.FirstOrDefault();
		if (firstItem == null)
			return;
		Navigation navi = new Navigation();
		navi.mode = Navigation.Mode.Explicit;
		navi.selectOnLeft = firstItem.GetSelectable();
        navi.selectOnRight = firstItem.GetSelectable();
        navi.selectOnUp = firstItem.GetSelectable();
        navi.selectOnDown = firstItem.GetSelectable();
		MainBg_Button.navigation = navi;
		EventSystem.current.SetSelectedGameObject(MainBg_Button.gameObject);
    }
}
