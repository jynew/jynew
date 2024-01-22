/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using System;
using System.Collections.Generic;
using System.Linq;
using ch.sycoforge.Decal;
using Jyx2;
using Jyx2.InputCore;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleboxHelper : MonoBehaviour,IJyx2_InputContext
{
	void OnDestroy()
	{
		InputContextManager.Instance.RemoveInputContext(this);
	}

	public static BattleboxHelper Instance
	{
		get
		{
			if (_instance == null) _instance = FindObjectOfType<BattleboxHelper>();
			return _instance;
		}
	}
	private static BattleboxHelper _instance;

	//绘制区域（主角身边的范围）
	public int m_MoveZoneDrawRange = 20;

	private BattleboxManager _currentBattlebox;

	private const string RootPath = "BattleboxRoot";
	private bool _isInit = false;
	private BattleboxManager[] _boxList;
	private GameObject _boxRoot;
	private BattleBlockType m_CurrentType;

	void Start()
	{
		Init();
	}

    void RegisterInput()
	{
		//不能选中UI，不然按A键就触发按钮回调了
		EventSystem.current?.SetSelectedGameObject(null);
		EventSystem.current.sendNavigationEvents = false;
        
        InputContextManager.Instance.AddInputContext(this);
		//这一帧UI的A键确认了，等下一帧在响应
		m_InputEnableFrame = Time.frameCount + 1;
    }

    void UnRegisterInput()
	{
        InputContextManager.Instance.RemoveInputContext(this);
		EventSystem.current.sendNavigationEvents = true;
    }

	//某位置是否能触发战斗
	public bool CanEnterBattle(Vector3 pos)
	{
		if (!_isInit || _boxList == null || _boxList.Length == 0)
		{
			Debug.Log($"BattleboxHelper还没有初始化成功或者盒子列表为空");
			return false;
		}

		foreach (var box in _boxList)
		{
			if (box.ColliderContain(pos))
			{
				Debug.Log($"找到了战斗盒子，可以进入战斗；玩家坐标：{pos.x}：{pos.y}：{pos.z}");
				return true;
			}
		}

		Debug.Log($"BattleboxHelper找不到合适的战斗盒子");
		return false;
	}

	//先清空所有战斗格子
	//如果该位置可以战斗，则设置当前战斗盒子
	//初始化当前战斗盒子
	//生成所有战斗格子（默认为inactive）
	public bool EnterBattle(Vector3 pos)
	{
		ClearAllBlocks();
		foreach (var box in _boxList)
		{
			if (!box.ColliderContain(pos)) continue;
			Debug.Log($"找到了战斗盒子，玩家坐标：{pos.x}：{pos.y}：{pos.z}");
			_currentBattlebox = box;
			_currentBattlebox.Init();
			_currentBattlebox.DrawAreaBlocks(pos, m_MoveZoneDrawRange);

			return true;
		}
		return false;
	}

	public void OnTestControl(Vector3 pos)
	{
		if (Input.GetKey("g"))
		{
			if (!CanEnterBattle(pos)) return;
			EnterBattle(pos);
			ShowMoveZone(Jyx2Player.GetPlayer().transform.position);
		}
		else if (Input.GetKey("h"))
		{
			HideAllBlocks();
		}
	}

	public BattleBlockData GetBlockData(int xindex, int yindex)
	{
		if (!GeneralPreJudge()) return null;

		return _currentBattlebox.GetBlockData(xindex, yindex);
	}

	//清除当前
	//脱离战斗的时候必须调用
	private void ClearAllBlocks()
	{
		if (_isInit && _currentBattlebox != null)
			_currentBattlebox.ClearAllBlocks();
	}


	//获取当前战斗格子，待改成实际每个战场预先计算好的
	public List<BattleBlockData> GetBattleBlocks()
	{
		if (!GeneralPreJudge()) return null;
		return _currentBattlebox.GetBattleBlocks();
	}

	//获取坐标对应的格子
	public BattleBlockData GetLocationBattleBlock(Vector3 pos)
	{
		var tempXY = _currentBattlebox.GetXYIndex(pos.x, pos.z);
		var centerX = (int)tempXY.X;
		var centerY = (int)tempXY.Y;
		return GetBlockData(centerX, centerY);
	}

	//判断格子是否存在（必须是有效格子）
	public bool IsBlockExists(int xindex, int yindex)
	{
		if (!GeneralPreJudge()) return false;

		if (!_currentBattlebox.Exist(xindex, yindex)) return false;

		var block = _currentBattlebox.GetBlockData(xindex, yindex);
		if (block != null && block.BoxBlock != null && !block.BoxBlock.IsValid) return false;

		return true;
	}

	private int[] xPositions = new int[0];
	private int xMiddlePos = -1;
	private int[] yPositions = new int[0];
	private int yMiddlePos;
	private int CurPosX;
	private int CurPosY;
    
	public bool AnalogMoved = false;

	private int m_InputEnableFrame = int.MaxValue;
	public bool CanUpdate => m_InputEnableFrame <= Time.frameCount;
    

	public void OnUpdate()
	{
		if (xPositions.Length == 0 || yPositions.Length == 0)
			return;
        if(Jyx2_Input.GetNegativeButtonDown(Jyx2ActionConst.MoveHorizontal))
		{
			TryMoveHorizontal(CurPosX + 1);
		}
        else if(Jyx2_Input.GetButtonDown(Jyx2ActionConst.MoveHorizontal))
		{
            TryMoveHorizontal(CurPosX - 1);
        }

        if (Jyx2_Input.GetNegativeButtonDown(Jyx2ActionConst.MoveVertical))
        {
            TryMoveVertical(CurPosY + 1);
        }
        else if (Jyx2_Input.GetButtonDown(Jyx2ActionConst.MoveVertical))
        {
            TryMoveVertical(CurPosY - 1);
        }


        if (Jyx2_Input.GetButtonDown(Jyx2ActionConst.UIConfirm))
		{
			if (AnalogMoved && !IsMoveSelectAndBlocked(CurPosX, CurPosY))
			{
				var selectedBlock =  _currentBattlebox.GetBlockData(CurPosX, CurPosY);
				if (selectedBlock != null && !selectedBlock.Inaccessible)
				{
					OnBlockConfirmed?.Invoke(selectedBlock);
				}
			}
		}

        if (IsCancelBoxSelection())
        {
			TryCancelBoxSelection();
        }

    }
    
    private void TryMoveHorizontal(int newPosX)
	{
        if (newPosX < xPositions.First() || newPosX > xPositions.Last())
            return;
        if (TrySelectNewBlock(newPosX, CurPosY))
            CurPosX = newPosX;
    }

    private void TryMoveVertical(int newPosY)
	{
        if (newPosY < yPositions.First() || newPosY > yPositions.Last())
            return;
		if (TrySelectNewBlock(CurPosX, newPosY))
			CurPosY = newPosY;
    }

	public bool IsMoveSelectAndBlocked(int x, int y)
	{
		return m_CurrentType == BattleBlockType.MoveZone && IsRoleStandingInBlock(x, y);
	}

	public bool IsRoleStandingInBlock(int x, int y)
    {
		var battleModel = BattleManager.Instance.GetModel();
		if (battleModel == null)
			return false;
		return battleModel.BlockHasRole(x, y);
	}

    private bool IsCancelBoxSelection()
    {
        if (Jyx2_Input.GetButtonDown(Jyx2ActionConst.UIClose))
            return true;
        if (Input.GetMouseButtonDown(1) && !Application.isMobilePlatform)
            return true;
        return false;
    }

    public void TryCancelBoxSelection()
	{
        var ui = Jyx2_UIManager.Instance.GetUI<BattleActionUIPanel>();
        if (ui != null)
            ui.OnCancelBoxSelection();
    }


	private bool TrySelectNewBlock(int newX, int newY)
	{
		var newSelectedBlock =  _currentBattlebox.GetBlockData(newX, newY);
		if (newSelectedBlock != null && newSelectedBlock.IsActive)
		{
			if (_selectedBlock != null)
			{
				_selectedBlock.gameObject.GetComponent<EasyDecal>().DecalRenderer.material.SetColor("_TintColor", _oldColor);
			}

			_selectedBlock = newSelectedBlock;
			_oldColor = newSelectedBlock.gameObject.GetComponent<EasyDecal>().DecalRenderer.material.GetColor("_TintColor");
			Color hiliteColor = newSelectedBlock.Inaccessible ?				
				new Color(0.4f, 0.4f, 0.4f, BattleboxManager.BATTLEBLOCK_DECAL_ALPHA) : //gray color for inaccessible blocks
				new Color(1, 0, 1, BattleboxManager.BATTLEBLOCK_DECAL_ALPHA);
			_selectedBlock.gameObject.GetComponent<EasyDecal>().DecalRenderer.material.SetColor("_TintColor", hiliteColor);

			AnalogMoved = true;

			if (OnBlockSelectMoved != null)
				OnBlockSelectMoved(newSelectedBlock);

			return true;
		}

		return false;
	}

	public event Action<BattleBlockData> OnBlockSelectMoved;
	public event Action<BattleBlockData> OnBlockConfirmed;

	private void initXPos()
	{
		xPositions = this._currentBattlebox
		.GetBattleBlocks()
		.Where(b => b.IsActive)
		.Select(b => b.BattlePos.X)
		.OrderBy(p => p)
		.Distinct()
		.ToArray();
	}

	private void initYPos()
	{
		yPositions = this._currentBattlebox
		.GetBattleBlocks()
		.Where(b => b.IsActive)
		.Select(b => b.BattlePos.Y)
		.OrderBy(p => p)
		.Distinct()
		.ToArray();
	}

	bool rangeMode = false;
	private BattleBlockData _selectedBlock;
	private Color _oldColor;

	public void ShowBlocks(RoleInstance role, IEnumerable<BattleBlockVector> list, BattleBlockType type = BattleBlockType.MoveZone,
		bool selectMiddlePos = false)
	{
		if (!GeneralPreJudge()) return;
		HideAllBlocks();
		m_CurrentType = type;
        if (type == BattleBlockType.MoveZone)
		{
			_currentBattlebox.SetAllBlockColor(new Color(1, 1, 1, BattleboxManager.BATTLEBLOCK_DECAL_ALPHA));
			_selectedBlock = null;
		}
		else if (type == BattleBlockType.AttackZone)
		{
			_currentBattlebox.SetAllBlockColor(new Color(1, 0, 0, BattleboxManager.BATTLEBLOCK_DECAL_ALPHA));
			_selectedBlock = null;
		}

		foreach (var vector in list)
		{
			var block = _currentBattlebox.GetBlockData(vector.X, vector.Y);
			if (block != null && block.BoxBlock.IsValid)
			{
				if (vector.Inaccessible)
				{
					_currentBattlebox.SetBlockInaccessible(block);
				}

				block.Inaccessible = vector.Inaccessible;
				block.Show();
			}
		}

		xMiddlePos = role.Pos.X;
		yMiddlePos = role.Pos.Y;
		initShownPositions();

		if (selectMiddlePos)
		{
			TrySelectNewBlock(CurPosX, CurPosY);
		}
		RegisterInput();
		rangeMode = false;
	}

	private void initShownPositions()
	{
		initXPos();
		CurPosX = xMiddlePos;

		initYPos();
		CurPosY = yMiddlePos;
	}

	public void ShowRangeBlocks(IEnumerable<BattleBlockVector> list)
	{
		//todo: debug skillCast that has range instead of just one point
		if (!GeneralPreJudge()) return;
		_currentBattlebox.HideAllRangeBlocks();

		foreach (var vector in list)
		{
			var block = _currentBattlebox.GetRangelockData(vector.X, vector.Y);
			if (block != null && block.BoxBlock.IsValid) 
				block.Show();
		}

		//initShownPositions();
		rangeMode = true;
	}

	public void HideAllBlocks(bool hideRangeBlock = false)
	{
		if (!GeneralPreJudge()) return;

		_currentBattlebox.HideAllBlocks();
		if (hideRangeBlock)
		{
			_currentBattlebox.HideAllRangeBlocks();
		}
		_selectedBlock = null;
		UnRegisterInput();
	}



	public void ShowAllBlocks()
	{
		if (!GeneralPreJudge()) return;

		_currentBattlebox.ShowAllValidBlocks();
	}

	void ShowMoveZone(Vector3 center, int range = -1)
	{
		if (!GeneralPreJudge()) return;
		_currentBattlebox.HideAllBlocks();

		if (range == -1)
		{
			range = m_MoveZoneDrawRange;
		}

		_currentBattlebox.ShowBlocksCenterDist(center, range);
	}

	private void Init()
	{
		_boxRoot = GameObject.Find(RootPath);
		if (_boxRoot == null)
		{
			Debug.Log($"当前场景找不到Battlebox的根节点(BattleboxRoot)，初始化失败,本场景无法战斗！");
			return;
		}

		_boxList = _boxRoot.GetComponentsInChildren<BattleboxManager>();
		if (_boxList == null || _boxList.Length == 0)
		{
			Debug.Log($"当前场景BattleboxRoot节点下没有Battlebox，本场景无法战斗！");
			return;
		}

		_isInit = true;
	}

	private bool GeneralPreJudge()
	{
		if (!_isInit)
		{
			Debug.Log($"BattleboxHelper还没有初始化成功");
			return false;
		}

		if (_currentBattlebox == null)
		{
			Debug.Log($"BattleboxHelper没找到当前格子");
			return false;
		}
		return true;
	}
}
