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
using System.Collections;
using System.Collections.Generic;
using ch.sycoforge.Decal;
using Jyx2;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class BattleboxHelper : MonoBehaviour
{
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
    public int m_MoveZoneDrawRange = 16;

    private BattleboxManager _currentBattlebox;

    private const string RootPath = "BattleboxRoot";
    private bool _isInit = false;
    private BattleboxManager[] _boxList;
    private GameObject _boxRoot;

    void Start()
    {
        Init();
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
            if (box.ColliderContain(pos))
            {
                Debug.Log($"找到了战斗盒子，玩家坐标：{pos.x}：{pos.y}：{pos.z}");
                _currentBattlebox = box;
                _currentBattlebox.Init();
                _currentBattlebox.DrawAreaBlocks(pos, m_MoveZoneDrawRange);
                return true;
            }
        }
        return false;
    }

    public void OnTestControl(Vector3 pos)
    {
        if (Input.GetKey("g"))
        {
            if (!CanEnterBattle(pos)) return;
            EnterBattle(pos);
            ShowMoveZone(GameRuntimeData.Instance.Player.View.transform.position);
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
    public void ClearAllBlocks()
    {
        if(_isInit && _currentBattlebox != null)
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

    public void ShowBlocks(IEnumerable<BattleBlockVector> list, BattleBlockType type = BattleBlockType.MoveZone)
    {
        if (!GeneralPreJudge()) return;
        HideAllBlocks();

        if (type == BattleBlockType.MoveZone)
        {
            _currentBattlebox.SetAllBlockColor(new Color(1, 1, 1, 0.6f));
        }
        else if (type == BattleBlockType.AttackZone)
        {
            _currentBattlebox.SetAllBlockColor(new Color(1, 0, 0, 0.6f));
        }

        foreach (var vector in list)
        {
            var block = _currentBattlebox.GetBlockData(vector.X, vector.Y);
            if (block != null && block.BoxBlock.IsValid) block.Show();
        }
    }

    public void HideAllBlocks()
    {
        if (!GeneralPreJudge()) return;

        _currentBattlebox.HideAllBlocks();
    }

    public void ShowAllBlocks()
    {
        if (!GeneralPreJudge()) return;

        _currentBattlebox.ShowAllValidBlocks();
    }

    public void ShowMoveZone(Vector3 center, int range = -1)
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
