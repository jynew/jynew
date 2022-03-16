using System.Collections;
using System.Collections.Generic;

using Jyx2;
using MK.Toon;
using UnityEditor;
using UnityEngine;
using XNodeEditor;


[CustomNodeEditor(typeof(Jyx2JudgeScenePicNode))]
public class Jyx2JudgeScenePicEditor : NodeEditor
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
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("yes"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("no"));
        _sceneDrawer.DrawField();
        _sceneDrawer.DrawPopup();
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("eventId"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("picId"));
        // Apply property modifications
        serializedObject.ApplyModifiedProperties();
    }

}