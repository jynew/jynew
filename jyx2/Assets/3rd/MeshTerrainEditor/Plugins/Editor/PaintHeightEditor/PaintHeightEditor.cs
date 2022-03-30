using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace MTE
{
    internal class PaintHeightEditor : IEditor
    {
        public int Id { get; } = 2;

        public bool Enabled { get; set; } = true;

        public string Name { get; } = "PaintHeightEditor";
        
        public Texture Icon { get; } =
            EditorGUIUtility.IconContent("TerrainInspector.TerrainToolSetHeight").image;

        public bool WillEditMesh { get; } = true;

        public bool WantMouseMove { get; } = false;

        #region Parameters

        #region Constant
        // default
        const float DefaultBrushSize = 4;
        const float DefaultSpeed = 1f;
        // min/max
        const float MinHeight = -2000;
        const float MaxHeight = 2000;
        const float MinBrushSize = 0.1f;
        const float MaxBrushSize = 10f;
        const float MinSpeed = 0.01f;
        const float MaxSpeed = 5f;
        #endregion

        private float brushSize;
        public float theHeight;
        private float speed;

        /// <summary>
        /// Brush size (unit: meter in world space)
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
                    EditorPrefs.SetFloat("MTE_PaintHeightEditor.brushSize", value);
                }
            }
        }

        private float BrushSizeInU3D { get { return BrushSize * Settings.BrushUnit; } }

        /// <summary>
        /// Targeting height (unit: meter in local space)
        /// </summary>
        public float TheHeight
        {
            get { return theHeight; }
            set { theHeight = value; }
        }

        /// <summary>
        /// Speed
        /// </summary>
        public float Speed
        {
            get { return speed; }
            set
            {
                value = Mathf.Clamp(value, MinSpeed, MaxSpeed);
                if (System.Math.Abs(value - speed) > 0.0001f)
                {
                    speed = value;
                    EditorPrefs.SetFloat("MTE_PaintHeightEditor.speed", value);
                }
            }
        }

        #endregion

        public PaintHeightEditor()
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

            // Load default parameters
            brushSize = DefaultBrushSize;
            speed = DefaultSpeed;
        }

        private void LoadSavedParamter()
        {
            // Load parameters from the EditorPrefs
            brushSize = EditorPrefs.GetFloat("MTE_PaintHeightEditor.brushSize", DefaultBrushSize);
            speed = EditorPrefs.GetFloat("MTE_PaintHeightEditor.speed", DefaultSpeed);
        }

        public HashSet<Hotkey> DefineHotkeys()
        {
            var hotkeys = new HashSet<Hotkey>
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
            return hotkeys;
        }

        public string Header { get { return StringTable.Get(C.PaintHeight_Header); } }
        public string Description { get { return StringTable.Get(C.PaintHeight_Description); } }

        public void DoArgsGUI()
        {
            if (!Settings.CompactGUI)
            {
                GUILayout.Label(StringTable.Get(C.Settings), MTEStyles.SubHeader);
            }
            TheHeight = EditorGUILayoutEx.Slider(StringTable.Get(C.Height), " ", " ", TheHeight, MinHeight, MaxHeight);
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

            // do nothing when mouse middle/right button, alt key is pressed
            if (e.button != 0 || e.alt)
                return;

            // hold control key to sample height
            if (e.control && !e.isKey)
            {
                float sampledHeight;
                if (SampleHeight(e.mousePosition, out sampledHeight))
                {
                    if (Mathf.Abs(TheHeight - sampledHeight) > Mathf.Epsilon)
                    {
                        MTEEditorWindow.Instance.Repaint();
                    }
                    TheHeight = sampledHeight;
                }
            }
            else
            {
                HandleUtility.AddDefaultControl(0);
                RaycastHit raycastHit;
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                //Debug.Log(string.Format("mouse at ({0}, {1})", e.mousePosition.x, e.mousePosition.y));
                if(Physics.Raycast(ray, out raycastHit,
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
                        List<float> vDistance;
                        VertexMap.GetAffectedVertex(target, hitPointLocal, BrushSizeInU3D, out vIndex, out vDistance);

                        if (Settings.ShowAffectedVertex)
                        {
                            Utility.ShowAffectedVertices(target, mesh, vIndex);
                        }
                        if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
                        {
                            if (vIndex.Count != 0)
                            {
                                modifyGroups.Add(new MeshModifyGroup(target, mesh, meshCollider, vIndex, vDistance));
                            }
                        }
                    }

                    //record undo operation for targets that to be modified
                    if (e.type == EventType.MouseDown)
                    {
                        Utility.Record("Paint Height", raycastHit.point, this.BrushSizeInU3D);
                    }

                    // execute the modification
                    if (modifyGroups.Count != 0)
                    {
                        for (int i = 0; i < modifyGroups.Count; i++)
                        {
                            var modifyGroup = modifyGroups[i];
                            var mesh = modifyGroup.mesh;
                            var meshCollider = modifyGroup.meshCollider;
                            var vIndex = modifyGroup.vIndex;
                            PaintHeight(mesh, meshCollider, vIndex);
                        }
                    }

                    if (e.type == EventType.MouseUp)
                    {
                        MTEEditorWindow.Instance.UpdateDirtyMeshCollidersImmediately();
                        MTEEditorWindow.Instance.HandleMeshSave();
                    }

                }
            }
            SceneView.RepaintAll();
        }

        /// <summary>
        /// sample height(y)
        /// </summary>
        /// <param name="mousePosition">screen position of mouse</param>
        /// <param name="height">sampled height</param>
        /// <returns>true: success/false: failed</returns>
        private static bool SampleHeight(Vector2 mousePosition, out float height)
        {
            height = 0;

            HandleUtility.AddDefaultControl(0);
            RaycastHit raycastHit;
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);

            if (Physics.Raycast(ray, out raycastHit))
            {
                if (!MTEContext.IsMatchFilter(raycastHit.transform.gameObject))
                {
                    return false;
                }

                Handles.color = Color.green;
                var size = HandleUtility.GetHandleSize(raycastHit.point) * Settings.PointSize;
                Handles.SphereHandleCap(0, raycastHit.point, Quaternion.identity, size, EventType.Repaint);

                var hitPointLocal = raycastHit.transform.InverseTransformPoint(raycastHit.point);//convert hitpoint's position to mesh's local coordinate

                height = hitPointLocal.y;
                return true;
            }
            return false;
        }


        /// <summary>
        /// Move height of vertex towards the specified height
        /// </summary>
        /// <param name="mesh">mesh to modify</param>
        /// <param name="meshCollider">meshcollider related to the mesh</param>
        /// <param name="vIndex">indexes of modifying vertexes</param>
        private void PaintHeight(Mesh mesh, MeshCollider meshCollider, List<int> vIndex)
        {
            var vertexes = mesh.vertices;
            for (var i = 0; i < vIndex.Count; ++i)
            {
                var index = vIndex[i];
                vertexes[index].y =
                    Mathf.MoveTowards(vertexes[index].y, TheHeight, Speed);
            }

            mesh.vertices = vertexes;

            MTEEditorWindow.Instance.SetMeshDirty(meshCollider.gameObject);
            MTEEditorWindow.Instance.SetMeshColliderDirty(meshCollider, mesh.vertexCount);
        }
    }
}