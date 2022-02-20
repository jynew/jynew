using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Text;

namespace MTE
{
    internal class RaiseLowerEditor : IEditor
    {
        public int Id { get; } = 1;

        public bool Enabled { get; set; } = true;

        public string Name { get; } = "RaiseLowerEditor";
        
        public Texture Icon { get; } =
            EditorGUIUtility.IconContent("TerrainInspector.TerrainToolRaise").image;

        public bool WillEditMesh { get; } = true;

        public bool WantMouseMove { get; } = false;

        #region Parameters

        #region Constant
        // default
        const float DefaultBrushSize = 4;
        const float DefaultSpeed = 1f;
        // min/max
        const float MinBrushSize = 0.1f;
        const float MaxBrushSize = 10f;
        const float MinStrength = 0.01f;
        const float MaxStrength = 1f;
        #endregion

        /// <summary>
        /// Current brush index (not used)
        /// </summary>
        public int brushIndex = 0;

        /// <summary>
        /// Going up/down
        /// </summary>
        public bool GoingUp = true;

        /// <summary>
        /// Brush size (cell count)
        /// </summary>
        private float brushSize;

        /// <summary>
        /// Brush strength
        /// </summary>
        private float strength;

        /// <summary>
        /// Falloff curve
        /// </summary>
        private AnimationCurve falloffCurve;

        /// <summary>
        /// Brush size (unit: brush unit)
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
                    EditorPrefs.SetFloat("MTE_RaiseLowerEditor.brushSize", value);
                }
            }
        }

        private float BrushSizeInU3D
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return BrushSize * Settings.BrushUnit; }
        }

        /// <summary>
        /// Brush strength
        /// </summary>
        public float Strength
        {
            get { return strength; }
            set
            {
                value = Mathf.Clamp(value, MinStrength, MaxStrength);
                if (!MathEx.AmostEqual(value, strength))
                {
                    strength = value;
                    EditorPrefs.SetFloat("MTE_RaiseLowerEditor.strength", value);
                }
            }
        }

        #endregion

        public RaiseLowerEditor()
        {
            MTEContext.EnableEvent += (sender, args) =>
            {
                if (MTEContext.editor == this)
                {
                    LoadSavedParamter();
                }
            };

            // Load default parameters
            brushSize = DefaultBrushSize;
            strength = DefaultSpeed;
            falloffCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }

        private void LoadSavedParamter()
        {
            // Load parameters from the EditorPrefs
            brushSize = EditorPrefs.GetFloat("MTE_RaiseLowerEditor.brushSize", DefaultBrushSize);
            strength = EditorPrefs.GetFloat("MTE_RaiseLowerEditor.strength", DefaultSpeed);
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
                    Strength -= 0.01f;
                    MTEEditorWindow.Instance.Repaint();
                }),
                new Hotkey(this, KeyCode.RightBracket, () =>
                {
                    Strength += 0.01f;
                    MTEEditorWindow.Instance.Repaint();
                }),
                new Hotkey(this, KeyCode.Space, () =>
                {
                    //toggle push up/down
                    GoingUp = !GoingUp;
                    MTEEditorWindow.Instance.Repaint();
                })
            };
        }

        public string Header { get { return StringTable.Get(C.RaiseLower_Header); } }
        public string Description { get { return StringTable.Get(C.RaiseLower_Description); } }

        public void DoArgsGUI()
        {
            var defaultGUIColor = GUI.contentColor;
            var defaultFontSize = GUI.skin.toggle.fontSize;
            GUI.color = GoingUp ? Color.green : Color.green*0.8f;
            GUI.skin.button.fontSize = 40;
            GUI.enabled = false;
            GUILayout.Toggle(GoingUp, GoingUp ? @"⇧" : @"⇩", "Button");
            GUI.color = defaultGUIColor;
            GUI.skin.button.fontSize = defaultFontSize;
            GUI.enabled = true;

            //Settings
            if (!Settings.CompactGUI)
            {
                GUILayout.Label(StringTable.Get(C.Settings), MTEStyles.SubHeader);
            }

            BrushSize = EditorGUILayoutEx.Slider(StringTable.Get(C.Size), "-", "+", BrushSize, MinBrushSize, MaxBrushSize);
            Strength = EditorGUILayoutEx.Slider(StringTable.Get(C.Strength), "[", "]", Strength, MinStrength, MaxStrength);

            EditorGUILayout.BeginHorizontal();
            {
                var label = new GUIContent(StringTable.Get(C.Falloff));
                var size = GUIStyle.none.CalcSize(label);
                GUILayout.Label(label, GUILayout.Width(size.x + 10), GUILayout.MinWidth(60));
                EditorGUILayout.LabelField("", GUILayout.Width(15));
                var rect = GUILayoutUtility.GetRect(128, 128, GUILayout.ExpandWidth(false));
                falloffCurve = EditorGUI.CurveField(rect, falloffCurve, Color.green, new Rect(0, 0, 1, 1));
                EditorGUILayout.LabelField("", GUILayout.Width(15));
            }
            EditorGUILayout.EndHorizontal();
        }

        private readonly List<MeshModifyGroup> modifyGroups = new List<MeshModifyGroup>(4);
        private readonly StringBuilder notice = new StringBuilder();

        public void OnSceneGUI()
        {
            notice.Length = 0;

            do
            {
                var e = Event.current;
                if (e.commandName == "UndoRedoPerformed")
                {
                    SceneView.RepaintAll();
                    break;
                }

                if (!(EditorWindow.mouseOverWindow is SceneView))
                {
                    notice.AppendLine("Mouse not in SceneView.");
                    break;
                }

                // do nothing when mouse middle/right button, control/alt key is pressed
                if (e.button != 0 || e.control || e.alt)
                {
                    break;
                }

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
                        notice.AppendLine("No target was hit.");
                        break;
                    }

                    if (Settings.ShowBrushRect)
                    {
                        Utility.ShowBrushRect(raycastHit.point, BrushSizeInU3D);
                        if (Settings.DebugMode)
                        {
                            var oldZTest = Handles.zTest;
                            Handles.color = Color.red;
                            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
                            Handles.ArrowHandleCap(0, raycastHit.point + new Vector3(0, 200, 0), Quaternion.LookRotation(Vector3.down), 1000f, EventType.Repaint);
                            Handles.zTest = oldZTest;
                        }
                    }

                    if (!MTEContext.Targets.Any())
                    {
                        notice.AppendLine("No targets. Check if editing mesh-terrain(s) match the tag and layer of MTE editor.");
                    }
                    else
                    {
                        // collect modify group
                        modifyGroups.Clear();
                        foreach (var target in MTEContext.Targets)
                        {
                            var meshFilter = target.GetComponent<MeshFilter>();
                            var meshCollider = target.GetComponent<MeshCollider>();
                            var mesh = meshFilter.sharedMesh;

                            var hitPointLocal = target.transform.InverseTransformPoint(raycastHit.point);
                            List<int> vIndex;
                            List<float> vDistance;
                            VertexMap.GetAffectedVertex(target, hitPointLocal, BrushSizeInU3D,
                                out vIndex, out vDistance);
                            
                            notice.AppendFormat("About to modify {0} vertices in {1}.\n", vIndex.Count, target.name);
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
                            Utility.Record("Raise/Lower", raycastHit.point, this.BrushSizeInU3D);
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
                                var vDistance = modifyGroup.vDistance;
                                RaiseLower(mesh, meshCollider, vIndex, vDistance);
                                notice.AppendFormat("Modified {0} vertices in {1}.\n", vIndex.Count, modifyGroup.gameObject.name);
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
            } while (false);

            if (Settings.DebugMode)
            {
                Handles.BeginGUI();
                GUILayout.Label(this.notice.ToString());
                Handles.EndGUI();
            }
        }

        /// <summary>
        /// Do raise/lower height for selecting vertexes
        /// </summary>
        /// <param name="mesh">mesh to modify</param>
        /// <param name="meshCollider">meshcollider related to the mesh</param>
        /// <param name="vIndex">indexes of modifying vertexes</param>
        /// <param name="vDistance">distances of modifying vertexes to the center, corresponding to vVertexIndex</param>
        public void RaiseLower(Mesh mesh, MeshCollider meshCollider, List<int> vIndex, List<float> vDistance)
        {
            var vertexes = mesh.vertices;
            var brushSizeInU3D = this.BrushSizeInU3D;
            var brushStrengthInU3D = Strength * Settings.BrushUnit;

            for (var i = 0; i < vIndex.Count; ++i)
            {
                var index = vIndex[i];
                var distance = vDistance[i];

                var x = Mathf.Clamp01(1 - distance / brushSizeInU3D);
                var falloffFactor = falloffCurve.Evaluate(x);//get falloff factor from the curve
                var offset = brushStrengthInU3D * falloffFactor;
                if (GoingUp)
                {
                    vertexes[index].y += offset;
                }
                else
                {
                    vertexes[index].y -= offset;
                }
            }

            mesh.vertices = vertexes;

            MTEEditorWindow.Instance.SetMeshDirty(meshCollider.gameObject);
            MTEEditorWindow.Instance.SetMeshColliderDirty(meshCollider, mesh.vertexCount);
        }
    }

}