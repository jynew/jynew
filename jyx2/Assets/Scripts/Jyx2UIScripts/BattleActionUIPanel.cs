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
using Jyx2.Middleware;

using Jyx2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Jyx2.Battle;
using Jyx2Configs;
using UnityEngine;
using UnityEngine.UI;

public partial class BattleActionUIPanel : Jyx2_UIBase
{
	public RoleInstance GetCurrentRole()
	{
		return m_currentRole;
	}

	RoleInstance m_currentRole;

	List<SkillUIItem> m_curItemList = new List<SkillUIItem>();
	ChildGoComponent childMgr;

	private bool isSelectMove;
	private Action<BattleLoop.ManualResult> callback;
	private List<BattleBlockVector> moveRange;
	private BattleFieldModel battleModel;
	private BattleZhaoshiInstance currentZhaoshi;
	private Dictionary<Button, Action> zhaoshiList = new Dictionary<Button, Action>();

	protected override void OnCreate()
	{
		InitTrans();
		childMgr = GameUtil.GetOrAddComponent<ChildGoComponent>(Skills_RectTransform);
		childMgr.Init(SkillItem_RectTransform);

		BindListener(Move_Button, OnMoveClick);
		BindListener(UsePoison_Button, OnUsePoisonClick);
		BindListener(Depoison_Button, OnDepoisonClick);
		BindListener(Heal_Button, OnHealClick);
		BindListener(Item_Button, OnUseItemClick);
		BindListener(Wait_Button, OnWaitClick);
		BindListener(Rest_Button, OnRestClick);
		BindListener(Cancel_Button, OnCancelClick);
	}

	protected override bool captureGamepadAxis { get { return true; } }

	protected override Text getButtonText(Button button)
	{
		if (button.gameObject.transform.childCount == 1)
			return base.getButtonText(button);

		for (var i = 0; i < button.gameObject.transform.childCount; i++)
		{
			var text = button.gameObject.transform.GetChild(i).GetComponent<Text>();
			if (text != null)
				return text;
		}

		return null;
	}

	protected Image getZhaoshiButtonImage(Button button)
	{
		Transform trans = button.gameObject.transform;
		for (var i = 0; i < trans.childCount; i++)
		{
			var image = trans.GetChild(i).GetComponent<Image>();
			if (image != null && image.name == "ActionIcon")
				return image;
		}

		return null;
	}

	protected override void OnShowPanel(params object[] allParams)
	{
		base.OnShowPanel(allParams);
		m_currentRole = allParams[0] as RoleInstance;
		if (m_currentRole == null)
			return;

		moveRange = (List<BattleBlockVector>)allParams[1];
		isSelectMove = (bool)allParams[2];
		callback = (Action<BattleLoop.ManualResult>)allParams[3];
		battleModel = BattleManager.Instance.GetModel();

		BattleboxHelper.Instance.analogLeftMovedToBlock += onBattleBlockMove;
		BattleboxHelper.Instance.blockConfirmed += gamepadBlockConfirmed;

		//Cancel_Button.gameObject.SetActive(false);
		SetActionBtnState();
		RefreshSkill();
		//SetPanelState();

		if (isSelectMove)
		{
			_lastHitRangeOverBlock = null;
			BattleboxHelper.Instance.ShowBlocks(m_currentRole, moveRange, BattleBlockType.MoveZone, false);
		}
		else
		{
			if (m_curItemList.Count > 0)
			{
				//fix issue of mp deplition causes zhaoshi not showing, and previously recorded current index
				//out of range.
				if (m_currentRole.CurrentSkill >= m_curItemList.Count)
					m_currentRole.CurrentSkill = 0;

				var zhaoshi = m_curItemList[m_currentRole.CurrentSkill].GetSkill();
				ShowAttackRangeSelector(zhaoshi);
			}
		}

		changeCurrentSelection(-1);
	}

	private void onBattleBlockMove(BattleBlockData block)
	{
		//hide the hilite
		changeCurrentSelection(-1);
		//hide the zhaoshi selection
		changeCurrentZhaoshiSelection(-1);

		showZhaoshiHitRange(block);
	}

	private void gamepadBlockConfirmed(BattleBlockData obj)
	{
		showZhaoshiHitRange();
		blockConfirm(obj, false);
	}


	//显示攻击范围选择指示器
	void ShowAttackRangeSelector(BattleZhaoshiInstance zhaoshi)
	{
		currentZhaoshi = zhaoshi;

		isSelectMove = false;

		BattleboxHelper.Instance.HideAllBlocks();
		var blockList = BattleManager.Instance.GetSkillUseRange(m_currentRole, zhaoshi);

		//prevent reselecting causing not showing hit range
		_lastHitRangeOverBlock = null;
		BattleboxHelper.Instance.ShowBlocks(m_currentRole, blockList, BattleBlockType.AttackZone, true);
	}

	private BattleBlockData _lastHitRangeOverBlock = null;
	private bool rightDpadPressed;
	private bool leftDpadPressed;

	private int cur_zhaoshi = 0;

	private void changeCurrentZhaoshiSelection(int number)
	{
		if (zhaoshiList.Count == 0)
			return;

		cur_zhaoshi = number;

		if (number > -1)
		{
			changeCurrentSelection(-1);
			BattleboxHelper.Instance.AnalogMoved = false;
		}

		var curBtnKey = number < 0 || number > zhaoshiList.Count ?
			null :
			zhaoshiList.ElementAt(number).Key;

		foreach (var btn in zhaoshiList)
		{
			bool isInvokedButton = btn.Key == curBtnKey;
			var text = getButtonText(btn.Key);
			if (text != null)
			{
				text.color = isInvokedButton ?
					base.selectedButtonColor() :
					base.normalButtonColor();
				text.fontStyle = isInvokedButton ?
					FontStyle.Bold :
					FontStyle.Normal;
			}

			var action = getZhaoshiButtonImage(btn.Key);
			if (action != null)
			{
				action.gameObject.SetActive(isInvokedButton);
			}
		}
	}

	protected override void changeCurrentSelection(int num)
	{
		if (num > -1)
		{
			changeCurrentZhaoshiSelection(-1);
			BattleboxHelper.Instance.AnalogMoved = false;
		}

		base.changeCurrentSelection(num);
	}

	protected override bool resetCurrentSelectionOnShow => false;

	protected override void OnDirectionalRight()
	{
		if (zhaoshiList.Count == 0)
			return;

		changeCurrentSelection(-1);

		var nextZhaoshi = (cur_zhaoshi >= zhaoshiList.Count - 1) ?
			0 :
			cur_zhaoshi + 1;

		changeCurrentZhaoshiSelection(nextZhaoshi);
	}

	protected override void OnDirectionalUp()
	{
		base.OnDirectionalUp();
	}

	protected override void OnDirectionalDown()
	{
		base.OnDirectionalDown();
	}

	protected override void OnDirectionalLeft()
	{
		if (zhaoshiList.Count == 0)
			return;

		changeCurrentSelection(-1);

		var nextZhaoshi = (cur_zhaoshi <= 0) ?
			cur_zhaoshi = zhaoshiList.Count - 1 :
			cur_zhaoshi - 1;

		changeCurrentZhaoshiSelection(nextZhaoshi);
	}

	public override void Update()
	{
		base.Update();

		//显示当前攻击范围
		showZhaoshiHitRange();

		//寻找玩家点击的格子
		var block = InputManager.Instance.GetMouseUpBattleBlock();

		//没有选择格子
		if (block == null) return;

		//格子隐藏（原则上应该不会出现）
		if (block.gameObject.activeSelf == false) return;

		//选择移动，但位置站人了
		if (isSelectMove && battleModel.BlockHasRole(block.BattlePos.X, block.BattlePos.Y)) return;


		//以下进行回调

		//移动
		blockConfirm(block, true);
	}

	private void showZhaoshiHitRange(BattleBlockData block = null)
	{
		if (!isSelectMove)
		{
			var overBlock = block ?? InputManager.Instance.GetMouseOverBattleBlock();
			if (overBlock != null && overBlock != _lastHitRangeOverBlock)
			{
				_lastHitRangeOverBlock = overBlock;
				var range = BattleManager.Instance.GetSkillCoverBlocks(currentZhaoshi, overBlock.BattlePos, m_currentRole.Pos);
				BattleboxHelper.Instance.ShowRangeBlocks(range);
			}
		}
	}

	protected override void handleGamepadButtons()
	{
		base.handleGamepadButtons();

		if (gameObject.activeSelf)
		{
			if (GamepadHelper.IsCancel())
				//取消
				OnCancelClick();
			else if (GamepadHelper.IsJump())
				OnHealClick();
			else if (GamepadHelper.IsTabRight())
				OnRestClick();
			else if (GamepadHelper.IsAction()) // x/square button invoke zhaoshi 
			{
				if (m_curItemList.Count == 0)
					return;

				if (cur_zhaoshi < 0 || cur_zhaoshi >= m_curItemList.Count)
				{
					cur_zhaoshi = 0;
					changeCurrentZhaoshiSelection(cur_zhaoshi);
				}

				onZhaoshiStart(m_curItemList[cur_zhaoshi], cur_zhaoshi);
			}
		}
	}

	protected override void buttonClickAt(int position)
	{
		if (!BattleboxHelper.Instance.AnalogMoved && cur_zhaoshi == -1)
			base.buttonClickAt(position);
	}

	private void blockConfirm(BattleBlockData block, bool isMouseClick)
	{
		if (!BattleboxHelper.Instance.AnalogMoved && !isMouseClick)
			return;

		changeCurrentSelection(-1);

		if (isSelectMove)
		{
			TryCallback(new BattleLoop.ManualResult() { movePos = block.BattlePos }); //移动
		}
		else  //选择攻击
		{
			AIResult rst = new AIResult();
			rst.AttackX = block.BattlePos.X;
			rst.AttackY = block.BattlePos.Y;

			rst.Zhaoshi = currentZhaoshi;

			TryCallback(new BattleLoop.ManualResult() { aiResult = rst });
		}
	}

	void TryCallback(BattleLoop.ManualResult ret)
	{
		BattleboxHelper.Instance.HideAllBlocks(true);
		callback?.Invoke(ret);
	}

	//点击了自动
	public void OnAutoClicked()
	{
		TryCallback(new BattleLoop.ManualResult() { isAuto = true });
	}

	protected override void OnHidePanel()
	{
		base.OnHidePanel();
		m_currentRole = null;
		m_curItemList.Clear();

		//隐藏格子
		BattleboxHelper.Instance?.HideAllBlocks();
		zhaoshiList.Clear();
	}

	void SetActionBtnState()
	{
		bool canPoison = m_currentRole.UsePoison >= 20 && m_currentRole.Tili >= 10;
		UsePoison_Button.gameObject.SetActive(canPoison);
		bool canDepoison = m_currentRole.DePoison >= 20 && m_currentRole.Tili >= 10;
		Depoison_Button.gameObject.SetActive(canDepoison);
		bool canHeal = m_currentRole.Heal >= 20 && m_currentRole.Tili >= 50;
		Heal_Button.gameObject.SetActive(canHeal);

		bool lastRole = BattleManager.Instance.GetModel().IsLastRole(m_currentRole);
		Wait_Button.gameObject.SetActive(!lastRole);
	}

	void RefreshSkill()
	{
		m_curItemList.Clear();
		var zhaoshis = m_currentRole.GetZhaoshis(true).ToList();
		childMgr.RefreshChildCount(zhaoshis.Count);
		List<Transform> childTransList = childMgr.GetUsingTransList();
		zhaoshiList.Clear();

		for (int i = 0; i < zhaoshis.Count; i++)
		{
			int index = i;
			SkillUIItem item = GameUtil.GetOrAddComponent<SkillUIItem>(childTransList[i]);
			item.RefreshSkill(zhaoshis[i]);
			item.SetSelect(i == m_currentRole.CurrentSkill);

			Button btn = item.GetComponent<Button>();
			bindZhaoshi(btn, () => { onZhaoshiStart(item, index); });
			m_curItemList.Add(item);
		}

		if (m_currentRole.CurrentSkill > -1 && m_currentRole.CurrentSkill < zhaoshis.Count)
		{
			changeCurrentZhaoshiSelection(m_currentRole.CurrentSkill);
		}
	}

	void bindZhaoshi(Button btn, Action callback)
	{
		BindListener(btn, callback, false);
		zhaoshiList[btn] = callback;
	}



	void onZhaoshiStart(SkillUIItem item, int index)
	{
		// clear current zhaoshi selection selected color only
		if (index > -1)
			changeCurrentSelection(-1);

		m_currentRole.CurrentSkill = index;

		m_curItemList.ForEach(t =>
		{
			t.SetSelect(t == item);
		});

		m_currentRole.SwitchAnimationToSkill(item.GetSkill().Data);
		ShowAttackRangeSelector(item.GetSkill());
	}

	void OnCancelClick()
	{
		TryCallback(new BattleLoop.ManualResult() { isRevert = true });
	}

	void OnMoveClick()
	{

	}

	void OnUsePoisonClick()
	{
		var zhaoshi = new PoisonZhaoshiInstance(m_currentRole.UsePoison);
		ShowAttackRangeSelector(zhaoshi);
	}

	void OnDepoisonClick()
	{
		var zhaoshi = new DePoisonZhaoshiInstance(m_currentRole.DePoison);
		ShowAttackRangeSelector(zhaoshi);
	}

	void OnHealClick()
	{
		if (!Heal_Button.gameObject.activeSelf)
			return;

		var zhaoshi = new HealZhaoshiInstance(m_currentRole.Heal);
		ShowAttackRangeSelector(zhaoshi);
	}

	async void OnUseItemClick()
	{
		bool Filter(Jyx2ConfigItem item) => (int)item.ItemType == 3 || (int)item.ItemType == 4;

		await Jyx2_UIManager.Instance.ShowUIAsync(nameof(BagUIPanel), GameRuntimeData.Instance.Items, new Action<int>((itemId) =>
		{

			if (itemId == -1)
				return;

			var item = GameConfigDatabase.Instance.Get<Jyx2ConfigItem>(itemId);
			if ((int)item.ItemType == 3) //使用道具逻辑
			{
				if (m_currentRole.CanUseItem(itemId))
				{
					TryCallback(new BattleLoop.ManualResult() { aiResult = new AIResult() { Item = item } });
				}
			}
			else if ((int)item.ItemType == 4) //使用暗器逻辑
			{
				var zhaoshi = new AnqiZhaoshiInstance(m_currentRole.Anqi, item);
				ShowAttackRangeSelector(zhaoshi);
			}

		}), (Func<Jyx2ConfigItem, bool>)Filter);
	}

	void OnWaitClick()
	{
		TryCallback(new BattleLoop.ManualResult() { isWait = true });
	}

	void OnRestClick()
	{
		TryCallback(new BattleLoop.ManualResult() { aiResult = new AIResult() { IsRest = true } });
	}
}
