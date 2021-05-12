/* TILE WORLD CREATOR RUNTIME EDITOR
 * Copyright (c) 2015 doorfortyfour OG / developed by Marc Egli
 * 
 * Create awesome tile worlds in seconds.
 *
 * 
 * Documentation: http://tileworldcreator.doofortyfour.com
 * Like us on Facebook: http://www.facebook.com/doorfortyfour2013
 * Web: http://www.doorfortyfour.com
 * Contact: mail@doorfortyfour.com Contact us for help, bugs or 
 * share your awesome work you've made with TileWorldCreator
*/

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using TileWorld;


[CustomEditor(typeof(TileWorldCreatorRTE))]
public class TileWorldCreatorRTEeditor : Editor
{
   
    //REFERENCE
    TileWorldCreatorRTE rte;
    Texture2D topLogo;

    private void OnEnable()
    {
        //get script Reference
        rte = (TileWorldCreatorRTE)target;

        LoadResources();
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button(topLogo, "TextArea"))
        {

        }

        bool _noTWCprefab = false;
        bool _noCameraPrefab = false;

        if (rte.creator == null)
        {
            EditorGUILayout.HelpBox("No TileWorldCreator prefab assigned!", MessageType.Warning);
            _noTWCprefab = true;
        }

        if (_noTWCprefab)
        {
            GUI.color = Color.yellow;
        }
        
        rte.creator = EditorGUILayout.ObjectField("TileWorldCreator prefab:", rte.creator, typeof(TileWorldCreator), true) as TileWorldCreator;
        GUI.color = Color.white;
        

        if (rte.editorCam == null)
        {
            EditorGUILayout.HelpBox("No camera assigned!", MessageType.Warning);
            _noCameraPrefab = true;
        }

        if (_noCameraPrefab)
        {
            GUI.color = Color.yellow;
        }

        rte.editorCam = EditorGUILayout.ObjectField("Camera:", rte.editorCam, typeof(Camera), true) as Camera;
        GUI.color = Color.white;

        rte.showGrid = EditorGUILayout.Toggle("Generate grid:", rte.showGrid);
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Grid texture:");
        rte.gridTexture = EditorGUILayout.ObjectField(rte.gridTexture, typeof(Texture), true) as Texture;
        EditorGUILayout.EndHorizontal();

        rte.mainMaterial = EditorGUILayout.ObjectField("Main material:", rte.mainMaterial, typeof(Material), true) as Material;
        rte.cellAddColor = EditorGUILayout.ColorField("Cell add color:", rte.cellAddColor);
        rte.cellRemoveColor = EditorGUILayout.ColorField("Cell remove color:", rte.cellRemoveColor);
        rte.cursorColor = EditorGUILayout.ColorField("Cursor color:", rte.cursorColor);

        rte.path = EditorGUILayout.TextField("Path:", rte.path);
        rte.fileExtension = EditorGUILayout.TextField("File extension:", rte.fileExtension);

    }

    void LoadResources()
    {
        var _path = GetInstallPath();

       
        topLogo = AssetDatabase.LoadAssetAtPath(_path + "Res/topLogoRTE.png", typeof(Texture2D)) as Texture2D;
    }

    //return install path
    string GetInstallPath()
    {
        string _scriptFilePath = "";
        string _scriptFolder = "";

        MonoScript ms = MonoScript.FromScriptableObject(this);
        _scriptFilePath = AssetDatabase.GetAssetPath(ms);

        FileInfo fi = new FileInfo(_scriptFilePath);

        _scriptFolder = fi.Directory.ToString();
        _scriptFolder = _scriptFolder.Replace(Path.DirectorySeparatorChar, '/'); //Path.DirectorySeparatorChar

        string _subString = Application.dataPath;
        _subString = _subString.Substring(0, _subString.Length - 6);
        _scriptFolder = _scriptFolder.Replace(_subString, "");

        //replace inspectors folder
        _scriptFolder = _scriptFolder.Replace("Editor", "");

        return _scriptFolder;
    }
}
#endif