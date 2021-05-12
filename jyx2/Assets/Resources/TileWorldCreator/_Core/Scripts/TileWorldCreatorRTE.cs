/* TILE WORLD CREATOR RUNTIME EDITOR
 * Copyright (c) 2015 doorfortyfour OG
 * 
 * Create awesome tile worlds in seconds.
 *
 * Using:
 * Drag the prefab called TileWorldCreatorRTE to your scene.
 * Make sure a TileWorldCreator prefab exists already in the
 * scene. Assign it at the TileWorldCreatorRTE prefab.
 *
 * Documentation: http://tileworldcreator.doofortyfour.com
 * Like us on Facebook: http://www.facebook.com/doorfortyfour2013
 * Web: http://www.doorfortyfour.com
 * Contact: mail@doorfortyfour.com Contact us for help, bugs or 
 * share your awesome work you've made with TileWorldCreator
*/

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using TileWorld;

namespace TileWorld
{
    public class TileWorldCreatorRTE : MonoBehaviour
    {

        //References
        public TileWorldCreator creator;
        public Camera editorCam;
        
        //Properties
        public bool showGrid;
        public Texture gridTexture;

        public Material mainMaterial; //use this material for grid, cell and cursor
        public Color cellAddColor;
        public Color cellRemoveColor;
        public Color cursorColor;
        
        
        Material matCellAdd;
        Material matCellRemove;
        Material matCursor;
        Material matGrid;
        Color gridColorActive = new Color(255f/255f, 255f/255f, 255f/255f, 30f/255f);
        Color gridColorDeactive = new Color(255f / 255f, 255f / 255f, 255f / 255f, 0f / 255f);
        
        //settings
        public string path = ""; // root is "Assets/" 
        public string fileExtension = ".xml";

        // UI 
        bool showGUINewGrid = false;
        bool showGUISave = false;
        bool showGUILoad = false;
        Vector2 scrollViewVector = Vector2.zero;

        // UI Objects
        GameObject grid;
        GameObject cell;
        GameObject cursor;

        // UI painted cells arrays
        bool[,] cellMap = new bool[0, 0];
        bool[,] cellInstancedMap = new bool[0, 0];
        List<GameObject> cellList = new List<GameObject>();

        // build / map properties       
        bool paintOK = true;
        bool mapReady = false;
        string w = "";
        string h = "";
        string[] layerNames;
        //int selectedLayer = 0;
        float x = 0f;
        float z = 0f;

        //navigation properties
        bool screenPanning = false;
        Vector3 lastPosition;
        float camDistance;
        float maxDistance;
        float minDistance = 5;

        //save map properties
        string saveName = "";
            

        public void Start()
        {
            bool _error1 = false;
            bool _error2 = false;

            if (creator == null)
            {
                //try to find tileworldcreator prefab
                creator = GameObject.FindObjectOfType(typeof(TileWorldCreator)) as TileWorldCreator;
                if (creator == null)
                {
                    _error1 = true;
                    Debug.LogWarning("no TileWorldCreator prefab in the scene");
                }
                else
                {
                    _error1 = false;
                }
            }

            if (editorCam == null)
            {
                editorCam = GameObject.FindObjectOfType(typeof(Camera)) as Camera;

                if (editorCam == null)
                {
                    _error2 = true;
                }
                else
                {
                    _error2 = false;
                }
            }


            if (!_error1 && !_error2)
            {
                //init Editor and generate map
                //Init(true);
                creator.configuration.ui.autoBuild = true;
            }
        }


        //Initialize Editor
        public void Init(bool _buildMap)
        {

            creator.configuration.ui.autoBuild = true;

            if (_buildMap)
            {            
                //use the settings from the tileworldcreator prefab
                //if this is false we will have to assign new settings like in the 
                //runtime demoscene (new TileWorldCreator.Settings();)
                //creator.useSettingsFromReference = true;
                //first generate a new map then we build the complete map 
                //with coroutine set to false, merge false and optimization set to true.
                creator.GenerateMaps();
                creator.BuildMapComplete(false, false, true);

                //assign a new mask map to layers who are using one
                for (int l = 0; l < creator.configuration.global.layerCount; l++)
                {
                    if (creator.configuration.worldMap[l].useMask)
                    {                      
                        creator.AddNewMask(l);
                    }
                }

                //bool array for painted cells
                cellMap = new bool[creator.configuration.global.width, creator.configuration.global.height];
                cellInstancedMap = new bool[creator.configuration.global.width, creator.configuration.global.height];

                for (int w = 0; w < creator.configuration.global.width; w++)
                {
                    for (int h = 0; h < creator.configuration.global.height; h++)
                    {
                        cellMap[w, h] = false;
                        cellInstancedMap[w, h] = false;
                    }
                }
            }


            //setup camera
            editorCam.transform.position = new Vector3(creator.configuration.global.width / 2 + creator.transform.position.x, 10, creator.configuration.global.height / 2 + creator.transform.position.z);
            editorCam.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
            editorCam.orthographic = true;

            float _h = ((float)creator.configuration.global.height / 2);
            float _w = ((float)creator.configuration.global.width / (float)creator.configuration.global.height) * 3;
            editorCam.orthographicSize = _h + _w;
            maxDistance = _h + _w;
            camDistance = maxDistance;

            if (grid != null)
            {
                Destroy(grid.gameObject);
            }
            if (cell != null)
            {
                Destroy(cell.gameObject);
            }
            if (cursor != null)
            {
                Destroy(cursor.gameObject);
            }

            //setup editor
            //generate grid plane  
            //matGrid = new Material(Shader.Find("Particles/Additive")); -> does not work in builds, becuase shader will no be included in build
            matGrid = new Material(mainMaterial);
            if (showGrid)
            {
                matGrid.SetTexture("_MainTex", gridTexture);
                matGrid.SetColor("_TintColor", gridColorActive);
            }
            else
            {
                matGrid.SetColor("_TintColor", gridColorDeactive);
            }

            grid = GeneratePlane(creator.configuration.global.width, creator.configuration.global.height, matGrid, "grid");

            //generate cell plane and material add and material remove
            //which will be used for the cells

            //material add cell
            //matCellAdd = new Material(Shader.Find("Particles/Additive"));
            matCellAdd = new Material(mainMaterial);
            matCellAdd.SetColor("_TintColor", cellAddColor);

            ////material remove cell
            //matCellRemove = new Material(Shader.Find("Particles/Additive"));
            matCellRemove = new Material(mainMaterial);
            matCellRemove.SetColor("_TintColor", cellRemoveColor);

            cell = GeneratePlane(1, 1, matCellAdd, "cell");
            //move cell
            cell.transform.position = new Vector3(-1000, -1000, -1000);

            //generate cursor
            //matCursor = new Material(Shader.Find("Particles/Additive"));
            matCursor = new Material(mainMaterial);
            matCursor.SetColor("_TintColor", cursorColor);
            cursor = GeneratePlane(2, 2, matCursor, "cursor");

            mapReady = true;

            //parent grid, cell and cursor object 
            cell.transform.parent = this.transform;
            grid.transform.parent = this.transform;
            cursor.transform.parent = this.transform;

            this.transform.position = creator.transform.position;

            //initialize layers and build arrays
            InitLayers();

        }


        void InitLayers()
        {
            List<string> layers = new List<string>();
            for (int i = 0; i < creator.configuration.global.layerCount; i++)
            {
                layers.Add("layer: " + i.ToString());
            }

            layerNames = new string[layers.Count];

            for (int l = 0; l < layers.Count; l++)
            {
                layerNames[l] = layers[l];
            }
        }

        //Update method is used to place cursor plane on grid and detecting if user has
        //clicked on the grid.
        void Update()
        {
 
            if (!mapReady || !paintOK)
                return;

            //set grid plane according to selected layer height
            if (showGrid)
            {
                grid.transform.localPosition = new Vector3(0, 1.1f + creator.configuration.ui.mapIndex, 0); // new Vector3(0, 1.1f + creator.mapIndex, 0);
            }

            Ray _ray = editorCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit _hit = new RaycastHit();

            if (Physics.Raycast(_ray, out _hit, 1000)) // && _hit.transform.gameObject.tag == "grid")
            {
                //if (_hit.transform.gameObject.name == "grid")
                //{
                    if (_hit.point.x - creator.transform.position.x < 0)
                    {
                        x = (int)(_hit.point.x / 1);
                    }
                    else if (_hit.point.x - creator.transform.position.x < creator.configuration.global.width - 1)
                    {
                        x = (int)(_hit.point.x / 1 + 1.0f);
                    }

                    x *= 1;

                    if (_hit.point.z - creator.transform.position.z < 0)
                    {
                        z = (int)(_hit.point.z / 1);
                    }
                    else if (_hit.point.z - creator.transform.position.z < creator.configuration.global.height - 1)
                    {
                        z = (int)(_hit.point.z / 1 + 1.0f);
                    }

                    z *= 1;


                    cursor.transform.position = new Vector3(x - (1), 1.2f + creator.configuration.ui.mapIndex, z - (1));

                    if (Input.GetMouseButtonDown(0))
                    {
                        PaintCell(creator.configuration.global.invert);
                        InstantiateCells(true);
                    }
                    else if (Input.GetMouseButton(0))
                    {
                        PaintCell(creator.configuration.global.invert);
                        InstantiateCells(true);
                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        BuildMap();
                    }

                    if (Input.GetMouseButtonDown(1))
                    {
                        PaintCell(!creator.configuration.global.invert);
                        InstantiateCells(false);
                    }
                    else if (Input.GetMouseButton(1))
                    {
                        PaintCell(!creator.configuration.global.invert);
                        InstantiateCells(false);
                    }
                    else if (Input.GetMouseButtonUp(1))
                    {
                        BuildMap();
                    }


                    if (Input.GetMouseButtonUp(1))
                    {
                        BuildMap();
                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        BuildMap();
                    }

                //}
            }



            //NAVIGATION
            //----------

            // PANNING
            // Hold middle Mouse Button to pan the screen in a direction
            if (Input.GetMouseButtonDown(2))
            {
                screenPanning = true;
                lastPosition = Input.mousePosition;
            }

            //If panning, find the angle to pan based on camera angle not screen
            if (screenPanning == true)
            {
                if (Input.GetMouseButtonUp(2))
                {
                    screenPanning = false;
                }

                var delta = Input.mousePosition - lastPosition;


                editorCam.transform.Translate((-(delta.x * (0.1f / 2))), (-(delta.y * (0.1f / 2))), 0);

                //clamp panning
                editorCam.transform.localPosition = new Vector3(Mathf.Clamp(editorCam.transform.localPosition.x, 0, creator.transform.position.x + creator.configuration.global.width), editorCam.transform.localPosition.y, Mathf.Clamp(editorCam.transform.localPosition.z, 0, creator.transform.position.z + creator.configuration.global.height));


                lastPosition = Input.mousePosition;

            }

            //scrolling
            //---------
            camDistance -= Input.GetAxis("Mouse ScrollWheel") * 10;
            camDistance = Mathf.Clamp(camDistance, minDistance, maxDistance);

            editorCam.orthographicSize = camDistance;

            //--------------
        }

        //build map if user has clicked and automatic build is enabled 
        void BuildMap()
        {
            if (creator.configuration.ui.autoBuild)
            {

                for (int l = 0; l < creator.configuration.global.layerCount; l++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        for (int c = 0; c < cellMap.GetLength(0); c++)
                        {
                            for (int d = 0; d < cellMap.GetLength(1); d++)
                            {
                                if (cellInstancedMap[c, d])
                                {
                                    creator.OptimizePassPartial(l, c, d);
                                }
                            }
                        }
                    }

                }
 

                if (creator.firstTimeBuild)
                {
                    creator.BuildMapComplete(false, false, true);
                }
                else
                {
                    if (creator.configuration.global.buildOverlappingTiles)
                    {
                        //set optimization to false because we do the optmizitation above
                        //creator.BuildMapPartial(false, false, 0, 0);
                        creator.BuildMapPartial(false, false);
                    }
                    else
                    {
                        creator.firstTimeBuild = true;

                        creator.BuildMapComplete(false, false, true);
                    }
                }
                //reset the painted cells
                ResetCells();
            }
        }

        //paint cells so user knows where he has painted
        void InstantiateCells(bool _add)
        {
            for (int y = 0; y < creator.configuration.global.height; y++)
            {
                for (int x = 0; x < creator.configuration.global.width; x++)
                {
                    if (cellMap[x, y])
                    {
                        if (!cellInstancedMap[x, y])
                        {
                            var _inst = Instantiate(cell, new Vector3(x + creator.transform.position.x, 1.2f + creator.configuration.ui.mapIndex, y + creator.transform.position.z), Quaternion.identity) as GameObject;
                            _inst.transform.parent = this.transform;
                            cellList.Add(_inst);
                            cellInstancedMap[x, y] = true;

                            if (_add)
                            {
                                _inst.GetComponent<Renderer>().material = matCellAdd;
                            }
                            else
                            {
                                _inst.GetComponent<Renderer>().material = matCellRemove;
                            }
                        }
                    }
                }
            }
        }

        //reset painted cells
        void ResetCells()
        {
            for (int i = 0; i < cellList.Count; i++)
            {
                Destroy(cellList[i].gameObject);
            }

            cellMap = new bool[creator.configuration.global.width, creator.configuration.global.height];
            cellInstancedMap = new bool[creator.configuration.global.width, creator.configuration.global.height];
            cellList = new List<GameObject>();
        }

        //paint map, add or remove
        void PaintCell(bool _add)
        {
            if (creator.configuration.worldMap.Count < 1)
                return;

            if (_add)
            {
                Vector3 _gP = GetGridPosition(cursor.transform.position);


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


                            cellMap[(int)_gP.x + x, (int)_gP.z + y] = true;

                        }
                    }
                }

                //creator.UpdateMap();

            }
            else
            {

                Vector3 _gP = GetGridPosition(cursor.transform.position);

              

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


                            cellMap[(int)_gP.x + x, (int)_gP.z + y] = true;

                        }
                    }
                }

                //creator.UpdateMap();

            }
        }


        //return the exact grid / cell position
        Vector3 GetGridPosition(Vector3 _mousePos)
        {
            Vector3 _gridPos = new Vector3((Mathf.Floor(_mousePos.x - creator.transform.position.x / 1) / creator.configuration.global.globalScale), 0.05f, (Mathf.Floor(_mousePos.z - creator.transform.position.z / 1) / creator.configuration.global.globalScale));

            return _gridPos;
        }


        //Returns an empty plane
        public GameObject GeneratePlane(int _width, int _height, Material _mat, string _name)
        {

            Mesh _gridMesh = new Mesh();
            Vector3[] _verts = new Vector3[4];
            Vector2[] _uvs = new Vector2[4];
            int[] _tris = new int[6] { 0, 2, 1, 1, 2, 3 };
            int _count = 0;


            for (int y = 0; y < 2; y++)
            {
                for (int x = 0; x < 2; x++)
                {
                    _verts[_count] = new Vector3(0 + (x * (1 * _width)), 0, 0 + (y * (1 * _height)));
                    _count++;

                }
            }

            _uvs[0] = new Vector2(0, 1);
            _uvs[1] = new Vector2(1, 1);
            _uvs[2] = new Vector2(0, 0);
            _uvs[3] = new Vector2(1, 0);


            _gridMesh.vertices = _verts;
            _gridMesh.triangles = _tris;
            _gridMesh.uv = _uvs;

            _gridMesh.RecalculateNormals();

            GameObject _newGO = new GameObject();
            _newGO.AddComponent<MeshFilter>().mesh = _gridMesh;
            _newGO.AddComponent<MeshRenderer>();
            _newGO.AddComponent<BoxCollider>();

            _newGO.transform.position = new Vector3(0, 0, 0); // new Vector3(((1 * _width) / 2), 0.0f, ((1 * _height) / 2));


            _newGO.name = _name;

            if (_mat != null)
            {
                _mat.mainTextureScale = new Vector2(_width, _height);
                _newGO.GetComponent<Renderer>().material = _mat;
                _newGO.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            return _newGO;
        }


        //----------------
        // SAVE
        //----------------
        void Save(string _name)
        {
            string _finalPath = Path.Combine(Application.dataPath, Path.Combine(path, _name + fileExtension));

            TileWorldCreatorSaveLoad.Save(_finalPath, creator);
        }

     

        //----------------
        // LOAD
        //----------------
        void Load(string _path)
        {
            //before loading and building the map we have to delete the old map
            GameObject _container = GameObject.Find("TWC_World");
            Destroy(_container.gameObject);

            TileWorldCreatorSaveLoad.Load(_path, creator);

            creator.firstTimeBuild = true;
            BuildMap();

            Init(false);
         
        }


        //---------------------
        // GUI
        //---------------------

        public void OnGUI()
        {
            if (creator == null)
                return;

            //Show toolbar (new map, random map)
            ShowToolbar();
            //show layers
            ShowGUILayers();
            //show create new map window
            if (showGUINewGrid)
            {
                ShowGUINewGrid();
            }

            //show save window
            if (showGUISave)
            {
                ShowGUISave();
            }
            //show load window
            if (showGUILoad)
            {
                ShowGUILoad();
            }

            //show paint tools (fill, clear)
            ShowGUIPaintTools();
            //if automatic build is false, show manual build button
            if (!creator.configuration.ui.autoBuild)
            {
                if (GUI.Button(new Rect((Screen.width / 2) - 100, Screen.height - 50, 200, 50), "Build"))
                {
                    //StartCoroutine(creator.BuildMap(false));
                    creator.BuildMapComplete(false, false, true);
                    ResetCells();
                }
            }
        }

        //Create new map
        void ShowGUINewGrid()
        {
            GUI.Box(centerRect(200, 200, 0, 0), "");

            GUI.Label(centerRect(180, 25, 0, -75), "width:");

            w = GUI.TextField(centerRect(180, 25, 0, -55), w);

            GUI.Label(centerRect(180, 25, 0, -30), "height:");

            h = GUI.TextField(centerRect(180, 25, 0, -10), h);

            //generate new map
            if (GUI.Button(centerRect(90, 25, 45, 75), "new"))
            {

                //check input
                bool _inputCorrect = false;

                if (int.TryParse(w, out creator.configuration.global.width))
                {
                    _inputCorrect = true;

                    if (int.TryParse(h, out creator.configuration.global.height))
                    {
                        _inputCorrect = true;
                    }
                    else
                    {
                        _inputCorrect = false;
                    }
                }

                if (_inputCorrect)
                {
                    Init(true);

                    showGUINewGrid = false;
                    paintOK = true;
                }
            }

            //cancel window
            if (GUI.Button(centerRect(90, 25, -45, 75), "cancel"))
            {
                showGUINewGrid = false;
                paintOK = true;
            }
        }

        //layer window
        void ShowGUILayers()
        {
            if (creator.configuration.worldMap.Count == 0)
                return;

            GUI.Box(new Rect(Screen.width - 150, Screen.height - 200, 150, 300), "");

            for (int l = creator.configuration.global.layerCount - 1; l >= 0; l --)
            {
                int _buttonWidth = 140;

                if (creator.configuration.worldMap[l].useMask)
                {
                    if (GUI.Button(new Rect(Screen.width - 140, Screen.height - 40 - (l * 25), 25, 25), "m"))
                    {
                        creator.configuration.ui.mapIndex = l;
                        creator.configuration.worldMap[l].paintMask = true;
                    }

                    _buttonWidth = 115;
                }

                if (GUI.Button(new Rect(Screen.width - _buttonWidth, Screen.height - 40 - (l * 25), _buttonWidth - 10, 25), "layer: " + l))
                {
                    creator.configuration.ui.mapIndex = l;
                    creator.configuration.worldMap[l].paintMask = false;
                }              
            }
        }

        //Top toolbar
        void ShowToolbar()
        {
            GUI.Box(new Rect(0, 0, Screen.width, 30), "");

            if (GUI.Button(new Rect(2, 2, 50, 25), "new"))
            {
                //block painting
                paintOK = false;
                //show UI new grid
                showGUINewGrid = true;
            }

            if (GUI.Button(new Rect(55, 2, 50, 25), "random"))
            {
                Init(true);
            }

            if (GUI.Button(new Rect(110, 2, 50, 25), "save"))
            {
                //block painting
                paintOK = false;
                showGUISave = true;
                showGUILoad = false;
                showGUINewGrid = false;
            }

            if (GUI.Button(new Rect(165, 2, 50, 25), "load"))
            {
                //block painting
                paintOK = false;
                showGUILoad = true;
                showGUISave = false;
                showGUINewGrid = false;
            }
        }

        //paint tools (clear / fill)
        void ShowGUIPaintTools()
        {
            GUI.Box(new Rect(Screen.width - 150, Screen.height - 255, 150, 50), "");

            if (GUI.Button(new Rect(Screen.width - 140, Screen.height - 250, 40, 40), "clear"))
            {
                creator.FillLayerGround();
                BuildMap();
            }

            if (GUI.Button(new Rect(Screen.width - 95, Screen.height - 250, 40, 40), "fill"))
            {
                creator.FillLayerBlock();
                BuildMap();
            }
        }

        //Save window
        void ShowGUISave()
        {
            GUI.Box(centerRect(200, 200, 0, 0), "");

            GUI.Label(centerRect(180, 25, 0, -75), "name:");
            saveName = GUI.TextField(centerRect(180, 25, 0, -55), saveName);

            //save
            if (GUI.Button(centerRect(90, 25, 45, 75), "save"))
            {
                Save(saveName);

                paintOK = true;
                showGUISave = false;
            }

            //cancel window
            if (GUI.Button(centerRect(90, 25, -45, 75), "cancel"))
            {

                paintOK = true;
                showGUISave = false;
            }

        }

        //Load window
        void ShowGUILoad()
        {
            GUI.Box(centerRect(220, 250, 0, 0), "");


            string _finalPath = Path.Combine(Application.dataPath, path);

            string[] _fileInfo = Directory.GetFiles(_finalPath, "*" + fileExtension);

            scrollViewVector = GUI.BeginScrollView(centerRect(200, 190, 0, -20), scrollViewVector, new Rect(0, 0, 180, _fileInfo.Length * 30));

            for (int i = 0; i < _fileInfo.Length; i++)
            {

                if (GUI.Button(new Rect(0, 0 + (30 * i), 180, 25), Path.GetFileName(_fileInfo[i])))
                {
                    Load(_fileInfo[i]);

                    paintOK = true;
                    showGUILoad = false;
                }
            }

            GUI.EndScrollView();

            //cancel window
            if (GUI.Button(new Rect((Screen.width / 2) - 100, (Screen.height / 2) + 90, 200, 25), "cancel"))
            {
                paintOK = true;
                showGUILoad = false;
            }

        }

        Rect centerRect(int _width, int _height, int _xOffset, int _yOffset)
        {
            Rect _rect = new Rect((Screen.width / 2) - (_width / 2) + _xOffset, (Screen.height / 2) - (_height / 2) + _yOffset, _width, _height);
            return _rect;
        }

        // --------------
        // end UI Methods

        void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            if (PrefabUtility.GetPrefabType(this.gameObject) != PrefabType.DisconnectedPrefabInstance)
            {
                PrefabUtility.DisconnectPrefabInstance(this.gameObject);
            }
#endif
        }
    }
}
