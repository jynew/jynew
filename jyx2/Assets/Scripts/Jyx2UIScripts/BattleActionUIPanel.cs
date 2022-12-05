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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Jyx2.Battle;
using Jyx2Configs;
using UnityEngine;
using UnityEngine.UI;
using i18n.TranslatorDef;
using Jyx2.Util;
using Jyx2.UINavigation;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;

public partial class BattleActionUIPanel : Jyx2_UIBase
{
	struct LastOperateAction
	{
		public int RoleId;

		public int SkillIdx;

		public int RightActionIdx;
        
        public void ResetData() 
		{
            RoleId = -1;
            SkillIdx = -1;
            RightActionIdx = -1;
        }
    }
    
	[SerializeField]
	private string m_SkillItemPrefabPath;

	[ShowInInspector, ReadOnly]
	private List<Button> m_RightButtons = new List<Button>();
    
    public List<Button> RightActionBtns
	{
		get
		{
            if (m_RightButtons.Count == 0)
            {
				var btns = RightActions_VerticalLayoutGroup.GetComponentsInChildren<Button>(true);
                m_RightButtons.AddRange(btns);
            }
            return m_RightButtons;
        }
	}

	public RoleInstance GetCurrentRole()
	{
		return m_currentRole;
	}

	RoleInstance m_currentRole;

	List<SkillUIItem> m_CurSkillItems = new List<SkillUIItem>();

	private List<SkillUIItem> m_CacheSkillItems = new List<SkillUIItem>();

	private bool isSelectMove;
	private Action<BattleLoop.ManualResult> OnManualAction;
	private List<BattleBlockVector> moveRange;
	private BattleFieldModel battleModel;
	private SkillCastInstance currentSkill;

	private LastOperateAction m_LastOperateData = new LastOperateAction();

	private bool IsSelectingBox => !MainActions_RectTransform.gameObject.activeSelf;

    
	protected override void OnCreate()
	{
		InitTrans();

		BindListener(Move_Button, OnMoveClick);
		BindListener(UsePoison_Button, OnUsePoisonClick);
		BindListener(Depoison_Button, OnDepoisonClick);
		BindListener(Heal_Button, OnHealClick);
		BindListener(Item_Button, OnUseItemClick);
		BindListener(Wait_Button, OnWaitClick);
		BindListener(Rest_Button, OnRestClick);
		BindListener(Surrender_Button, OnSurrenderClick);
		BindListener(Cancel_Button, OnCancelClick);
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
		OnManualAction = (Action<BattleLoop.ManualResult>)allParams[3];
		battleModel = BattleManager.Instance.GetModel();

        BattleboxHelper.Instance.OnBlockSelectMoved -= OnControllerBlockMove;
        BattleboxHelper.Instance.OnBlockConfirmed -= OnControllerBlockConfirmed;
        BattleboxHelper.Instance.OnBlockSelectMoved += OnControllerBlockMove;
		BattleboxHelper.Instance.OnBlockConfirmed += OnControllerBlockConfirmed;

        SetActionsVisible(true);
        SetActionBtnState();
		RefreshSkill();
		RestoreLastOperation();
    }

	private void OnControllerBlockMove(BattleBlockData block)
	{
		ShowSkillCastHitRange(block);
	}

	private void OnControllerBlockConfirmed(BattleBlockData block)
	{
		ShowSkillCastHitRange();
		OnBlockConfirmed(block, false);
	}


	//显示攻击范围选择指示器
	void ShowAttackRangeSelector(SkillCastInstance skillCast)
	{
		currentSkill = skillCast;

		isSelectMove = false;

		BattleboxHelper.Instance.HideAllBlocks();
        SetActionsVisible(false);
        var blockList = BattleManager.Instance.GetSkillUseRange(m_currentRole, skillCast);

		//prevent reselecting causing not showing hit range
		_lastHitRangeOverBlock = null;
		BattleboxHelper.Instance.ShowBlocks(m_currentRole, blockList, BattleBlockType.AttackZone, true);
	}

	private BattleBlockData _lastHitRangeOverBlock = null;

	public override void Update()
	{
		base.Update();

		//显示当前攻击范围
		ShowSkillCastHitRange();

		//寻找玩家点击的格子
		var block = InputManager.Instance.GetMouseUpBattleBlock();

		//没有选择格子
		if (block == null) return;

		//格子隐藏（原则上应该不会出现）
		if (!block.gameObject.activeSelf) return;

		//选择移动，但位置站人了
		if (isSelectMove && battleModel.BlockHasRole(block.BattlePos.X, block.BattlePos.Y)) return;


		//以下进行回调

		//移动
		OnBlockConfirmed(block, true);
	}

	private void ShowSkillCastHitRange(BattleBlockData block = null)
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

	private void OnBlockConfirmed(BattleBlockData block, bool isMouseClick)
	{
		if (!BattleboxHelper.Instance.AnalogMoved && !isMouseClick)
			return;
        
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
		OnManualAction?.Invoke(ret);
	}

	//点击了自动
	public void OnAutoClicked()
	{
		TryCallback(new BattleLoop.ManualResult() { isAuto = true });
	}

	protected override void OnHidePanel()
	{
        SaveLastRightAction();
		base.OnHidePanel();
		m_currentRole = null;
		m_CurSkillItems.Clear();
        isSelectMove = false;
        //隐藏格子
        BattleboxHelper.Instance?.HideAllBlocks();
	}

	void SetActionBtnState()
	{
		bool canMove = m_currentRole.movedStep <= 0;
		Move_Button.gameObject.BetterSetActive(canMove);
		bool canPoison = m_currentRole.UsePoison >= 20 && m_currentRole.Tili >= 10;
		UsePoison_Button.gameObject.BetterSetActive(canPoison);
		bool canDepoison = m_currentRole.DePoison >= 20 && m_currentRole.Tili >= 10;
		Depoison_Button.gameObject.BetterSetActive(canDepoison);
		bool canHeal = m_currentRole.Heal >= 20 && m_currentRole.Tili >= 50;
		Heal_Button.gameObject.BetterSetActive(canHeal);

		bool isLastRole = BattleManager.Instance.GetModel().IsLastRole(m_currentRole);
		Wait_Button.gameObject.BetterSetActive(!isLastRole);
		RefreshRightButtonNavigation();

    }

    private List<Selectable> _ActiveButtons = new List<Selectable>();
    void RefreshRightButtonNavigation()
    {
        _ActiveButtons.Clear();
        _ActiveButtons.AddRange(RightActionBtns.Where(btn => btn.gameObject.activeSelf));
        NavigateUtil.SetUpNavigation(_ActiveButtons, _ActiveButtons.Count, 1, true);
    }


    void RefreshSkill()
	{
		m_CurSkillItems.Clear();
		var skillCastList = m_currentRole.GetSkills(true).ToList();
		Action<int, SkillUIItem, SkillCastInstance> OnSkillItemCreate = (idx, item, data) =>
		{
			m_CurSkillItems.Add(item);
			item.OnSkillItemClick -= OnSelectSkillItem;
			item.OnSkillItemClick += OnSelectSkillItem;
        };

		MonoUtil.GenerateMonoElementsWithCacheList(m_SkillItemPrefabPath, skillCastList, m_CacheSkillItems, Skills_RectTransform, OnSkillItemCreate);
		NavigateUtil.SetUpNavigation(m_CurSkillItems, 1, m_CurSkillItems.Count);
	}

	void OnSelectSkillItem(SkillUIItem item)
	{
		m_currentRole.CurrentSkill = m_CurSkillItems.IndexOf(item);
		m_LastOperateData.RightActionIdx = -1;
        m_LastOperateData.SkillIdx = m_currentRole.CurrentSkill;
        m_currentRole.SwitchAnimationToSkill(item.GetSkill().Data);
		ShowAttackRangeSelector(item.GetSkill());
	}

	public void OnCancelClick()
	{
		if (IsSelectingBox)
			OnCancelBoxSelection();
        else
			TryCallback(new BattleLoop.ManualResult() { isRevert = true });
	}

	public void OnMoveClick()
	{
        ResetSkillOperateIndex();
        isSelectMove = true;
        _lastHitRangeOverBlock = null;
        SetActionsVisible(false);
        BattleboxHelper.Instance.ShowBlocks(m_currentRole, moveRange, BattleBlockType.MoveZone, false);
    }

	public void OnUsePoisonClick()
	{
        ResetSkillOperateIndex();
        var skillCast = new PoisonSkillCastInstance(m_currentRole.UsePoison);
		ShowAttackRangeSelector(skillCast);
	}

    public void OnDepoisonClick()
	{
        ResetSkillOperateIndex();
        var skillCast = new DePoisonSkillCastInstance(m_currentRole.DePoison);
		ShowAttackRangeSelector(skillCast);
	}

	void OnHealClick()
	{
		if (!Heal_Button.gameObject.activeSelf)
			return;
        ResetSkillOperateIndex();
        var skillCast = new HealSkillCastInstance(m_currentRole.Heal);
		ShowAttackRangeSelector(skillCast);
	}
    
	async void OnUseItemClick()
    {
        ResetSkillOperateIndex();
        Func<Jyx2ConfigItem, bool> itemFilterFunc = item => item.GetItemType() == Jyx2ItemType.Costa 
														 || item.GetItemType() == Jyx2ItemType.Anqi;

		Action<int> OnItemSelected = itemId =>
		{
			if (itemId == -1)
				return;

			var item = GameConfigDatabase.Instance.Get<Jyx2ConfigItem>(itemId);
			if (item == null)
				return;
			if (item.GetItemType() == Jyx2ItemType.Costa) //使用道具逻辑
			{
				if (m_currentRole.CanUseItem(itemId))
				{
					TryCallback(new BattleLoop.ManualResult() { aiResult = new AIResult() { Item = item } });
				}
			}
			else if (item.GetItemType() == Jyx2ItemType.Anqi) //使用暗器逻辑
			{
				var skillCast = new AnqiSkillCastInstance(m_currentRole.Anqi, item);
				ShowAttackRangeSelector(skillCast);
			}

		};

        await Jyx2_UIManager.Instance.ShowUIAsync(nameof(BagUIPanel), OnItemSelected, itemFilterFunc);
	}

	void OnWaitClick()
	{
		TryCallback(new BattleLoop.ManualResult() { isWait = true });
		ResetSkillOperateIndex();

    }

	void OnRestClick()
	{
		TryCallback(new BattleLoop.ManualResult() { aiResult = new AIResult() { IsRest = true } });
        ResetSkillOperateIndex();
    }

	public void OnSurrenderClick()
	{
		Action onConfirm = () => TryCallback(new BattleLoop.ManualResult() { isSurrender = true });

		MessageBox.ConfirmOrCancel("确定投降并放弃本场战斗?".GetContent(nameof(BattleActionUIPanel)), onConfirm);
	}

    public void OnCancelBoxSelection()
	{
        BattleboxHelper.Instance.HideAllBlocks(true);
		SetActionsVisible(true);
    }
    

    public void TryFocusOnCurrentSkill()
	{
		if (m_CurSkillItems.Count == 0)
			return;
        var idx = m_LastOperateData.SkillIdx;
        idx = Mathf.Clamp(idx, 0, m_CurSkillItems.Count - 1);
		m_CurSkillItems[idx].Select(false);
    }

    public void TryFocusOnRightAction()
    {
		var selectedBtn = RightActionBtns.SafeGet(m_LastOperateData.RightActionIdx);
        if(selectedBtn == null || !selectedBtn.gameObject.activeSelf)
			selectedBtn = RightActionBtns.FirstOrDefault(btn => btn.gameObject.activeSelf);
        
        if(selectedBtn != null)
			EventSystem.current.SetSelectedGameObject(selectedBtn.gameObject);
        else
            EventSystem.current.SetSelectedGameObject(Rest_Button.gameObject);
    }

	private void SetActionsVisible(bool isVisible)
	{
		MainActions_RectTransform.gameObject.BetterSetActive(isVisible);
        BlockNotice_RectTransform.gameObject.BetterSetActive(!isVisible);
    }

    public bool IsFocusOnSkillsItems
	{
		get
		{
			var curSelect = EventSystem.current.currentSelectedGameObject;
			if (curSelect == null)
				return false;
			if (!curSelect.activeInHierarchy)
				return false;
			return m_CurSkillItems.Any(item => item.gameObject == curSelect);
		}
	}

    #region 操作缓存

    private void ResetOperationIfNotSameRole()
	{
        if (m_currentRole == null || m_currentRole.GetJyx2RoleId() != m_LastOperateData.RoleId)
        {
            m_LastOperateData.ResetData();
        }
    }

    private void ResetSkillOperateIndex()
	{
		m_LastOperateData.SkillIdx = -1;
	}
    

    private void RestoreLastOperation()
	{
        ResetOperationIfNotSameRole();
        if (m_LastOperateData.SkillIdx >= 0)
        {
            TryFocusOnCurrentSkill();

        }
        else
        {
            TryFocusOnRightAction();
        }
    }

    private void SaveLastRightAction()
	{
		if (m_currentRole == null)
			return;
        var curSelect = EventSystem.current?.currentSelectedGameObject;
        m_LastOperateData.RoleId = m_currentRole.GetJyx2RoleId();
		m_LastOperateData.RightActionIdx = RightActionBtns.FindIndex(btn => btn.gameObject == curSelect);
	}
    #endregion
}
