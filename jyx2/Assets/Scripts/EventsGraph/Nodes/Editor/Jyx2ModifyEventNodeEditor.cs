using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using DG.DemiLib;
using HSFrameWork.ConfigTable;
using Jyx2;
using Jyx2Editor;
using MK.Toon;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using XNodeEditor;


[CustomNodeEditor(typeof(Jyx2ModifyEventNode))]
public class Jyx2ModifyEventNodeEditor : NodeEditor
{
    private Jyx2ModifyEventNode myNode;

    private bool isDefaultScene;
    private bool isDefaultEvt;


    private readonly string[] evtModifyTypes = new string[3] {"保留", "清空", "指定ID"};

    public override void OnCreate()
    {
        base.OnCreate();
        if (myNode == null) myNode = target as Jyx2ModifyEventNode;

        isDefaultScene = serializedObject.FindProperty("SceneId").intValue == -2;
        isDefaultEvt = serializedObject.FindProperty("EventId").intValue == -2;
        

    }

    public override void OnBodyGUI() {
        
        if (myNode == null) myNode = target as Jyx2ModifyEventNode;
        
        // Update serialized object's representation
        serializedObject.Update();
        
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("prev"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("next"));

        DrawField("本场景", -2, "SceneId", ref isDefaultScene);
        if (!isDefaultScene)
        {
            int sceneId = serializedObject.FindProperty("SceneId").intValue;
            Jyx2NodeEditorHelperQuickButtons.NavigateToSceneButton(sceneId);
        }
        DrawField("本触发器", -2, "EventId", ref isDefaultEvt);
        
        //有指定具体场景具体触发器的情况下，可以直接导航到该GameObject
        if (!isDefaultScene && !isDefaultEvt)
        {
            int sceneId = serializedObject.FindProperty("SceneId").intValue;
            int gameEventId = serializedObject.FindProperty("EventId").intValue;
            Jyx2NodeEditorHelperQuickButtons.NavigateToGameEventObjButton(sceneId, gameEventId);
        }

        
        DrawDropdownField("交互事件", nameof(myNode.InteractiveEventId));
        DrawDropdownField("道具事件", nameof(myNode.UseItemEventId));
        DrawDropdownField("进入事件", nameof(myNode.EnterEventId));

        // Apply property modifications
        serializedObject.ApplyModifiedProperties();
    }
    
    void DrawField(string labelName, int disableValue, string fieldName, ref bool checkValue)
    {
        int id = serializedObject.FindProperty(fieldName).intValue;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(labelName);
        checkValue = EditorGUILayout.Toggle(checkValue);
        EditorGUILayout.EndHorizontal();
        if (!checkValue)
        {
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(fieldName));
        }
        else
        {
            serializedObject.FindProperty(fieldName).intValue = disableValue;
        }
    }

    void DrawDropdownField(string labelName, string fieldName, int keepValue = -2, int clearValue = -1)
    {
        int id = serializedObject.FindProperty(fieldName).intValue;
        int selectIndex = 0;
        if (id == -2)
        {
            selectIndex = 0;
        }
        else if (id == -1)
        {
            selectIndex = 1;
        }
        else
        {
            selectIndex = 2;
        }
        
        int sel = EditorGUILayout.Popup(labelName, selectIndex, evtModifyTypes);

        if (sel == 0) //保留
        {
            serializedObject.FindProperty(fieldName).intValue = keepValue;
        }
        else if (sel == 1) //清空
        {
            serializedObject.FindProperty(fieldName).intValue = clearValue;
        }
        else //手动指定ID
        {
            if (serializedObject.FindProperty(fieldName).intValue < 0)
            {
                serializedObject.FindProperty(fieldName).intValue = 0;
            }
                
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(fieldName));
            
            //跳转event按钮
            Jyx2NodeEditorHelperQuickButtons.NavigateToEventButton(serializedObject.FindProperty(fieldName).intValue);
        }
        
    }
}