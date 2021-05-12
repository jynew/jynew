using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TileWorld;

[CustomEditor(typeof(TileWorldObjectScatterConfiguration), true)]
public class TileWorldObjectScatterConfigEditor : Editor {

    TileWorldObjectScatterConfiguration config;
    static TileWorldObjectScatter twos;
    static TileWorldCreator creator;

    static string[] placeLayers = new string[] { "layer 1" };
    //static string[] options = new string[] { "", "", "", "", "", "", "", "", "" };

    static string[] dragdroptype = new string[] { "paint objects", "position based objects", "procedural objects" };
    static int dragDropTypeSelection;

    static bool startEndObjectsFoldout;
    static bool ruleBasedObjectsFoldout;
    static bool paintObjectsFoldout;

    static Color guiRed = new Color(255f / 255f, 100f / 255f, 100f / 255f);
    static Color colorGUI0 = new Color(130f / 255f, 165f / 255f, 200f / 255f);
    static Color colorGUI1 = new Color(50f / 255f, 100f / 255f, 160f / 255f);
    static Color colorGUI2 = new Color(0f / 255f, 50f / 255f, 100f / 255f);
    static Color guiBlue = new Color(0f / 255f, 180f / 255f, 255f / 255f);

    static Texture2D iconMaskLayerArrow;


    public void OnEnable()
    {
        config = target as TileWorldObjectScatterConfiguration;

        LoadResources();
    }

    [MenuItem("Assets/Create/TileWorldCreator/ObjectScatterConfiguration", false)]
    public static TileWorldObjectScatterConfiguration CreateConfigFileFromProjectView()
    {
        return Create("TileWorldObjectScatterConfiguration", null);
    }

    public static TileWorldObjectScatterConfiguration CreateConfigFile(TileWorldObjectScatter _creator)
    {
        return Create("TileWorldObjectScatterConfiguration", _creator);
    }

    public static TileWorldObjectScatterConfiguration Create(string _name, TileWorldObjectScatter __config)
    {
        var _path = EditorUtility.SaveFilePanel("new TileWorldCreatorObjectScatter configuration file", "Assets", _name, "asset");
        if (string.IsNullOrEmpty(_path))
            return null;

        var _config = ScriptableObject.CreateInstance<TileWorldObjectScatterConfiguration>();

        Path.ChangeExtension(_path, ".asset");
	    
	    //_config.hideFlags = HideFlags.None;
	    
        AssetDatabase.CreateAsset(_config, makeRelativePath(_path));
        AssetDatabase.Refresh();

        if (__config != null)
        {
            __config.configuration = _config;
        }
        else
        {
            Selection.activeObject = _config;
        }


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
        ShowOSConfigurationEditor(config, null, null, new string[] { "layer 1" }, iconMaskLayerArrow);

        EditorUtility.SetDirty(config);

        
        //LoadResources();
        
    }

    public static void ShowOSConfigurationEditor(TileWorldObjectScatterConfiguration _config, TileWorldObjectScatter _twos, TileWorldCreator _creator, string[] _placeLayers, Texture2D _iconMaskLayerArrow)
    {

        if (_config == null)
        {
            EditorGUILayout.HelpBox("Please create or load a configuration file.", MessageType.Info);
            return;
        }

        twos = _twos;
        creator = _creator;

        placeLayers = _placeLayers;

        DragDropArea(_config);

        ShowPaintObjects(_config);
        ShowPositionBasedObjects(_config);
        ShowRuleBasedObjects(_config);

    }


    static void DragDropArea(TileWorldObjectScatterConfiguration _config)
    {

        Event evt = Event.current;
        Rect drop_area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(drop_area, "Drag and drop prefabs here for automatic assignment. Do not use scene objects, they won't get saved in the asset file.");

        EditorGUILayout.BeginHorizontal();
        dragDropTypeSelection = GUILayout.SelectionGrid(dragDropTypeSelection, dragdroptype, 3, EditorStyles.toolbarButton);
        EditorGUILayout.EndHorizontal();

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


                    Object[] _obj = DragAndDrop.objectReferences;

                    for (int o = 0; o < _obj.Length; o++)
                    {
                        switch (dragDropTypeSelection)
                        {
                            case 0:
                                _config.paintObjects.Add(new TileWorldObjectScatterConfiguration.PaintObjectConfiguration(_obj[o] as GameObject));
                                break;
                            case 1:
                                _config.positionObjects.Add(new TileWorldObjectScatterConfiguration.PositionObjectConfiguration(_obj[o] as GameObject));
                                break;
                            case 2:
                                _config.proceduralObjects.Add(new TileWorldObjectScatterConfiguration.ProceduralObjectConfiguration(_obj[o] as GameObject));
                                break;
                        }
                    }

                }

                break;
        }
    }


    #region paintobjects

    static void ShowPaintObjects(TileWorldObjectScatterConfiguration _config)
    {
        GUI.color = colorGUI0;
        EditorGUILayout.BeginVertical("Box");

        GUI.color = Color.white;
        EditorGUILayout.BeginHorizontal("Box");

        paintObjectsFoldout = GUILayout.Toggle(paintObjectsFoldout, "Paint objects", GUI.skin.GetStyle("foldout"), GUILayout.ExpandWidth(true), GUILayout.Height(18));
        EditorGUILayout.EndHorizontal();

        // Show paint grid
        if (twos != null)
        {
            twos.showGrid = paintObjectsFoldout;
        }

        if (creator != null)
        {
            creator.showGrid = !paintObjectsFoldout;
        }

        if (paintObjectsFoldout)
        {
            if (twos == null ? GUI.enabled = false : GUI.enabled = true)
            if (GUILayout.Button("Delete all painted objects in scene"))
            {
                DestroyImmediate(twos.paintObjectsContainer);
            }
            GUI.enabled = true;

            Event evt = Event.current;
            if (evt.type != EventType.DragPerform)
            {

                EditorGUILayout.BeginVertical("TextArea");

                for (int i = 0; i < _config.paintObjects.Count; i++)
                {
                    if (_config.paintObjects[i].paintThisObject)
                    {
                        GUI.color = guiBlue;
                    }
                    else
                    {
                        GUI.color = Color.white;
                    }
                    EditorGUILayout.BeginHorizontal();
                    //GUI.color = Color.white;

                    if (_config.paintObjects[i].isChild)
                    {
                        //GUILayout.Label(iconMaskLayerArrow, GUILayout.Width(20), GUILayout.Height(18));
                        GUILayout.Label("->", GUILayout.Width(30));
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal("Box");

                        GUI.color = colorGUI0;
                        GUILayout.Box(" ", GUILayout.Width(5));
                        GUI.color = Color.white;
                    }

                    if (i > 0)
                    {
                        if (GUILayout.Button(_config.paintObjects[i].isChild ? "<<" : ">>", "toolbarButton", GUILayout.Width(20)))
                        {
                            _config.paintObjects[i].isChild = !_config.paintObjects[i].isChild;
                            _config.paintObjects[i].paintThisObject = false;
                        }
                    }

                    

                    _config.paintObjects[i].showPanel = GUILayout.Toggle(_config.paintObjects[i].showPanel, (_config.paintObjects[i].go != null ? _config.paintObjects[i].go.name : "missing game object"), GUI.skin.GetStyle("foldout"), GUILayout.ExpandWidth(true), GUILayout.Height(18));

                    if (!_config.paintObjects[i].isChild)
                    {
                        _config.paintObjects[i].paintThisObject = GUILayout.Toggle(_config.paintObjects[i].paintThisObject, "paint", GUI.skin.GetStyle("toolbarButton"));
                    }

                    GUI.color = guiRed;
                    if (GUILayout.Button("x", "toolbarButton", GUILayout.Width(20)))
                    {
                        DeletePaintObjects(_config, i);
                    }
                    GUI.color = Color.white;

                    EditorGUILayout.EndHorizontal();

                    if (i >= _config.paintObjects.Count)
                        return;

                    if (!_config.paintObjects[i].isChild)
                    {
                        EditorGUILayout.EndHorizontal();
                    }



                    if (_config.paintObjects[i].showPanel)
                    {

                        if (!_config.paintObjects[i].isChild)
                        {
                            EditorGUI.indentLevel = 2;
                            EditorGUILayout.BeginVertical();

                            _config.paintObjects[i].go = EditorGUILayout.ObjectField("gameobject: ", _config.paintObjects[i].go, typeof(GameObject), false) as GameObject;

                            _config.paintObjects[i].offsetPosition = EditorGUILayout.Vector3Field("offset position:", _config.paintObjects[i].offsetPosition);
                            _config.paintObjects[i].offsetRotation = EditorGUILayout.Vector3Field("offset rotation:", _config.paintObjects[i].offsetRotation);

                            // RANDOM ROTATION
                            EditorGUILayout.BeginVertical("Box");
                            // Use tile rotation
                            EditorGUILayout.BeginHorizontal();
                            _config.paintObjects[i].useTileRotation = EditorGUILayout.Toggle(new GUIContent("use tile rotations:", "if true the painted object will get the rotation of the tile where it's placed"), _config.paintObjects[i].useTileRotation);
                            GUI.enabled = _config.paintObjects[i].useTileRotation;
                            _config.paintObjects[i].selectedLayer = EditorGUILayout.Popup("from selected layer:", _config.paintObjects[i].selectedLayer, placeLayers);
                            EditorGUILayout.EndHorizontal();

                            //GUI.enabled = true;
                            GUI.enabled = !_config.paintObjects[i].useTileRotation;
                            _config.paintObjects[i].useRandomRotation = EditorGUILayout.Toggle("use random rotations:", _config.paintObjects[i].useRandomRotation);
                            GUI.enabled = _config.paintObjects[i].useRandomRotation;





                            _config.paintObjects[i].randomRotationMin = EditorGUILayout.Vector3Field("min random rotation:", _config.paintObjects[i].randomRotationMin);
                            _config.paintObjects[i].randomRotationMax = EditorGUILayout.Vector3Field("max random rotation:", _config.paintObjects[i].randomRotationMax);
                            GUI.enabled = true;
                            EditorGUILayout.EndVertical();

                            // RANDOM SCALING
                            EditorGUILayout.BeginVertical("Box");
                            _config.paintObjects[i].useRandomScaling = EditorGUILayout.Toggle("use random scaling:", _config.paintObjects[i].useRandomScaling);
                            GUI.enabled = _config.paintObjects[i].useRandomScaling;
                            _config.paintObjects[i].uniformScaling = EditorGUILayout.Toggle("uniform scaling:", _config.paintObjects[i].uniformScaling);

                            if (_config.paintObjects[i].uniformScaling)
                            {
                                _config.paintObjects[i].randomScalingMin.x = EditorGUILayout.FloatField("min random scaling:", _config.paintObjects[i].randomScalingMin.x);
                                _config.paintObjects[i].randomScalingMax.x = EditorGUILayout.FloatField("max random scaling:", _config.paintObjects[i].randomScalingMax.x);


                                _config.paintObjects[i].randomScalingMin = new Vector3(_config.paintObjects[i].randomScalingMin.x, _config.paintObjects[i].randomScalingMin.x, _config.paintObjects[i].randomScalingMin.x);
                                _config.paintObjects[i].randomScalingMax = new Vector3(_config.paintObjects[i].randomScalingMax.x, _config.paintObjects[i].randomScalingMax.x, _config.paintObjects[i].randomScalingMax.x);
                            }
                            else
                            {
                                _config.paintObjects[i].randomScalingMin = EditorGUILayout.Vector3Field("min random scaling: ", _config.paintObjects[i].randomScalingMin);
                                _config.paintObjects[i].randomScalingMax = EditorGUILayout.Vector3Field("max random scaling: ", _config.paintObjects[i].randomScalingMax);
                            }
                            GUI.enabled = true;

                            EditorGUILayout.EndVertical();

                            EditorGUILayout.EndVertical();
                            EditorGUI.indentLevel = 0;
                        }
                        else
                        {
                            ShowChildSpawnProperties(_config.paintObjects[i]);
                        }
                    }

                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();
        }
        else
        {
            EditorGUILayout.EndVertical();
        }
    }

    #endregion paintobjects

    #region positionbasedobjects
    static void ShowPositionBasedObjects(TileWorldObjectScatterConfiguration _config)
    {
        GUI.color = colorGUI1;
        EditorGUILayout.BeginVertical("Box");

        GUI.color = Color.white;
        EditorGUILayout.BeginHorizontal("Box");

        startEndObjectsFoldout = GUILayout.Toggle(startEndObjectsFoldout, "Position based objects", GUI.skin.GetStyle("foldout"), GUILayout.ExpandWidth(true), GUILayout.Height(18));
        EditorGUILayout.EndHorizontal();


        if (startEndObjectsFoldout)
        {

            Event evt = Event.current;
            if (evt.type != EventType.DragPerform)
            {

                EditorGUILayout.BeginVertical("TextArea");


                for (int i = 0; i < _config.positionObjects.Count; i++)
                {

                    EditorGUILayout.BeginHorizontal();

                    if (_config.positionObjects[i].isChild)
                    {
                        GUILayout.Label("->", GUILayout.Width(30));
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal("Box");

                        GUI.color = colorGUI1;
                        GUILayout.Box(" ", GUILayout.Width(5));
                        GUI.color = Color.white;
                    }

                    if (i > 0)
                    {
                        if (GUILayout.Button(_config.positionObjects[i].isChild ? "<<" : ">>", "toolbarButton", GUILayout.Width(20)))
                        {
                            _config.positionObjects[i].isChild = !_config.positionObjects[i].isChild;
                        }
                    }

                    _config.positionObjects[i].active = GUILayout.Toggle(_config.positionObjects[i].active, _config.positionObjects[i].active ? "on" : "off", GUI.skin.GetStyle("toolbarButton"), GUILayout.Width(22));

                    GUI.enabled = _config.positionObjects[i].active;

                    _config.positionObjects[i].showPanel = GUILayout.Toggle(_config.positionObjects[i].showPanel, (_config.positionObjects[i].go != null ? _config.positionObjects[i].go.name : "missing game object"), GUI.skin.GetStyle("foldout"), GUILayout.ExpandWidth(true), GUILayout.Height(18));

                    // move up
                    if (i > 0)
                    {
                        GUI.enabled = true;
                    }
                    else
                    {
                        GUI.enabled = false;
                    }

                    if (GUILayout.Button("▲", "toolbarButton", GUILayout.Width(20)))
                    {
                        MovePositionObjectsUp(_config, i);
                    }

                    GUI.enabled = true;

                    // move down
                    if (_config.positionObjects.Count > 1 && i < _config.proceduralObjects.Count - 1)
                    {
                        GUI.enabled = true;
                    }
                    else
                    {
                        GUI.enabled = false;
                    }
                    if (GUILayout.Button("▼", "toolbarButton", GUILayout.Width(20)))
                    {
                        MovePositionObjectsDown(_config, i);
                    }

                    GUI.enabled = true;

                    GUI.color = guiRed;
                    if (GUILayout.Button("x", "toolbarButton", GUILayout.Width(20)))
                    {
                        DeleteStartEndObject(_config, i);
                    }
                    GUI.color = Color.white;

                    EditorGUILayout.EndHorizontal();

                    if (i >= _config.positionObjects.Count)
                        return;

                    if (!_config.positionObjects[i].isChild)
                    {
                        EditorGUILayout.EndHorizontal();
                    }

                    if (_config.positionObjects[i].showPanel)
                    {
                        if (!_config.positionObjects[i].isChild)
                        {
                            EditorGUI.indentLevel = 2;
                            EditorGUILayout.BeginVertical();

                            _config.positionObjects[i].go = EditorGUILayout.ObjectField("gameobject: ", _config.positionObjects[i].go, typeof(GameObject), false) as GameObject;
                            _config.positionObjects[i].placementType = (TileWorldObjectScatterConfiguration.PositionObjectConfiguration.PlacementType)EditorGUILayout.EnumPopup("placement type:", _config.positionObjects[i].placementType);

                            if (_config.positionObjects[i].placementType == TileWorldObjectScatterConfiguration.PositionObjectConfiguration.PlacementType.mapBased && creator.configuration.global.selectedAlgorithm == 1)
                            {
                                EditorGUILayout.HelpBox("Does not work on cellular algorithm", MessageType.Info);
                            }

                            if (_config.positionObjects[i].placementType != TileWorldObjectScatterConfiguration.PositionObjectConfiguration.PlacementType.bestGuess)
                            {
                                _config.positionObjects[i].mapBasedSpawnPosition = (TileWorldObjectScatterConfiguration.PositionObjectConfiguration.MapBasedSpawnPosition)EditorGUILayout.EnumPopup("position: ", _config.positionObjects[i].mapBasedSpawnPosition);
                                _config.positionObjects[i].selectedLayer = 0;
                            }
                            else
                            {
                                _config.positionObjects[i].bestGuessSpawnPosition = (TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BestGuessSpawnPosition)EditorGUILayout.EnumPopup("position: ", _config.positionObjects[i].bestGuessSpawnPosition);
                                _config.positionObjects[i].selectedLayer = EditorGUILayout.Popup("place on layer:", _config.positionObjects[i].selectedLayer, placeLayers);
                                _config.positionObjects[i].blockType = (TileWorldObjectScatterConfiguration.PositionObjectConfiguration.BlockType)EditorGUILayout.EnumPopup("type:", _config.positionObjects[i].blockType);
                            }

                            _config.positionObjects[i].offsetPosition = EditorGUILayout.Vector3Field("offset position:", _config.positionObjects[i].offsetPosition);
                            _config.positionObjects[i].offsetRotation = EditorGUILayout.Vector3Field("offset rotation:", _config.positionObjects[i].offsetRotation);

                            //_config.startEndObjects[i].selectedLayer = EditorGUILayout.Popup(_config.startEndObjects[i].selectedLayer, placeLayers);
                            EditorGUILayout.EndVertical();
                            EditorGUI.indentLevel = 0;
                        }
                        else
                        {
                            ShowChildSpawnProperties(_config.positionObjects[i]);
                        }
                    }

                    GUI.enabled = true;
                }

                EditorGUILayout.EndVertical();

                if (twos == null ? GUI.enabled = false : GUI.enabled = true)

                if (!creator.mergeReady)
                {
                    EditorGUILayout.HelpBox("Build map before spawning position based objects", MessageType.Info);
                }

                if (creator != null)
                {
                    GUI.enabled = creator.mergeReady;
                }
                else
                {
                    GUI.enabled = false;
                }

                if (GUILayout.Button("Spawn position based objects", GUILayout.Height(30)))
                {
                    twos.ScatterPositionBasedObjects();
                }

                GUI.enabled = true;
            }

            EditorGUILayout.EndVertical();
        }
        else
        {
            EditorGUILayout.EndVertical();
        }
    }
    #endregion positionbasedobjects

    #region rulebasedobjects

    static void ShowRuleBasedObjects(TileWorldObjectScatterConfiguration _config)
    {
        GUI.color = colorGUI2;
        EditorGUILayout.BeginVertical("Box");
        GUI.color = Color.white;
        EditorGUILayout.BeginHorizontal("Box");

        ruleBasedObjectsFoldout = GUILayout.Toggle(ruleBasedObjectsFoldout, "Procedural objects", GUI.skin.GetStyle("foldout"), GUILayout.ExpandWidth(true), GUILayout.Height(18));
        EditorGUILayout.EndHorizontal();


        if (ruleBasedObjectsFoldout)
        {
            Event evt = Event.current;
            if (evt.type != EventType.DragPerform)
            {

                EditorGUILayout.BeginVertical("TextArea");


                for (int i = 0; i < _config.proceduralObjects.Count; i++)
                {

                    EditorGUILayout.BeginHorizontal();

                    if (_config.proceduralObjects[i].isChild)
                    {
                        GUILayout.Label("->", GUILayout.Width(30));
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal("Box");

                        GUI.color = colorGUI2;
                        GUILayout.Box(" ", GUILayout.Width(5));
                        GUI.color = Color.white;
                    }



                    if (i > 0)
                    {
                        if (GUILayout.Button(_config.proceduralObjects[i].isChild ? "<<" : ">>", "toolbarButton", GUILayout.Width(20)))
                        {
                            _config.proceduralObjects[i].isChild = !_config.proceduralObjects[i].isChild;
                        }
                    }

                    _config.proceduralObjects[i].active = GUILayout.Toggle(_config.proceduralObjects[i].active, _config.proceduralObjects[i].active ? "on" : "off", GUI.skin.GetStyle("toolbarButton"), GUILayout.Width(22));

                    GUI.enabled = _config.proceduralObjects[i].active;

                    _config.proceduralObjects[i].showPanel = GUILayout.Toggle(_config.proceduralObjects[i].showPanel, (_config.proceduralObjects[i].go != null ? _config.proceduralObjects[i].go.name : "missing game object"), GUI.skin.GetStyle("foldout"), GUILayout.ExpandWidth(true), GUILayout.Height(18));

                    // move up
                    if (i > 0)
                    {
                        GUI.enabled = true;
                    }
                    else
                    {
                        GUI.enabled = false;
                    }

                    if (GUILayout.Button("▲", "toolbarButton", GUILayout.Width(20)))
                    {
                        MoveProceduralObjectsUp(_config, i);
                    }

                    GUI.enabled = true;

                    // move down
                    if (_config.proceduralObjects.Count > 1 && i < _config.proceduralObjects.Count - 1)
                    {
                        GUI.enabled = true;
                    }
                    else
                    {
                        GUI.enabled = false;
                    }
                        if (GUILayout.Button("▼", "toolbarButton", GUILayout.Width(20)))
                        {
                            MoveProceduralObjectsDown(_config, i);
                        }

                    GUI.enabled = true;

                    // duplicate
                    if (GUILayout.Button("+", "toolbarButton", GUILayout.Width(20)))
                    {
                        DuplicateProceduralObject(_config, i);
                    }

                    // delete
                    GUI.color = guiRed;
                    if (GUILayout.Button("x", "toolbarButton", GUILayout.Width(20)))
                    {
                        DeleteProceduralObject(_config, i);
                    }
                    GUI.color = Color.white;

                    EditorGUILayout.EndHorizontal();

                    if (i >= _config.proceduralObjects.Count)
                        return;

                    if (!_config.proceduralObjects[i].isChild)
                    {
                        EditorGUILayout.EndHorizontal();
                    }

                    if (_config.proceduralObjects[i].showPanel)
                    {
                        if (!_config.proceduralObjects[i].isChild)
                        {
                            EditorGUI.indentLevel = 2;

                            EditorGUILayout.BeginVertical();
                            _config.proceduralObjects[i].go = EditorGUILayout.ObjectField("gameobject:", _config.proceduralObjects[i].go, typeof(GameObject), false) as GameObject;
                            _config.proceduralObjects[i].placeOnOccupiedCell = EditorGUILayout.Toggle("place on occupied cell:", _config.proceduralObjects[i].placeOnOccupiedCell);
	                        
	                        if (creator)
	                        {
	                            if (_config.proceduralObjects[i].selectedLayer < creator.configuration.global.layerCount && placeLayers.Length > 0)
	                            {
	                                _config.proceduralObjects[i].selectedLayer = EditorGUILayout.Popup("place on layer:", _config.proceduralObjects[i].selectedLayer, placeLayers);
	                            }
	                            else
	                            {
	                                _config.proceduralObjects[i].selectedLayer = 0;
	                            }
	                        }
	                        else
	                        {
	                        	_config.proceduralObjects[i].selectedLayer = 0;
	                        }

                            // select rule
                            EditorGUILayout.BeginVertical("Box");
                            _config.proceduralObjects[i].ruleType = (TileWorldObjectScatterConfiguration.ProceduralObjectConfiguration.RuleTypes)EditorGUILayout.EnumPopup("rule type:", _config.proceduralObjects[i].ruleType);

                            if (_config.proceduralObjects[i].ruleType == TileWorldObjectScatterConfiguration.ProceduralObjectConfiguration.RuleTypes.pattern)
                            {

                                //EditorGUILayout.BeginVertical("Box");
                                EditorGUILayout.HelpBox("Place the object on every Nth tiletype.", MessageType.Info);

                                _config.proceduralObjects[i].everyNTile = EditorGUILayout.IntField(new GUIContent("Nth:", "place the object on every Nth(every, second, third, fourth...) tile"), _config.proceduralObjects[i].everyNTile);

                                if (_config.proceduralObjects[i].everyNTile < 1)
                                {
                                    _config.proceduralObjects[i].everyNTile = 1;
                                }

                                _config.proceduralObjects[i].tileTypes = (TileWorldObjectScatterConfiguration.ProceduralObjectConfiguration.TileTypes)EditorGUILayout.EnumPopup("tiletype:", _config.proceduralObjects[i].tileTypes);
                                _config.proceduralObjects[i].inset = EditorGUILayout.IntField("inset:", _config.proceduralObjects[i].inset);
                                //EditorGUILayout.EndVertical();
                            }
                            else
                            {
                                //EditorGUILayout.BeginVertical("Box");

                                _config.proceduralObjects[i].weight = EditorGUILayout.Slider("weight:", _config.proceduralObjects[i].weight, 0, 1);
                                _config.proceduralObjects[i].tileTypes = (TileWorldObjectScatterConfiguration.ProceduralObjectConfiguration.TileTypes)EditorGUILayout.EnumPopup("place on:", _config.proceduralObjects[i].tileTypes);
                                _config.proceduralObjects[i].inset = EditorGUILayout.IntField("inset:", _config.proceduralObjects[i].inset);
                                //EditorGUILayout.EndVertical();
                            }

                            EditorGUILayout.EndVertical();

                            _config.proceduralObjects[i].offsetPosition = EditorGUILayout.Vector3Field("offset position:", _config.proceduralObjects[i].offsetPosition);
                            _config.proceduralObjects[i].offsetRotation = EditorGUILayout.Vector3Field("offset rotation:", _config.proceduralObjects[i].offsetRotation);

                            // Use random rotation
                            EditorGUILayout.BeginVertical("Box");
                            _config.proceduralObjects[i].useTileRotation = EditorGUILayout.Toggle("use tile rotations:", _config.proceduralObjects[i].useTileRotation);
                            GUI.enabled = !_config.proceduralObjects[i].useTileRotation;
                            _config.proceduralObjects[i].useRandomRotation = EditorGUILayout.Toggle("use random rotation:", _config.proceduralObjects[i].useRandomRotation);
                            GUI.enabled = _config.proceduralObjects[i].useRandomRotation;


                            GUI.enabled = !_config.proceduralObjects[i].useTileRotation && _config.proceduralObjects[i].useRandomRotation;
                            _config.proceduralObjects[i].randomRotationMin = EditorGUILayout.Vector3Field("min random rotation:", _config.proceduralObjects[i].randomRotationMin);
                            _config.proceduralObjects[i].randomRotationMax = EditorGUILayout.Vector3Field("max random rotation:", _config.proceduralObjects[i].randomRotationMax);
                            GUI.enabled = true;

                            EditorGUILayout.EndVertical();

                            EditorGUILayout.BeginVertical("Box");

                            _config.proceduralObjects[i].useRandomScaling = EditorGUILayout.Toggle("use random scaling:", _config.proceduralObjects[i].useRandomScaling);
                            GUI.enabled = _config.proceduralObjects[i].useRandomScaling;
                            _config.proceduralObjects[i].uniformScaling = EditorGUILayout.Toggle("uniform scaling:", _config.proceduralObjects[i].uniformScaling);

                            if (_config.proceduralObjects[i].uniformScaling)
                            {
                                _config.proceduralObjects[i].randomScalingMin.x = EditorGUILayout.FloatField("min random scaling:", _config.proceduralObjects[i].randomScalingMin.x);
                                _config.proceduralObjects[i].randomScalingMax.x = EditorGUILayout.FloatField("max random scaling:", _config.proceduralObjects[i].randomScalingMax.x);


                                _config.proceduralObjects[i].randomScalingMin = new Vector3(_config.proceduralObjects[i].randomScalingMin.x, _config.proceduralObjects[i].randomScalingMin.x, _config.proceduralObjects[i].randomScalingMin.x);
                                _config.proceduralObjects[i].randomScalingMax = new Vector3(_config.proceduralObjects[i].randomScalingMax.x, _config.proceduralObjects[i].randomScalingMax.x, _config.proceduralObjects[i].randomScalingMax.x);
                            }
                            else
                            {
                                _config.proceduralObjects[i].randomScalingMin = EditorGUILayout.Vector3Field("min random scaling: ", _config.proceduralObjects[i].randomScalingMin);
                                _config.proceduralObjects[i].randomScalingMax = EditorGUILayout.Vector3Field("max random scaling: ", _config.proceduralObjects[i].randomScalingMax);
                            }
                            GUI.enabled = true;
                            EditorGUILayout.EndVertical();



                            EditorGUILayout.EndVertical();
                            EditorGUI.indentLevel = 0;
                        }
                        else
                        {
                            ShowChildSpawnProperties(_config.proceduralObjects[i]);
                        }
                    }

                    GUI.enabled = true;
                }

                EditorGUILayout.EndVertical();

                if (twos == null ? GUI.enabled = false : GUI.enabled = true)

                if (!creator.mergeReady)
                {
                    EditorGUILayout.HelpBox("Build map before spawning procedural objects", MessageType.Info);
                }

                if (creator != null)
                {
                    GUI.enabled = creator.mergeReady;
                }
                else
                {
                    GUI.enabled = false;
                }

                if (GUILayout.Button("Spawn procedural objects", GUILayout.Height(30)))
                {
                    twos.ScatterProceduralObjects();
                }

                GUI.enabled = true;
            }

            EditorGUILayout.EndVertical();
        }
        else
        {
            EditorGUILayout.EndVertical();
        }
    }

    #endregion rulebasedobjects


    static void DeleteStartEndObject(TileWorldObjectScatterConfiguration _config, int _index)
    {
        if (_index + 1 < _config.positionObjects.Count)
        {
            _config.positionObjects[_index + 1].isChild = false;
        }

        _config.positionObjects.RemoveAt(_index);

        EditorUtility.SetDirty(_config);
    
    }

    static void DeleteProceduralObject(TileWorldObjectScatterConfiguration _config, int _index)
    {
        if (_index + 1 < _config.proceduralObjects.Count)
        {
            _config.proceduralObjects[_index + 1].isChild = false;
        }

        _config.proceduralObjects.RemoveAt(_index);

        EditorUtility.SetDirty(_config);
    }

    static void DeletePaintObjects(TileWorldObjectScatterConfiguration _config, int _index)
    {
        if (_index + 1 < _config.paintObjects.Count)
        {
            _config.paintObjects[_index + 1].isChild = false;
        }

        _config.paintObjects.RemoveAt(_index);

        EditorUtility.SetDirty(_config);
    }

    static void DuplicateProceduralObject(TileWorldObjectScatterConfiguration _config, int _index)
    {
        _config.proceduralObjects.Add(new TileWorldObjectScatterConfiguration.ProceduralObjectConfiguration(_config.proceduralObjects[_index]));
    }

    static void MovePositionObjectsUp(TileWorldObjectScatterConfiguration _config, int _index)
    {
        var _item = _config.positionObjects[_index];
        _config.positionObjects.RemoveAt(_index);
        _config.positionObjects.Insert(_index - 1, _item);
    }


    static void MovePositionObjectsDown(TileWorldObjectScatterConfiguration _config, int _index)
    {
        var _item = _config.positionObjects[_index];
        _config.positionObjects.RemoveAt(_index);
        _config.positionObjects.Insert(_index + 1, _item);
    }


    static void MoveProceduralObjectsUp(TileWorldObjectScatterConfiguration _config, int _index)
    {
        var _item = _config.proceduralObjects[_index];
        _config.proceduralObjects.RemoveAt(_index);
        _config.proceduralObjects.Insert(_index - 1, _item);
    }


    static void MoveProceduralObjectsDown(TileWorldObjectScatterConfiguration _config, int _index)
    {
        var _item = _config.proceduralObjects[_index];
        _config.proceduralObjects.RemoveAt(_index);
        _config.proceduralObjects.Insert(_index + 1, _item);
    }

    static void ShowChildSpawnProperties(TileWorldObjectScatterConfiguration.DefaultObjectConfiguration _class)
    {
        EditorGUI.indentLevel = 2;
        _class.go = EditorGUILayout.ObjectField("gameobject: ", _class.go, typeof(GameObject), false) as GameObject;
        _class.radius = EditorGUILayout.FloatField("radius: ", _class.radius);
        _class.spawnCount = EditorGUILayout.IntField("spawn count: ", _class.spawnCount);
        _class.offsetPosition = EditorGUILayout.Vector3Field("offset position:", _class.offsetPosition);

        EditorGUILayout.BeginVertical("Box");
        _class.useRandomRotation = EditorGUILayout.Toggle("use random rotation:", _class.useRandomRotation);

        GUI.enabled = _class.useRandomRotation;
        _class.randomRotationMin = EditorGUILayout.Vector3Field("min random rotation: ", _class.randomRotationMin);
        _class.randomRotationMax = EditorGUILayout.Vector3Field("max random rotation: ", _class.randomRotationMax);
        GUI.enabled = true;
        EditorGUILayout.EndVertical();


        EditorGUILayout.BeginVertical("Box");

        _class.useRandomScaling = EditorGUILayout.Toggle("use random scaling:", _class.useRandomScaling);
        GUI.enabled = _class.useRandomScaling;
        _class.uniformScaling = EditorGUILayout.Toggle("uniform scaling:", _class.uniformScaling);

        if (_class.uniformScaling)
        {
            _class.randomScalingMin.x = EditorGUILayout.FloatField("min random scaling:", _class.randomScalingMin.x);
            _class.randomScalingMax.x = EditorGUILayout.FloatField("max random scaling:", _class.randomScalingMax.x);


            _class.randomScalingMin = new Vector3(_class.randomScalingMin.x, _class.randomScalingMin.x, _class.randomScalingMin.x);
            _class.randomScalingMax = new Vector3(_class.randomScalingMax.x, _class.randomScalingMax.x, _class.randomScalingMax.x);
        }
        else
        {
            _class.randomScalingMin = EditorGUILayout.Vector3Field("min random scaling: ", _class.randomScalingMin);
            _class.randomScalingMax = EditorGUILayout.Vector3Field("max random scaling: ", _class.randomScalingMax);
        }
        GUI.enabled = true;
        EditorGUILayout.EndVertical();

        EditorGUI.indentLevel = 0;
    }

    /// <summary>
    /// Load editor icons
    /// </summary>
    public void LoadResources()
    {
        var _path = ReturnInstallPath.GetInstallPath("Editor", this); // GetInstallPath();
  
        iconMaskLayerArrow = AssetDatabase.LoadAssetAtPath(_path + "Res/masklayerarrow.png", typeof(Texture2D)) as Texture2D;
    }

}
