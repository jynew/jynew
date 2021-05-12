 using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace GameObjectBrush {

    /// <summary>
    /// The main class of this extension/tool that handles the ui and the brush/paint functionality
    /// </summary>
    [InitializeOnLoad]
    public class GameObjectBrushEditor : EditorWindow
    {

        #region Properties

        //version variable
        private static string version = "v3.2";

        //colors
        public static Color red = ColorFromRGB( 239, 80, 80);
        public static Color green = ColorFromRGB( 93, 173, 57);
        public static Color yellow = ColorFromRGB( 237, 199, 61);
        public static Color SelectedColor = ColorFromRGB( 139, 150, 165);
        public static Color PrimarySelectedColor = ColorFromRGB( 10, 153, 220);

        //some utility vars used to determine if the editor window is open
        public static GameObjectBrushEditor Instance { get; private set; }
        public static bool IsOpen
        {
            get { return Instance != null; }
        }

        //custom vars that hold the brushes, the current brush, the copied brush settings/details, the scroll position of the scroll view and all previously spawned objects
        public BrushCollection brushes;
        public int selectedBrushCollectionIndex = 0;

        private Vector2 scrollViewScrollPosition = new Vector2();
        private BrushObject copy = null;

        private bool isErasingEnabled = true;
        private bool isPlacingEnabled = true;

        private Dictionary<GameObject, Vector3> lastPlacementPositions = new Dictionary<GameObject, Vector3>();

        #endregion

        #region Editor Window Functionality


        /// <summary>
        /// Method that creates the window initially
        /// </summary>
        [MenuItem("Tools/GameObject Brush")]
        public static void ShowWindow() {
            //Show existing window instance. If one doesn't exist, make one.
            DontDestroyOnLoad(EditorWindow.GetWindow<GameObjectBrushEditor>("GO Brush " + version));
        }

        void OnEnable() {
            //Get last used brush collection
            KeyValuePair<int, BrushCollection> lastUsedBCollInfo = BrushCollection.GetLastUsedBrushCollection();
            selectedBrushCollectionIndex = lastUsedBCollInfo.Key;
            brushes = lastUsedBCollInfo.Value;

            //add thene delegate
            SceneView.onSceneGUIDelegate += OnSceneGUI;
            Instance = this;
            this.autoRepaintOnSceneChange = true;
        }
        void OnDestroy() {
            if (brushes != null) {
                brushes.Save();
            }
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
        }
        void OnGUI() {

            if (!Application.isPlaying) {
                SerializedObject serializedObject_brushObject = null;
                int prevSelectedBrushCollectionIndex = selectedBrushCollectionIndex;
                if (brushes != null) {
                    serializedObject_brushObject = new SerializedObject(brushes);
                }
                EditorGUIUtility.wideMode = true;


                #region Header

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Brush Collection:", EditorStyles.boldLabel);
                BrushCollection.BrushCollectionList brushCollectionList = BrushCollection.GetBrushCollectionsInProject();
                selectedBrushCollectionIndex = EditorGUILayout.Popup(selectedBrushCollectionIndex, brushCollectionList.GetNameList());
                if (prevSelectedBrushCollectionIndex != selectedBrushCollectionIndex) {                                                     //select only when brush collection changed
                    brushes = brushCollectionList.brushCollections[selectedBrushCollectionIndex];
                    brushes.SetLastUsedBrushCollection();
                }
                if (GUILayout.Button("+")) {
                    StringPopupWindow.Init(this, BrushCollection.CreateInstance, BrushCollection.newBrushCollectionName, "Create new BrushCollection", "Asset Name");
                }
                EditorGUILayout.EndHorizontal();


                //The active BrushList asset
                //brushes = EditorGUILayout.ObjectField(brushes, typeof(Object), true) as BrushCollection;


                #endregion

                #region Scroll view 
                //scroll view
                scrollViewScrollPosition = EditorGUILayout.BeginScrollView(scrollViewScrollPosition, false, false);
                int rowLength = 1;
                int maxRowLength = Mathf.FloorToInt((this.position.width - 35) / 100);
                if (maxRowLength < 1) {
                    maxRowLength = 1;
                }

                foreach (BrushObject brObj in brushes.brushes) {
                    //check if brushObject is null, if so skip this brush
                    if (brObj == null || brObj.brushObject == null) {
                        continue;
                    }

                    //check if row is longer than max row length
                    if (rowLength > maxRowLength) {
                        rowLength = 1;
                        EditorGUILayout.EndHorizontal();
                    }
                    //begin row if rowLength == 1
                    if (rowLength == 1) {
                        EditorGUILayout.BeginHorizontal();
                    }

                    //change color
                    Color guiColor = GUI.backgroundColor;
                    if (brushes.selectedBrushes.Contains(brObj)) {
                        GUI.backgroundColor = SelectedColor;
                        if (brushes.primarySelectedBrush == brObj) {
                            GUI.backgroundColor = PrimarySelectedColor;
                        }
                    }

                    //Create the brush entry in the scroll view and check if the user clicked on the created button (change the currently selected/edited brush accordingly and add it to the current brushes if possible)
                    GUIContent btnContent = new GUIContent(AssetPreview.GetAssetPreview(brObj.brushObject), brObj.brushObject.name);
                    if (GUILayout.Button(btnContent, GUILayout.Width(100), GUILayout.Height(100))) {
                        //Add and remove brushes from the current brushes list
                        if (Event.current.control && !brushes.selectedBrushes.Contains(brObj)) {
                            brushes.selectedBrushes.Add(brObj);
                        } else if (brushes.selectedBrushes.Contains(brObj)) {
                            brushes.selectedBrushes.Remove(brObj);
                        }

                        //select the currently edited brush and deselect all selected brushes
                        if (!Event.current.control) {
                            brushes.selectedBrushes.Clear();
                            brushes.primarySelectedBrush = brObj;
                            brushes.selectedBrushes.Add(brObj);
                        }
                    }

                    GUI.backgroundColor = guiColor;
                    rowLength++;
                }

                //check if row is longer than max row length
                if (rowLength > maxRowLength) {
                    rowLength = 1;
                    EditorGUILayout.EndHorizontal();
                }
                //begin row if rowLength == 1
                if (rowLength == 1) {
                    EditorGUILayout.BeginHorizontal();
                }

                //add button
                if (GUILayout.Button("+", GUILayout.Width(100), GUILayout.Height(100))) {
                    AddObjectPopup.Init(brushes.brushes, this);
                }
                Color guiColorBGC = GUI.backgroundColor;

                //end horizontal and scroll view again
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndScrollView();

                #endregion

                #region Actions Group


                EditorGUILayout.BeginHorizontal();

                GUI.backgroundColor = green;
                if (GUILayout.Button(new GUIContent("Add Brush", "Add a new brush to the selection."))) {
                    AddObjectPopup.Init(brushes.brushes, this);
                }

                EditorGUI.BeginDisabledGroup(brushes.selectedBrushes.Count == 0 || brushes.primarySelectedBrush == null);
                GUI.backgroundColor = red;
                //remove selected brushes button
                if (GUILayout.Button(new GUIContent("Remove Current Brush(es)", "Removes the currently selected brush."))) {
                    if (brushes.selectedBrushes != null) {
                        foreach (BrushObject brush in brushes.selectedBrushes) {
                            brushes.brushes.Remove(brush);
                        }
                        brushes.selectedBrushes = new List<BrushObject>();
                    }
                }
                EditorGUI.EndDisabledGroup();
                //remove all brushes button
                EditorGUI.BeginDisabledGroup(brushes.brushes.Count == 0);
                if (GUILayout.Button(new GUIContent("Clear Brushes", "Removes all brushes.")) && RemoveAllBrushes_Dialog(brushes.brushes.Count)) {
                    brushes.RemoveAllBrushes();
                    copy = null;
                }
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndHorizontal();
                GUI.backgroundColor = guiColorBGC;

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();
                isPlacingEnabled = EditorGUILayout.Toggle(new GUIContent("Painting ebanled", "Should painting of gameobjects via left click be enabled?"), isPlacingEnabled);
                isErasingEnabled = EditorGUILayout.Toggle(new GUIContent("Erasing ebanled", "Should erasing of gameobjects via right click be enabled?"), isErasingEnabled);
                EditorGUILayout.EndHorizontal();
                guiColorBGC = GUI.backgroundColor;

                if (brushes.selectedBrushes.Count > 0) {
                    EditorGUI.BeginDisabledGroup(brushes.spawnedObjects.Count == 0);

                    GUI.backgroundColor = green;
                    if (GUILayout.Button(new GUIContent("Permanently Apply Spawned GameObjects (" + brushes.spawnedObjects.Count + ")", "Permanently apply the gameobjects that have been spawned with GO brush, so they can not be erased by accident anymore."))) {
                        brushes.ApplyCachedObjects();
                        lastPlacementPositions.Clear();
                    }


                    GUI.backgroundColor = red;
                    if (GUILayout.Button(new GUIContent("Remove All Spawned GameObjects (" + brushes.spawnedObjects.Count + ")", "Removes all spawned objects from the scene that have not been applied before.")) && RemoveAllCachedObjects_Dialog(brushes.spawnedObjects.Count)) {
                        brushes.DeleteSpawnedObjects();
                        lastPlacementPositions.Clear();
                    }
                    EditorGUI.EndDisabledGroup();
                }



                GUI.backgroundColor = guiColorBGC;

                #endregion

                #region Brush Details
                //don't show the details of the current brush if we do not have selected a current brush
                if (brushes.selectedBrushes != null && brushes.primarySelectedBrush != null && brushes.brushes.Count > 0 && brushes.selectedBrushes.Count > 0) {
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();
                    GUI.backgroundColor = yellow;
                    EditorGUILayout.LabelField("Brush Details" + " - (" + brushes.primarySelectedBrush.brushObject.name + ")", EditorStyles.boldLabel);
                    if (GUILayout.Button(new GUIContent("Copy", "Copies the brush."), GUILayout.MaxWidth(50))) {
                        copy = brushes.primarySelectedBrush;
                    }
                    EditorGUI.BeginDisabledGroup(copy == null);
                    if (GUILayout.Button(new GUIContent("Paste", "Pastes the details of the brush in the clipboard."), GUILayout.MaxWidth(50))) {
                        brushes.primarySelectedBrush.PasteDetails(copy);
                    }
                    GUI.backgroundColor = guiColorBGC;
                    EditorGUI.EndDisabledGroup();
                    if (GUILayout.Button(new GUIContent("Reset", "Restores the defaults settings of the brush details."), GUILayout.MaxWidth(50))) {
                        brushes.primarySelectedBrush.ResetDetails();
                    }
                    EditorGUILayout.EndHorizontal();

                    brushes.primarySelectedBrush.parentContainer = EditorGUILayout.ObjectField("Parent", brushes.primarySelectedBrush.parentContainer, typeof(Transform), true) as Transform;
                    brushes.primarySelectedBrush.density = EditorGUILayout.Slider(new GUIContent("Density", "Changes the density of the brush, i.e. how many gameobjects are spawned inside the radius of the brush."), brushes.primarySelectedBrush.density, 0f, 5f);
                    brushes.primarySelectedBrush.brushSize = EditorGUILayout.Slider(new GUIContent("Brush Size", "The radius of the brush."), brushes.primarySelectedBrush.brushSize, 0f, 25f);
                    brushes.primarySelectedBrush.offsetFromPivot = EditorGUILayout.Vector3Field(new GUIContent("Offset from Pivot", "Changes the offset of the spawned gameobject from the calculated position. This allows you to correct the position of the spawned objects, if you find they are floating for example due to a not that correct pivot on the gameobject/prefab."), brushes.primarySelectedBrush.offsetFromPivot);
                    brushes.primarySelectedBrush.rotOffsetFromPivot = EditorGUILayout.Vector3Field(new GUIContent("Rotational Offset", "Changes the rotational offset that is applied to the prefab/gameobject when spawning it. This allows you to current the rotation of the spawned objects."), brushes.primarySelectedBrush.rotOffsetFromPivot);


                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(new GUIContent("Min and Max Scale", "The min and max range of the spawned gameobject. If they are not the same value a random value in between the min and max is going to be picked."));
                    EditorGUILayout.MinMaxSlider(ref brushes.primarySelectedBrush.minScale, ref brushes.primarySelectedBrush.maxScale, 0.001f, 50);
                    brushes.primarySelectedBrush.minScale = EditorGUILayout.FloatField(brushes.primarySelectedBrush.minScale);
                    brushes.primarySelectedBrush.maxScale = EditorGUILayout.FloatField(brushes.primarySelectedBrush.maxScale);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    brushes.primarySelectedBrush.randomizeXRotation = EditorGUILayout.Toggle(new GUIContent("Randomize X Rotation", "Should the rotation be randomized on the x axis?"), brushes.primarySelectedBrush.randomizeXRotation);
                    brushes.primarySelectedBrush.randomizeYRotation = EditorGUILayout.Toggle(new GUIContent("Randomize Y Rotation", "Should the rotation be randomized on the y axis?"), brushes.primarySelectedBrush.randomizeYRotation);
                    brushes.primarySelectedBrush.randomizeZRotation = EditorGUILayout.Toggle(new GUIContent("Randomize Z Rotation", "Should the rotation be randomized on the z axis?"), brushes.primarySelectedBrush.randomizeZRotation);
                    EditorGUILayout.EndHorizontal();

                    brushes.primarySelectedBrush.alignToSurface = EditorGUILayout.Toggle(new GUIContent("Align to Surface", "This option allows you to align the instantiated gameobjects to the surface you are painting on."), brushes.primarySelectedBrush.alignToSurface);

                    brushes.primarySelectedBrush.allowIntercollision = EditorGUILayout.Toggle(new GUIContent("Allow Intercollision", "Should the spawned objects be considered for the spawning of new objects? If so, newly spawned objects can be placed on top of previously (not yet applied) objects."), brushes.primarySelectedBrush.allowIntercollision);


                    EditorGUILayout.Space();
                    EditorGUILayout.Space();


                    GUI.backgroundColor = yellow;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Filters" + " - (" + brushes.primarySelectedBrush.brushObject.name + ")", EditorStyles.boldLabel);
                    if (GUILayout.Button(new GUIContent("Copy", "Copies the brush."), GUILayout.MaxWidth(50))) {
                        copy = brushes.primarySelectedBrush;
                    }
                    EditorGUI.BeginDisabledGroup(copy == null);
                    if (GUILayout.Button(new GUIContent("Paste", "Pastes the filters of the brush in the clipboard."), GUILayout.MaxWidth(50))) {
                        brushes.primarySelectedBrush.PasteFilters(copy);
                    }
                    EditorGUI.EndDisabledGroup();
                    GUI.backgroundColor = guiColorBGC;
                    if (GUILayout.Button(new GUIContent("Reset", "Restores the defaults settings of the brush filters."), GUILayout.MaxWidth(50))) {
                        brushes.primarySelectedBrush.ResetFilters();
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(new GUIContent("Min and Max Slope", "The range of slope that is required for an object to be placed. If the slope is not in that range, no object is going to be placed."));
                    EditorGUILayout.MinMaxSlider(ref brushes.primarySelectedBrush.minSlope, ref brushes.primarySelectedBrush.maxSlope, 0, 360);
                    brushes.primarySelectedBrush.minSlope = EditorGUILayout.FloatField(brushes.primarySelectedBrush.minSlope);
                    brushes.primarySelectedBrush.maxSlope = EditorGUILayout.FloatField(brushes.primarySelectedBrush.maxSlope);
                    EditorGUILayout.EndHorizontal();

                    SerializedProperty sp = serializedObject_brushObject.FindProperty("primarySelectedBrush").FindPropertyRelative("layerFilter");

                    EditorGUILayout.BeginHorizontal();
                    brushes.primarySelectedBrush.isTagFilteringEnabled = EditorGUILayout.Toggle("Enable Tag Filtering", brushes.primarySelectedBrush.isTagFilteringEnabled);
                    if (brushes.primarySelectedBrush.isTagFilteringEnabled) {
                        brushes.primarySelectedBrush.tagFilter = EditorGUILayout.TagField(new GUIContent("Tag Filter", "Limits the painting to objects that have a specific tag on them."), brushes.primarySelectedBrush.tagFilter);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    serializedObject_brushObject.ApplyModifiedProperties();
                }

                //save AssetDatabase on any change
                if (GUI.changed && brushes != null) {
                    brushes.Save();
                }
                #endregion
            }
        }
        /// <summary>
        /// Delegate that handles Scene GUI events
        /// </summary>
        /// <param name="sceneView"></param>
        void OnSceneGUI(SceneView sceneView) {

            //don't do anything if the gameobject brush window is not open
            if (!IsOpen) {
                return;
            }

            //Draw Brush in the scene view
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;
            if (isPlacingEnabled && brushes.selectedBrushes != null && Physics.Raycast(ray, out hit)) {
                Color color = Color.cyan;
                color.a = 0.25f;
                Handles.color = color;
                Handles.DrawSolidArc(hit.point, hit.normal, Vector3.Cross(hit.normal, ray.direction), 360, brushes.GetMaximumBrushSizeFromCurrentBrushes());
                Handles.color = Color.white;
                Handles.DrawLine(hit.point, hit.point + hit.normal * 5);
            }

            //Check for the currently selected tool
            if (Tools.current != Tool.View) {
                //check if the bursh is used
                if (Event.current.rawType == EventType.MouseDown) {
                    //check used mouse button
                    if (Event.current.button == 0 && PlaceObjects()) {
                        Event.current.Use();
                        GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                    }
                    //check used mouse button
                    if (Event.current.button == 1) {
                        if (RemoveObjects()) {
                            Event.current.Use();
                        }
                    }
                }
                //check if the bursh is used
                if (Event.current.rawType == EventType.MouseDrag) {
                    //check used mouse button
                    if (Event.current.button == 0 && PlaceObjects()) {
                        Event.current.Use();
                        GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                    }
                    //check used mouse button
                    if (Event.current.button == 1) {
                        if (RemoveObjects()) {
                            Event.current.Use();
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Switches to the given brushCollection, to edit it. (now this collection it the currently displayed one in the editor window)
        /// </summary>
        /// <param name="collection"></param>
        public static void SwitchToBrushCollection(BrushCollection collection) {
            if (Instance != null) {
                if (collection != null) {
                    Instance.selectedBrushCollectionIndex = collection.GetIndex();
                    Instance.brushes = collection;
                }
            }
        }
        /// <summary>
        /// Creates a popup dialog to get the confirmation of the user that all brushes should be removed
        /// </summary>
        /// <param name="brushCount">How many brushes are going to be removed?</param>
        /// <returns></returns>
        bool RemoveAllBrushes_Dialog(int brushCount) {
            return EditorUtility.DisplayDialog(
                "Remove all Brushes?",
                "Are you sure you want to remove all brushes (" + brushCount + ") from this brushcollection?",
                "Remove all",
                "Cancel");
        }
        /// <summary>
        /// Creates a popup dialog to get the confirmation of the user that all placed objects should be removed
        /// </summary>
        /// <param name="count">How many objects are going to be removed?</param>
        /// <returns></returns>
        bool RemoveAllCachedObjects_Dialog(int count) {
            return EditorUtility.DisplayDialog(
                "Delete all cached GameObjects?",
                "Are you sure you want to delete all cached  GameObjects (" + count + ") from the scene?",
                "Delete all",
                "Cancel");
        }


        #endregion

        #region GO Brush functionality methods


        /// <summary>
        /// Places the objects
        /// returns true if objects were placed, false otherwise
        /// </summary>
        private bool PlaceObjects() {
            Debug.Log(lastPlacementPositions.Count);


            //only paint if painting is ebanled
            if (!isPlacingEnabled) {
                return false;
            }

            bool hasPlacedObjects = false;

            foreach (BrushObject brush in brushes.selectedBrushes) {

                //loop as long as we have not reached the max ammount of objects to spawn per call/brush usage (calculated by currentBrush.density * currentBrush.brushSize)
                int spawnCount = Mathf.RoundToInt(brush.density * brush.brushSize);
                if (spawnCount < 1) {
                    spawnCount = 1;
                }

                for (int i = 0; i < spawnCount; i++) {

                    //create gameobjects of the given type if possible
                    if (brush.brushObject != null && IsOpen) {

                        //raycast from the scene camera to find the position of the brush and create objects there
                        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                        ray.origin += new Vector3(Random.Range(-brush.brushSize, brush.brushSize), Random.Range(-brush.brushSize, brush.brushSize), Random.Range(-brush.brushSize, brush.brushSize));
                        Vector3 startPoint = ray.origin;
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit)) {

                            //return if we are too close to the previous placement position
                            if (arePositionsWithinRange(lastPlacementPositions, hit.point, brush.brushSize, brush.density)) {
                                continue;
                            }


                            //return if we are hitting an object that we have just spawned or don't if allowIntercollisionPlacement is enabled on the current brush
                            if (brushes.spawnedObjects.Contains(hit.collider.gameObject) && !brush.allowIntercollision) {
                                continue;
                            }

                            //calculate the angle and abort if it is not in the specified range/filter
                            float angle = Vector3.Angle(Vector3.up, hit.normal);
                            if (angle < brush.minSlope || angle > brush.maxSlope) {
                                continue;
                            }

                            //check if the layer of the hit object is in our layermask filter
                            if (brush.layerFilter != (brush.layerFilter | (1 << hit.transform.gameObject.layer))) {
                                continue;
                            }

                            //check if tag filtering is active, if so check the tags
                            if (brush.isTagFilteringEnabled && hit.transform.tag != brush.tagFilter) {
                                continue;
                            }

                            //randomize position
                            Vector3 position = hit.point + brush.offsetFromPivot;

                            //instantiate prefab or clone object
                            GameObject obj;
                            if (brush.brushObject.gameObject.scene.name != null) {
                                obj = Instantiate(brush.brushObject, position, Quaternion.identity);
                            } else {
                                obj = PrefabUtility.InstantiatePrefab(brush.brushObject) as GameObject;
                                obj.transform.position = position;
                                obj.transform.rotation = Quaternion.identity;
                            }

                            //check for parent container
                            if (brush.parentContainer != null) {
                                obj.transform.parent = brush.parentContainer;
                            }

                            hasPlacedObjects = true;

                            //register created objects to the undo stack
                            Undo.RegisterCreatedObjectUndo(obj, "Created " + obj.name + " with brush");

                            //check if we should align the object to the surface we are "painting" on
                            if (brush.alignToSurface) {
                                obj.transform.up = hit.normal;
                            }

                            //Randomize rotation
                            Vector3 rot = brush.rotOffsetFromPivot;
                            if (brush.randomizeXRotation)
                                rot.x = Random.Range(0, 360);
                            if (brush.randomizeYRotation)
                                rot.y = Random.Range(0, 360);
                            if (brush.randomizeZRotation)
                                rot.z = Random.Range(0, 360);

                            //apply rotation
                            obj.transform.Rotate(rot, Space.Self);

                            //randomize scale
                            float scale = Random.Range(brush.minScale, brush.maxScale);
                            obj.transform.localScale = new Vector3(scale, scale, scale);

                            //Add object to list so it can be removed later on
                            brushes.spawnedObjects.AddRange(GetAllChildren(obj));

                            //save placement position
                            lastPlacementPositions.Add(obj, hit.point);
                        }
                    }
                }
            }

            return hasPlacedObjects;
        }
        /// <summary>
        /// remove objects that are in the brush radius around the brush.
        /// It returns true if it removed something, false otherwise
        /// </summary>
        private bool RemoveObjects() {

            //return if erasing is disabled
            if (!isErasingEnabled) {
                return false;
            }

            bool hasRemovedSomething = false;

            foreach (BrushObject brush in brushes.selectedBrushes) {
                //raycast to fin brush position
                RaycastHit hit;
                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                List<GameObject> objsToRemove = new List<GameObject>();
                if (Physics.Raycast(ray, out hit)) {

                    //loop over all spawned objects to find objects that can be removed
                    foreach (GameObject obj in brushes.spawnedObjects) {
                        if (obj != null && Vector3.Distance(obj.transform.position, hit.point) < brush.brushSize) {
                            objsToRemove.Add(obj);
                        }
                    }

                    //delete the before found objects
                    foreach (GameObject obj in objsToRemove) {
                        brushes.spawnedObjects.Remove(obj);
                        if (lastPlacementPositions.ContainsKey(obj))
                            lastPlacementPositions.Remove(obj);
                   
                        DestroyImmediate(obj);
                        hasRemovedSomething = true;
                    }
                    objsToRemove.Clear();
                }
            }

            return hasRemovedSomething;
        }


        #endregion

        #region Utility


        /// <summary>
        /// Generates a Color object by r g b values (Range 0, 256)
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Color ColorFromRGB(int r, int g, int b) {
            return new Color((float)r / 256, (float)g / 256, (float)b / 256);
        }

        /// <summary>
        /// Get all child game objects parrented to this one.
        /// Includes the object itself
        /// </summary>
        /// <param name="obj">The object to look for child objects</param>
        /// <returns></returns>
        public static GameObject[] GetAllChildren(GameObject obj) {
            List<GameObject> children = new List<GameObject>();
            if (obj != null) {
                foreach(Transform child in obj.transform) {
                    children.Add(child.gameObject);
                }
            }
            children.Add(obj);
            return children.ToArray();
        }

        /// <summary>
        /// Checks if any of the provided positions is in range to a given point
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="point"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static bool arePositionsWithinRange(Dictionary<GameObject, Vector3> positions, Vector3 point, float range, float density) {
            var values = positions.Values;
            float adjustedRange = (float) range / density;
            Debug.Log(range + " - " + adjustedRange);
            foreach(Vector3 position in values) {
                if (Vector3.Distance(position, point) <= adjustedRange) {
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}

