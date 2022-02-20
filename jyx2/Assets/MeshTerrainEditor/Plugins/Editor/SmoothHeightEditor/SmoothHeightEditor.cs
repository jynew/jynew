using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MTE
{
    internal class SmoothHeightEditor : IEditor
    {
        public int Id { get; } = 3;

        public bool Enabled { get; set; } = true;

        public string Name { get; } = "SmoothHeightEditor";

        public Texture Icon { get; } =
            EditorGUIUtility.IconContent("TerrainInspector.TerrainToolSmoothHeight").image;

        public bool WantMouseMove { get; } = false;

        public bool WillEditMesh { get; } = true;

        #region Parameters

        #region Constant
        // default
        const float DefaultBrushSize = 4;
        const float DefaultSpeed = 0.5f;
        // min/max
        const float MinBrushSize = 0.1f;
        const float MaxBrushSize = 10f;
        const float MinSpeed = 0.01f;
        const float MaxSpeed = 1f;
        #endregion

        private float brushSize;
        private float speed;

        /// <summary>
        /// Brush size (unit: meter)
        /// </summary>
        public float BrushSize
        {
            get { return brushSize; }
            set
            {
                value = Mathf.Clamp(value, MinBrushSize, MaxBrushSize);
                if (!MathEx.AmostEqual(value, brushSize))
                {
                    brushSize = value;
                    EditorPrefs.SetFloat("MTE_SmoothHeightEditor.brushSize", value);
                }
            }
        }

        private float BrushSizeInU3D { get { return BrushSize * Settings.BrushUnit; } }

        /// <summary>
        /// Speed
        /// </summary>
        public float Speed
        {
            get { return speed; }
            set
            {
                value = Mathf.Clamp(value, MinSpeed, MaxSpeed);
                if (!MathEx.AmostEqual(value, speed))
                {
                    speed = value;
                    EditorPrefs.SetFloat("MTE_SmoothHeightEditor.speed", value);
                }
            }
        }

        public int JumpUpdateFrameCount = 5;

        #endregion

        public SmoothHeightEditor()
        {
            MTEContext.EnableEvent += (sender, args) =>
            {
                if (MTEContext.editor == this)
                {
                    LoadSavedParamter();
                }
            };

            MTEContext.EditTypeChangedEvent += (sender, args) =>
            {
                if (MTEContext.editor == this)
                {
                    LoadSavedParamter();
                }
            };

            brushSize = DefaultBrushSize;
            speed = DefaultSpeed;
        }

        private void LoadSavedParamter()
        {
            // Load parameters from the EditorPrefs
            brushSize = EditorPrefs.GetFloat("MTE_SmoothHeightEditor.brushSize", DefaultBrushSize);
            speed = EditorPrefs.GetFloat("MTE_SmoothHeightEditor.speed", DefaultSpeed);
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
                    Speed -= 0.01f;
                    MTEEditorWindow.Instance.Repaint();
                }),
                new Hotkey(this, KeyCode.RightBracket, () =>
                {
                    Speed += 0.01f;
                    MTEEditorWindow.Instance.Repaint();
                })
            };
        }

        public string Header { get { return StringTable.Get(C.SmoothHeight_Header); } }
        public string Description { get { return StringTable.Get(C.SmoothHeight_Description); } }

        public void DoArgsGUI()
        {
            if (!Settings.CompactGUI)
            {
                GUILayout.Label(StringTable.Get(C.Settings), MTEStyles.SubHeader);
            }
            BrushSize = EditorGUILayoutEx.Slider(StringTable.Get(C.Size), "-", "+", BrushSize, MinBrushSize, MaxBrushSize);
            Speed = EditorGUILayoutEx.Slider(StringTable.Get(C.Speed), "[", "]", Speed, MinSpeed, MaxSpeed);
        }

        private readonly List<MeshModifyGroup> modifyGroups = new List<MeshModifyGroup>(4);

        public void OnSceneGUI()
        {
            var e = Event.current;

            if (!(EditorWindow.mouseOverWindow is SceneView))
            {
                return;
            }

            // do nothing when mouse middle/right button, control/alt key is pressed
            if (e.button != 0 || e.control || e.alt)
                return;

            HandleUtility.AddDefaultControl(0);
            RaycastHit raycastHit;
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            //Debug.Log(string.Format("mouse at ({0}, {1})", e.mousePosition.x, e.mousePosition.y));
            if (Physics.Raycast(ray, out raycastHit,
                Mathf.Infinity,
                1 << MTEContext.TargetLayer//only hit target layer
            ))
            {
                //check tag
                if (!raycastHit.transform.CompareTag(MTEContext.TargetTag))
                {
                    return;
                }
                if (Settings.ShowBrushRect)
                {
                    Utility.ShowBrushRect(raycastHit.point, BrushSizeInU3D);
                }

                // collect modifiy group
                modifyGroups.Clear();
                foreach (var target in MTEContext.Targets)
                {
                    var meshFilter = target.GetComponent<MeshFilter>();
                    var meshCollider = target.GetComponent<MeshCollider>();
                    var mesh = meshFilter.sharedMesh;

                    var hitPointLocal = target.transform.InverseTransformPoint(raycastHit.point);
                    List<int> vIndex;
                    VertexMap.GetAffectedVertex(target, hitPointLocal, BrushSizeInU3D, out vIndex);

                    if (Settings.ShowAffectedVertex)
                    {
                        Utility.ShowAffectedVertices(target, mesh, vIndex);
                    }
                    if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
                    {
                        if (vIndex.Count != 0)
                        {
                            modifyGroups.Add(new MeshModifyGroup(target, mesh, meshCollider, vIndex, vDistance: null));
                        }
                    }
                }

                //record undo operation for targets that to be modified
                if (e.type == EventType.MouseDown)
                {
                    Utility.Record("Smooth Height", raycastHit.point, this.BrushSizeInU3D);
                }

                // execute the modification
                if (modifyGroups.Count != 0)
                {
                    for (int i = 0; i < modifyGroups.Count; i++)
                    {
                        var modifyGroup = modifyGroups[i];
                        SmoothHeight(modifyGroup.gameObject, modifyGroup.meshCollider, modifyGroup.vIndex);
                    }
                }

                if (e.type == EventType.MouseUp)
                {
                    MTEEditorWindow.Instance.UpdateDirtyMeshCollidersImmediately();
                    MTEEditorWindow.Instance.HandleMeshSave();
                }

            }
            SceneView.RepaintAll();
        }

        /// <summary>
        /// Smooth heights of vertexes
        /// </summary>
        public void SmoothHeight(GameObject obj, MeshCollider meshCollider, List<int> vIndex)
        {
            float currentBrushSize = BrushSizeInU3D;

            var mesh = obj.GetComponent<MeshFilter>().sharedMesh;
            Transform transform = obj.transform;
            var copied_vertexes = mesh.vertices;

            for (var i = 0; i < vIndex.Count; ++i)
            {
                var index = vIndex[i];
                var vertex = copied_vertexes[index];
                var vertex_world = transform.TransformPoint(vertex);

                var nearbyAverage = VertexMap.GetNearByVerticesAverageHeight(vertex_world, currentBrushSize);
                var originalHeight = vertex.y;
                var average = 0.5f * nearbyAverage + 0.5f * originalHeight;
                var detla = average - originalHeight;

                var smoothRise = detla * Speed;
                copied_vertexes[index].y += smoothRise;
            }

            mesh.vertices = copied_vertexes;
            
            // rebuild vertex map, because smooth needs the real heights of each vertex
            VertexMap.Rebuild(obj);

            MTEEditorWindow.Instance.SetMeshDirty(obj);
            MTEEditorWindow.Instance.SetMeshColliderDirty(meshCollider, mesh.vertexCount);
        }
    }
}