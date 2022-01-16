/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using Jyx2.Middleware;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public interface IUIAnimator
{
	void DoShowAnimator();
	void DoHideAnimator();
}

public abstract class Jyx2_UIBase : MonoBehaviour
{
	private bool downDpadPressed;
	protected volatile bool currentlyReleased = true;
	private bool upDpadPressed;

	protected Dictionary<Button, Action> _buttonList = new Dictionary<Button, Action>();

	public virtual UILayer Layer { get; } = UILayer.NormalUI;
	public virtual bool IsOnly { get; } = false;//同一层只能单独存在
	public virtual bool IsBlockControl { get; set; } = false;
	public virtual bool AlwaysDisplay { get; } = false;
	private bool IsChangedBlockControl = false;

	protected int current_selection = 0;

	protected abstract void OnCreate();

	protected virtual void OnShowPanel(params object[] allParams) { }
	protected virtual void OnHidePanel() { }

	public void Init()
	{
		var rt = GetComponent<RectTransform>();
		rt.localPosition = Vector3.zero;
		rt.localScale = Vector3.one;
		rt.anchorMin = Vector2.zero;
		rt.anchorMax = Vector2.one;
		rt.offsetMin = Vector2.zero;
		rt.offsetMax = Vector2.zero;

		OnCreate();
	}


	public void Show(params object[] allParams)
	{
		this.gameObject.SetActive(true);
		this.transform.SetAsLastSibling();
		this.OnShowPanel(allParams);
		if (this is IUIAnimator)
		{
			(this as IUIAnimator).DoShowAnimator();
		}

		if (IsBlockControl && !IsChangedBlockControl && !BattleManager.Instance.IsInBattle && LevelMaster.Instance.IsPlayerCanControl())
		{
			IsChangedBlockControl = true;
			LevelMaster.Instance.SetPlayerCanController(false);
		}

		if (captureGamepadAxis && _buttonList.Count > 0)
			changeCurrentSelection(0);
	}

	public void Hide()
	{
		if (AlwaysDisplay) return;

		this.gameObject.SetActive(false);
		this.OnHidePanel();
		if (IsBlockControl && IsChangedBlockControl)
		{
			IsChangedBlockControl = false;
			LevelMaster.Instance.SetPlayerCanController(true);
		}
	}

	protected virtual void changeCurrentSelection(int num)
	{
		current_selection = num;

		for (int i = 0; i < _buttonList.Count; i++)
		{
			var buttonText = getButtonText(_buttonList.ElementAt(i));

			if (buttonText != null)
				buttonText.color = i == current_selection ? selectedButtonColor() : normalButtonColor();
		}
	}

	protected virtual Text getButtonText(KeyValuePair<Button, Action> button)
	{
		return button.Key.gameObject.transform.GetChild(0).GetComponent<Text>();
	}

	protected virtual Color selectedButtonColor()
	{
		return ColorStringDefine.system_item_selected;
	}

	protected virtual Color normalButtonColor()
	{
		return ColorStringDefine.system_item_normal;
	}


	public virtual void BindListener(Button button, Action callback, bool supportGamepadButtonsNav = true)
	{
		if (button != null)
		{
			if (supportGamepadButtonsNav)
				_buttonList[button] = callback;

			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(() =>
			{
				callback();
			});
			var nav = Navigation.defaultNavigation;
			nav.mode = Navigation.Mode.None;
			button.navigation = nav;
		}
	}

	public void ClearChildren(Transform transform)
	{
		HSUnityTools.DestroyChildren(transform);
	}

	protected virtual bool captureGamepadAxis
	{
		get
		{
			return false;
		}
	}

	protected virtual void OnDirectionalDown()
	{
		if (_buttonList.Count == 0)
			return;

		if (current_selection == _buttonList.Count - 1)
			current_selection = 0;
		else
			current_selection++;

		changeCurrentSelection(current_selection);
	}

	protected virtual void OnDirectionalUp()
	{
		if (_buttonList.Count == 0)
			return;

		if (current_selection == 0)
			current_selection = _buttonList.Count - 1;
		else
			current_selection--;

		changeCurrentSelection(current_selection);
	}

	public virtual void Update()
	{
		if (captureGamepadAxis && gameObject.activeSelf)
		{
			var dpadY = Input.GetAxis("DPadY");
			if (dpadY == -1)
			{
				downDpadPressed = true;
				if (downDpadPressed && currentlyReleased)
				{
					OnDirectionalDown();
					currentlyReleased = false;

					delayedAxisRelease();
				}
			}
			else if (dpadY == 1)
			{
				upDpadPressed = true;
				if (upDpadPressed && currentlyReleased)
				{
					OnDirectionalUp();
					currentlyReleased = false;
					delayedAxisRelease();
				}
			}
		}

		if (Input.GetButtonDown(confirmButtonName()) && gameObject.activeSelf)
		{
			//trigger button click
			if (captureGamepadAxis && _buttonList.Count > 0)
				buttonClickAt(current_selection);
		}
	}

	protected virtual string confirmButtonName()
	{
		return "JFire2";
	}

	protected void buttonClickAt(int position)
	{
		if (position > -1 && position < _buttonList.Count)
		{
			Action callback = _buttonList.ElementAt(position)
							.Value;

			if (callback != null)
				callback();
		}
	}


	protected void delayedAxisRelease()
	{
		Task.Run(() =>
		{
			Thread.Sleep(500);
			currentlyReleased = true;
		});
	}
}
