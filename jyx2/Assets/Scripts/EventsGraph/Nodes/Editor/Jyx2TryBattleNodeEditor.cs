using System.Collections;
using System.Collections.Generic;
using HSFrameWork.ConfigTable;
using Jyx2;
using MK.Toon;
using UnityEditor;
using UnityEngine;
using XNodeEditor;


[CustomNodeEditor(typeof(Jyx2TryBattleNode))]
public class Jyx2TryBattleNodeEditor : NodeEditor
{
    private Jyx2TryBattleNode myNode;

    private bool loseGameOver = false;
    public override void OnCreate()
    {
        base.OnCreate();
        if (myNode == null) myNode = target as Jyx2TryBattleNode;
        serializedObject.Update();


        loseGameOver = serializedObject.FindProperty("lose").objectReferenceValue == null;
    }

    public override void OnBodyGUI() {
        
        if (myNode == null) myNode = target as Jyx2TryBattleNode;
        
        // Update serialized object's representation
        serializedObject.Update();
        
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("prev"));

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("失败直接GAME OVER");
        loseGameOver = EditorGUILayout.Toggle(loseGameOver);
        EditorGUILayout.EndHorizontal();
        if (!loseGameOver)
        {
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("lose"));
        }
        
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("win"));
        
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(myNode.BattleId)));

        // Apply property modifications
        serializedObject.ApplyModifiedProperties();
    }
}