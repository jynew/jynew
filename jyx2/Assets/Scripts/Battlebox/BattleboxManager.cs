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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ch.sycoforge.Decal;

using Jyx2;
using ProtoBuf;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class BattleboxManager : MonoBehaviour
{
    //排除拥挤点半径
    public float m_DetechRadius = 0.8f;
    public float m_SpriteToGroundHeight = 0.01f;
    public Color m_InvalidColor = new Color(1,1,1,0.2f);

    private SpriteRenderer m_BlockPrefab;

    [HideInInspector]
    public BattleboxDataset m_Dataset;

    private Collider[] m_colliders;

    //存储逻辑数据
    private List<BattleBlockData> battleBlocks = new List<BattleBlockData>();
    private GameObject _parent;

    // Use this for initialization
    void Awake()
    {
        Init();
    }

    public void Init()
    {
        InitCollider();

        if (m_Dataset == null)
            InitFromFile();

        if (m_Dataset == null || !CheckSize())
            CreateDataset();
    }

    private void InitCollider()
    {
        m_colliders = GetComponentsInChildren<Collider>();
        foreach (var col in m_colliders)
        {
            var mesh = col.GetComponent<MeshCollider>();
            if (mesh != null) mesh.convex = true;
        }
    }

    private Bounds GetBounds()
    {
        var bound = m_colliders[0].bounds;
        foreach (var mCollider in m_colliders)
        {
            bound.Encapsulate(mCollider.bounds);
        }
        return bound;
    }

    public bool ColliderContain(Vector3 pos)
    {
        foreach (var mCollider in m_colliders)
        {
            var temp = mCollider.ClosestPoint(pos);
            if (Vector3.Distance(pos, temp) < 1e-6) return true;
        }

        return false;
    }

    public void InitFromFile()
    {
        var filePath = GetFilePath();

        Jyx2ResourceHelper.GetBattleboxDataset(filePath, r =>
        {
            m_Dataset = r;
            if(m_Dataset != null)
                Debug.Log($"载入文件结束：{m_Dataset.GetCount()}个格子中，一共有多少格子有效：{m_Dataset.GetValidCount()}");
        });
    }

    public void SaveToFile()
    {
        if (m_Dataset == null) return;

        var filePath = GetFilePath();
        byte[] bs;
        using (var memory = new MemoryStream())
        {
            Serializer.Serialize(memory, m_Dataset);
            bs = memory.ToArray();
        }
        
        Directory.CreateDirectory(ConStr.BattleboxDatasetPath);
        File.WriteAllBytes(filePath, bs);

        Debug.Log($"保存格子数据完成：{m_Dataset.GetCount()}个格子中，一共有多少格子有效：{m_Dataset.GetValidCount()}");
    }

    public bool CheckSize()
    {
        if (m_colliders == null || m_colliders.Length == 0)
        {
            Debug.LogError($"没有找到子碰撞盒，无法初始化战斗盒子");
            return false;
        }
        
        var bound = GetBounds();
        var minx = bound.min.x;
        var miny = bound.min.z;
        var length = bound.size.x;
        var width = bound.size.z;

        if (!Mathf.Approximately(length, m_Dataset.BoxLength) ||
            !Mathf.Approximately(width, m_Dataset.BoxWidth) ||
            !Mathf.Approximately(minx, m_Dataset.MinX) ||
            !Mathf.Approximately(miny, m_Dataset.MinY))
        {
            Debug.LogError($"载入的格子数据和当前box的尺寸不匹配，请重新生成格子数据");
            return false;
        }

        return true;
    }

    public void CreateDataset()
    {
        m_Dataset = null;
        InitCollider();
        if (m_colliders == null || m_colliders.Length == 0)
        {
            Debug.LogError($"没有找到子碰撞盒，无法初始化战斗盒子");
            return;
        }

        var bound = GetBounds();
        var sceneName = SceneManager.GetActiveScene().name;
        var objName = gameObject.name;
        var length = bound.size.x;
        var width = bound.size.z;
        var minx = bound.min.x;
        var miny = bound.min.z;

        m_Dataset = new BattleboxDataset(sceneName, objName, length, width, minx, miny);
        Debug.Log($"重新生成格子，x轴格子数{m_Dataset.CountX}, y轴格子数{m_Dataset.CountY}，理论总格子数目{m_Dataset.GetSizeCount()}");

        var height = bound.size.y;
        var maxHeight = bound.min.y + height;
        for (int i = 0; i < m_Dataset.CountX; i++)
        {
            for (int j = 0; j < m_Dataset.CountY; j++)
            {
                //计算格子的x和z坐标
                var tempPos = m_Dataset.CalcPos(i, j);
                //该格子在box顶层的投影点
                var topProj = new Vector3(tempPos.X, maxHeight, tempPos.Y);

                Vector3 pos, normal;
                var bFlag = JudgeCoord(topProj, height, out pos, out normal);
                if (bFlag)
                {
                    //该格子有效，存入dataset
                    var block = new BattleboxBlock
                    {
                        XIndex = i,
                        YIndex = j,
                        WorldPosX = pos.x,
                        WorldPosY = pos.y,
                        WorldPosZ = pos.z,
                        NormalX = normal.x,
                        NormalY = normal.y,
                        NormalZ = normal.z,
                        IsValid = true,
                    };
                    m_Dataset.Blocks.Add(block);
                }
            }
        }
        Debug.Log($"重新生成格子结束：一共生成了{m_Dataset.GetCount()}个格子");

        if(Application.isEditor) SaveToFile();
    }

    public List<BattleBlockData> GetBattleBlocks()
    {
        return battleBlocks;
    }

    public System.Numerics.Vector2 GetXYIndex(float x, float z)
    {
        return m_Dataset.GetXYIndex(x, z);
    }

    //1.一定要打到Ground
    //2.将交点信息和法线信息拿到
    public bool JudgeCoord(Vector3 top, float height, out Vector3 pos, out Vector3 normal)
    {
        pos = new Vector3(0f, 0f, 0f);
        normal = new Vector3(0f, 0f, 0f);
        var ray = new Ray(top, Vector3.down);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, height, 1 << LayerMask.NameToLayer("Ground")))
        {
            if (!ColliderContain(hitInfo.point)) return false;

            //寻找最近的导航网格边缘，排除过于“拥挤”的点
            NavMeshHit hit;
            if (NavMesh.FindClosestEdge(hitInfo.point, out hit, NavMesh.AllAreas))
            {
                if (hit.distance >= m_DetechRadius)
                {
                    pos = hitInfo.point;
                    normal = hitInfo.normal;
                    return true;
                }
            }
        }
        return false;
    }
    
    private string GetFilePath()
    {
        var scene = SceneManager.GetActiveScene().name;
        var objName = gameObject.name;
        return $"{ConStr.BattleboxDatasetPath}{scene}_{objName}_coord_dataset.bytes";
    }

    public void ShowAllValidBlocks()
    {
        foreach (var block in battleBlocks)
        {
            if(block.BoxBlock.IsValid)
                block.Show();
            else
                block.Hide();
        }
    }

    public void ShowAllBlocks()
    {
        foreach (var block in battleBlocks)
        {
            block.Show();
        }
    }

    public void HideAllBlocks()
    {
        foreach (var block in battleBlocks)
        {
            block.Hide();
        }
    }

    public BattleBlockData GetBlockData(int xindex, int yindex)
    {
        return battleBlocks.FirstOrDefault(x => x.BattlePos.X == xindex && x.BattlePos.Y == yindex);
    }

    public bool Exist(int xindex, int yindex)
    {
        return m_Dataset.Exist(xindex, yindex);
    }

    public bool IsValid(int xindex, int yindex)
    {
        return m_Dataset.IsValid(xindex, yindex);
    }

    //清除所有格子（所有格子的parent为当前box）
    public void ClearAllBlocks()
    {
        foreach (var block in battleBlocks)
        {
            DestroyImmediate(block.gameObject);
        }
        battleBlocks.Clear();

        var parent = FindOrCreateBlocksParent();
        if (parent == null) return;
        DestroyImmediate(parent);
    }

    private GameObject FindOrCreateBlocksParent()
    {
        if (_parent == null)
            _parent = new GameObject("block_parent");
        return _parent;
    }

    //绘制战斗格子，默认不显示
    void DrawBattleBlock(Vector3 pos, Color c, int x, int y, Vector3 normal, BattleboxBlock boxBlock)
    {
        var parent = FindOrCreateBlocksParent();
        
        var block = Resources.Load<GameObject>("BattleboxBlock");
        var obj = EasyDecal.Project(block, pos, Quaternion.identity);
        obj.Quality = 2;
        obj.Distance = 0.05f;
        obj.transform.SetParent(parent.transform, false);

        var bPos = new BattleBlockVector(x, y);
        var bbd = new BattleBlockData();
        bbd.BattlePos = bPos;
        bbd.WorldPos = pos;
        bbd.gameObject = obj.gameObject;
        bbd.BoxBlock = boxBlock;
        battleBlocks.Add(bbd);
    }

    public void ChangeValid(int xindex, int yindex)
    {
        var block = GetBlockData(xindex, yindex);
        var data = block.BoxBlock;
        data.IsValid = !data.IsValid;
        //var sp = block.gameObject.GetComponent<SpriteRenderer>();
        //sp.color = block.BoxBlock.IsValid ? Color.white : m_InvalidColor;
    }
    
    public void DrawAllBlocks(bool showAll = false)
    {
        for (int i = 0; i < m_Dataset.CountX; i++)
        {
            for (int j = 0; j < m_Dataset.CountY; j++)
            {
                if (!m_Dataset.Exist(i, j)) continue;
                var data = m_Dataset.GetBLock(i, j);
                var pos = new Vector3(data.WorldPosX, data.WorldPosY, data.WorldPosZ);
                var normal = new Vector3(data.NormalX, data.NormalY, data.NormalZ);
                DrawBattleBlock(pos, data.IsValid ? Color.white : m_InvalidColor, i, j, normal, data);
            }
        }

        if(showAll) ShowAllBlocks();
    }

    public void DrawAreaBlocks(Vector3 center, int range, bool show = false)
    {
        var xy = m_Dataset.GetXYIndex(center.x, center.z);
        var centerX = (int) xy.X;
        var centerY = (int) xy.Y;
        _battleBoxBlockList.Clear();
        CreateBlockMap(centerX, centerY, centerX, centerY, range);
        foreach(var b in _battleBoxBlockList)
        {
            if (!m_Dataset.Exist(b.X, b.Y)) continue;
            var data = m_Dataset.GetBLock(b.X, b.Y);
            if (!data.IsValid) continue;
            var pos = new Vector3(data.WorldPosX, data.WorldPosY, data.WorldPosZ);
            var normal = new Vector3(data.NormalX, data.NormalY, data.NormalZ);
            DrawBattleBlock(pos, data.IsValid ? Color.white : m_InvalidColor, b.X, b.Y, normal, data);
        }
        //for (int i = centerX - range; i < centerX + range; i++)
        //{
        //    if (i < 0 || i >= m_Dataset.CountX) continue;
        //    for (int j = centerY - range; j < centerY + range; j++)
        //    {
        //        if (j < 0 || j >= m_Dataset.CountY) continue;
        //        if (!m_Dataset.Exist(i, j)) continue;
        //        var data = m_Dataset.GetBLock(i, j);
        //        if (!data.IsValid) continue;
        //        var pos = new Vector3(data.WorldPosX, data.WorldPosY, data.WorldPosZ);
        //        var normal = new Vector3(data.NormalX, data.NormalY, data.NormalZ);
        //        DrawBattleBlock(pos, data.IsValid ? Color.white : m_InvalidColor, i, j, normal, data);
        //    }
        //}

        if (show) ShowAllValidBlocks();
    }

    public void ShowBlocksCenterDist(Vector3 center, int range)
    {
        var xy = m_Dataset.GetXYIndex(center.x, center.z);
        var centerX = (int) xy.X;
        var centerY = (int) xy.Y;

        for (int i = centerX - range; i < centerX + range; i++)
        {
            if (i < 0 || i >= m_Dataset.CountX) continue;
            for (int j = centerY - range; j < centerY + range; j++)
            {
                if (j < 0 || j >= m_Dataset.CountY) continue;
                if (!Exist(i, j)) continue;
                var data = GetBlockData(i, j);
                if (!data.BoxBlock.IsValid) continue;
                data.Show();
            }
        }
    }

    public void SetAllBlockColor(Color color)
    {
        foreach (var block in battleBlocks)
        {
            block.gameObject.GetComponent<EasyDecal>().DecalMaterial.SetColor("_TintColor", color);
        }
    }

    public void SetBlockColor(int xindex, int yindex, Color color)
    {
        //if (!Exist(xindex, yindex)) return;
        //var block = GetBlockData(xindex, yindex);
        //block.gameObject.GetComponent<SpriteRenderer>().color = color;
    }

    private List<BattleBlockVector> _battleBoxBlockList = new List<BattleBlockVector>();

    public void CreateBlockMap(int x, int y, int ox, int oy, int range)
    {
        AddToBlockMap(x, y, ox, oy, range);
    }

    public void AddToBlockMap(int x, int y, int ox, int oy, int range)
    {
        if (_battleBoxBlockList.Exists(b => b.X == x && b.Y == y)) return;
        if (Math.Abs(x - ox) >= range || Math.Abs(y - oy) >= range) return;

        if (m_Dataset.Blocks.Exists(b => b.XIndex == x && b.YIndex == y))
        {
            _battleBoxBlockList.Add(new BattleBlockVector(x, y));
            foreach (var b in GetNearBlocks(x, y))
            {
                AddToBlockMap(b.X, b.Y, ox, oy, range);
            }
        }
        else if (x == ox && y == oy)
        {
            foreach (var b in GetNearBlocks(x, y))
            {
                AddToBlockMap(b.X, b.Y, ox, oy, range);
            }
        }
    }

    //y是奇数
    //static readonly int[] dx_odd = new int[] { 1, 1, 1, 0, -1, 0 };

    ////y是偶数
    //static readonly int[] dx_even = new int[] { 0, 1, 0, -1, -1, -1 };
    //static readonly int[] dy = new int[] { 1, 0, -1, -1, 0, 1 };


    static readonly int[] dx = new int[] { 1, 0, -1, 0 };
    static readonly int[] dy = new int[] { 0, 1, 0, -1 };

    public IEnumerable<BattleBlockVector> GetNearBlocks(int x, int y)
    {
        //int[] dx = y % 2 == 0 ? dx_even : dx_odd;
        //for (int i = 0; i < 6; ++i)
        //{
        //    int xx = x + dx[i];
        //    int yy = y + dy[i];
        //    if (xx < 0) continue;
        //    if (yy < 0) continue;

        //    var rst = new BattleBlockVector(xx, yy);
        //    yield return rst;
        //}
        for (int i = 0; i < 4; ++i)
        {
            int xx = x + dx[i];
            int yy = y + dy[i];
            if (xx < 0) continue;
            if (yy < 0) continue;
            var rst = new BattleBlockVector(xx, yy);
            yield return rst;
        }
    }
}
