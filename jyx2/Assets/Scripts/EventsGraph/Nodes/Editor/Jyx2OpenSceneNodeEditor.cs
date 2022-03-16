using System.Collections;
using System.Collections.Generic;

using Jyx2;
using MK.Toon;
using UnityEditor;
using UnityEngine;
using XNodeEditor;


[CustomNodeEditor(typeof(Jyx2OpenSceneNode))]
public class Jyx2OpenSceneNodeEditor : NodeEditor
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
        _sceneDrawer.DrawField();
        _sceneDrawer.DrawPopup();
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("next"));
        // Apply property modifications
        serializedObject.ApplyModifiedProperties();
    }

}