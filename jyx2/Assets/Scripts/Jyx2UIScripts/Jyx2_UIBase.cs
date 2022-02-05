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
	protected volatile bool currentlyReleased = true;

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

	public event Action<Jyx2_UIBase, bool> VisibilityToggled;

	protected void visiblityToggle(bool show)
	{
		VisibilityToggled?.Invoke(this, show);
	}

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

		if (IsBlockControl && !IsChangedBlockControl && LevelMaster.Instance != null && !BattleManager.Instance.IsInBattle && LevelMaster.Instance.IsPlayerCanControl())
		{
			IsChangedBlockControl = true;
			LevelMaster.Instance.SetPlayerCanController(false);
		}

		VisibilityToggled?.Invoke(this, true);

		if (resetCurrentSelectionOnShow && GamepadHelper.GamepadConnected && captureGamepadAxis && activeButtons.Length > 0)
			changeCurrentSelection(0);
	}

	protected virtual bool resetCurrentSelectionOnShow
	{
		get
		{
			return true;
		}
	}

	//temporarily comment out the game pad connection state change handler
	//since cannot poll input joystick names in another thread
	//private void onGameConnectionStateChange(bool hasGamepad)
	//{
	//	if (hasGamepad)
	//	{
	//		//hilite the selected item
	//		changeCurrentSelection(Math.Max(current_selection, 0));
	//	}
	//	else
	//	{
	//		//unhilite selected item
	//		if (current_selection > -1)
	//		{
	//			changeCurrentSelection(-1);
	//		}
	//	}
	//}

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

		VisibilityToggled.Invoke(this, false);
	}

	protected Button[] activeButtons
	{
		get
		{
			return _buttonList.Keys
				.Where(b => b != null && !b.IsDestroyed() && b.gameObject.activeSelf)
				.ToArray();
		}
	}

	protected virtual void changeCurrentSelection(int num)
	{
		current_selection = num;

		for (int i = 0; i < activeButtons.Length; i++)
		{
			var buttonText = getButtonText(activeButtons[i]);

			if (buttonText != null)
				buttonText.color = i == current_selection ? selectedButtonColor() : normalButtonColor();
		}
	}

	protected virtual Text getButtonText(Button button)
	{
		return button.gameObject.transform.GetChild(0).GetComponent<Text>();
	}

	protected virtual Image getButtonImage(Button button)
	{
		Transform trans = button.gameObject.transform;
		if (trans.childCount > 1)
			return trans.GetChild(1).GetComponentInChildren<Image>();

		return null;
	}

	protected virtual Color selectedButtonColor()
	{
		return ColorStringDefine.system_item_selected;
	}

	protected virtual Color normalButtonColor()
	{
		return ColorStringDefine.system_item_normal;
	}

	//TODO: handle bottom buttons navigation with gamepad

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

	protected virtual void cleanupDestroyedButtons()
	{
		var buttons = _buttonList.Keys.ToArray();
		foreach (var button in buttons)
		{
			if (button.IsDestroyed())
			{
				_buttonList.Remove(button);
			}
		}
	}


	protected void toggleGamepadButtonImage(Button button, bool forceOff = false)
	{
		//toggle image visibility is there is an image on this button
		var image = getButtonImage(button);
		if (image != null)
		{

			if (forceOff)
			{
				image.gameObject.SetActive(false);
			}
			else
				image.gameObject.SetActive(GamepadHelper.GamepadConnected);
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

	protected virtual void OnDirectionalLeft()
	{
		//do nothing by default, since most UI menus are vertical
	}

	protected virtual void OnDirectionalRight()
	{
		//do nothing by default, since most UI menus are vertical
	}



	protected virtual void OnDirectionalDown()
	{
		if (activeButtons.Length == 0)
			return;

		if (current_selection >= activeButtons.Length - 1)
			current_selection = 0;
		else
			current_selection++;

		changeCurrentSelection(current_selection);
	}

	protected virtual void OnDirectionalUp()
	{
		if (activeButtons.Length == 0)
			return;

		if (current_selection <= 0)
			current_selection = activeButtons.Length - 1;
		else
			current_selection--;

		changeCurrentSelection(current_selection);
	}

	bool gamepadConnected = false;
	private bool scrollSizeAdjusted;

	public virtual void Update()
	{
		//show hide button icons
		if (GamepadHelper.GamepadConnected != gamepadConnected)
		{
			gamepadConnected = GamepadHelper.GamepadConnected;
			//toggle all button images
			var buttons = this.gameObject.GetComponentsInChildren<Button>(true)
				.Where(b => b != null);
			foreach (var button in buttons)
			{
				toggleGamepadButtonImage(button);
			}
		}

		if (isOnTop())
		{
			handleDpadMove();
			handleGamepadButtons();
		}
	}

	protected virtual void handleGamepadButtons()
	{
		if (GamepadHelper.IsButtonPressed(confirmButtonName()) && gameObject.activeSelf)
		{
			//trigger button click
			if (captureGamepadAxis && activeButtons.Length > 0)
				buttonClickAt(current_selection);
		}
	}

	protected bool isOnTop()
	{
		return Jyx2_UIManager.Instance.IsTopVisibleUI(this);
	}

	protected virtual bool handleDpadMove()
	{
		bool dpadMoved = false;
		if (captureGamepadAxis && gameObject.activeSelf)
		{
			if (GamepadHelper.IsDadYMove(true))
			{
				if (currentlyReleased)
				{
					OnDirectionalDown();
					currentlyReleased = false;

					dpadMoved = true;

					delayedAxisRelease();
				}
			}
			else if (GamepadHelper.IsDadYMove(false))
			{
				if (currentlyReleased)
				{
					OnDirectionalUp();
					currentlyReleased = false;
					dpadMoved = true;
					delayedAxisRelease();
				}
			}
			if (GamepadHelper.IsDadXMove(false))
			{
				if (currentlyReleased)
				{
					OnDirectionalLeft();
					currentlyReleased = false;

					dpadMoved = true;

					delayedAxisRelease();
				}
			}
			else if (GamepadHelper.IsDadXMove(true))
			{
				if (currentlyReleased)
				{
					OnDirectionalRight();
					currentlyReleased = false;
					dpadMoved = true;
					delayedAxisRelease();
				}
			}
		}

		return dpadMoved;
	}

	protected virtual string confirmButtonName()
	{
		return GamepadHelper.CONFIRM_BUTTON;
	}

	protected virtual void buttonClickAt(int position)
	{
		if (position > -1 && position < activeButtons.Length)
		{
			Action callback = _buttonList[activeButtons[position]];

			if (callback != null)
				callback();
		}
	}

	protected virtual int axisReleaseDelay
	{
		get
		{
			return 200;
		}
	}

	protected void delayedAxisRelease()
	{
		Task.Run(() =>
		{
			Thread.Sleep(axisReleaseDelay);
			currentlyReleased = true;
		});
	}

	protected void setAreasHeightForItemCompleteView(float itemHeight, RectTransform[] areasToAdjust )
	{
		if (areasToAdjust.Length == 0)
			return;

		if (!scrollSizeAdjusted)
		{
			//shrink scroll and description boxes, to make sure no half items showing
			var boxHeight = areasToAdjust[0].rect.height;
			boxHeight = (float)Math.Floor(boxHeight / itemHeight) * itemHeight;

			foreach(var area in areasToAdjust)
			{
				var scrollSize = area.sizeDelta;
				area.sizeDelta = new Vector2(scrollSize.x, boxHeight);
			}

			scrollSizeAdjusted = true;
		}
	}

	protected void scrollIntoView(ScrollRect area, RectTransform item, GridLayoutGroup layout, float padding)
	{
		var itemHeight = layout.cellSize.y + layout.spacing.y;

		Canvas.ForceUpdateCanvases();

		var areaPos = area.transform.InverseTransformPoint(area.content.position).y;
		var childPos = area.transform.InverseTransformPoint(item.position).y;
		var scrollRelativePos = areaPos - childPos - area.content.anchoredPosition.y;

		//check if the item is fully visible, if yes, no need to scroll
		var areaHeight = (area.transform as RectTransform).rect.height - (padding * 2);

		var scrollFinalRelativePos = (float) Math.Floor(scrollRelativePos / areaHeight) * areaHeight;

		//if item display half, snap to top of the item
		scrollFinalRelativePos =  (float)Math.Floor(scrollFinalRelativePos / itemHeight) * itemHeight;

		area.content.anchoredPosition = new Vector2(0, area.content.anchoredPosition.y + scrollFinalRelativePos);
	}
}
