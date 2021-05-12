using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TileWorld;
using TileWorld.Events;

public class RuntimeConfigurationCreation : MonoBehaviour
{
    public TileWorldCreator creator;
    public TileWorldObjectScatter objectScatter;

    [System.Serializable]
    public class Themes
    {
        public string name;
        public TileWorldConfiguration configuration;
        public TileWorldObjectScatterConfiguration objectScatterConfiguration;
    }

    public List<Themes> themes = new List<Themes>();

    // GUI Properties
    //---------------------
    string width = "20";
    string height = "20";
    bool invert = false;
    bool scatterObjects = false;
    bool merge = false;
    int selectedAlgorithm;
    string[] availableAlgorithms = new string[] { "BSP dungeon", "cellular", "maze", "simple dungeon" };
    float progress;
    float mergeProgress;

    int objectCount = 0;

    int selectedTheme = 0;
    string[] availableThemes;

    bool buildEnabled = true;
    //----------------------

    void OnEnable()
    {
        // Register events
        TileWorldEvents.OnBuildComplete += MapBuildComplete;
        TileWorldEvents.OnMergeComplete += MergeComplete;
        TileWorldEvents.BuildProgress += BuildProgress;
        TileWorldEvents.MergeProgress += MergeProgress;
    }

    void OnDisable()
    {
        // Unregister events
        TileWorldEvents.OnBuildComplete -= MapBuildComplete;
        TileWorldEvents.OnMergeComplete -= MergeComplete;
        TileWorldEvents.BuildProgress -= BuildProgress;
        TileWorldEvents.MergeProgress -= MergeProgress;
    }

    void Start()
    {
        availableThemes = new string[themes.Count];

        for (int i = 0; i < themes.Count; i ++)
        {
            availableThemes[i] = themes[i].name;
        }
    }

    void OnGUI()
    {

        selectedTheme = GUILayout.SelectionGrid(selectedTheme, availableThemes, themes.Count);

        width = GUILayout.TextField(width);
        height = GUILayout.TextField(height);

        selectedAlgorithm = GUILayout.SelectionGrid(selectedAlgorithm, availableAlgorithms, 3);

        invert = GUILayout.Toggle(invert, "invert:");
        scatterObjects = GUILayout.Toggle( scatterObjects, "scatterObjects:");
        merge = GUILayout.Toggle(merge, "merge:");

        GUI.enabled = buildEnabled;
        if (GUILayout.Button("Build"))
        {
            //create new configuration
            //==============================
            //var _c = TileWorldConfiguration.NewConfiguration(creator.configuration, true);
            //==============================
            // or use current configuration
            //var _c = creator.configuration;
            //==============================
            // or assign new configuration from list
            var _creatorConfig = themes[selectedTheme].configuration;           
            //==============================


            // Assign properties
            // assign width and height
            int.TryParse(width, out _creatorConfig.global.width);
            int.TryParse(height, out _creatorConfig.global.height);
            // assign invert option
            // if maze algorithm is selected change invert to always false
            // if true maze wont work
            if (selectedAlgorithm == 2)
            {
                invert = false;
                _creatorConfig.global.invert = false;
            }
            else
            {
                _creatorConfig.global.invert = invert;
            }

            // assign selected algorithm
            _creatorConfig.global.selectedAlgorithm = selectedAlgorithm;

            creator.configuration = _creatorConfig;

            creator.GenerateAndBuild(merge);

            // disable build button
            buildEnabled = false;
        }
        GUI.enabled = true;

        // Show progress bar
        GUI.Box(new Rect(Screen.width / 2, 40, progress, 25), "building: " + progress.ToString() + " %");
        // Show merge progress bar
        GUI.Box(new Rect(Screen.width / 2, 75, mergeProgress, 25), "merging: " + mergeProgress.ToString() + " %");


        GUILayout.Label("object count: " + objectCount.ToString());
    }

   

    // Event, gets called when map is builded
    void MapBuildComplete()
    {
        Debug.Log("map build complete");

        StartCoroutine(CountObjects());

        if ( !merge )
        {
            buildEnabled = true;
        }

        // Scatter objects after map build is complete
        if (scatterObjects)
        {
            // assign configuration based on selected theme
            var _objectScatterConfig = themes[selectedTheme].objectScatterConfiguration;
            objectScatter.configuration = _objectScatterConfig;

            // Call scatter method
            objectScatter.ScatterProceduralObjects();
        }
    }

    void MergeComplete()
    {
        Debug.Log("merge complete");
        StartCoroutine(CountObjects());

        buildEnabled = true;
    }

    // Event, returns current build progress
    void BuildProgress(float _progress)
    {
        progress = _progress;
    }

    void MergeProgress(float _progress)
    {
        mergeProgress = _progress;
    }


    IEnumerator CountObjects()
    {
        yield return new WaitForEndOfFrame();
        objectCount = objectsInScene();
    }

    int objectsInScene()
    {
        GameObject _container = GameObject.Find(creator.configuration.global.worldName) as GameObject;
        var _objs = _container.GetComponentsInChildren<Transform>();
        return _objs.Length;
    }


}
