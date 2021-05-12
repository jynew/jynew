/* TILE WORLD CREATOR TileWorldObjectScatterEditor
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
using UnityEngine;
using UnityEditor;
#if  UNITY_5_3_OR_NEWER || UNITY_5_3
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
#endif
using System.Collections;
using System.Collections.Generic;
using TileWorld;

[CustomEditor(typeof(TileWorldObjectScatter))]
public class TileWorldObjectScatterEditor : Editor
{

    TileWorldObjectScatter twos;
    TileWorldCreator creator;

    bool startEndObjectsFoldout;
    bool ruleBasedObjectsFoldout;
    bool paintObjectsFoldout;

    Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, 0f, 0f));
    Vector3 currentCameraPosition;
    Vector3 lastCameraPosition;
    Vector3 lastGridPosition;


    Texture2D iconMaskLayerArrow;


    //string currentScene;
#if  UNITY_5_3_OR_NEWER || UNITY_5_3
    Scene currentScene;
#else
    string currentScene;
#endif

    string[] placeLayers = new string[] { "layer 1" };

    void OnEnable()
    {
        twos = (TileWorldObjectScatter)target;
        creator = twos.GetComponent<TileWorldCreator>();

        placeLayers = twos.GetLayers();

#if  UNITY_5_3_OR_NEWER || UNITY_5_3
        currentScene = EditorSceneManager.GetActiveScene();
#else
    
        currentScene = EditorApplication.currentScene;

#endif
        EditorApplication.hierarchyWindowChanged += HierarchyWindowChanged;
    }

    void HierarchyWindowChanged()
    {
#if  UNITY_5_3_OR_NEWER || UNITY_5_3
        if (currentScene != EditorSceneManager.GetActiveScene())
#else
        if (currentScene != EditorApplication.currentScene)
#endif
        {
#if  UNITY_5_3_OR_NEWER || UNITY_5_3
            currentScene = EditorSceneManager.GetActiveScene();
#else
            currentScene = EditorApplication.currentScene;
#endif
        }
    }

    public override void OnInspectorGUI()
    {
        var errorMsg = "";
        if (creator.configuration == null)
        {
            errorMsg = "no TileWorldCreator configuration file loaded. Please create or load a new one.";
        }
        else if (creator.configuration.worldMap.Count == 0)
        {
            errorMsg = "please generate and build a new map";
        }

        if (errorMsg != "")
        {
            EditorGUILayout.HelpBox(errorMsg, MessageType.Warning);
        }

        //
        EditorGUILayout.BeginHorizontal();
        twos.configuration = (TileWorldObjectScatterConfiguration)EditorGUILayout.ObjectField(twos.configuration, typeof(TileWorldObjectScatterConfiguration), true) as TileWorldObjectScatterConfiguration;

        if (GUILayout.Button("create config.", "ToolBarButton"))
        {
            TileWorldObjectScatterConfigEditor.CreateConfigFile(twos);
        }

        if (GUILayout.Button("load config.", "ToolBarButton"))
        {
            var _configPath = EditorUtility.OpenFilePanel("load configuration file", "Assets", "asset");
            // make path relative
            if (_configPath != "")
            {
                _configPath = "Assets" + _configPath.Substring(Application.dataPath.Length);

                twos.configuration = (TileWorldObjectScatterConfiguration)UnityEditor.AssetDatabase.LoadAssetAtPath(_configPath, typeof(TileWorldObjectScatterConfiguration));
            }
        }

        EditorGUILayout.EndHorizontal();

        if (twos.configuration == null)
        {
            if (GUI.changed)
            {
#if UNITY_5_3_OR_NEWER || UNITY_5_3
                EditorSceneManager.MarkSceneDirty(this.currentScene);
#else
            EditorApplication.MarkSceneDirty();
#endif
            }

            //return;
        }

    

        // Show configuration editor
        //--------------------------
        TileWorldObjectScatterConfigEditor.ShowOSConfigurationEditor(twos.configuration, twos, creator, placeLayers, iconMaskLayerArrow);

      

        if (GUI.changed)
        {
            creator.firstTimeBuild = true;
#if UNITY_5_3_OR_NEWER || UNITY_5_3
            EditorSceneManager.MarkSceneDirty(this.currentScene);
#else

            EditorApplication.MarkSceneDirty();
#endif
        }

	    EditorUtility.SetDirty(twos);
	    if (twos.configuration != null)
	    {
		    EditorUtility.SetDirty(twos.configuration);
	    }
    }


    private void OnSceneGUI()
    {
        if (creator.configuration == null || !twos.showGrid)
            return;

        Event _event = Event.current;

        //get mouse worldposition
        creator.mouseWorldPosition = GetWorldPosition(_event.mousePosition);

        if (_event.type == EventType.MouseDown)
        {
            //On Mouse Down store current camera position and set last position to current
            currentCameraPosition = Camera.current.transform.position;
            lastCameraPosition = currentCameraPosition;

            if (IsMouseOverGrid(_event.mousePosition) && lastCameraPosition == currentCameraPosition)
            {
                _event.Use();
            }
        }
        else if (_event.type == EventType.MouseUp)
        {
            if (_event.button == 0)
            {
                if (IsMouseOverGrid(_event.mousePosition) && lastCameraPosition == currentCameraPosition)
                { 
                    // Place Objects
                    Vector3 _gP = GetGridPosition(creator.mouseWorldPosition);

                    if (_gP != lastGridPosition)
                    {
                        twos.PaintObjects(_gP);
                        lastGridPosition = _gP;
                        _event.Use();
                    }
                }
            }
        }
        else if (_event.type == EventType.MouseDrag && _event.type != EventType.KeyDown)
        {
            currentCameraPosition = Camera.current.transform.position;

            if (_event.button == 0)
            {
                if (IsMouseOverGrid(_event.mousePosition) && lastCameraPosition == currentCameraPosition)
                {
                    // Place Objects
                    Vector3 _gP = GetGridPosition(creator.mouseWorldPosition);

                    if (_gP != lastGridPosition)
                    {
                        twos.PaintObjects(_gP);
                        lastGridPosition = _gP;
                        _event.Use();
                    }
                }
            }
        }
        else if (_event.type == EventType.MouseMove)
        {
            currentCameraPosition = Camera.current.transform.position;
            lastCameraPosition = currentCameraPosition;

            twos.mouseOverGrid = IsMouseOverGrid(_event.mousePosition);
        }
        else if (_event.type == EventType.Layout)
        {
            //this allows _event.Use() to actually function and block mouse input
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
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
            _off = -1;


            if (_worldPos.x > _off + creator.transform.position.x && _worldPos.x < (creator.configuration.global.width * creator.configuration.global.globalScale) + twos.creator.transform.position.x && _worldPos.z > _off + twos.creator.transform.position.z && _worldPos.z < (twos.creator.configuration.global.height * twos.creator.configuration.global.globalScale) + twos.creator.transform.position.z)
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
            _gridPos = new Vector3((Mathf.Floor(_mousePos.x - creator.transform.position.x / 1) / creator.configuration.global.globalScale), 0.05f, (Mathf.Floor(_mousePos.z - twos.creator.transform.position.z / 1) / twos.creator.configuration.global.globalScale));
        }
        else
        {
            _gridPos = new Vector3((Mathf.Floor(_mousePos.x - creator.transform.position.x / 1) / creator.configuration.global.globalScale), 0.05f, (Mathf.Floor(_mousePos.y - twos.creator.transform.position.y / 1) / twos.creator.configuration.global.globalScale));
        }

        return _gridPos;
    }


    /// <summary>
    /// Load editor icons
    /// </summary>
    void LoadResources()
    {
        var _path = ReturnInstallPath.GetInstallPath("Editor", this); // GetInstallPath();
 
        iconMaskLayerArrow = AssetDatabase.LoadAssetAtPath(_path + "Res/masklayerarrow.png", typeof(Texture2D)) as Texture2D;
    }
}
