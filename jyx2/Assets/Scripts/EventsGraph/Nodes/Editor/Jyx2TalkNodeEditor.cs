using System.Collections;
using System.Collections.Generic;
using HSFrameWork.ConfigTable;
using Jyx2;
using MK.Toon;
using UnityEditor;
using UnityEngine;
using XNodeEditor;


[CustomNodeEditor(typeof(Jyx2TalkNode))]
public class Jyx2TalkNodeEditor : NodeEditor
{
    private Jyx2TalkNode myNode;
    private int _selectedIndex = 0;

    public override void OnCreate()
    {
        base.OnCreate();
        if (myNode == null) myNode = target as Jyx2TalkNode;
        serializedObject.Update();
        _selectedIndex = serializedObject.FindProperty("roleId").intValue;
        
        EditorStyles.textField.wordWrap = true; // 自动换行
    }

    public override void OnBodyGUI() {
        
        if (myNode == null) myNode = target as Jyx2TalkNode;
        
        // Update serialized object's representation
        serializedObject.Update();
        
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("prev"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("next"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("roleId"));

        _selectedIndex = serializedObject.FindProperty("roleId").intValue;
        
        //选择角色
        int sel = EditorGUILayout.Popup("角色", _selectedIndex, Jyx2EventsGraphStatic.s_roleList);
        if (sel != _selectedIndex)
        {
            serializedObject.FindProperty("roleId").intValue = sel;
            _selectedIndex = sel;
        }
        
        //角色头像
        var roleHeadContent = new GUIContent(GetRoleHeadTexture());
        
        EditorGUIUtility.labelWidth = 25.0f; // Replace this with any width
        EditorGUILayout.PropertyField(serializedObject.FindProperty("content"),
            roleHeadContent, GUILayout.MinHeight(40f), GUILayout.MaxHeight(100f));
        

        // Apply property modifications
        serializedObject.ApplyModifiedProperties();
    }
    
    public Texture2D GetRoleHeadTexture()
    {
        int id = serializedObject.FindProperty("roleId").intValue;

        return AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/BuildSource/head/{id}.png");
    }
}