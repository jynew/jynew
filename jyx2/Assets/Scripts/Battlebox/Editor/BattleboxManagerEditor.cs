using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(BattleboxManager))]
public class BattleboxManagerEditor : Editor
{
    private SerializedObject obj; //序列化
    private BattleboxManager myScript;
    private bool IsSceneEditing = false;

    void OnEnable()
    {
        obj = new SerializedObject(target);
    }

    public override void OnInspectorGUI()
    {
        this.serializedObject.Update();
        myScript = (BattleboxManager)target;

        DrawDefaultInspector();


//        EditorGUILayout.Space();
//        EditorGUILayout.Space();
//        if (GUILayout.Button("从文件载入格子数据"))
//        {
//            myScript.InitFromFile();
//        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        if (GUILayout.Button(IsSceneEditing ? $"结束编辑（G）" : "开始编辑（G）"))
        {
            ChangeEditStatus(!IsSceneEditing);
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        if (GUILayout.Button("初始化格子数据（慎用）"))
        {
            myScript.CreateDataset();
        }
    }

    private void ChangeEditStatus(bool value)
    {
        IsSceneEditing = value;
        Debug.Log(IsSceneEditing ? $"开始编辑" : "结束编辑");
        if (IsSceneEditing)
        {
            myScript.Init();
            myScript.DrawAllBlocks(true);
        }
        else
        {
            myScript.SaveToData();
            myScript.ClearAllBlocks();
        } 
    }

    private void ChangeHideStatus(bool value)
    {
        m_Hide = value;
        if (m_Hide)
        {
            myScript.ShowAllValidBlocks();
        }
        else
        {
            myScript.ShowAllBlocks();
        }
    }
    
    private bool m_Hide = false;
    void OnSceneGUI()
    {
        if (Event.current.type == EventType.KeyDown)
        {
            if (Event.current.keyCode == KeyCode.G)
            {
                ChangeEditStatus(!IsSceneEditing);
            }
        }

        //编辑模式下，按住ctrl，点鼠标右键，可以显示/隐藏无效格子
        if (Event.current.type == EventType.MouseDown && IsSceneEditing && Event.current.button == 1
            && Event.current.control)
        {
            ChangeHideStatus(!m_Hide);
        }

        //编辑模式下，按住ctrl，点鼠标左键，可以编辑格子
        if (Event.current.type == EventType.MouseDown && IsSceneEditing && Event.current.button == 0
            && Event.current.control)
        {
            if (m_Hide)
            {
                ChangeHideStatus(false);
            }
            EditBlocks();
        }
    }
    
    private void EditBlocks()
    {
        //            var sceneCamera = SceneView.lastActiveSceneView.camera;
        //            var pixelPos = Event.current.mousePosition;
        //            var v3 = new Vector3(pixelPos.x, pixelPos.y, 0.0f);
        //            var ray = sceneCamera.ScreenPointToRay(v3);
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10000, 1 << LayerMask.NameToLayer("Ground")))
        {
            if (myScript.m_Dataset == null)
                myScript.CreateDataset();

            Debug.Log($"选中了坐标点：{hit.point.x}:{hit.point.z}");

            var pos = myScript.m_Dataset.GetXYIndex(hit.point.x, hit.point.z);
            var xindex = (int) pos.X;
            var yindex = (int) pos.Y;
            if (myScript.Exist(xindex, yindex))
            {
                Debug.Log($"编辑格子状态，{xindex}:{yindex}");
                myScript.ChangeValid(xindex, yindex);
                myScript.SaveToData();
            }
            else 
            {
                Debug.Log($"没有选中格子，{pos.X}:{pos.Y}");
            }
        }
    }
}
