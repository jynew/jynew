using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 导出场景（包括GameObject和Terrian）到.obj文件，支持自定义裁剪区域导出和自动裁剪导出
/// 
/// author by monitor1394@gmail.com
/// 
/// </summary>
public class ExportScene : EditorWindow
{
    private const string CUT_LB_OBJ_PATH = "export/bound_lb";
    private const string CUT_RT_OBJ_PATH = "export/bound_rt";

    private static float autoCutMinX = 1000;
    private static float autoCutMaxX = 0;
    private static float autoCutMinY = 1000;
    private static float autoCutMaxY = 0;

    private static float cutMinX = 0;
    private static float cutMaxX = 0;
    private static float cutMinY = 0;
    private static float cutMaxY = 0;

    private static long startTime = 0;
    private static int totalCount = 0;
    private static int count = 0;
    private static int counter = 0;
    private static int progressUpdateInterval = 10000;

    [MenuItem("ExportScene/ExportSceneToObj")]
    [MenuItem("GameObject/ExportScene/ExportSceneToObj")]
    public static void Export()
    {
        ExportSceneToObj(false);
    }

    [MenuItem("ExportScene/ExportSceneToObj(AutoCut)")]
    [MenuItem("GameObject/ExportScene/ExportSceneToObj(AutoCut)")]
    public static void ExportAutoCut()
    {
        ExportSceneToObj(true);
    }

    [MenuItem("ExportScene/ExportSelectedObj")]
    [MenuItem("GameObject/ExportScene/ExportSelectedObj", priority = 44)]
    public static void ExportObj()
    {
        GameObject[] selectObjs = Selection.gameObjects;
        if (selectObjs == null)
        {
            Debug.LogWarning("Select a GameObject");
            return;
        }

        string path = GetSavePath(false, selectObjs[0]);
        if (string.IsNullOrEmpty(path)) return;
        List<MeshFilter> meshs = new List<MeshFilter>();
        List<SkinnedMeshRenderer> skinMeshs = new List<SkinnedMeshRenderer>();
        for (int i = 0; i < selectObjs.Length; i++)
        {
            meshs.AddRange(selectObjs[i].GetComponentsInChildren<MeshFilter>());
            skinMeshs.AddRange(selectObjs[i].GetComponentsInChildren<SkinnedMeshRenderer>());
        }
        ExportSceneToObj(path, null, meshs.ToArray(), skinMeshs.ToArray(), false, false);
    }

    public static void ExportSceneToObj(bool autoCut)
    {
        string path = GetSavePath(autoCut, null);
        if (string.IsNullOrEmpty(path)) return;
        Terrain terrain = UnityEngine.Object.FindObjectOfType<Terrain>();
        MeshFilter[] mfs = UnityEngine.Object.FindObjectsOfType<MeshFilter>();
        SkinnedMeshRenderer[] smrs = UnityEngine.Object.FindObjectsOfType<SkinnedMeshRenderer>();
        ExportSceneToObj(path, terrain, mfs, smrs, autoCut, true);
    }

    public static void ExportSceneToObj(string path, Terrain terrain, MeshFilter[] mfs,
        SkinnedMeshRenderer[] smrs, bool autoCut, bool needCheckRect)
    {
        int vertexOffset = 0;
        string title = "export GameObject to .obj ...";
        StreamWriter writer = new StreamWriter(path);

        startTime = GetMsTime();
        UpdateCutRect(autoCut);
        counter = count = 0;
        progressUpdateInterval = 5;
        totalCount = (mfs.Length + smrs.Length) / progressUpdateInterval;
        foreach (var mf in mfs)
        {
            UpdateProgress(title);
            if (mf.GetComponent<Renderer>() != null && 
                (!needCheckRect || (needCheckRect && IsInCutRect(mf.gameObject))))
            {
                ExportMeshToObj(mf.gameObject, mf.sharedMesh, ref writer, ref vertexOffset);
            }
        }
        foreach (var smr in smrs)
        {
            UpdateProgress(title);
            if (!needCheckRect || (needCheckRect && IsInCutRect(smr.gameObject)))
            {
                ExportMeshToObj(smr.gameObject, smr.sharedMesh, ref writer, ref vertexOffset);
            }
        }
        if (terrain)
        {
            ExportTerrianToObj(terrain.terrainData, terrain.GetPosition(),
                ref writer, ref vertexOffset, autoCut);
        }
        writer.Close();
        EditorUtility.ClearProgressBar();

        long endTime = GetMsTime();
        float time = (float)(endTime - startTime) / 1000;
        Debug.Log("Export SUCCESS:" + path);
        Debug.Log("Export Time:" + time + "s");
        OpenDir(path);
    }

    private static void OpenDir(string path)
    {
        DirectoryInfo dir = Directory.GetParent(path);
        int index = path.LastIndexOf("/");
        OpenCmd("explorer.exe", dir.FullName);
    }

    private static void OpenCmd(string cmd,string args)
    {
        System.Diagnostics.Process.Start(cmd,args);
    }

    private static string GetSavePath(bool autoCut, GameObject selectObject)
    {
        string dataPath = Application.dataPath;
        string dir = dataPath.Substring(0, dataPath.LastIndexOf("/"));
        string sceneName = SceneManager.GetActiveScene().name;
        string defaultName = "";
        if (selectObject == null)
        {
            defaultName = (autoCut ? sceneName + "(autoCut)" : sceneName);
        }
        else
        {
            defaultName = (autoCut ? selectObject.name + "(autoCut)" : selectObject.name);
        }
        return EditorUtility.SaveFilePanel("Export .obj file", dir, defaultName, "obj");
    }

    private static long GetMsTime()
    {
        return System.DateTime.Now.Ticks / 10000;
        //return (System.DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
    }

    private static void UpdateCutRect(bool autoCut)
    {
        cutMinX = cutMaxX = cutMinY = cutMaxY = 0;
        if (!autoCut)
        {
            Vector3 lbPos = GetObjPos(CUT_LB_OBJ_PATH);
            Vector3 rtPos = GetObjPos(CUT_RT_OBJ_PATH);
            cutMinX = lbPos.x;
            cutMaxX = rtPos.x;
            cutMinY = lbPos.z;
            cutMaxY = rtPos.z;
        }
    }

    private static void UpdateAutoCutRect(Vector3 v)
    {
        if (v.x < autoCutMinX) autoCutMinX = v.x;
        if (v.x > autoCutMaxX) autoCutMaxX = v.x;
        if (v.z < autoCutMinY) autoCutMinY = v.z;
        if (v.z > autoCutMaxY) autoCutMaxY = v.z;
    }

    private static bool IsInCutRect(GameObject obj)
    {
        if (cutMinX == 0 && cutMaxX == 0 && cutMinY == 0 && cutMaxY == 0) return true;
        Vector3 pos = obj.transform.position;
        if (pos.x >= cutMinX && pos.x <= cutMaxX && pos.z >= cutMinY && pos.z <= cutMaxY)
            return true;
        else
            return false;
    }

    private static void ExportMeshToObj(GameObject obj, Mesh mesh, ref StreamWriter writer, ref int vertexOffset)
    {
        Quaternion r = obj.transform.localRotation;
        StringBuilder sb = new StringBuilder();
        foreach (Vector3 vertice in mesh.vertices)
        {
            Vector3 v = obj.transform.TransformPoint(vertice);
            UpdateAutoCutRect(v);
            sb.AppendFormat("v {0} {1} {2}\n", -v.x, v.y, v.z);
        }
        foreach (Vector3 nn in mesh.normals)
        {
            Vector3 v = r * nn;
            sb.AppendFormat("vn {0} {1} {2}\n", -v.x, -v.y, v.z);
        }
        foreach (Vector3 v in mesh.uv)
        {
            sb.AppendFormat("vt {0} {1}\n", v.x, v.y);
        }
        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            int[] triangles = mesh.GetTriangles(i);
            for (int j = 0; j < triangles.Length; j += 3)
            {
                sb.AppendFormat("f {1} {0} {2}\n",
                    triangles[j] + 1 + vertexOffset,
                    triangles[j + 1] + 1 + vertexOffset,
                    triangles[j + 2] + 1 + vertexOffset);
            }
        }
        vertexOffset += mesh.vertices.Length;
        writer.Write(sb.ToString());
    }

    private static void ExportTerrianToObj(TerrainData terrain, Vector3 terrainPos,
        ref StreamWriter writer, ref int vertexOffset, bool autoCut)
    {
        int tw = terrain.heightmapWidth;
        int th = terrain.heightmapHeight;

        Vector3 meshScale = terrain.size;
        meshScale = new Vector3(meshScale.x / (tw - 1), meshScale.y, meshScale.z / (th - 1));
        Vector2 uvScale = new Vector2(1.0f / (tw - 1), 1.0f / (th - 1));

        Vector2 terrainBoundLB, terrainBoundRT;
        if (autoCut)
        {
            terrainBoundLB = GetTerrainBoundPos(new Vector3(autoCutMinX, 0, autoCutMinY), terrain, terrainPos);
            terrainBoundRT = GetTerrainBoundPos(new Vector3(autoCutMaxX, 0, autoCutMaxY), terrain, terrainPos);
        }
        else
        {
            terrainBoundLB = GetTerrainBoundPos(CUT_LB_OBJ_PATH, terrain, terrainPos);
            terrainBoundRT = GetTerrainBoundPos(CUT_RT_OBJ_PATH, terrain, terrainPos);
        }

        int bw = (int)(terrainBoundRT.x - terrainBoundLB.x);
        int bh = (int)(terrainBoundRT.y - terrainBoundLB.y);

        int w = bh != 0 && bh < th ? bh : th;
        int h = bw != 0 && bw < tw ? bw : tw;

        int startX = (int)terrainBoundLB.y;
        int startY = (int)terrainBoundLB.x;
        if (startX < 0) startX = 0;
        if (startY < 0) startY = 0;

        Debug.Log(string.Format("Terrian:tw={0},th={1},sw={2},sh={3},startX={4},startY={5}",
            tw, th, bw, bh, startX, startY));

        float[,] tData = terrain.GetHeights(0, 0, tw, th);
        Vector3[] tVertices = new Vector3[w * h];
        Vector2[] tUV = new Vector2[w * h];

        int[] tPolys = new int[(w - 1) * (h - 1) * 6];

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                Vector3 pos = new Vector3(-(startY + y), tData[startX + x, startY + y], (startX + x));
                tVertices[y * w + x] = Vector3.Scale(meshScale, pos) + terrainPos;
                tUV[y * w + x] = Vector2.Scale(new Vector2(x, y), uvScale);
            }
        }
        int index = 0;
        for (int y = 0; y < h - 1; y++)
        {
            for (int x = 0; x < w - 1; x++)
            {
                tPolys[index++] = (y * w) + x;
                tPolys[index++] = ((y + 1) * w) + x;
                tPolys[index++] = (y * w) + x + 1;
                tPolys[index++] = ((y + 1) * w) + x;
                tPolys[index++] = ((y + 1) * w) + x + 1;
                tPolys[index++] = (y * w) + x + 1;
            }
        }
        count = counter = 0;
        progressUpdateInterval = 10000;
        totalCount = (tVertices.Length + tUV.Length + tPolys.Length / 3) / progressUpdateInterval;
        string title = "export Terrain to .obj ...";
        for (int i = 0; i < tVertices.Length; i++)
        {
            UpdateProgress(title);
            StringBuilder sb = new StringBuilder(22);
            sb.AppendFormat("v {0} {1} {2}\n", tVertices[i].x, tVertices[i].y, tVertices[i].z);
            writer.Write(sb.ToString());
        }
        for (int i = 0; i < tUV.Length; i++)
        {
            UpdateProgress(title);
            StringBuilder sb = new StringBuilder(20);
            sb.AppendFormat("vt {0} {1}\n", tUV[i].x, tUV[i].y);
            writer.Write(sb.ToString());
        }
        for (int i = 0; i < tPolys.Length; i += 3)
        {
            UpdateProgress(title);
            int x = tPolys[i] + 1 + vertexOffset; ;
            int y = tPolys[i + 1] + 1 + vertexOffset;
            int z = tPolys[i + 2] + 1 + vertexOffset;
            StringBuilder sb = new StringBuilder(30);
            sb.AppendFormat("f {0} {1} {2}\n", x, y, z);
            writer.Write(sb.ToString());
        }
        vertexOffset += tVertices.Length;
    }

    private static Vector2 GetTerrainBoundPos(string path, TerrainData terrain, Vector3 terrainPos)
    {
        var go = GameObject.Find(path);
        if (go)
        {
            Vector3 pos = go.transform.position;
            return GetTerrainBoundPos(pos, terrain, terrainPos);
        }
        return Vector2.zero;
    }

    private static Vector2 GetTerrainBoundPos(Vector3 worldPos, TerrainData terrain, Vector3 terrainPos)
    {
        Vector3 tpos = worldPos - terrainPos;
        return new Vector2((int)(tpos.x / terrain.size.x * terrain.heightmapWidth),
            (int)(tpos.z / terrain.size.z * terrain.heightmapHeight));
    }

    private static Vector3 GetObjPos(string path)
    {
        var go = GameObject.Find(path);
        if (go)
        {
            return go.transform.position;
        }
        return Vector3.zero;
    }

    private static void UpdateProgress(string title)
    {
        if (counter++ == progressUpdateInterval)
        {
            counter = 0;
            float process = Mathf.InverseLerp(0, totalCount, ++count);
            long currTime = GetMsTime();
            float sec = ((float)(currTime - startTime)) / 1000;
            string text = string.Format("{0}/{1}({2:f2} sec.)", count, totalCount, sec);
            EditorUtility.DisplayProgressBar(title, text, process);
        }
    }
}
