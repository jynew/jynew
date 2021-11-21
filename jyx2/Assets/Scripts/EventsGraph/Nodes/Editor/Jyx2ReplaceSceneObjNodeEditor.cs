using System.Collections;
using System.Collections.Generic;

using Jyx2;
using MK.Toon;
using UnityEditor;
using UnityEngine;
using XNodeEditor;


[CustomNodeEditor(typeof(Jyx2ReplaceSceneObjNode))]
public class Jyx2ReplaceSceneObjNodeEditor : NodeEditor
{
    private Jyx2NodeEditorHelperScene _sceneDrawer;

    public override void OnCreate()
    {
        base.OnCreate();
        _sceneDrawer = new Jyx2NodeEditorHelperScene(this);
    }

    public override void OnBodyGUI() 
    {    
        // Update serialized object's representation
        serializedObject.Update();
        
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("prev"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("next"));
        _sceneDrawer.DrawField();
        _sceneDrawer.DrawPopup();
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("isShow"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("path"));
        // Apply property modifications
        serializedObject.ApplyModifiedProperties();
    }

}