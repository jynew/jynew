using System.Collections;
using System.Collections.Generic;

using Jyx2;
using MK.Toon;
using UnityEditor;
using UnityEngine;
using XNodeEditor;


[CustomNodeEditor(typeof(Jyx2TryBattleNode))]
public class Jyx2TryBattleNodeEditor : NodeEditor
{
    private Jyx2TryBattleNode myNode;

    public override void OnCreate()
    {
        base.OnCreate();
        if (myNode == null) myNode = target as Jyx2TryBattleNode;
        serializedObject.Update();

    }

    public override void OnBodyGUI() {
        
        if (myNode == null) myNode = target as Jyx2TryBattleNode;
        
        // Update serialized object's representation
        serializedObject.Update();
        
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("prev"));

        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(myNode.BattleId)));

        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(myNode.loseGameOver)));

        bool loseGameOver = serializedObject.FindProperty(nameof(myNode.loseGameOver)).boolValue;
        if (!loseGameOver)
        {
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("lose"));
        }
        
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("win"));
        
        // Apply property modifications
        serializedObject.ApplyModifiedProperties();
    }
}