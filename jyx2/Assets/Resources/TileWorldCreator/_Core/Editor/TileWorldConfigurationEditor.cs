/* TILE WORLD CREATOR
 * Copyright (c) 2015 doorfortyfour OG / developed by Marc Egli
 * 
 * Configuration Editor class
*/
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TileWorld;

[CustomEditor(typeof(TileWorldConfiguration), true)]
public class TileWorldConfigurationEditor : Editor
{

    TileWorldConfiguration config;
    TileWorldCreator creator;

    //GUI Images
    static Texture2D guiPresetImage1;
    static Texture2D guiPresetImage2;
    static Texture2D guiPresetImage3;
    static Texture2D guiPresetImage4;
    static Texture2D guiPresetImage5;
    static Texture2D iconDuplicate;
    static Texture2D iconUndockWindow;

    //GUI Colors
    static Color guiRed = new Color(255f / 255f, 100f / 255f, 100f / 255f);
    static Color guiBlue = new Color(190f / 255f, 230f / 255f, 230f / 255f);

    static Color colorGUIFoldout1 = new Color(130f / 255f, 165f / 255f, 200f / 255f);
    static Color colorGUIFoldout2 = new Color(50f / 255f, 100f / 255f, 160f / 255f);
    

    static string[] algorithmNames;

    public void OnEnable()
    {
        config = target as TileWorldConfiguration;

        //get all available algorithms by name
        TileWorldAlgorithmLookup.GetAlgorithmNames(out algorithmNames);

        LoadResources();
    }

    [MenuItem("Assets/Create/TileWorldCreator/Configuration", false)]
    public static TileWorldConfiguration CreateConfigFileFromProjectView()
    {
        return Create("TileWorldConfiguration", null);
    }

    public static TileWorldConfiguration CreateConfigFile(TileWorldCreator _creator)
    {
        return Create("TileWorldConfiguration", _creator);
    }

    public static TileWorldConfiguration Create(string name, TileWorldCreator _creator)
    {
        var _path = EditorUtility.SaveFilePanel("new TileWorldCreator configuration file", "Assets", name, "asset");
        if (string.IsNullOrEmpty(_path))
            return null;

        var _config = ScriptableObject.CreateInstance<TileWorldConfiguration>();

        Path.ChangeExtension(_path, ".asset");

        AssetDatabase.CreateAsset(_config, makeRelativePath(_path));
        AssetDatabase.Refresh();

        if (_creator != null)
        {
            _creator.configuration = _config;
            _creator.firstTimeBuild = true;
        }
        else
        {
            Selection.activeObject = _config;
        }

        

        //add start preset
        AddNewPreset(_config);

        return _config;
    }


    private static string makeRelativePath(string _path)
    {
        if (string.IsNullOrEmpty(_path))
        {
            return "";
        }

        return _path.Substring(_path.IndexOf("Assets/", System.StringComparison.OrdinalIgnoreCase));
    }



    public override void OnInspectorGUI()
    {
        ShowConfigurationEditor(config);

        EditorUtility.SetDirty(config);
    }



    public static void ShowConfigurationEditor(TileWorldConfiguration _config)
    {

        if (_config == null)
        {
            EditorGUILayout.HelpBox("Please create or load a configuration file.", MessageType.Info);         
            return;
        }


        //Show global settings foldout
        ShowGlobalSettingsFoldout(_config);
        //Show preset settings foldout
        ShowPresetSettingsFoldout(_config);

        //EditorUtility.SetDirty(_config);
    }

    static void ShowGlobalSettingsFoldout(TileWorldConfiguration _config)
    {
        // Show global settings
        GUI.color = colorGUIFoldout1;
        EditorGUILayout.BeginVertical("Box");
        GUI.color = Color.white;
        GUILayout.BeginHorizontal("Box");

        _config.ui.showGeneralSettings = GUILayout.Toggle(_config.ui.showGeneralSettings, ("Settings"), GUI.skin.GetStyle("foldout"), GUILayout.ExpandWidth(true), GUILayout.Height(18));

        if (GUILayout.Button(iconUndockWindow, "ToolbarButton", GUILayout.Width(25)))
        {
            TileWorldCreator _c = GameObject.FindObjectOfType<TileWorldCreator>();
            TileWorldCreatorEditorSettingsWindow.InitWindow(_c.gameObject);
        }

        GUILayout.EndHorizontal();

        if (_config.ui.showGeneralSettings)
        {
            ShowGlobalSettings(_config);
        }
        EditorGUILayout.EndVertical();
    }

    static void ShowPresetSettingsFoldout(TileWorldConfiguration _config)
    {

        // Show preset settings
        GUI.color = colorGUIFoldout2;
        EditorGUILayout.BeginVertical("Box");
        GUI.color = Color.white;
        GUILayout.BeginHorizontal("Box");
        _config.ui.showPresetSettings = GUILayout.Toggle(_config.ui.showPresetSettings, ("Presets"), GUI.skin.GetStyle("foldout"), GUILayout.ExpandWidth(true), GUILayout.Height(18));

        if (GUILayout.Button(iconUndockWindow, "ToolbarButton", GUILayout.Width(25)))
        {
            TileWorldCreator _c = GameObject.FindObjectOfType<TileWorldCreator>();
            TileWorldCreatorEditorPresetsWindow.InitWindow(_c.gameObject);
        }

        GUILayout.EndHorizontal();

        if (_config.ui.showPresetSettings)
        {
            ShowPresetSettings(_config);
        }
        EditorGUILayout.EndVertical();
    }


    public static void ShowGlobalSettings(TileWorldConfiguration _config)
    {
        var _editor = Editor.CreateEditor(_config);

        GUILayout.BeginVertical("Box");
        _config.global.worldName = EditorGUILayout.TextField("Name:", _config.global.worldName);
        _config.global.invert = EditorGUILayout.Toggle("Invert:", _config.global.invert);
        //_config.global.floodUnjoined = EditorGUILayout.Toggle("Flood unjoined:", _config.global.floodUnjoined);
        //_config.global.floodHoles = EditorGUILayout.Toggle("Flood holes:", _config.global.floodHoles);
        _config.global.buildGroundTiles = EditorGUILayout.Toggle("Build ground tiles:", _config.global.buildGroundTiles);
        _config.global.mapOrientation = (TileWorldConfiguration.GlobalConfiguration.MapOrientations)EditorGUILayout.EnumPopup("Map orientation: ", _config.global.mapOrientation);
        GUILayout.EndVertical();


        GUILayout.BeginVertical("Box");
        _config.global.layerCount = EditorGUILayout.IntField("Number of layers:", _config.global.layerCount);
        _config.global.layerInset = EditorGUILayout.IntField("Layer inset:", _config.global.layerInset);

        if (_config.global.layerCount > 1)
        {
            _config.global.buildOverlappingTiles = EditorGUILayout.Toggle("Build overlapping tiles", _config.global.buildOverlappingTiles);
        }
        GUILayout.EndVertical();


        GUILayout.BeginVertical("Box");
            _config.global.width = EditorGUILayout.IntField("Grid width:", _config.global.width);
            _config.global.height = EditorGUILayout.IntField("Grid height:", _config.global.height);
            _config.global.globalScale = EditorGUILayout.Slider("Scale:", _config.global.globalScale, 1, 100);
            _config.global.scaleTiles = EditorGUILayout.Toggle("Scale tiles:", _config.global.scaleTiles);
            GUILayout.EndVertical();

            //SELECT MAP ALGORITHM
            GUILayout.BeginVertical("Box");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Selected algorithm: ");
            _config.global.selectedAlgorithm = EditorGUILayout.Popup(_config.global.selectedAlgorithm, algorithmNames);

            // change properties based on algorithms to make sure the algorithm works properly
            if (algorithmNames[_config.global.selectedAlgorithm] == "MazeAlgorithm")
            {
                _config.global.invert = false;
            }

            GUILayout.EndHorizontal();

            if (algorithmNames[_config.global.selectedAlgorithm] == "CellularAlgorithm")
            {
                if (_config.cellularAlgorithm.minCellCount >= (_config.global.width * _config.global.height) * 0.5f)
                {
                    EditorGUILayout.HelpBox("Minimum cell count size can only be max 50% of the width * height size of the map." + "\n" +
                        "Try to set a lower minimum cell count", MessageType.Warning);
                    _config.cellularAlgorithm.minCellCount = (_config.global.width * _config.global.height) / 2;
                }
            }

            //draw default algorithm attributes
            EditorGUI.indentLevel = 1;
            _editor.DrawDefaultInspector();
            EditorGUI.indentLevel = 0;

            EditorGUILayout.BeginVertical("Box");
            _config.global.randomSeed = EditorGUILayout.IntField("Random seed:", _config.global.randomSeed);


            //_config.global.minSize = EditorGUILayout.IntField("Minimum size:", _config.global.minSize);

          

        EditorGUILayout.EndVertical();

        GUILayout.EndVertical();


        GUILayout.BeginVertical("Box");
            GUILayout.Label("Merge settings:");

            EditorGUILayout.HelpBox("Cluster size defines the square size in tiles of a cluster." + "\n" +
                "Please note that when merging the map, TileWorldCreator tries to merge all tiles inside of each cluster. If the result exceeds the max allowed vertex count an error occurs - Try the Automatic setting instead.", MessageType.Info);

            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Cluster Size:");
                
            _config.global.automaticClustersize = EditorGUILayout.Toggle("Automatic:", _config.global.automaticClustersize);

            GUI.enabled = !_config.global.automaticClustersize;        
            _config.global.clusterSize = EditorGUILayout.IntField("Size:", _config.global.clusterSize);
            GUI.enabled = true;

            EditorGUILayout.EndVertical();
        _config.global.createMultiMaterialMesh = EditorGUILayout.Toggle("Multi material mesh: ", _config.global.createMultiMaterialMesh);
            _config.global.addMeshCollider = EditorGUILayout.Toggle("Add mesh collider: ", _config.global.addMeshCollider);
        GUILayout.EndVertical();
    }



    public static void ShowPresetSettings(TileWorldConfiguration _config)
    {

        GUILayout.BeginVertical("Box", GUILayout.ExpandWidth(false));


        if (GUILayout.Button("Add new preset"))
        {
            AddNewPreset(_config);
        }

        EditorGUILayout.BeginVertical("TextArea");


        for (int i = 0; i < _config.presets.Count; i++)
        //for (int i = 0; i < creator.blocks.Count; i ++)
        {

            GUI.color = guiBlue;
            GUILayout.BeginVertical("Box");
            GUI.color = Color.white;

            GUILayout.BeginHorizontal();

            _config.presets[i].showPresetEditor = GUILayout.Toggle(_config.presets[i].showPresetEditor, ("Preset: " + i), GUI.skin.GetStyle("foldout"), GUILayout.ExpandWidth(true), GUILayout.Height(18));

            //duplicate current preset
            if (GUILayout.Button(iconDuplicate, "ToolbarButton", GUILayout.Height(18), GUILayout.Width(25)))
            {
                DuplicatePreset(i, _config);
            }

            //delete current preset
            GUI.color = guiRed;
            if (i > 0)
            {
                if (GUILayout.Button("x", "ToolbarButton", GUILayout.Height(18), GUILayout.Width(25)))
                {
                    DeletePreset(i, _config);
                }
            }
            GUI.color = Color.white;

            GUILayout.EndHorizontal();

            if (i < _config.presets.Count)
            {

                if (_config.presets[i].showPresetEditor)
                {


                    GUILayout.BeginHorizontal();
                    //GUILayout.Label("");	

                    if (GUILayout.Button("+ random tileset", GUILayout.Height(18), GUILayout.Width(120)))
                    {
                        AddNewTileSet(i, _config);
                    }

                    GUILayout.EndHorizontal();

                    //TILE SETS
                    //---------

                    for (int t = 0; t < _config.presets[i].tiles.Count; t++)
                    {

                        GUILayout.BeginVertical("TextArea");

                        //------------------------------------
                        GUILayout.BeginHorizontal("Box");

                        // Duplicate random tileset
                        if (GUILayout.Button(iconDuplicate, "MiniButton", GUILayout.Height(18), GUILayout.Width(20)))
                        {
                            DuplicateTileSet(i, t, _config);
                        }

                        GUI.color = guiRed;
                        if (t > 0)
                        {
                            if (GUILayout.Button("x", "MiniButton", GUILayout.Height(18), GUILayout.Width(20)))
                            {
                                DeleteTileSet(i, t, _config);
                            }
                        }
                        GUI.color = Color.white;

                        GUILayout.Label("Tileset: " + t);

                        GUILayout.EndHorizontal();
                        //--------------------------------------


                        DragDropArea(i, t, _config);


                        if (t < _config.presets[i].tiles.Count)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.BeginVertical();
                            GUILayout.Label(guiPresetImage1, GUILayout.MaxWidth(50), GUILayout.MaxHeight(50));
                            _config.presets[i].tiles[t].tileI = EditorGUILayout.ObjectField(_config.presets[i].tiles[t].tileI, typeof(GameObject), false, GUILayout.MaxWidth(50)) as GameObject;
                            GUILayout.Label("yRotOff:");
                            _config.presets[i].tiles[t].yRotationOffset[0] = EditorGUILayout.FloatField("", _config.presets[i].tiles[t].yRotationOffset[0], GUILayout.Width(50));
                            GUILayout.EndVertical();
                            GUILayout.FlexibleSpace();
                            GUILayout.BeginVertical();
                            GUILayout.Label(guiPresetImage2, GUILayout.MaxWidth(50), GUILayout.MaxHeight(50));
                            _config.presets[i].tiles[t].tileC = EditorGUILayout.ObjectField(_config.presets[i].tiles[t].tileC, typeof(GameObject), false, GUILayout.MaxWidth(50)) as GameObject;
                            GUILayout.Label("yRotOff:");
                            _config.presets[i].tiles[t].yRotationOffset[1] = EditorGUILayout.FloatField("", _config.presets[i].tiles[t].yRotationOffset[1], GUILayout.Width(50));
                            GUILayout.EndVertical();
                            GUILayout.FlexibleSpace();
                            GUILayout.BeginVertical();
                            GUILayout.Label(guiPresetImage3, GUILayout.MaxWidth(50), GUILayout.MaxHeight(50));
                            _config.presets[i].tiles[t].tileCi = EditorGUILayout.ObjectField(_config.presets[i].tiles[t].tileCi, typeof(GameObject), false, GUILayout.MaxWidth(50)) as GameObject;
                            GUILayout.Label("yRotOff:");
                            _config.presets[i].tiles[t].yRotationOffset[2] = EditorGUILayout.FloatField("", _config.presets[i].tiles[t].yRotationOffset[2], GUILayout.Width(50));
                            GUILayout.EndVertical();
                            GUILayout.FlexibleSpace();
                            GUILayout.BeginVertical();
                            GUILayout.Label(guiPresetImage4, GUILayout.MaxWidth(50), GUILayout.MaxHeight(50));
                            _config.presets[i].tiles[t].tileB = EditorGUILayout.ObjectField(_config.presets[i].tiles[t].tileB, typeof(GameObject), false, GUILayout.MaxWidth(50)) as GameObject;
                            GUILayout.Label("yRotOff:");
                            _config.presets[i].tiles[t].yRotationOffset[3] = EditorGUILayout.FloatField("", _config.presets[i].tiles[t].yRotationOffset[3], GUILayout.Width(50));
                            GUILayout.EndVertical();
                            GUILayout.FlexibleSpace();
                            GUILayout.BeginVertical();
                            GUILayout.Label(guiPresetImage5, GUILayout.MaxWidth(50), GUILayout.MaxHeight(50));
                            _config.presets[i].tiles[t].tileF = EditorGUILayout.ObjectField(_config.presets[i].tiles[t].tileF, typeof(GameObject), false, GUILayout.MaxWidth(50)) as GameObject;
                            GUILayout.Label("yRotOff:");
                            _config.presets[i].tiles[t].yRotationOffset[4] = EditorGUILayout.FloatField("", _config.presets[i].tiles[t].yRotationOffset[4], GUILayout.Width(50));
                            GUILayout.EndVertical();
                            GUILayout.EndHorizontal();

                            GUILayout.Space(20);
                            _config.presets[i].tiles[t].blockOffset = EditorGUILayout.Vector3Field("block offset: ", _config.presets[i].tiles[t].blockOffset);
                            _config.presets[i].tiles[t].floorOffset = EditorGUILayout.Vector3Field("ground offset: ", _config.presets[i].tiles[t].floorOffset);

                        }

                        GUILayout.EndVertical();
                    }
                }

            }

            GUILayout.EndVertical();

        }
        GUILayout.EndVertical();


        GUILayout.EndVertical();
    }


    static void DragDropArea(int _i, int _t, TileWorldConfiguration _config)
    {
        Event evt = Event.current;
        Rect drop_area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(drop_area, "Drag and drop tile prefabs here for automatic assignment" + "\n (alphabetical order = tile assignment from left to right)");

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!drop_area.Contains(evt.mousePosition))
                    return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    var a = 0;
                    Object[] _obj = DragAndDrop.objectReferences;
                    List<Object> _objList = new List<Object>();
                    for (int o = 0; o < _obj.Length; o++)
                    {
                        _objList.Add(_obj[o]);
                    }

                    _objList.Sort(CompareByName);

                    for (int i = 0; i < _objList.Count; i++)
                    {
                        if (a == 0)
                        {
                            _config.presets[_i].tiles[_t].tileI = _objList[i] as GameObject;
                        }
                        else if (a == 1)
                        {
                            _config.presets[_i].tiles[_t].tileC = _objList[i] as GameObject;
                        }
                        else if (a == 2)
                        {
                            _config.presets[_i].tiles[_t].tileCi = _objList[i] as GameObject;
                        }
                        else if (a == 3)
                        {
                            _config.presets[_i].tiles[_t].tileB = _objList[i] as GameObject;
                        }
                        else if (a >= 4)
                        {
                            _config.presets[_i].tiles[_t].tileF = _objList[i] as GameObject;
                        }

                        a++;

                    }

                }
                break;
        }
    }

    static int CompareByName(Object _item1, Object _item2)
    {
        return _item1.name.CompareTo(_item2.name);
    }


    /// <summary>
    /// Editor method. Add a new preset.
    /// </summary>
    static void AddNewPreset(TileWorldConfiguration _config)
    {
        //blocks.Add(new TileSets());
        _config.presets.Add(new TileWorldConfiguration.PresetsConfiguration());

        ResizeStringArray(false, _config.presets.Count - 1, _config);
    }


    /// <summary>
    /// Editor method. Duplicate a preset
    /// </summary>
    /// <param name="_index"></param>
    static void DuplicatePreset(int _index, TileWorldConfiguration _config)
    {
        AddNewPreset(_config);

        if (_config.presets[_index].tiles.Count > 1)
        {
            for (int t1 = 0; t1 < _config.presets[_index].tiles.Count - 1; t1++)
            {
                _config.presets[_config.presets.Count - 1].tiles.Add(new TileWorldConfiguration.TileSets());
            }

        }


        for (int t2 = 0; t2 < _config.presets[_index].tiles.Count; t2++)
        {
            _config.presets[_config.presets.Count - 1].tiles[t2].tileI = _config.presets[_index].tiles[t2].tileI;
            _config.presets[_config.presets.Count - 1].tiles[t2].tileC = _config.presets[_index].tiles[t2].tileC;
            _config.presets[_config.presets.Count - 1].tiles[t2].tileCi = _config.presets[_index].tiles[t2].tileCi;
            _config.presets[_config.presets.Count - 1].tiles[t2].tileB = _config.presets[_index].tiles[t2].tileB;
            _config.presets[_config.presets.Count - 1].tiles[t2].tileF = _config.presets[_index].tiles[t2].tileF;

            _config.presets[_config.presets.Count - 1].tiles[t2].floorOffset = _config.presets[_index].tiles[t2].floorOffset;
            _config.presets[_config.presets.Count - 1].tiles[t2].blockOffset = _config.presets[_index].tiles[t2].blockOffset;

            _config.presets[_config.presets.Count - 1].tiles[t2].yRotationOffset = new float[_config.presets[_index].tiles[t2].yRotationOffset.Length];

            for (int f = 0; f < _config.presets[_index].tiles[t2].yRotationOffset.Length; f++)
            {
                _config.presets[_config.presets.Count - 1].tiles[t2].yRotationOffset[f] = _config.presets[_index].tiles[t2].yRotationOffset[f];
            }
        }
    }


    /// <summary>
    /// Editor method. Add a new tileset to the given preset index
    /// </summary>
    /// <param name="_index"></param>
    static void AddNewTileSet(int _index, TileWorldConfiguration _config)
    {
        _config.presets[_index].tiles.Add(new TileWorldConfiguration.TileSets());
    }


    /// <summary>
    /// Editor method. Delete tileset
    /// </summary>
    /// <param name="_presetIndex"></param>
    /// <param name="_tileIndex"></param>
    static void DeleteTileSet(int _presetIndex, int _tileIndex, TileWorldConfiguration _config)
    {
        _config.presets[_presetIndex].tiles.RemoveAt(_tileIndex);
    }

    /// <summary>
    /// Editor method. Duplicate tileset
    /// </summary>
    /// <param name="_presetIndex"></param>
    /// <param name="_tileIndex"></param>
    /// <param name="_config"></param>
    static void DuplicateTileSet(int _presetIndex, int _tileIndex, TileWorldConfiguration _config)
    {
        _config.presets[_presetIndex].tiles.Add(new TileWorldConfiguration.TileSets(_config.presets[_presetIndex].tiles[_tileIndex]));
    }

    /// <summary>
    /// Editor method. Delete preset
    /// </summary>
    /// <param name="_inx"></param>
    static void DeletePreset(int _inx, TileWorldConfiguration _config)
    {
        if (_inx != 0)
        {
            _config.presets.RemoveAt(_inx);
            ResizeStringArray(true, _inx, _config);    
        }
    }


    static void ResizeStringArray(bool _remove, int _index, TileWorldConfiguration _config)
    {
        List<string> _tmpString = new List<string>();

        for (int i = 0; i < _config.ui.availablePresets.Length; i++)
        {
            _tmpString.Add(_config.ui.availablePresets[i]);
        }

        if (_remove)
        {
            _tmpString.RemoveAt(_index);

            _config.ui.availablePresets = new string[_tmpString.Count];

            for (int m = 0; m < _config.ui.availablePresets.Length; m++)
            {
                _config.ui.availablePresets[m] = _tmpString[m];
            }
        }
        else
        {
            _tmpString.Add("Preset: " + _config.ui.availablePresets.Length.ToString());


            _config.ui.availablePresets = new string[_tmpString.Count];
            for (int a = 0; a < _tmpString.Count; a++)
            {
                _config.ui.availablePresets[a] = "Preset: " + a;
            }
        }
    }


    /// <summary>
    /// Load editor icons
    /// </summary>
    void LoadResources()
    {
        var _path = ReturnInstallPath.GetInstallPath("Editor", this); // GetInstallPath();

        guiPresetImage1 = AssetDatabase.LoadAssetAtPath(_path + "Res/guipreset1.png", typeof(Texture2D)) as Texture2D;
        guiPresetImage2 = AssetDatabase.LoadAssetAtPath(_path + "Res/guipreset2.png", typeof(Texture2D)) as Texture2D;
        guiPresetImage3 = AssetDatabase.LoadAssetAtPath(_path + "Res/guipreset3.png", typeof(Texture2D)) as Texture2D;
        guiPresetImage4 = AssetDatabase.LoadAssetAtPath(_path + "Res/guipreset4.png", typeof(Texture2D)) as Texture2D;
        guiPresetImage5 = AssetDatabase.LoadAssetAtPath(_path + "Res/guipreset5.png", typeof(Texture2D)) as Texture2D;

        iconDuplicate = AssetDatabase.LoadAssetAtPath(_path + "Res/icon_duplicate.png", typeof(Texture2D)) as Texture2D;       
        iconUndockWindow = AssetDatabase.LoadAssetAtPath(_path + "Res/icon_undock.png", typeof(Texture2D)) as Texture2D;
    }
}
