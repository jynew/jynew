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
    private Jyx2NodeEditorHelperRole _roleDrawer;

    public override void OnCreate()
    {
        base.OnCreate();
        _roleDrawer = new Jyx2NodeEditorHelperRole(this);
        EditorStyles.textField.wordWrap = true; // 自动换行
    }

    public override void OnBodyGUI() {
        // Update serialized object's representation
        serializedObject.Update();
        
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("prev"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("next"));
        
        //这里需要定制化角色头像和对话框，所以不使用_roleDrawer.DrawAll()，而是分步显示。
        _roleDrawer.DrawField();
        _roleDrawer.DrawPopup();
        //角色头像
        var roleHeadContent = new GUIContent(_roleDrawer.GetTexture());
        
        EditorGUIUtility.labelWidth = 25.0f; // Replace this with any width
        EditorGUILayout.PropertyField(serializedObject.FindProperty("content"),
            roleHeadContent, GUILayout.MinHeight(40f), GUILayout.MaxHeight(100f));
        
        // Apply property modifications
        serializedObject.ApplyModifiedProperties();
    }
}