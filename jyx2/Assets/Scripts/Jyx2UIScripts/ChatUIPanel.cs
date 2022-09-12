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
using Jyx2Configs;
using UnityEngine.EventSystems;
using Jyx2.ResourceManagement;
using Sirenix.Utilities;

public enum ChatType
{
	None = -1,
	RoleId = 1,
	Selection = 2,
}
public partial class ChatUIPanel : Jyx2_UIBase, IUIAnimator
{
	public override UILayer Layer => UILayer.NormalUI;
	public override bool IsOnly => true;

	Action _callback;
	ChatType _currentShowType = ChatType.None;
	string _currentText;//存一下要显示的文字 当文字要显示的时候 用一个指针显示当前显示到的索引 分多次显示，点击显示接下来的
	int _currentShowIndex = 0;
	protected override void OnCreate()
	{
		InitTrans();

		StorySelectionItem_Button.gameObject.SetActive(false);
		MainBg_Button.onClick.AddListener(OnMainBgClick);

		InitPanelTrigger();
	}

	void OnMainBgClick()
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

		//临时将触发按钮隐藏
		var panel = FindObjectOfType<InteractUIPanel>();
		if (panel != null && panel.gameObject.activeSelf)
		{
			_interactivePanel = panel.gameObject;
			panel.gameObject.SetActive(false);
		}
	}

	protected override void OnHidePanel()
	{
		base.OnHidePanel();
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
			if (_interactivePanel)
			{
				_interactivePanel.SetActive(true);
				_interactivePanel = null;
			}
			var c = _callback;
			_callback = null;
			
			c?.Invoke();
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
	}

	public void Show(int headId, string msg, int type, Action callback)
	{
		_currentText = $"{msg}";
		_callback = callback;
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
		var role = GameConfigDatabase.Instance.Get<Jyx2ConfigCharacter>(headId);
		return role.Name;
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

	protected override void handleGamepadButtons()
	{
		if (gameObject.activeSelf)
			if (GamepadHelper.IsConfirm())
			{
				if (selectionContentCount > 1)
				{
					Jyx2_UIManager.Instance.HideUI(nameof(ChatUIPanel));
					selectionCallback?.Invoke(0);
				}
				else
				{
					OnMainBgClick();
				}
			}
			else if (GamepadHelper.IsCancel())
			{
				if (selectionContentCount > 1)
				{
					Jyx2_UIManager.Instance.HideUI(nameof(ChatUIPanel));
					selectionCallback?.Invoke(1);
				}
			}
			else if (Input.GetKeyDown(KeyCode.Space))
				OnMainBgClick();
	}

	public void ShowSelection(string roleId, string msg, List<string> selectionContent, Action<int> callback)
	{
		ShowCharacter(int.Parse(roleId)).Forget();
		MainContent_Text.text = $"{msg}";

		selectionCallback = callback;
		selectionContentCount = selectionContent.Count;

		ClearChildren(Container_RectTransform.transform);
		for (int i = 0; i < selectionContent.Count; i++)
		{
			int currentIndex = i;
			Button selectionItem = Instantiate(StorySelectionItem_Button);
			selectionItem.gameObject.SetActive(true);
			selectionItem.transform.Find("Text").GetComponent<Text>().text = selectionContent[i];

			var image = getButtonImage(selectionItem);
			if (image != null)
			{
				image.gameObject.SetActive(GamepadHelper.GamepadConnected);
				UniTask.Void(async () =>
				{
					image.sprite = await getGamepadIconSprites(currentIndex);	
				});
			}
			selectionItem.transform.SetParent(Container_RectTransform, false);
			BindListener(selectionItem, delegate
			{
				Jyx2_UIManager.Instance.HideUI(nameof(ChatUIPanel));
				callback?.Invoke(currentIndex);
			}, false);
		}

		GlobalHotkeyManager.Instance.RegistHotkey(this, KeyCode.Y, () =>
		{
			Jyx2_UIManager.Instance.HideUI(nameof(ChatUIPanel));
			callback?.Invoke(0);
		});
		GlobalHotkeyManager.Instance.RegistHotkey(this, KeyCode.N, () =>
		{
			Jyx2_UIManager.Instance.HideUI(nameof(ChatUIPanel));
			callback?.Invoke(1);
		});
		SelectionPanel_RectTransform.gameObject.SetActive(true);
	}

	public static async UniTask<Sprite> getGamepadIconSprites(int i)
	{
		string iconPath;
		switch (i)
		{
			case 0:
				iconPath = "Assets/BuildSource/Gamepad/confirm.png";
				break;
			case 1:
				iconPath = "Assets/BuildSource/Gamepad/cancel.png";
				break;
			case 2:
				iconPath = "Assets/BuildSource/Gamepad/action.png";
				break;
			case 3:
				iconPath = "Assets/BuildSource/Gamepad/jump.png";
				break;
			default:
				iconPath = "";
				break;
		}

		if (iconPath.IsNullOrWhitespace())
		{
			return null;
		}
		else
		{
			return await ResLoader.LoadAsset<Sprite>(iconPath);
		}
	}

	public void DoShowAnimator()
	{
		//Content_RectTransform.anchoredPosition = Vector2.zero;
		//allTweenList.Add(Content_RectTransform.DOAnchorPosY(130, 0.5f));
		//HeadAvataPre_RectTransform.anchoredPosition = new Vector2(-300, 300);
		//allTweenList.Add(HeadAvataPre_RectTransform.DOAnchorPosX(300, 0.5f));
	}

	public void DoHideAnimator()
	{

	}

	public void OnDisable()
	{
		GlobalHotkeyManager.Instance.UnRegistHotkey(this, KeyCode.Y);
		GlobalHotkeyManager.Instance.UnRegistHotkey(this, KeyCode.N);
	}

	private void InitPanelTrigger()
	{
		List<EventTrigger.Entry> entries = Panel_Trigger.triggers;
		for (int i = 0; i < entries.Count; i++)
		{
			if (entries[i].eventID == EventTriggerType.PointerClick)
			{
				entries[i].callback = new EventTrigger.TriggerEvent();
				entries[i].callback.AddListener(new UnityEngine.Events.UnityAction<BaseEventData>((BaseEventData) =>
				{
					OnMainBgClick();
				}));
				break;
			}
		}
	}

	protected override Image getButtonImage(Button button)
	{
		return button.transform.Find("GamepadButtonIcon")?.GetComponent<Image>();
	}
}
