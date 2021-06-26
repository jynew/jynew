using System.Collections;
using System.Collections.Generic;
using HSFrameWork.ConfigTable;
using Jyx2;
using MK.Toon;
using UnityEditor;
using UnityEngine;
using XNodeEditor;


[CustomNodeEditor(typeof(Jyx2AddItemNode))]
public class Jyx2AddItemNodeEditor : NodeEditor
{
    private Jyx2AddItemNode myNode;
    private int _selectedIndex = 0;

    public override void OnCreate()
    {
        base.OnCreate();
        if (myNode == null) myNode = target as Jyx2AddItemNode;
        serializedObject.Update();
        _selectedIndex = serializedObject.FindProperty("itemId").intValue;
    }

    public override void OnBodyGUI() {
        
        if (myNode == null) myNode = target as Jyx2AddItemNode;
        
        // Update serialized object's representation
        serializedObject.Update();
        
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("prev"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("next"));

        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("itemId"));

        _selectedIndex = serializedObject.FindProperty("itemId").intValue;
        
        //道具图标
        int sel = EditorGUILayout.Popup("道具", _selectedIndex, Jyx2EventsGraphStatic.s_itemList);
        if (sel != _selectedIndex)
        {
            serializedObject.FindProperty("itemId").intValue = sel;
            _selectedIndex = sel;
        }
        
        //道具图标
        var itemContent = new GUIContent(GetItemTexture());
        EditorGUILayout.LabelField(itemContent, GUILayout.Height(50));

        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("count"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("isHint"));

        
        // Apply property modifications
        serializedObject.ApplyModifiedProperties();
    }
    
    public Texture2D GetItemTexture()
    {
        int id = serializedObject.FindProperty("itemId").intValue;

        return AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/BuildSource/Jyx2Items/{id}.png");
    }
}