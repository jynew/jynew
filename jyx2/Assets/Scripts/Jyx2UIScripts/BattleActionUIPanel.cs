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
	private SkillCastInstance currentSkill;
	private Dictionary<Button, Action> skillList = new Dictionary<Button, Action>();

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

	protected Image getSkillCastButtonImage(Button button)
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
				//fix issue of mp deplition causes skillCast not showing, and previously recorded current index
				//out of range.
				if (m_currentRole.CurrentSkill >= m_curItemList.Count)
					m_currentRole.CurrentSkill = 0;

				var skillCast = m_curItemList[m_currentRole.CurrentSkill].GetSkill();
				ShowAttackRangeSelector(skillCast);
			}
		}

		changeCurrentSelection(-1);
	}

	private void onBattleBlockMove(BattleBlockData block)
	{
		//hide the hilite
		changeCurrentSelection(-1);
		//hide the skillCast selection
		changeCurrentSkillCastSelection(-1);

		showSkillCastHitRange(block);
	}

	private void gamepadBlockConfirmed(BattleBlockData obj)
	{
		showSkillCastHitRange();
		blockConfirm(obj, false);
	}


	//显示攻击范围选择指示器
	void ShowAttackRangeSelector(SkillCastInstance skillCast)
	{
		currentSkill = skillCast;

		isSelectMove = false;

		BattleboxHelper.Instance.HideAllBlocks();
		var blockList = BattleManager.Instance.GetSkillUseRange(m_currentRole, skillCast);

		//prevent reselecting causing not showing hit range
		_lastHitRangeOverBlock = null;
		BattleboxHelper.Instance.ShowBlocks(m_currentRole, blockList, BattleBlockType.AttackZone, true);
	}

	private BattleBlockData _lastHitRangeOverBlock = null;
	private bool rightDpadPressed;
	private bool leftDpadPressed;

	private int cur_skillCast = 0;

	private void changeCurrentSkillCastSelection(int number)
	{
		if (skillList.Count == 0)
			return;

		cur_skillCast = number;

		if (number > -1)
		{
			changeCurrentSelection(-1);
			BattleboxHelper.Instance.AnalogMoved = false;
		}

		var curBtnKey = number < 0 || number > skillList.Count ?
			null :
			skillList.ElementAt(number).Key;

		foreach (var btn in skillList)
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

			var action = getSkillCastButtonImage(btn.Key);
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
			changeCurrentSkillCastSelection(-1);
			BattleboxHelper.Instance.AnalogMoved = false;
		}

		base.changeCurrentSelection(num);
	}

	protected override bool resetCurrentSelectionOnShow => false;

	protected override void OnDirectionalRight()
	{
		if (skillList.Count == 0)
			return;

		changeCurrentSelection(-1);

		var nextSkillCast = (cur_skillCast >= skillList.Count - 1) ?
			0 :
			cur_skillCast + 1;

		changeCurrentSkillCastSelection(nextSkillCast);
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
		if (skillList.Count == 0)
			return;

		changeCurrentSelection(-1);

		var nextSkillCast = (cur_skillCast <= 0) ?
			cur_skillCast = skillList.Count - 1 :
			cur_skillCast - 1;

		changeCurrentSkillCastSelection(nextSkillCast);
	}

	public override void Update()
	{
		base.Update();

		//显示当前攻击范围
		showSkillCastHitRange();

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

	private void showSkillCastHitRange(BattleBlockData block = null)
	{
		if (!isSelectMove)
		{
			var overBlock = block ?? InputManager.Instance.GetMouseOverBattleBlock();
			if (overBlock != null && overBlock != _lastHitRangeOverBlock)
			{
				_lastHitRangeOverBlock = overBlock;
				var range = BattleManager.Instance.GetSkillCoverBlocks(currentSkill, overBlock.BattlePos, m_currentRole.Pos);
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
			else if (GamepadHelper.IsAction()) // x/square button invoke skillCast 
			{
				if (m_curItemList.Count == 0)
					return;

				if (cur_skillCast < 0 || cur_skillCast >= m_curItemList.Count)
				{
					cur_skillCast = 0;
					changeCurrentSkillCastSelection(cur_skillCast);
				}

				onSkillCastStart(m_curItemList[cur_skillCast], cur_skillCast);
			}
		}
	}

	protected override void buttonClickAt(int position)
	{
		if (!BattleboxHelper.Instance.AnalogMoved && cur_skillCast == -1)
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

			rst.SkillCast = currentSkill;

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
		skillList.Clear();
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
		var skillCasts = m_currentRole.GetSkills(true).ToList();
		childMgr.RefreshChildCount(skillCasts.Count);
		List<Transform> childTransList = childMgr.GetUsingTransList();
		skillList.Clear();

		for (int i = 0; i < skillCasts.Count; i++)
		{
			int index = i;
			SkillUIItem item = GameUtil.GetOrAddComponent<SkillUIItem>(childTransList[i]);
			item.RefreshSkill(skillCasts[i]);
			item.SetSelect(i == m_currentRole.CurrentSkill);

			Button btn = item.GetComponent<Button>();
			bindSkillCast(btn, () => { onSkillCastStart(item, index); });
			m_curItemList.Add(item);
		}

		if (m_currentRole.CurrentSkill > -1 && m_currentRole.CurrentSkill < skillCasts.Count)
		{
			changeCurrentSkillCastSelection(m_currentRole.CurrentSkill);
		}
	}

	void bindSkillCast(Button btn, Action callback)
	{
		BindListener(btn, callback, false);
		skillList[btn] = callback;
	}



	void onSkillCastStart(SkillUIItem item, int index)
	{
		// clear current skillCast selection selected color only
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
		var skillCast = new PoisonSkillCastInstance(m_currentRole.UsePoison);
		ShowAttackRangeSelector(skillCast);
	}

	void OnDepoisonClick()
	{
		var skillCast = new DePoisonSkillCastInstance(m_currentRole.DePoison);
		ShowAttackRangeSelector(skillCast);
	}

	void OnHealClick()
	{
		if (!Heal_Button.gameObject.activeSelf)
			return;

		var skillCast = new HealSkillCastInstance(m_currentRole.Heal);
		ShowAttackRangeSelector(skillCast);
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
				var skillCast = new AnqiSkillCastInstance(m_currentRole.Anqi, item);
				ShowAttackRangeSelector(skillCast);
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
