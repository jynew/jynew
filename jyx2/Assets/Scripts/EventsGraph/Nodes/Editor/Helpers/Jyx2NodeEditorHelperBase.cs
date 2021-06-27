using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

abstract public class Jyx2NodeEditorHelperBase
{
    protected NodeEditor _nodeEditor;
    protected int _selectedIndex;
    
    protected abstract string Field { get; }
    protected abstract int TextureHeight { get; }
    protected abstract string PopupTitle { get; }
    protected abstract string[] SelectContent { get; }
    protected abstract string PathFormat { get; }
    
    public Jyx2NodeEditorHelperBase(NodeEditor nodeEditor)
    {
        nodeEditor.serializedObject.Update();
        _nodeEditor = nodeEditor;
        _selectedIndex = nodeEditor.serializedObject.FindProperty(Field).intValue;
    }
    
    public void DrawAll()
    {
        DrawField();
        DrawPopup();
        DrawTexture();
    }

    public void DrawField()
    {
        NodeEditorGUILayout.PropertyField(_nodeEditor.serializedObject.FindProperty(Field));
    }

    public void DrawPopup()
    {
        _selectedIndex = _nodeEditor.serializedObject.FindProperty(Field).intValue;
        
        //角色图标
        int sel = EditorGUILayout.Popup(PopupTitle, _selectedIndex, SelectContent);
        if (sel != _selectedIndex)
        {
            _nodeEditor.serializedObject.FindProperty(Field).intValue = sel;
            _selectedIndex = sel;
        }
    }

    public void DrawTexture()
    {
        //图标
        var roleContent = new GUIContent(GetTexture());
        EditorGUILayout.LabelField(roleContent, GUILayout.Height(TextureHeight));
    }
    
    public Texture2D GetTexture()
    {
        int id = _nodeEditor.serializedObject.FindProperty(Field).intValue;
        string path = string.Format(PathFormat, id);
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }
}

