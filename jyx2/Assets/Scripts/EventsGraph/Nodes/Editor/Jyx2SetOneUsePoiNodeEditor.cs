using System.Collections;
using System.Collections.Generic;

using Jyx2;
using MK.Toon;
using UnityEditor;
using UnityEngine;
using XNodeEditor;


[CustomNodeEditor(typeof(Jyx2SetOneUsePoiNode))]
public class Jyx2SetOneUsePoiNodeEditor : NodeEditor
{
    private Jyx2NodeEditorHelperRole _roleDrawer;

    public override void OnCreate()
    {
        base.OnCreate();
        _roleDrawer = new Jyx2NodeEditorHelperRole(this);
    }

    public override void OnBodyGUI() 
    {    
        // Update serialized object's representation
        serializedObject.Update();
        
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("prev"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("next"));
        _roleDrawer.DrawAll();
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("poiSkill"));
        // Apply property modifications
        serializedObject.ApplyModifiedProperties();
    }

}