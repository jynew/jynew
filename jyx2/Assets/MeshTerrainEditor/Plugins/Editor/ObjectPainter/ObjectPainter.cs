using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MTE
{
    internal class ObjectPainter : IEditor
    {
        public static ObjectPainter Instance;

        public int Id { get; } = 7;

        public string Header
        {
            get { return StringTable.Get(C.ObjectPainter_Header); }
        }

        public string Description
        {
            get { return StringTable.Get(C.ObjectPainter_Description); }
        }

        public bool Enabled { get; set; } = true;

        public string Name { get; } = "ObjectPainter";

        public Texture Icon { get; } =
            EditorGUIUtility.IconContent("Prefab Icon").image;

        public bool WantMouseMove { get; } = false;

        public bool WillEditMesh { get; } = false;

        #region Constant
        // default
        const float DefaultBrushSize = 1;
        const int DefaultBrushNumber = 1;
        const float DefaultBrushDirection = 0;
        const bool DefaultUseRandomDirection = true;
        const int DefaultReduction = 100;
        const bool DefaultAllowOverlap = false;
        // min/max
        const float MinBrushSize = 0.1f;
        const float MaxBrushSize = 10f;
        const int MinBrushNumber = 1;
        const int MaxBrushNumber = 50;
        private const int MinReduction = 1;
        private const int MaxReduction = 100;
        #endregion

        private ObjectDetail selectedDetail
        {
            get
            {
                return detailList[SelectedIndex];
            }
        }

        private GameObject target
        {
            get { return selectedDetail.Object; }
        }

        private Vector3 minScale
        {
            get { return selectedDetail.MinScale; }
        }
        
        private Vector3 maxScale
        {
            get { return selectedDetail.MaxScale; }
        }

        private float brushSize;
        private bool useRandomDirection;
        private float brushDirection = 0;
        public int brushNumber;
        public int reduction;
        private bool allowOverlap;
        private int containerInstanceId;

        private List<ObjectDetail> detailList = new List<ObjectDetail>();
        

        /// <summary>
        /// Selected object detail index
        /// </summary>
        public int SelectedIndex
        {
            get;
            set;
        }

        public Transform Container
        {
            get
            {
                if (containerInstanceId == 0)
                {
                    return null;
                }
                return EditorUtility.InstanceIDToObject(containerInstanceId) as Transform;
            }
            set
            {
                if (value == null)
                {
                    containerInstanceId = 0;
                }
                else
                {
                    containerInstanceId = value.GetInstanceID();
                    EditorPrefs.SetInt("MTE_ObjectPainter.containerInstanceId",
                        containerInstanceId);
                }
            }
        }

        /// <summary>
        /// Brush size (unit: 1 BrushUnit)
        /// </summary>
        public float BrushSize
        {
            get { return brushSize; }
            set
            {
                value = Mathf.Clamp(value, MinBrushSize, MaxBrushSize);

                if (!MathEx.AmostEqual(brushSize, value))
                {
                    brushSize = value;

                    EditorPrefs.SetFloat("MTE_ObjectPainter.brushSize", value);
                }
            }
        }

        //real brush size
        private float BrushSizeInU3D
        {
            get { return BrushSize * Settings.BrushUnit; }
        }

        /// <summary>
        ///
        /// </summary>
        public int BrushNumber
        {
            get
            {
                return brushNumber;
            }
            set
            {
                brushNumber = value;
                EditorPrefs.SetFloat("MTE_ObjectPainter.brushNumber", value);
            }
        }

        /// <summary>
        /// Brush direction, angle to north(+z)
        /// </summary>
        public float BrushDirection
        {
            get
            {
                return this.brushDirection;
            }

            set
            {
                value = Mathf.Clamp(value, 0, 2 * Mathf.PI);
                if (!MathEx.AmostEqual(value, this.brushDirection))
                {
                    EditorPrefs.SetFloat("MTE_ObjectPainter.brushDirection", this.brushDirection);
                    this.brushDirection = value;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool UseRandomDirection
        {
            get { return this.useRandomDirection; }
            set
            {
                if (value != useRandomDirection)
                {
                    useRandomDirection = value;
                    EditorPrefs.SetBool("MTE_ObjectPainter.useRandomDirection", value);
                }
            }
        }

        /// <summary>
        /// Removing strength (percent)
        /// </summary>
        public int Reduction
        {
            get
            {
                return this.reduction;
            }
            set
            {
                if (this.reduction != value)
                {
                    this.reduction = value;
                    EditorPrefs.SetInt("MTE_ObjectPainter.reduction", value);
                }
            }
        }
        
        public bool AllowOverlap
        {
            get
            {
                return this.allowOverlap;
            }

            set
            {
                if (value != this.allowOverlap)
                {
                    this.allowOverlap = value;
                    EditorPrefs.SetBool("MTE_ObjectPainter.allowOverlap", value);
                }
            }
        }

        private DetailListBox<ObjectDetail> detailListBox;

        public ObjectPainter()
        {
            MTEContext.EnableEvent += (sender, args) =>
            {
                if (MTEContext.editor == this)
                {
                    LoadSavedParamter();
                    LoadObjectDetailList();
                }
            };

            MTEContext.EditTypeChangedEvent += (sender, args) =>
            {
                if (MTEContext.editor == this)
                {
                    LoadSavedParamter();
                    LoadObjectDetailList();
                }
            };

            // Load default parameters
            brushSize = DefaultBrushSize;
            brushNumber = DefaultBrushNumber;
            brushDirection = DefaultBrushDirection;
            useRandomDirection = DefaultUseRandomDirection;
            reduction = DefaultReduction;
            allowOverlap = DefaultAllowOverlap;

            Instance = this;
        }
        
        public HashSet<Hotkey> DefineHotkeys()
        {
            return new HashSet<Hotkey>
            {
                new Hotkey(this, KeyCode.Minus, () =>
                {
                    BrushSize -= 1;
                    MTEEditorWindow.Instance.Repaint();
                }),
                new Hotkey(this, KeyCode.Equals, () =>
                {
                    BrushSize += 1;
                    MTEEditorWindow.Instance.Repaint();
                }),
                new Hotkey(this, KeyCode.LeftBracket, () =>
                {
                    BrushNumber -= 1;
                    MTEEditorWindow.Instance.Repaint();
                }),
                new Hotkey(this, KeyCode.RightBracket, () =>
                {
                    BrushNumber += 1;
                    MTEEditorWindow.Instance.Repaint();
                })
            };
        }

        private void LoadSavedParamter()
        {
            // Load parameters from the EditorPrefs
            brushSize = EditorPrefs.GetFloat("MTE_ObjectPainter.brushSize", DefaultBrushSize);
            brushNumber = EditorPrefs.GetInt("MTE_ObjectPainter.brushNumber", DefaultBrushNumber);
            brushDirection = EditorPrefs.GetFloat("MTE_ObjectPainter.brushDirection", DefaultBrushDirection);
            useRandomDirection = EditorPrefs.GetBool("MTE_ObjectPainter.useRandomDirection", DefaultUseRandomDirection);
            reduction = EditorPrefs.GetInt("MTE_ObjectPainter.reduction", DefaultReduction);
            containerInstanceId = EditorPrefs.GetInt("MTE_ObjectPainter.containerInstanceId", 0);
            allowOverlap =
                EditorPrefs.GetBool("MTE_ObjectPainter.allowOverlap", DefaultAllowOverlap);
        }

        public void DoArgsGUI()
        {
            // Details
            if (!Settings.CompactGUI)
            {
                GUILayout.Label(StringTable.Get(C.Prefab), MTEStyles.SubHeader);
            }

            // detail list box
            SelectedIndex = detailListBox.DoGUI(SelectedIndex);

            //Settings
            if (!Settings.CompactGUI)
            {
                EditorGUILayout.Space();
                GUILayout.Label(StringTable.Get(C.Settings), MTEStyles.SubHeader);
            }
            EditorGUILayout.BeginHorizontal();
            {
                var label = new GUIContent(StringTable.Get(C.Container));
                var size = GUIStyle.none.CalcSize(label);
                EditorGUILayout.LabelField(label, GUILayout.Width(size.x + 10), GUILayout.MinWidth(80));
                Container = (Transform)EditorGUILayout.ObjectField(Container, typeof(Transform), true);
            }
            EditorGUILayout.EndHorizontal();

            BrushSize = EditorGUILayoutEx.Slider(StringTable.Get(C.Size), "-", "+", BrushSize, MinBrushSize, MaxBrushSize);
            BrushNumber = EditorGUILayoutEx.IntSlider(StringTable.Get(C.Number), "[", "]", BrushNumber, MinBrushNumber, MaxBrushNumber);
            Reduction = EditorGUILayoutEx.IntSlider(StringTable.Get(C.Reduction), Reduction, MinReduction, MaxReduction);

            EditorGUILayout.BeginHorizontal();
            {
                var label = new GUIContent(StringTable.Get(C.Direction));
                var size = GUIStyle.none.CalcSize(label);
                EditorGUILayout.LabelField(label, GUILayout.Width(size.x + 10), GUILayout.MinWidth(60));

                EditorGUILayout.BeginVertical();
                UseRandomDirection = GUILayout.Toggle(UseRandomDirection, StringTable.Get(C.Random));
                if (!UseRandomDirection)
                {
                    EditorGUILayout.LabelField(string.Format("{0}°", Mathf.Rad2Deg * BrushDirection));
                    EditorGUILayout.HelpBox(StringTable.Get(C.Info_HowToRotate), MessageType.Info);
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                var label = new GUIContent(StringTable.Get(C.AllowOverlap));
                var size = GUIStyle.none.CalcSize(label);
                EditorGUILayout.LabelField(label, GUILayout.Width(size.x + 10), GUILayout.MinWidth(60));
                AllowOverlap = EditorGUILayout.Toggle(AllowOverlap);
            }
            EditorGUILayout.EndHorizontal();
        }

        private List<GameObject> items = new List<GameObject>();
        private List<Vector2> targetPositions = new List<Vector2>();
        private readonly List<GameObject> removeList = new List<GameObject>(256);
        private readonly RaycastHit[] raycastHits = new RaycastHit[256];
        public void OnSceneGUI()
        {
            var e = Event.current;
            if (e.commandName == "UndoRedoPerformed")
            {
                SceneView.RepaintAll();
                return;
            }

            if(!UseRandomDirection && e.control)
            {
                RaycastHit hit;
                Ray ray1 = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                if (Physics.Raycast(ray1, out hit,
                    Mathf.Infinity,
                    1 << MTEContext.TargetLayer//only hit target layer
                ))
                {
                    //check tag
                    if (!hit.transform.CompareTag(MTEContext.TargetTag))
                    {
                        return;
                    }

                    Handles.ArrowHandleCap(0, hit.point,
                        Quaternion.Euler(0, BrushDirection * Mathf.Rad2Deg, 0),
                        10 * Settings.PointSize, EventType.Repaint);
                }
            }

            // do nothing when mouse middle/right button, control/alt key is pressed
            if (e.button != 0 || e.alt)
                return;

            // no detail
            if (this.detailList.Count == 0)
            {
                return;
            }

            HandleUtility.AddDefaultControl(0);
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            var hitCount = Physics.RaycastNonAlloc(ray, raycastHits,
                Mathf.Infinity,
                1 << MTEContext.TargetLayer //only hit target layer
            );
            if (hitCount > 0)
            {
                RaycastHit raycastHit = new RaycastHit();
                for (var index = 0; index < hitCount; index++)
                {
                    var hit = raycastHits[index];
                    if (!hit.transform)
                    {
                        continue;
                    }

                    if (!MTEContext.Targets.Contains(hit.transform.gameObject))
                    {
                        continue;
                    }

                    raycastHit = hit;
                }

                if (Settings.ShowBrushRect)
                {
                    Utility.ShowBrushRect(raycastHit.point, BrushSizeInU3D);
                }

                var hitPoint = raycastHit.point;

                Handles.color = Color.green;
                Handles.DrawWireDisc(hitPoint, raycastHit.normal, BrushSizeInU3D);

                if (!UseRandomDirection)
                {
                    if (e.control)
                    {
                        GetObjectItemsInCircle(target, hitPoint, BrushSizeInU3D, items);
                    }
                }

                // not using random direction
                // hold control key and scroll wheel to change
                // 1. item's rotationY
                // 2. brush direction
                if (!UseRandomDirection && e.control && !e.isKey && e.type == EventType.ScrollWheel)
                {
                    float oldDirection = BrushDirection;
                    float direction = oldDirection;
                    ChangeDirection(e.delta.y, ref direction);

                    if (Mathf.Abs(direction - oldDirection) > Mathf.Epsilon)
                    {
                        UpdateObjects(items, Mathf.Rad2Deg * direction);

                        MTEEditorWindow.Instance.Repaint();
                        BrushDirection = direction;
                    }
                    e.Use();
                }
                else if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
                {
                    if (e.type == EventType.MouseDown)
                    {
                        objectPaintTransation =
                            new Undo.UndoTransaction(
                                e.shift ?
                                    "Object Painter: Delete Prefab Instances" :
                                    "Object Painter: Create Prefab Instances"
                                );
                        Undo.UndoRedoManager.Instance().StartTransaction(objectPaintTransation);
                    }

                    if (!e.shift)
                    {//adding
                        targetPositions.Clear();

                        MathEx.UniformPointsInCircle(
                            new Vector2(hitPoint.x, hitPoint.z),
                            this.BrushSizeInU3D,
                            this.BrushNumber,
                            ref targetPositions);
                        
                        CreateObjectInstances();
                    }
                    else
                    {//removing
                        removeList.Clear();
                        GetObjectItemsInCircle(target, hitPoint, BrushSizeInU3D, removeList);
                        int removeCount = Mathf.CeilToInt(this.Reduction / 100.0f * this.removeList.Count);
                        if (removeCount != 0)
                        {
                            var itemsRemoved = this.removeList.TakeRandom(removeCount).ToList();
                            RemoveObjectInstances(itemsRemoved);
                        }
                    }
                }

                if (e.type == EventType.MouseUp)
                {
                    if (objectPaintTransation != null)
                    {
                        Undo.UndoRedoManager.Instance().EndTransaction(objectPaintTransation);
                        Utility.RefreshHistoryViewer();
                        objectPaintTransation = null;
                    }
                }

            }

            SceneView.RepaintAll();
        }

        Undo.UndoTransaction objectPaintTransation;

        internal struct ObjectPainterUndoData
        {
            public Vector3 position;
            public Quaternion rotation;
            public Vector3 localScale;
            public Transform parent;
            public Object prefab;
        }

        private static Bounds GetBounds(GameObject gameObject)
        {
            bool found = false;
            Bounds bounds = new Bounds();
            var meshFilter = gameObject.GetComponent<MeshFilter>();
            if (meshFilter)
            {
                bounds = meshFilter.sharedMesh.bounds;
                found = true;
            }
            var collider = gameObject.GetComponent<Collider>();
            if (collider is MeshCollider)
            {
                var meshCollider = collider as MeshCollider;
                bounds = meshCollider.sharedMesh.bounds;
                found = true;
            }
            if (collider is BoxCollider)
            {
                var boxCollider = collider as BoxCollider;
                bounds = new Bounds(boxCollider.center, boxCollider.size);
                found = true;
            }

            if (found)
            {
                bounds = bounds.Transform(gameObject.transform);
                //TODO add bounds debugger in Debug mode
                return bounds;
            }

            MTEDebug.LogWarning("Failed to fetch bounds of gameObject:" +
                " fallback to default bounds at GameObject position with size of GameObject scale");

            return new Bounds(gameObject.transform.position, gameObject.transform.localScale);
        }

        private bool IntersectWithExistingObjects(GameObject gameObject)
        {
            var bounds = GetBounds(gameObject);
            if (Container != null)
            {
                int n = Container.childCount;
                for (int i = 0; i < n; i++)
                {
                    Transform child = Container.GetChild(i);
                    if (gameObject == child.gameObject)
                    {
                        continue;
                    }
                    Bounds existingObjectBounds = GetBounds(child.gameObject);
                    if (existingObjectBounds.Intersects(bounds))
                    {
                        return true;
                    }
                }
            }
            else//loop through all object listed in detail
            {
                var objects = UnityEngine.Object.FindObjectsOfType<GameObject>();
                foreach (var o in objects)
                {
                    if (gameObject == o)
                    {
                        continue;
                    }
                    foreach (var objectDetail in detailList)
                    {
                        var prefab = objectDetail.Object;
                        if (!CompatibilityUtil.IsInstanceOfPrefab(o, prefab))
                        {
                            continue;
                        }
                        
                        Bounds existingObjectBounds = GetBounds(o);
                        if (existingObjectBounds.Intersects(bounds))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void CreateObjectInstances()
        {
            List<GameObject> createdInstances = new List<GameObject>(targetPositions.Count);
            for (int j = 0; j < targetPositions.Count; j++)
            {
                var rotationY = UseRandomDirection ? Random.Range(0f, 180f) : Mathf.Rad2Deg * this.BrushDirection;
                var targetRotation = Quaternion.Euler(0, rotationY, 0);
                var targetPosition = targetPositions[j];
                RaycastHit hit;

                //If overlap is not allowed, we cannot place object on any position that hit any other collider
                //except mesh-terrains(editing target, namely MTEContent.Targets).
                if (!AllowOverlap)
                {
                    if (Physics.Raycast(
                        new Ray(new Vector3(targetPosition.x, 10000, targetPosition.y),
                            new Vector3(0, -1f, 0)),
                        out hit,
                        Mathf.Infinity
                    ))
                    {
                        if (!MTEContext.Targets.Contains(hit.transform.gameObject))
                        {
                            continue;
                        }
                    }
                }

                if (Physics.Raycast(
                    new Ray(new Vector3(targetPosition.x, 10000, targetPosition.y),
                        new Vector3(0, -1f, 0)),
                    out hit,
                    Mathf.Infinity,
                    1 << MTEContext.TargetLayer //only hit target layer
                ))
                {
                    if (!hit.transform)
                    {
                        continue;
                    }

                    if (!MTEContext.Targets.Contains(hit.transform.gameObject))
                    {
                        continue;
                    }

                    var o = PrefabUtility.InstantiatePrefab(target) as GameObject;
                    o.transform.position = hit.point;
                    o.transform.rotation = targetRotation;
                    o.transform.parent = Container;
                    if (selectedDetail.UseUnifiedScale)
                    {
                        var s = Random.Range(minScale.x, maxScale.x);
                        o.transform.localScale = new Vector3(s, s, s);
                    }
                    else
                    {
                        o.transform.localScale =
                            new Vector3(Random.Range(minScale.x, maxScale.x),
                                Random.Range(minScale.y, maxScale.y),
                                Random.Range(minScale.z, maxScale.z));
                    }

                    if (!AllowOverlap)
                    {
                        //remove object that will overlap with existing objects
                        //only object in detail list are considered
                        if (IntersectWithExistingObjects(o))
                        {
                            Object.DestroyImmediate(o);
                            continue;
                        }
                    }
                    createdInstances.Add(o);
                }
            }
            
            Undo.UndoRedoManager.Instance().Push(a =>
            {
                UndoCreate(a);
            }, createdInstances);
        }
        
        private void RemoveObjectInstances(List<GameObject> itemsRemoved)
        {
            List<ObjectPainterUndoData> removedObjects
                = new List<ObjectPainterUndoData>(itemsRemoved.Count);
            foreach (var instance in itemsRemoved)
            {
                var prefab = CompatibilityUtil.GetPrefabRoot(instance);
                if (!prefab)
                {//cannot get the prefab of this GameObject
                    continue;
                }
                var t = instance.transform;
                var position = t.position;
                var rotation = t.rotation;
                var localScale = t.localScale;
                var parent = t.parent;
                var undoData = new ObjectPainterUndoData
                {
                    position = position,
                    rotation = rotation,
                    localScale = localScale,
                    parent = parent,
                    prefab = prefab,
                };

                Object.DestroyImmediate(instance);

                removedObjects.Add(undoData);
            }
            
            Undo.UndoRedoManager.Instance().Push(a =>
            {
                RedoCreate(a);
            }, removedObjects);
        }

        private void RedoCreate(List<ObjectPainterUndoData> removedObjects)
        {
            List<GameObject> createdInstances = new List<GameObject>(removeList.Count);
            foreach (var undoData in removedObjects)
            {
                if (!undoData.prefab)
                {//ignore invalid prefab
                    continue;
                }
                var o = PrefabUtility.InstantiatePrefab(undoData.prefab) as GameObject;
                o.transform.position = undoData.position;
                o.transform.rotation = undoData.rotation;
                o.transform.parent = undoData.parent;
                o.transform.localScale = undoData.localScale;
                createdInstances.Add(o);
            }
            
            Undo.UndoRedoManager.Instance().Push(a =>
            {
                UndoCreate(a);
            }, createdInstances);
        }

        private void UndoCreate(List<GameObject> createdInstances)
        {
            List<ObjectPainterUndoData> removedObjects
                = new List<ObjectPainterUndoData>(createdInstances.Count);
            foreach (var instance in createdInstances)
            {
                if (!instance)
                {//already destroyed by others
                    continue;
                }

                var prefab = CompatibilityUtil.GetPrefabRoot(instance);
                if (!prefab)
                {//cannot get the prefab of this GameObject
                    continue;
                }
                var t = instance.transform;
                Vector3 position = t.position;
                Quaternion rotation = t.rotation;
                Vector3 localScale = t.localScale;
                Transform parent = t.parent;
                var undoData = new ObjectPainterUndoData
                {
                    position = position,
                    rotation = rotation,
                    localScale = localScale,
                    parent = parent,
                    prefab = prefab,
                };

                Object.DestroyImmediate(instance);

                removedObjects.Add(undoData);
            }

            Undo.UndoRedoManager.Instance().Push(a =>
            {
                RedoCreate(removedObjects);
            }, createdInstances);
        }


        private void ChangeDirection(float delta, ref float direction)
        {
            if(delta > 0)
            {
                direction -= Mathf.PI / 12;
            }
            else if(delta < 0)
            {
                direction += Mathf.PI / 12;
            }

            if(direction < 0)
            {
                direction += 2*Mathf.PI;
            }
            if (direction > 2*Mathf.PI)
            {
                direction -= 2*Mathf.PI;
            }
        }

        static Collider[] colliders = new Collider[256];
        private static void GetObjectItemsInCircle(GameObject prefab, Vector3 center, float radius, List<GameObject> result)
        {
            result.Clear();
            int length = Physics.OverlapSphereNonAlloc(center, radius, colliders, ~MTEContext.TargetLayer);
            for (int i = 0; i < length; i++)
            {
                var collider = colliders[i];
                var obj = collider.gameObject;
                if (CompatibilityUtil.IsPrefab(obj) && CompatibilityUtil.IsInstanceOfPrefab(obj, prefab))
                {
                    result.Add(collider.gameObject);
                }
            }
        }

        private void LoadObjectDetailList()
        {
            if (detailListBox == null)
            {
                detailListBox = new ObjectDetailListBox();
            }
            var path = Res.DetailDir + "SavedObjectDetailList.asset";
            var relativePath = Utility.GetUnityPath(path);
            var obj = AssetDatabase.LoadAssetAtPath<ObjectDetailList>(relativePath);
            if (obj != null && obj.list != null)
            {
                detailList = obj.list;
                detailListBox.SetEditingTarget(detailList);
                MTEDebug.Log($"ObjectDetailList loaded from {path}");
            }
            else
            {
                obj = ScriptableObject.CreateInstance<ObjectDetailList>();
                obj.list = new List<ObjectDetail>(4);
                AssetDatabase.CreateAsset(obj, relativePath);
                EditorUtility.SetDirty(obj);
                detailListBox.SetEditingTarget(detailList);
                MTEDebug.Log($"No ObjectDetailList found at {path}, created a new SavedObjectDetailList.asset.");
            }
        }

        /// <summary>
        /// Update height of GameObjects
        /// </summary>
        /// <param name="items"></param>
        private static void UpdateObjects(List<GameObject> items)
        {
            foreach (var item in items)
            {
                var pos = item.transform.position;
                var pos2D = new Vector2(pos.x, pos.y);
                var rayOrigin = new Vector3(pos2D.x, 99999f, pos2D.y);
                var ray = new Ray(rayOrigin, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit,
                    Mathf.Infinity,
                    1 << MTEContext.TargetLayer//only hit target layer
                ))
                {
                    //check tag
                    if (!hit.transform.CompareTag(MTEContext.TargetTag))
                    {
                        return;
                    }
                    pos.y = hit.point.y;
                    item.transform.position = pos;
                }
            }
        }

        /// <summary>
        /// Update rotation (Y) of GameObjects
        /// </summary>
        private static void UpdateObjects(List<GameObject> items, float rotationY)
        {
            foreach (var item in items)
            {
                item.transform.rotation = Quaternion.Euler(0, rotationY, 0);
            }
        }
    }
}