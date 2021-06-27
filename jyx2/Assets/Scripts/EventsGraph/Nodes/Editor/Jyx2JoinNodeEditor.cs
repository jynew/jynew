using System.Collections;
using System.Collections.Generic;
using HSFrameWork.ConfigTable;
using Jyx2;
using MK.Toon;
using UnityEditor;
using UnityEngine;
using XNodeEditor;


[CustomNodeEditor(typeof(Jyx2JoinNode))]
public class Jyx2JoinNodeEditor : NodeEditor
{
    private Jyx2JoinNode myNode;
    private int _selectedIndex = 0;

    public override void OnCreate()
    {
        base.OnCreate();
        if (myNode == null) myNode = target as Jyx2JoinNode;
        serializedObject.Update();
        _selectedIndex = serializedObject.FindProperty("roleId").intValue;
    }

    public override void OnBodyGUI() {
        
        if (myNode == null) myNode = target as Jyx2JoinNode;
        
        // Update serialized object's representation
        serializedObject.Update();
        
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("prev"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("next"));

        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("roleId"));

        _selectedIndex = serializedObject.FindProperty("roleId").intValue;
        
        //道具图标
        int sel = EditorGUILayout.Popup("角色", _selectedIndex, Jyx2EventsGraphStatic.s_roleList);
        if (sel != _selectedIndex)
        {
            serializedObject.FindProperty("roleId").intValue = sel;
            _selectedIndex = sel;
        }
        
        //图标
        var roleContent = new GUIContent(GetRoleTexture());
        EditorGUILayout.LabelField(roleContent, GUILayout.Height(50));

        // Apply property modifications
        serializedObject.ApplyModifiedProperties();
    }
    
    public Texture2D GetRoleTexture()
    {
        int id = serializedObject.FindProperty("roleId").intValue;

        return AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/BuildSource/head/{id}.png");
    }
}