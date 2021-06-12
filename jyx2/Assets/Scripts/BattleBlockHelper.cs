using System;
using System.Collections;
using System.Collections.Generic;
using ch.sycoforge.Decal;
using Jyx2;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BattleBlockHelper : MonoBehaviour {
    

    //绘制区域
    public int m_MovezoneDrawRange = 8;
    
    public GameObject m_DecalPrefab;

    [HideInInspector]
    public SceneCoordDataSet m_CoordDataSet;

    //存储逻辑数据
    private List<BattleBlockData> battleBlocks = new List<BattleBlockData>();

    private GameObject _parent;


    void Start()
    {
        var sceneName = SceneManager.GetActiveScene().name;
        SceneCoordDataSet.CreateBySceneName(sceneName, r =>
        {
            m_CoordDataSet = r;
            if (m_CoordDataSet == null)
            {
                Debug.LogWarning($"没有初始化格子信息，请在编辑器模式下初始化场景的格子信息（Level下的LevelEditor");
            }
        });
    }

    public void OnTestControl()
    {
        if (Input.GetKey("g"))
        {
            DrawMovezones(GameRuntimeData.Instance.Player.View.transform.position);
        }
        else if (Input.GetKey("h"))
        {
            ClearAllBlocks();
        }
    }
    
    //测试绘制主角周围移动区域
    public void InitMovezones(int range = -1)
    {
        SetBlockColor(Color.white);
        DrawMovezones(GameRuntimeData.Instance.Player.View.transform.position, range);
        StartCoroutine(SetDecalsHidenFrames(2));
    }

    //获取当前战斗格子，待改成实际每个战场预先计算好的
    public List<BattleBlockData> GetBattleBlocks()
    {
        return battleBlocks;
    }

    //获取坐标对应的格子
    public BattleBlockData GetLocationBattleBlock(Vector3 pos)
    {
        var tempXY = m_CoordDataSet.GetXYIndex(pos.x, pos.z);
        var centerX = (int)tempXY.X;
        var centerY = (int)tempXY.Y;
        return GetBlockData(centerX, centerY);
    }

    //判断格子是否存在
    public bool IsBlockExists(int xindex, int yindex)
    {
        return battleBlocks.Exists(x => x.BattlePos.X == xindex && x.BattlePos.Y == yindex);
    }

    public BattleBlockData GetBlockData(int xindex, int yindex)
    {
        return battleBlocks.Find(x => x.BattlePos.X == xindex && x.BattlePos.Y == yindex);
    }
    
    public void ClearAllBlocks()
    {
        foreach (var block in battleBlocks)
        {
            DestroyImmediate(block.gameObject);
        }

        battleBlocks.Clear();

        var parent = GameObject.Find("decal parent");
        if (parent == null) return;

        DestroyImmediate(parent);
    }
    #region 实际战斗相关

    public void ShowBlocks(List<BattleBlockVector> list, BattleBlockType type = BattleBlockType.MoveZone)
    {
        HideAllBlocks();

        if (type == BattleBlockType.MoveZone)
        {
            SetBlockColor(Color.white);
        }
        else if (type == BattleBlockType.AttackZone)
        {
            SetBlockColor(Color.red);
        }

        foreach (var vector in list)
        {
            var block = GetBlockData(vector.X, vector.Y);
            if(block != null) block.Show();
        }
    }

    public void HideAllBlocks()
    {
        //SetBlockColor(Color.white);
        if (battleBlocks != null && battleBlocks.Count > 0)
        {
            foreach (var block in battleBlocks)
            {
                block.Hide();
            }
        }
    }

    #endregion


    private void SetBlockColor(Color color)
    {
        color.a = 0.25f;
        m_DecalPrefab.GetComponent<EasyDecal>().DecalMaterial.SetColor("_TintColor", color);
    }

    private IEnumerator SetDecalsHidenFrames(int num)
    {
        for (int i = 0; i < num; i++)
        {
            yield return 0;
        }

        HideAllBlocks();
    }

    private void DrawMovezones(Vector3 center, int range = -1)
    {
        if (range == -1)
        {
            range = m_MovezoneDrawRange;
        }
        ClearAllBlocks();
        var tempXY = m_CoordDataSet.GetXYIndex(center.x, center.z);
        var centerX = (int)tempXY.X;
        var centerY = (int)tempXY.Y;

        for (int i = centerX - range; i < centerX + range; i++)
        {
            if (i < 0 || i >= m_CoordDataSet.CountX) continue;
            for (int j = centerY - range; j < centerY + range; j++)
            {
                if (j < 0 || j >= m_CoordDataSet.CountY) continue;

                var tempPos = m_CoordDataSet.CalcPos(i, j);
                var pos = new Vector3(tempPos.X, 10.0f, tempPos.Y);
                var ray = new Ray(pos, Vector3.down);
                RaycastHit hitInfo;
                //用射线穿透地表，求地上的那个点
                if (Physics.Raycast(ray, out hitInfo, 100f, 1 << LayerMask.NameToLayer("Ground")))
                {
                    //无效格子不显示
                    if (m_CoordDataSet.GetCoordValue(i, j) == 1)
                        DrawBattleBlock(hitInfo.point, Color.green, i, j);
                }

            }
        }
    }

    private GameObject FindOrCreateBlocksParent()
    {
        if (_parent == null)
            _parent = new GameObject("decal parent");
        return _parent;
    }

    private void DrawBattleBlock(Vector3 pos, Color c, int x, int y)
    {
        var parent = FindOrCreateBlocksParent();

        var obj = EasyDecal.Project(m_DecalPrefab, pos, Quaternion.identity);
        obj.transform.SetParent(parent.transform);

        //obj.LateBake();
        //StartCoroutine(SetDecalsHidenFrames(obj));

        var bPos = new BattleBlockVector(x, y);
        var bbd = new BattleBlockData();
        bbd.BattlePos = bPos;
        bbd.WorldPos = pos;
        bbd.gameObject = obj.gameObject;
        battleBlocks.Add(bbd);
    }
}
