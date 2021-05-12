/* TILE WORLD CREATOR
 * Copyright (c) 2015 doorfortyfour OG
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
#if UNITY_5_3_OR_NEWER || UNITY_5_3
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
#endif

using UnityEditor;
using System.IO;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using TileWorld;


[CustomEditor(typeof(TileWorldCreator))]
public class TileWorldCreatorEditor : Editor
{

    //-------------------------------------------------------

    //REFERENCE
    public TileWorldCreator creator;

    //Undo stack
    public TileWorldUndoStack<bool[]> undoStack;
    public TileWorldUndoStack<bool[]> redoStack;

    //Editor
    Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, 0f, 0f));
    Vector3 currentCameraPosition;
    Vector3 lastCameraPosition;


    //GUI Images
    Texture2D iconLayer;
    Texture2D iconFillFloor;
    Texture2D iconFillBlock;
    Texture2D iconDuplicate;
    Texture2D iconMaskLayerArrow;
    Texture2D iconUndockWindow;
    Texture2D iconCopyMap;
    Texture2D iconPasteMap;
    Texture2D iconCurrentCopyMode;
    Texture2D iconOptionMenu;
    //Texture2D iconStore;

    Texture2D topLogo;

    //GUI Colors
    //Color guiGreen = new Color (0f / 255f, 255f / 255f, 0f / 255f);
    Color guiRed = new Color(255f / 255f, 100f / 255f, 100f / 255f);
    Color guiBlue = new Color(190f / 255f, 230f / 255f, 230f / 255f); //new Color(0f / 255f, 180f / 255f, 255f / 255f);
    Color guiBluelight = new Color(180f / 255f, 240f / 255f, 255f / 255f);
    Color colorGUIFoldout3 = new Color(0f / 255f, 50f / 255f, 100f / 255f);

    bool[] tmpMap = new bool[0];
    //save the cells user has painted on, so we know where we should do an optimization pass
    //bool[,] paintRegisterMap = new bool[0, 0];


    string[] _maskNames;
    public string[] algorithmNames;

    //string currentScene;
#if UNITY_5_3_OR_NEWER || UNITY_5_3
    Scene currentScene;
#else
        string currentScene;
#endif

    //----------------------------------------------------------------

    private void OnEnable()
    {

        //get script Reference
        creator = (TileWorldCreator)target;

        //SceneView.onSceneGUIDelegate = GridUpdate;

        LoadResources();

        undoStack = new TileWorldUndoStack<bool[]>();
        redoStack = new TileWorldUndoStack<bool[]>();

        //get all available masks
        TileWorldMaskLookup.GetMasks(out creator.iMasks, out _maskNames);

        //get all available algorithms
        TileWorldAlgorithmLookup.GetAlgorithms(out creator.iAlgorithms, out algorithmNames);

#if UNITY_5_3_OR_NEWER || UNITY_5_3
        currentScene = EditorSceneManager.GetActiveScene();
#else
       currentScene = EditorApplication.currentScene;
#endif
        EditorApplication.hierarchyWindowChanged += HierarchyWindowChanged;
    }


    void HierarchyWindowChanged()
    {
#if UNITY_5_3_OR_NEWER || UNITY_5_3
        if (currentScene != EditorSceneManager.GetActiveScene())
#else
        if (currentScene != EditorApplication.currentScene)
#endif
        {

            //reset first time build on startup
            TileWorldCreator[] _creator = GameObject.FindObjectsOfType<TileWorldCreator>();
            for (int i = 0; i < _creator.Length; i++)
            {
                _creator[i].firstTimeBuild = true;
            }

#if  UNITY_5_3_OR_NEWER || UNITY_5_3
            currentScene = EditorSceneManager.GetActiveScene();
#else
            currentScene = EditorApplication.currentScene;
#endif
        }
    }


    void Callback(object obj)
    {
        //Debug.Log("selected: " + obj);
        switch (obj.ToString())
        {
            case "item 1":
                LoadMap();
                break;
            case "item 2":
                Save();
                break;
            //case "item 3":
            //    TileWorldTileStore.InitStore();
                //break;
            case "item 4":
                TileWorldAbout.InitAbout();
                break;
            case "item 5":
                Application.OpenURL("http://tileworldcreator.doorfortyfour.com/documentation");
                break;
        }
    }


    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal("Box");

        GUILayout.Label(topLogo);

        //if (GUILayout.Button(iconStore, GUILayout.Width(40), GUILayout.Height(40)))
        //{
        //    TileWorldTileStore.InitStore();
        //}

        if (GUILayout.Button(iconOptionMenu, GUILayout.Height(40), GUILayout.Width(40)))
        {
            Event evt = Event.current;
            Rect contextRect = new Rect(0, 0, Screen.width, 300);

            Vector2 mousePos = evt.mousePosition;
            if (contextRect.Contains(mousePos))
            {
                // create menu
                GenericMenu menu = new GenericMenu();

                menu.AddItem(new GUIContent("Load Map"), false, Callback, "item 1");
                menu.AddItem(new GUIContent("Save Map"), false, Callback, "item 2");
                menu.AddSeparator("");
                //menu.AddItem(new GUIContent("Check for new tiles"), false, Callback, "item 3");
                menu.AddItem(new GUIContent("About"), false, Callback, "item 4");
                menu.AddItem(new GUIContent("Help"), false, Callback, "item 5");

                menu.ShowAsContext();
                evt.Use();
            }
        }

        EditorGUILayout.EndHorizontal();


        if (creator.configuration == null)
        {
            EditorGUILayout.HelpBox("Please create or load an asset configuration file.", MessageType.Info);
        }

        //CONFIGURATION FILE OPTIONS
        EditorGUILayout.BeginHorizontal();

        creator.configuration = (TileWorldConfiguration)EditorGUILayout.ObjectField(creator.configuration, typeof(TileWorldConfiguration), true) as TileWorldConfiguration;

        if (GUILayout.Button("create config.", "ToolBarButton"))
        {
            TileWorldConfigurationEditor.CreateConfigFile(creator);
        }

        if (GUILayout.Button("load config.", "ToolBarButton"))
        {

            var _configPath = EditorUtility.OpenFilePanel("load configuration file", "Assets", "asset");
            //make path relative
            if (_configPath != "")
            {
                _configPath = "Assets" + _configPath.Substring(Application.dataPath.Length);

                creator.configuration = (TileWorldConfiguration)UnityEditor.AssetDatabase.LoadAssetAtPath(_configPath, typeof(TileWorldConfiguration));
            }
        }

        EditorGUILayout.EndHorizontal();



        if (creator.configuration == null)
        {
            if (GUI.changed)
            {
                creator.firstTimeBuild = true;
#if  UNITY_5_3_OR_NEWER || UNITY_5_3
                EditorSceneManager.MarkSceneDirty(this.currentScene);
#else
                EditorApplication.MarkSceneDirty();
#endif
            }

            return;
        }

        // Show configuration editor
        //--------------------------
        TileWorldConfigurationEditor.ShowConfigurationEditor(creator.configuration);


        //SHOW EDIT MAP
        //-------------
        GUI.color = colorGUIFoldout3;
        EditorGUILayout.BeginVertical("Box");
        GUI.color = Color.white;
        GUILayout.BeginHorizontal("Box");
        creator.configuration.ui.showEdit = GUILayout.Toggle(creator.configuration.ui.showEdit, ("Edit map"), GUI.skin.GetStyle("foldout"), GUILayout.ExpandWidth(true), GUILayout.Height(18));

        if (GUILayout.Button(iconUndockWindow, "ToolbarButton", GUILayout.Width(25)))
        {
            TileWorldCreatorEditorEditMapWindow.InitWindow(creator.gameObject);
        }

        GUILayout.EndHorizontal();

        if (creator.configuration.ui.showEdit)
        {
            ShowEdit();
        }
        EditorGUILayout.EndVertical();

        if (GUILayout.Button("Generate New", GUILayout.Height(30)))
        {
            //reset undo stack
            undoStack = new TileWorldUndoStack<bool[]>();
            redoStack = new TileWorldUndoStack<bool[]>();

            //paintRegisterMap = new bool[creator.configuration.global.width, creator.configuration.global.height];


            creator.GenerateMaps();

            for (int li = 0; li < creator.configuration.global.layerCount; li++)
            {
                creator.configuration.worldMap[li].maskMap = new bool[creator.configuration.global.width, creator.configuration.global.height];
                creator.configuration.worldMap[li].maskMap = creator.iMasks[creator.configuration.worldMap[li].selectedMask].ApplyMask(creator.configuration.worldMap[li].cellMap, creator.GetComponent<TileWorldCreator>(), creator.configuration);
            }
        }

        if (GUILayout.Button("Build", GUILayout.Height(30)))
        {
            if (creator.firstTimeBuild)
            {
                //creator.BuildMapEditor();
                creator.BuildMapComplete(false, false, true);

            }
            else
            {
                if (creator.configuration.global.buildOverlappingTiles)
                {
                    //creator.BuildMapPartial(true);
                    //creator.BuildMapPartial(false, false, 0, 0);
                    creator.BuildMapPartial(false, false);
                }
                else
                {
                    creator.firstTimeBuild = true;
                    //creator.BuildMapEditor();
                    creator.BuildMapComplete(false, false, true);
                }
            }
        }


        if (creator.mergeReady)
        {
            GUI.enabled = true;
        }
        else
        {
            GUI.enabled = false;
        }

        if (GUILayout.Button("Merge", GUILayout.Height(30)))
        {
            Merge();
            creator.mergeReady = false;
        }

        GUI.enabled = true;


        SceneView.RepaintAll();

        if (GUI.changed)
        {
            creator.firstTimeBuild = true;
#if  UNITY_5_3_OR_NEWER || UNITY_5_3
            EditorSceneManager.MarkSceneDirty(this.currentScene);
#else
            EditorApplication.MarkSceneDirty();
#endif
        }
	    
	    EditorUtility.SetDirty(creator);
	    
	    if (creator.configuration != null)
	    {
	    	EditorUtility.SetDirty(creator.configuration);
	    }
	    
        

    }


    //SHOW EDIT
    //------------------
    public void ShowEdit()
    {
        //SHOW EDIT
        GUILayout.BeginVertical("Box", GUILayout.ExpandWidth(false));


        EditorGUILayout.HelpBox("Left Click Add / Right Click Remove Cell" + "\n" + "Undo/Redo: Z + Y" + "\n" + "Show/Hide grid: H", MessageType.Info);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Show Grid:");
        creator.configuration.ui.showGrid = EditorGUILayout.Toggle(creator.configuration.ui.showGrid);

        GUILayout.Label("Show Grid on deselect:");
        creator.configuration.ui.showGridAlways = EditorGUILayout.Toggle(creator.configuration.ui.showGridAlways);
        GUILayout.EndHorizontal();

        creator.configuration.ui.autoBuild = EditorGUILayout.Toggle("Automatic build:", creator.configuration.ui.autoBuild);

        //GUILayout.BeginHorizontal();
        //creator.brushx2 = EditorGUILayout.Toggle("Brush size x2:", creator.brushx2);
        //GUILayout.Label("enable recommended");
        //GUILayout.EndHorizontal();
        creator.configuration.ui.brushSize = EditorGUILayout.IntSlider("Brush size:", creator.configuration.ui.brushSize, 2, 5);

        creator.configuration.ui.cellColor = EditorGUILayout.ColorField("Cell Color:", creator.configuration.ui.cellColor);
        creator.configuration.ui.floorCellColor = EditorGUILayout.ColorField("Floor Color:", creator.configuration.ui.floorCellColor);
        creator.configuration.ui.gridColor = EditorGUILayout.ColorField("Grid Color:", creator.configuration.ui.gridColor);
        creator.configuration.ui.brushColor = EditorGUILayout.ColorField("Brush Color:", creator.configuration.ui.brushColor);
        
        creator.configuration.ui.maskColor = EditorGUILayout.ColorField("Mask Color:", creator.configuration.ui.maskColor);

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.BeginHorizontal();


        //fill layer
        if (GUILayout.Button(iconFillBlock, GUILayout.Width(35), GUILayout.Height(35)))
        {
            if (!creator.configuration.worldMap[creator.configuration.ui.mapIndex].paintMask)
            {
                creator.FillLayerBlock();
            }
            else
            {
                creator.configuration.worldMap[creator.configuration.ui.mapIndex].maskMap = new bool[creator.configuration.global.width, creator.configuration.global.height];
            }
        }

        if (GUILayout.Button(iconFillFloor, GUILayout.Width(35), GUILayout.Height(35)))
        {
            if (!creator.configuration.worldMap[creator.configuration.ui.mapIndex].paintMask)
            {
                creator.FillLayerGround();
            }
            else
            {
                for (int ym = 0; ym < creator.configuration.worldMap[creator.configuration.ui.mapIndex].maskMap.GetLength(1); ym++)
                {
                    for (int xm = 0; xm < creator.configuration.worldMap[creator.configuration.ui.mapIndex].maskMap.GetLength(0); xm++)
                    {
                        creator.configuration.worldMap[creator.configuration.ui.mapIndex].maskMap[xm, ym] = true;
                    }
                }
            }
        }

        //copy / paste layer    
        //copy map from layer
        if (GUILayout.Button(iconCopyMap, GUILayout.Width(35), GUILayout.Height(35)))
        {
            creator.CopyMapFromLayer();
        }
        //paste map to layer
        if (GUILayout.Button(iconPasteMap, GUILayout.Width(35), GUILayout.Height(35)))
        {
            creator.PasteMapToLayer();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        GUILayout.BeginVertical("Box");
        GUILayout.BeginHorizontal();
        GUILayout.Label(iconLayer, GUILayout.Width(20), GUILayout.Height(20));
        GUILayout.Label("Layers");
        GUILayout.EndHorizontal();


        GUILayout.BeginVertical("TextArea");

        //show layers
        for (int l = creator.configuration.worldMap.Count - 1; l >= 0; l--)
        {
            if (creator.configuration.ui.mapIndex == l)
            {
                GUI.color = guiBlue;
            }

            
            GUILayout.BeginHorizontal("Box");
            GUI.color = Color.white;

            GUILayout.Label(iconLayer, GUILayout.Width(20), GUILayout.Height(18));
            if (GUILayout.Button("Layer: " + (l + 1).ToString(), "Label", GUILayout.Height(18)))
            {
                creator.configuration.ui.mapIndex = l;

                for (int pm = 0; pm < creator.configuration.worldMap.Count; pm++)
                {
                    creator.configuration.worldMap[pm].paintMask = false;
                }

            }

            creator.configuration.global.layerPresetIndex[l] = EditorGUILayout.Popup(creator.configuration.global.layerPresetIndex[l], creator.configuration.ui.availablePresets);
            //EditorGUILayout.LayerField(0, GUILayout.Width(50));         

            //duplicate layer
            if (GUILayout.Button(iconDuplicate, "ToolbarButton", GUILayout.Height(18), GUILayout.Width(20)))
            {
                creator.AddNewLayer(true, false, 0, true, l);
            }


            GUI.color = guiRed;
            if (l != 0)
            {
                if (GUILayout.Button("x", "ToolbarButton", GUILayout.Height(18), GUILayout.Width(20)))
                {
                    creator.RemoveLayer(l);
                }
            }
            GUI.color = Color.white;

            GUILayout.EndHorizontal();


            //Mask Layers
            if (l < creator.configuration.worldMap.Count)
            {
                if (creator.configuration.worldMap[l].useMask)
                {
                    if (creator.configuration.worldMap[l].paintMask)
                    {
                        GUI.color = guiBluelight;
                    }

                    GUILayout.BeginHorizontal();

                    GUILayout.Label(iconMaskLayerArrow, GUILayout.Width(20), GUILayout.Height(18));

                    GUILayout.BeginHorizontal("Box");
                    GUI.color = Color.white;

                    if (GUILayout.Button("Select", "ToolbarButton", GUILayout.Height(18)))
                    {
                        creator.configuration.ui.mapIndex = l;


                        for (int pm = 0; pm < creator.configuration.worldMap.Count; pm++)
                        {


                            if (pm == creator.configuration.ui.mapIndex)
                            {
                                if (!creator.configuration.worldMap[l].paintMask)
                                {
                                    creator.configuration.worldMap[l].paintMask = true;
                                }
                                else
                                {
                                    creator.configuration.worldMap[l].paintMask = false;
                                }
                            }
                            else { creator.configuration.worldMap[pm].paintMask = false; }
                        }


                    }

                    GUILayout.Label("Mask: " + (l + 1).ToString());

                    GUI.color = Color.white;

                    creator.configuration.worldMap[l].selectedMask = EditorGUILayout.Popup(creator.configuration.worldMap[l].selectedMask, _maskNames);

                    if (GUILayout.Button("apply", "ToolbarButton", GUILayout.Height(15), GUILayout.Width(50)))
                    {
                        creator.configuration.worldMap[l].maskMap = new bool[creator.configuration.global.width, creator.configuration.global.height];
                        creator.configuration.worldMap[l].maskMap = creator.iMasks[creator.configuration.worldMap[l].selectedMask].ApplyMask(creator.configuration.worldMap[l].cellMap, creator.GetComponent<TileWorldCreator>(), creator.configuration);


                        if (creator.firstTimeBuild)
                        {
                            //creator.BuildMapEditor();
                            creator.BuildMapComplete(false, false, true);


                        }
                        else
                        {
                            if (creator.configuration.global.buildOverlappingTiles)
                            {
                                //creator.BuildMapPartial(true);
                                //creator.BuildMapPartial(false, false, 0, 0);
                                creator.BuildMapPartial(false, false);
                            }
                            else
                            {
                                creator.firstTimeBuild = true;
                                //creator.BuildMapEditor();
                                creator.BuildMapComplete(false, false, true);
                            }
                        }

                        //update one dim array to make sure they are updated when saving map
                        creator.UpdateMap();
                    }

                    GUI.color = guiRed;
                    if (GUILayout.Button("x", "ToolbarButton", GUILayout.Height(15), GUILayout.Width(20)))
                    {
                        creator.configuration.worldMap[l].useMask = false;
                    }
                    GUI.color = Color.white;

                    GUILayout.EndHorizontal();

                    GUILayout.EndHorizontal();
                    GUILayout.Space(4);
                }
            }


            //GUILayout.Space(6);
        }
        GUILayout.Space(6);
        GUILayout.EndVertical();


        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Add new layer"))
        {
            creator.AddNewLayer(true, false, 0, false, 0);
        }

        if (GUILayout.Button("Add mask"))
        {
            if (!creator.configuration.worldMap[creator.configuration.ui.mapIndex].useMask)
            {
                creator.AddNewMask(creator.configuration.ui.mapIndex);
            }
        }

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();


        GUILayout.EndVertical();
    }


    private void OnSceneGUI()
    {
	    if (creator.configuration == null || !creator.showGrid || EditorApplication.isPlaying)
            return;

        if (!creator.configuration.ui.showGrid)
        {
            Event _eventH = Event.current;

            //Show grid
            if (_eventH.type == EventType.KeyDown)
            {
                if (_eventH.keyCode == KeyCode.H)
                {
                    creator.configuration.ui.showGrid = true;
                }
            }

            return;
        }


        Event _event = Event.current;

        //get mouse worldposition
        creator.mouseWorldPosition = GetWorldPosition(_event.mousePosition);

        //Check Mouse Events
        if (_event.type == EventType.MouseDown)
        {
            //paintRegisterMap = new bool[creator.configuration.global.width, creator.configuration.global.height];

            //On Mouse Down store current camera position and set last position to current
            currentCameraPosition = Camera.current.transform.position;
            lastCameraPosition = currentCameraPosition;

            if (_event.button == 0)
            {

                if (IsMouseOverGrid(_event.mousePosition) && lastCameraPosition == currentCameraPosition)
                {
                    RegisterMap();
                    if (creator.configuration.global.invert)
                    {
                        PaintCell(true);
                    }
                    else
                    {
                        PaintCell(false);
                    }
                }
            }
            else if (_event.button == 1)
            {
                if (IsMouseOverGrid(_event.mousePosition) && lastCameraPosition == currentCameraPosition)
                {
                    RegisterMap();

                    if (creator.configuration.global.invert)
                    {
                        PaintCell(false);
                    }
                    else
                    {
                        PaintCell(true);
                    }
                    //block right mouse button navigation input
                    _event.Use();
                }
            }
        }

        // If Mouse Drag and left click. Start paint
        else if (_event.type == EventType.MouseDrag && _event.type != EventType.KeyDown)
        {
            //get current camera position, if camera position were unchanged we can be sure
            //user did not navigate in the scene
            currentCameraPosition = Camera.current.transform.position;
            //add cell
            if (_event.button == 0)
            {
                if (IsMouseOverGrid(_event.mousePosition) && lastCameraPosition == currentCameraPosition)
                {

                    if (creator.configuration.global.invert)
                    {
                        PaintCell(true);
                    }
                    else
                    {
                        PaintCell(false);
                    }
                }
            }

            //remove cell
            else if (_event.button == 1)
            {
                if (IsMouseOverGrid(_event.mousePosition) && lastCameraPosition == currentCameraPosition)
                {
                    if (creator.configuration.global.invert)
                    {
                        PaintCell(false);
                    }
                    else
                    {
                        PaintCell(true);
                    }


                    //block right mouse button navigation input
                    _event.Use();
                }
            }

        }

        else if (_event.type == EventType.MouseMove)
        {
            //check if mouse pointer is over grid
            creator.mouseOverGrid = IsMouseOverGrid(_event.mousePosition);
        }

        else if (_event.type == EventType.MouseUp)
        {
            currentCameraPosition = Camera.current.transform.position;
            lastCameraPosition = currentCameraPosition;

            if (IsMouseOverGrid(_event.mousePosition) && lastCameraPosition == currentCameraPosition)
            {
                Vector3 _gP = GetGridPosition(creator.mouseWorldPosition);
                RegisterUndo(_gP);
            }

#if UNITY_4_6
            Screen.showCursor = true;
#elif UNITY_5_0
		Cursor.visible = true;
#endif
        }

        else if (_event.type == EventType.Layout)
        {
            //this allows _event.Use() to actually function and block mouse input
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
        }



        else if (_event.type == EventType.KeyDown)
        {
            //UNDO
            if (_event.keyCode == (KeyCode.Z))
            {
                _event.Use();
                DoUndo();
            }

            //REDO
            else if (_event.keyCode == (KeyCode.Y))
            {
                _event.Use();
                DoRedo();
            }

            //Hide grid
            else if (_event.keyCode == KeyCode.H)
            {

                creator.configuration.ui.showGrid = false;

            }

        }
#if  UNITY_5_3_OR_NEWER || UNITY_5_3
        EditorSceneManager.MarkSceneDirty(this.currentScene);
#else
        EditorUtility.SetDirty(this);
#endif
    }


    //Paint cell true = add // false = delete
    void PaintCell(bool _add)
    {
        if (creator.configuration.worldMap.Count < 1)
            return;

        if (_add)
        {

            Vector3 _gP = GetGridPosition(creator.mouseWorldPosition);

            //if (!creator.brushx2)
            //{
            //    //register painted cell
            //    paintRegisterMap[(int)_gP.x, (int)_gP.z] = true;


            //    creator.configuration.worldMap[creator.configuration.ui.mapIndex].map[(int)_gP.x, (int)_gP.z] = true;
            //}
            //else
            //{
                //if x or z position of mouse is out of grid
                //set a correct int value
                if (_gP.x < 0)
                {
                    _gP.x = -1;
                }
                if (_gP.z < 0)
                {
                    _gP.z = -1;
                }


                for (int y = 0; y < creator.configuration.ui.brushSize; y++)
                {
                    for (int x = 0; x < creator.configuration.ui.brushSize; x++)
                    {
                        if (_gP.x + x >= 0 && _gP.z + y >= 0 && _gP.x + x < creator.configuration.worldMap[creator.configuration.ui.mapIndex].cellMap.GetLength(0) && _gP.z + y < creator.configuration.worldMap[creator.configuration.ui.mapIndex].cellMap.GetLength(1))
                        {
                            if (!creator.configuration.worldMap[creator.configuration.ui.mapIndex].paintMask)
                            {
                                creator.configuration.worldMap[creator.configuration.ui.mapIndex].cellMap[(int)_gP.x + x, (int)_gP.z + y] = true;
                            }
                            else
                            {
                                creator.configuration.worldMap[creator.configuration.ui.mapIndex].maskMap[(int)_gP.x + x, (int)_gP.z + y] = true;
                            }

                            //register painted cell
                            //paintRegisterMap[(int)_gP.x + x, (int)_gP.z + y] = true;

                        }
                    }
                }


            //}

            creator.UpdateMap();
            EditorUtility.SetDirty(creator);

        }
        else
        {

            Vector3 _gP = GetGridPosition(creator.mouseWorldPosition);

            //if (!creator.brushx2)
            //{
            //    //register painted cell
            //    paintRegisterMap[(int)_gP.x, (int)_gP.z] = true;


            //    creator.configuration.worldMap[creator.configuration.ui.mapIndex].map[(int)_gP.x, (int)_gP.z] = false;
            //}
            //else
            //{

                if (_gP.x < 0)
                {
                    _gP.x = -1;
                }
                if (_gP.z < 0)
                {
                    _gP.z = -1;
                }


                for (int y = 0; y < creator.configuration.ui.brushSize; y++)
                {
                    for (int x = 0; x < creator.configuration.ui.brushSize; x++)
                    {
                        if (_gP.x + x >= 0 && _gP.z + y >= 0 && _gP.x + x < creator.configuration.worldMap[creator.configuration.ui.mapIndex].cellMap.GetLength(0) && _gP.z + y < creator.configuration.worldMap[creator.configuration.ui.mapIndex].cellMap.GetLength(1))
                        {
                            if (!creator.configuration.worldMap[creator.configuration.ui.mapIndex].paintMask)
                            {
                                creator.configuration.worldMap[creator.configuration.ui.mapIndex].cellMap[(int)_gP.x + x, (int)_gP.z + y] = false;
                            }
                            else
                            {
                                creator.configuration.worldMap[creator.configuration.ui.mapIndex].maskMap[(int)_gP.x + x, (int)_gP.z + y] = false;
                            }

                            //register painted cell
                            //paintRegisterMap[(int)_gP.x + x, (int)_gP.z + y] = true;

                        }
                    }
                }

            //}

            creator.UpdateMap();
            EditorUtility.SetDirty(creator);

#if UNITY_4_6
            Screen.showCursor = false;
#elif UNITY_5_0
		Cursor.visible = false;
#endif
        }




    }

    private void DoUndo()
    {
        if (undoStack.Count > 0)
        {
            redoStack.Push(creator.configuration.worldMap[creator.configuration.ui.mapIndex].cellMapSingle);

            bool[] _tmp = undoStack.Pop();

            if (_tmp != null)
            {
                creator.configuration.worldMap[creator.configuration.ui.mapIndex].cellMap = new bool[creator.configuration.global.width, creator.configuration.global.height];
                creator.configuration.worldMap[creator.configuration.ui.mapIndex].cellMapSingle = _tmp;
                int _index = 0;

                for (int y = 0; y < creator.configuration.global.height; y++)
                {
                    for (int x = 0; x < creator.configuration.global.width; x++)
                    {
                        if (_index < _tmp.Length)
                        {
                            creator.configuration.worldMap[creator.configuration.ui.mapIndex].cellMap[x, y] = _tmp[_index];
                            _index++;
                        }
                    }
                }

                creator.UpdateMap();

                if (creator.configuration.ui.autoBuild)
                {
                    if (creator.firstTimeBuild)
                    {
                        creator.BuildMapComplete(false, false, true);
                    }
                    else
                    {
                        if (creator.configuration.global.buildOverlappingTiles)
                        {
                            //creator.BuildMapPartial(false, false, 0, 0);
                            creator.BuildMapPartial(false, false);
                        }
                        else
                        {
                            creator.firstTimeBuild = true;
                            creator.BuildMapComplete(false, false, true);
                        }
                    }

                }
            }
        }
    }


    private void DoRedo()
    {
        if (redoStack.Count > 0)
        {
            undoStack.Push(creator.configuration.worldMap[creator.configuration.ui.mapIndex].cellMapSingle);

            bool[] _tmp = redoStack.Pop();

            if (_tmp != null)
            {
                creator.configuration.worldMap[creator.configuration.ui.mapIndex].cellMap = new bool[creator.configuration.global.width, creator.configuration.global.height];
                creator.configuration.worldMap[creator.configuration.ui.mapIndex].cellMapSingle = _tmp;
                int _index = 0;

                for (int y = 0; y < creator.configuration.global.height; y++)
                {
                    for (int x = 0; x < creator.configuration.global.width; x++)
                    {
                        creator.configuration.worldMap[creator.configuration.ui.mapIndex].cellMap[x, y] = _tmp[_index];
                        _index++;
                    }
                }

                creator.UpdateMap();

                if (creator.configuration.ui.autoBuild)
                {
                    if (creator.firstTimeBuild)
                    {
                        creator.BuildMapComplete(false, false, true);
                    }
                    else
                    {
                        if (creator.configuration.global.buildOverlappingTiles)
                        {
                            //creator.BuildMapPartial(false, false, 0, 0);
                            creator.BuildMapPartial(false, false);
                        }
                        else
                        {
                            creator.firstTimeBuild = true;
                            creator.BuildMapComplete(false, false, true);
                        }
                    }
                }
            }
        }
    }

    //Register Map
    //store mapSingle array to temp map
    private void RegisterMap()
    {
        if (creator.configuration.worldMap.Count < 1)
            return;

        tmpMap = new bool[creator.configuration.worldMap[creator.configuration.ui.mapIndex].cellMapSingle.Length];
        for (int t = 0; t < creator.configuration.worldMap[creator.configuration.ui.mapIndex].cellMapSingle.Length; t++)
        {
            tmpMap[t] = creator.configuration.worldMap[creator.configuration.ui.mapIndex].cellMapSingle[t];
        }
    }

    //check if temp map is different than mapSingle
    //if it is, store it as new undo
    private void RegisterUndo(Vector2 _gP)
    {
        if (TileWorldCreatorSaveLoad.isLoading)
            return;

        if (creator.configuration.worldMap.Count > 0)
        {
            if (tmpMap != creator.configuration.worldMap[creator.configuration.ui.mapIndex].cellMapSingle)
            {
                undoStack.Push(tmpMap);

                if (creator.configuration.ui.autoBuild)
                {
                    if (creator.firstTimeBuild)
                    {
                        creator.BuildMapComplete(false, false, true);
                    }
                    else
                    {
                       
                        if (creator.configuration.global.buildOverlappingTiles)
                        {
                          
                            creator.BuildMapPartial(false, true);

                        }
                        else
                        {
                            creator.firstTimeBuild = true;
                            creator.BuildMapComplete(false, false, true);
                        }
                    }
                }
            }
        }


    }



    // check if mouse pointer is over grid
    bool IsMouseOverGrid(Vector2 _mousePos)
    {
        Ray _ray = HandleUtility.GUIPointToWorldRay(_mousePos); //Camera.current.ScreenToWorldPoint(_pos);
        float _dist = 0.0f;
        bool _return = false;

        if (creator.configuration.global.mapOrientation == TileWorldConfiguration.GlobalConfiguration.MapOrientations.xz)
        {
            groundPlane = new Plane(Vector3.up, new Vector3(0f, 0f, 0f));
        }
        else
        {
            groundPlane = new Plane(Vector3.forward, new Vector3(0f, 0f, 0f));
        }

        if (groundPlane.Raycast(_ray, out _dist))
        {
            Vector3 _worldPos = _ray.origin + _ray.direction.normalized * _dist;

            int _off = 0;
            //if (!creator.brushx2)
            //{
            //    _off = 0;
            //}
            //else
            //{
                _off = -1;
            //}

            if (_worldPos.x > _off + creator.transform.position.x && _worldPos.x < (creator.configuration.global.width * creator.configuration.global.globalScale) + creator.transform.position.x && _worldPos.z > _off + creator.transform.position.z && _worldPos.z < (creator.configuration.global.height * creator.configuration.global.globalScale) + creator.transform.position.z)
            {
                _return = true;
            }
        }

        return _return;
    }

    //return mouse world position
    Vector3 GetWorldPosition(Vector2 _mousePos)
    {
        Ray _ray = HandleUtility.GUIPointToWorldRay(_mousePos);
        float _dist = 0.0f;
        Vector3 _return = new Vector3(0, 0);

        if (creator.configuration.global.mapOrientation == TileWorldConfiguration.GlobalConfiguration.MapOrientations.xz)
        {
            groundPlane = new Plane(Vector3.up, new Vector3(0f, 0f, 0f));
        }
        else
        {
            groundPlane = new Plane(Vector3.forward, new Vector3(0f, 0f, 0f));
        }

        if (groundPlane.Raycast(_ray, out _dist))
        {
            _return = _ray.origin + _ray.direction.normalized * _dist;
        }

        return _return;
    }

    //return the exact grid / cell position
    Vector3 GetGridPosition(Vector3 _mousePos)
    {
        Vector3 _gridPos = Vector3.zero;
        if (creator.configuration.global.mapOrientation == TileWorldConfiguration.GlobalConfiguration.MapOrientations.xz)
        {
            _gridPos = new Vector3((Mathf.Floor(_mousePos.x - creator.transform.position.x / 1) / creator.configuration.global.globalScale), 0.05f, (Mathf.Floor(_mousePos.z - creator.transform.position.z / 1) / creator.configuration.global.globalScale));
        }
        else
        {
            _gridPos = new Vector3((Mathf.Floor(_mousePos.x - creator.transform.position.x / 1) / creator.configuration.global.globalScale), 0.05f, (Mathf.Floor(_mousePos.y - creator.transform.position.y / 1) / creator.configuration.global.globalScale));
        }

        return _gridPos;
    }


    //Save and Load Methods
    //--------
    void Save()
    {
        var _path = EditorUtility.SaveFilePanel("Save", Application.dataPath, "map", "xml");

        TileWorldCreatorSaveLoad.Save(_path, creator);
    }



    void LoadMap()
    {
        var _path = EditorUtility.OpenFilePanel("Load", Application.dataPath, "xml");

        TileWorldCreatorSaveLoad.Load(_path, creator);
    }
    //---------------


    //void GridUpdate(SceneView sceneview)
    //{
    //    //Event e = Event.current;
    //    //if (e.isMouse && e.button == 0)
    //    //{
    //    //	Debug.Log("hello");
    //    //}
    //}


    void Merge()
    {
        if (creator.clusterContainers.Count < 1)
        {
            creator.mergeReady = true;
        }

        for (int c = 0; c < creator.clusterContainers.Count; c++)
        {
            TileWorldCombiner.CombineMesh(creator.clusterContainers[c], creator.configuration.global.addMeshCollider);

            if (creator.configuration.global.createMultiMaterialMesh)
            {
                TileWorldCombiner.CombineMulitMaterialMesh(creator.clusterContainers[c], creator.configuration.global.addMeshCollider);
            }
        }
    }


    void LoadResources()
    {
        var _path = ReturnInstallPath.GetInstallPath("Editor", this); // GetInstallPath();


        iconLayer = AssetDatabase.LoadAssetAtPath(_path + "Res/icon_layer.png", typeof(Texture2D)) as Texture2D;
        iconFillFloor = AssetDatabase.LoadAssetAtPath(_path + "Res/icon_fillfloor.png", typeof(Texture2D)) as Texture2D;
        iconFillBlock = AssetDatabase.LoadAssetAtPath(_path + "Res/icon_fillblock.png", typeof(Texture2D)) as Texture2D;
        iconDuplicate = AssetDatabase.LoadAssetAtPath(_path + "Res/icon_duplicate.png", typeof(Texture2D)) as Texture2D;
        iconMaskLayerArrow = AssetDatabase.LoadAssetAtPath(_path + "Res/masklayerarrow.png", typeof(Texture2D)) as Texture2D;
        iconUndockWindow = AssetDatabase.LoadAssetAtPath(_path + "Res/icon_undock.png", typeof(Texture2D)) as Texture2D;
        iconCopyMap = AssetDatabase.LoadAssetAtPath(_path + "Res/icon_copy.png", typeof(Texture2D)) as Texture2D;
        iconPasteMap = AssetDatabase.LoadAssetAtPath(_path + "Res/icon_paste.png", typeof(Texture2D)) as Texture2D;
        iconOptionMenu = AssetDatabase.LoadAssetAtPath(_path + "Res/icon_optionMenu.png", typeof(Texture2D)) as Texture2D;
        //iconStore = AssetDatabase.LoadAssetAtPath(_path + "Res/icon_store.png", typeof(Texture2D)) as Texture2D;

        topLogo = AssetDatabase.LoadAssetAtPath(_path + "Res/topLogo.png", typeof(Texture2D)) as Texture2D;
    }

}
#endif