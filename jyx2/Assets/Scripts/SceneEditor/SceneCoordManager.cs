#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using ch.sycoforge.Decal;
using Jyx2;
using ServiceStack;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class SceneCoordManager : MonoBehaviour {

    public List<Collider> m_SceneCollider;

    //排除拥挤点半径
    public float m_DetechRadius = 0.8f;
    public int m_BlockCountX = 7;
    public int m_BlockCountY = 7;
    public float m_RedrawBlockDelta = 5.0f;

    public GameObject m_DecalPrefab;

    private bool m_IsInitiated = false;
    private float m_SceneLength;
    private float m_SceneWidth;
    private Bounds m_TerrainBounds;

    // Use this for initialization
    void Start ()
    {
        InitSceneSize();
    }
	
	// Update is called once per frame
	void Update () {

	}
    
    //存储逻辑数据
    List<BattleBlockData> battleBlocks = new List<BattleBlockData>();

    [HideInInspector]
    public SceneCoordDataSet m_CoordDataSet;

    public void UpdateBlocks()
    {
        foreach (var block in battleBlocks)
        {
            var isBake = m_CoordDataSet.GetCoordValue(block.BattlePos.X, block.BattlePos.Y) == 1;
            block.gameObject.GetComponent<EasyDecal>().Baked = isBake;
        }
    }

    public void HideUnvalidBlocks()
    {
        foreach (var block in battleBlocks)
        {
            var isShow = m_CoordDataSet.GetCoordValue(block.BattlePos.X, block.BattlePos.Y) == 1;
            if (!isShow)
                block.gameObject.SetActive(false);
        }
    }

    public void DrawBlocks(Ray ray, bool isEditing = false)
    {
        RaycastHit hitInfo;
        if (!Physics.Raycast(ray, out hitInfo, 1000, 1 << LayerMask.NameToLayer("Ground")))
        {
            return;
        }

        Debug.Log($"DrawBlocks 中心点 坐标 {hitInfo.point.x} {hitInfo.point.y} {hitInfo.point.z}");

        ClearAllBlocks();

        var tempXY = m_CoordDataSet.GetXYIndex(hitInfo.point.x, hitInfo.point.z);

        var centerX = (int)tempXY.X;
        var centerY = (int)tempXY.Y;

        for (int i = centerX - m_BlockCountX; i < centerX + m_BlockCountX; i++)
        {
            if (i < 0 || i >= m_CoordDataSet.CountX) continue;
            for (int j = centerY - m_BlockCountY; j < centerY + m_BlockCountY; j++)
            {
                if (j < 0 || j >= m_CoordDataSet.CountY) continue;
                
                var tempPos = m_CoordDataSet.CalcPos(i, j);
                var pos = new Vector3(tempPos.X, hitInfo.point.y, tempPos.Y);

                if (isEditing)
                {
                    //编辑模式下，所有格子都显示
                    DrawBattleBlock(pos, Color.green, i, j);
                }
                else
                {
                    //非编辑模式下，无效格子不显示
                    if(m_CoordDataSet.GetCoordValue(i, j) == 1)
                        DrawBattleBlock(pos, Color.green, i, j);
                }
            }
        }
    }

    public void TestDrawBlocks()
    {
        LoadCoordDataSet(() =>
        {
            var sceneCamera = SceneView.lastActiveSceneView.camera;
            var pixelPos = new Vector2(sceneCamera.scaledPixelWidth * 0.5f, sceneCamera.scaledPixelHeight * 0.5f);

            var v3 = new Vector3(pixelPos.x, pixelPos.y, 0.0f);
            var ray = sceneCamera.ScreenPointToRay(v3);
            DrawBlocks(ray);
        });
    }

    //根据导航判断格子是否有效
    //有效则返回true
    public bool JudgeCoord(Vector3 pos, float height)
    {
        var ray = new Ray(pos, Vector3.down);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, height, 1 << LayerMask.NameToLayer("Ground")))
        {
            var trans = hitInfo.transform;
            var obj = trans.gameObject;

            //寻找最近的导航网格边缘，排除过于“拥挤”的点
            NavMeshHit hit;
            if (NavMesh.FindClosestEdge(hitInfo.point, out hit, NavMesh.AllAreas))
            {
                if (hit.distance >= m_DetechRadius)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void LoadCoordDataSet(Action callback)
    {
        //if (m_CoordDataSet != null) return;
        InitSceneSize();
        var sceneName = SceneManager.GetActiveScene().name;
        SceneCoordDataSet.CreateBySceneName(sceneName, (r) =>
        {
            m_CoordDataSet = r;
            if (m_CoordDataSet == null)
            {
                RecreateCoordDataSet();
            }
            Debug.Log($"载入结束：{m_CoordDataSet.GetCount()}个格子中，一共有多少格子有效：{m_CoordDataSet.GetValueSum()}");

            if (!Mathf.Approximately(m_SceneLength, m_CoordDataSet.TerrainLength) ||
                !Mathf.Approximately(m_SceneWidth, m_CoordDataSet.TerrainWidth) ||
                !Mathf.Approximately(m_TerrainBounds.min.x, m_CoordDataSet.MinX) ||
                !Mathf.Approximately(m_TerrainBounds.min.z, m_CoordDataSet.MinY))
            {
                Debug.LogError($"载入的格子数据和当前scene的terrain尺寸不匹配，请重新生成格子数据");
            }

            callback?.Invoke();
        });
    }

    public void SaveCoordDataSet()
    {
        m_CoordDataSet.SaveToFile();
        Debug.Log($"保存格子数据完成：{m_CoordDataSet.GetCount()}个格子中，一共有多少格子有效：{m_CoordDataSet.GetValueSum()}");
    }
    
    public void RecreateCoordDataSet()
    {
        InitSceneSize();
        
        var sceneName = SceneManager.GetActiveScene().name;

        m_CoordDataSet = new SceneCoordDataSet(sceneName, m_SceneLength, m_SceneWidth, m_TerrainBounds.min.x,
            m_TerrainBounds.min.z);

        Debug.Log($"重新生成，The count x is {m_CoordDataSet.CountX}, the count y is {m_CoordDataSet.CountY}");

        for (int i = 0; i < m_CoordDataSet.CountX; i++)
        {
            for (int j = 0; j < m_CoordDataSet.CountY; j++)
            {
                var tempPos = m_CoordDataSet.CalcPos(i, j);
                var pos = new Vector3(tempPos.X, 900f, tempPos.Y);
                var bFlag = JudgeCoord(pos, 1000f);
                //格子的value==1表示可以画格子
                var value = bFlag ? 1 : 0;
                m_CoordDataSet.SetPoint(i, j, value);
            }
        }
        Debug.Log($"重新生成结束，{m_CoordDataSet.GetCount()}个格子中，一共有多少格子有效：{m_CoordDataSet.GetValueSum()}");
        SaveCoordDataSet();
    }

    public bool IsBlockShown(int xindex, int yindex)
    {
        return battleBlocks.Exists(x => x.BattlePos.X == xindex && x.BattlePos.Y == yindex);
    }

    public BattleBlockData GetBlockData(int xindex, int yindex)
    {
        return battleBlocks.Find(x => x.BattlePos.X == xindex && x.BattlePos.Y == yindex);
    }

    //绘制战斗格子
    void DrawBattleBlock(Vector3 pos, Color c, int x, int y)
    {
        //Debug.DrawRay(pos, Vector3.up, c, 1f);

        var parent = GameObject.Find("decal parent");
        if(parent == null)
            parent = new GameObject("decal parent");

        var obj = EasyDecal.Project(m_DecalPrefab, pos, Quaternion.identity);
        obj.transform.SetParent(parent.transform);
        
        obj.LateBake();

        var bPos = new BattleBlockVector(x, y);
        var bbd = new BattleBlockData();
        bbd.BattlePos = bPos;
        bbd.WorldPos = pos;
        bbd.gameObject = obj.gameObject;
        battleBlocks.Add(bbd);
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

    //初始化整个场景的长宽高
    public void InitSceneSize()
    {
        //if (m_IsInitiated) return;

        m_IsInitiated = true;
        if (m_SceneCollider != null)
        {
            Bounds bounds = new Bounds();
            foreach(var collider in m_SceneCollider)
            {
                if(collider != null && collider.bounds != null)
                {
                    bounds.Encapsulate(collider.bounds);
                }
            }

            m_TerrainBounds = bounds;

            var size = m_TerrainBounds.size;
            m_SceneLength = size.x;
            m_SceneWidth = size.z;
            Debug.Log($"The Terrain size init success: {size.x} {size.z} {size.y}");
        }
        else
        {
            Debug.LogWarning($"The terrain is null!");
        }
    }
}

#endif