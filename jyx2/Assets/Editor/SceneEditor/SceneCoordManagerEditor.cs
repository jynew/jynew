using System;
using System.Collections;
using System.Collections.Generic;
using ch.sycoforge.Decal;
using ServiceStack;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(SceneCoordManager))]
public class SceneCoordManagerEditor : Editor
{
    private SerializedObject obj; //序列化
    private SceneCoordManager myScript;
    private bool IsSceneEditing = false;

    void OnEnable()
    {
        obj = new SerializedObject(target);
        //m_IsOldXiake = obj.FindProperty("m_IsOldXiake");

    }

    public override void OnInspectorGUI()
    {
        this.serializedObject.Update();
        myScript = (SceneCoordManager) target;

        DrawDefaultInspector();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (GUILayout.Button(IsSceneEditing ? $"结束编辑" : "开始编辑"))
        {
            IsSceneEditing = !IsSceneEditing;
            Debug.Log(IsSceneEditing ? $"开始编辑" : "结束编辑");
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        if (GUILayout.Button("绘制当前屏幕"))
        {
            myScript.TestDrawBlocks();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        if (GUILayout.Button("初始化格子数据（慎用）"))
        {
            myScript.RecreateCoordDataSet();
        }
    }

    private int m_MouseRightCounter = 0;
    void OnSceneGUI()
    {
        if (Event.current.type == EventType.KeyDown)
        {
            if (Event.current.keyCode == KeyCode.G)
            {
                IsSceneEditing = !IsSceneEditing;
                Debug.Log(IsSceneEditing ? $"开始编辑，实时保存" : "结束编辑");
            }
        }

        if (Event.current.type == EventType.MouseDown && IsSceneEditing)
        {
            //Debug.Log($"the keycode is {Event.current.button}");
            if (Event.current.button == 1)
            {
                if (m_MouseRightCounter % 3 == 0)
                    DrawBlocks();
                else if (m_MouseRightCounter % 3 == 1)
                    myScript.UpdateBlocks();
                else
                    myScript.HideUnvalidBlocks();
                m_MouseRightCounter++;
            }

            if (Event.current.button == 0)
            {
                EditBlocks();
            }

            if (Event.current.button == 2)
            {
                myScript.ClearAllBlocks();
            }
        }
    }
    
    private void DrawBlocks()
    {
        if (myScript.m_CoordDataSet == null)
            myScript.LoadCoordDataSet(null);
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        myScript.DrawBlocks(ray, true);
    }

    private void EditBlocks()
    {
        //            var sceneCamera = SceneView.lastActiveSceneView.camera;
        //            var pixelPos = Event.current.mousePosition;
        //            var v3 = new Vector3(pixelPos.x, pixelPos.y, 0.0f);
        //            var ray = sceneCamera.ScreenPointToRay(v3);
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000, 1 << LayerMask.NameToLayer("Ground")))
        {
            if (myScript.m_CoordDataSet == null)
                myScript.LoadCoordDataSet(null);

            Debug.Log($"选中了坐标点：{hit.point.x}:{hit.point.z}");

            var pos = myScript.m_CoordDataSet.GetXYIndex(hit.point.x, hit.point.z);
            if (myScript.IsBlockShown((int) pos.X, (int) pos.Y))
            {
                Debug.Log($"编辑格子，{pos.X}:{pos.Y}");
                var block = myScript.GetBlockData((int) pos.X, (int) pos.Y);
                var blockValue = myScript.m_CoordDataSet.GetCoordValue((int) pos.X, (int) pos.Y);
                myScript.m_CoordDataSet.SetPoint((int) pos.X, (int) pos.Y, blockValue == 0 ? 1 : 0);

                var ed = block.gameObject.GetComponent<EasyDecal>();
                ed.Baked = !ed.Baked;
                myScript.SaveCoordDataSet();
            }
            else
            {
                Debug.Log($"格子没有显示，{pos.X}:{pos.Y}");
            }
        }
    }
}
