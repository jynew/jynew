using System;
using MTE.Undo;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MTE
{
    internal class VertexColorPainter : IEditor
    {
        public int Id { get; } = 5;

        public bool Enabled { get; set; } = true;

        public string Name { get; } = "VertexColorPainter";

        public Texture Icon { get; } = MTEStyles.PaintVertexColorToolIcon;

        public bool WantMouseMove { get; } = true;

        public bool WillEditMesh { get; } = false;

        #region Parameters

        #region Constant
        // default
        static readonly Color DefaultTheColor = new Color32(229, 96, 33, 255);
        const float DefaultBrushSize = 1f;
        const float DefaultSpeed = 0.05f;
        const float DefaultIntensity = 1f;
        const bool DefaultSingleChannelOnly = false;
        const int DefaultTheChannel = 3;//Alpha channel

        private static readonly Color[] DefaultColorPreset = { Color.red, Color.green, Color.blue, Color.yellow};

        // min/max
        const float MinBrushSize = 0.1f;
        const float MaxBrushSize = 10f;
        const float MinSpeed = 0.01f;
        const float MaxSpeed = 1f;
        const float MinIntensity = 0.0f;
        const float MaxIntensity = 2.0f;
        #endregion

        private int brushIndex = 0;
        private Color theColor = DefaultTheColor;
        private float brushSize;
        private float speed;
        private float intensity;

        /// <summary>
        /// color preset
        /// </summary>
        private readonly Color[] colorPreset = new Color[4];

        /// <summary>
        /// Is painting on a single channel?
        /// </summary>
        private bool singleChannelOnly = DefaultSingleChannelOnly;

        /// <summary>
        /// index of the channel: 0:r/1:g/2:b/3:a
        /// </summary>
        private int theChannel = DefaultTheChannel;

        /// <summary>
        /// Brush index
        /// </summary>
        public int BrushIndex
        {
            get { return brushIndex; }
            set
            {
                brushIndex = value;
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
                if (!MathEx.AmostEqual(value, brushSize))
                {
                    brushSize = value;
                    EditorPrefs.SetFloat("MTE_VertexColorPainter.brushSize", value);
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
                    EditorPrefs.SetFloat("MTE_VertexColorPainter.speed", value);
                }
            }
        }

        /// <summary>
        /// the color
        /// </summary>
        public Color TheColor
        {
            get { return theColor; }
            set { theColor = value; }
        }

        /// <summary>
        /// color intensity
        /// </summary>
        /// <remarks>[1.0f, 2.0f]</remarks>
        public float Intensity
        {
            get { return intensity; }
            set
            {
                value = Mathf.Clamp(value, MinIntensity, MaxIntensity);
                if (!MathEx.AmostEqual(value, intensity))
                {
                    intensity = value;
                    EditorPrefs.SetFloat("MTE_VertexColorPainter.intensity", value);
                }
            }
        }

        #endregion

        public VertexColorPainter()
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
            intensity = DefaultIntensity;
            colorPreset[0] = Color.red;
            colorPreset[1] = Color.green;
            colorPreset[2] = Color.blue;
            colorPreset[3] = Color.yellow;
        }

        private void LoadSavedParamter()
        {
            // Load parameters from the EditorPrefs
            brushSize = EditorPrefs.GetFloat("MTE_VertexColorPainter.brushSize", DefaultBrushSize);
            speed = EditorPrefs.GetFloat("MTE_VertexColorPainter.speed", DefaultSpeed);
            intensity = EditorPrefs.GetFloat("MTE_VertexColorPainter.intensity", DefaultIntensity);
            for (int i = 0; i < 4; ++i)
            {
                float r = EditorPrefs.GetFloat("MTE_ColorPreset[" + i + "].r", DefaultTheColor.r);
                float g = EditorPrefs.GetFloat("MTE_ColorPreset[" + i + "].g", DefaultTheColor.g);
                float b = EditorPrefs.GetFloat("MTE_ColorPreset[" + i + "].b", DefaultTheColor.b);
                float a = EditorPrefs.GetFloat("MTE_ColorPreset[" + i + "].a", DefaultTheColor.a);
                Color color = new Color(r, g, b, a);
                if (Mathf.Abs(r * g * b * a) < 0.001f)
                {
                    color = DefaultColorPreset[i];
                }
                colorPreset[i] = color;
            }
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

        public string Header { get { return StringTable.Get(C.PaintVertexColor_Header); } }
        public string Description { get { return StringTable.Get(C.PaintVertexColor_Description); } }

        public void DoArgsGUI()
        {
            // Brushes
            if (!Settings.CompactGUI)
            {
                GUILayout.Label(StringTable.Get(C.Brushes), MTEStyles.SubHeader);
            }
            EditorGUILayout.BeginHorizontal("box");
            {
                GUILayout.FlexibleSpace();
                BrushIndex = GUILayout.SelectionGrid(BrushIndex, MTEStyles.meshColorBrushTextures, 2,
                    MTEStyles.hoverableElementStyle, GUILayout.Height(58));
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            // Settings
            if (!Settings.CompactGUI)
            {
                EditorGUILayout.Space();
                GUILayout.Label(StringTable.Get(C.Settings), MTEStyles.SubHeader);
            }
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    var label = new GUIContent(StringTable.Get(C.ColorPresets));
                    GUILayout.Label(label, GUILayout.Width(LabelWidth));
                    Rect presetRect = GUILayoutUtility.GetRect(120, 45);
                    for (int i = 0; i < 4; i++)
                    {
                        if (GUI.Button(new Rect(presetRect.x + i*30, presetRect.y, 30, 30), string.Empty))
                        {
                            TheColor = colorPreset[i];
                        }
                        if (GUI.Button(new Rect(presetRect.x + i*30, presetRect.y + 30, 30, 15), "\u25b2"))
                        {
                            colorPreset[i] = TheColor;
                            EditorPrefs.SetFloat("MTE_ColorPreset[" + i + "].r", TheColor.r);
                            EditorPrefs.SetFloat("MTE_ColorPreset[" + i + "].g", TheColor.g);
                            EditorPrefs.SetFloat("MTE_ColorPreset[" + i + "].b", TheColor.b);
                            EditorPrefs.SetFloat("MTE_ColorPreset[" + i + "].a", TheColor.a);
                        }
                        EditorGUI.DrawRect(new Rect(presetRect.x + i*30 + 2, presetRect.y + 2, 26, 26), colorPreset[i]);
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    var label = new GUIContent(StringTable.Get(C.Color));
                    GUILayout.Label(label, GUILayout.Width(LabelWidth));
                    TheColor = EditorGUILayout.ColorField(TheColor);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    var label = new GUIContent(StringTable.Get(C.SingleChannel));
                    GUILayout.Label(label, GUILayout.Width(LabelWidth));
                    singleChannelOnly = GUILayout.Toggle(singleChannelOnly, GUIContent.none, GUILayout.Width(20));
                    if (singleChannelOnly)
                    {
                        theChannel = GUILayout.Toolbar(theChannel, RGBA, GUILayout.Width(160));
                        GUILayout.Label(TheColor[theChannel].ToString());
                    }
                }
                EditorGUILayout.EndHorizontal();
                
                BrushSize = EditorGUILayoutEx.Slider(StringTable.Get(C.Size), "-", "+", BrushSize, MinBrushSize, MaxBrushSize);
                Speed = EditorGUILayoutEx.Slider(StringTable.Get(C.Speed), "[", "]", Speed, MinSpeed, MaxSpeed);

                EditorGUILayout.BeginHorizontal();
                {
                    var label = new GUIContent(StringTable.Get(C.Intensity));
                    GUILayout.Label(label, GUILayout.Width(LabelWidth));
                    EditorGUILayout.LabelField(" ", GUILayout.Width(15));
                    Intensity = EditorGUILayout.Slider(Intensity, MinIntensity, MaxIntensity);
                    EditorGUILayout.LabelField(" ", GUILayout.Width(15));
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            // Tools
            if (!Settings.CompactGUI)
            {
                EditorGUILayout.Space();
                GUILayout.Label(StringTable.Get(C.Tools), MTEStyles.SubHeader);
            }
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button(StringTable.Get(C.Fill), GUILayout.Width(100), GUILayout.Height(40)))
                    {
                        FillTool();
                    }
                    GUILayout.Space(20);
                    EditorGUILayout.LabelField(StringTable.Get(C.Info_ToolDescription_VertexColorFill), MTEStyles.labelFieldWordwrap);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button(StringTable.Get(C.Randomize), GUILayout.Width(100), GUILayout.Height(40)))
                    {
                        RandomizeTool();
                    }
                    GUILayout.Space(20);
                    EditorGUILayout.LabelField(StringTable.Get(C.Info_ToolDescription_VertexColorRandomize), MTEStyles.labelFieldWordwrap);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                {
                    GUI.enabled = CanAttachVertexColor();
                    if (GUILayout.Button(StringTable.Get(C.AttachVertexColor), GUILayout.Width(100), GUILayout.Height(40)))
                    {
                        AttachVertexColor();
                    }
                    GUILayout.Space(20);
                    EditorGUILayout.LabelField(StringTable.Get(C.Info_ToolDescription_AttachVertexColor), MTEStyles.labelFieldWordwrap);
                    GUI.enabled = true;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }
            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            EditorGUILayout.HelpBox(StringTable.Get(C.Info_WillBeSavedInstantly),
                MessageType.Info, true);
        }

        private readonly List<MeshModifyGroup> modifyGroups = new List<MeshModifyGroup>(4);
        public void OnSceneGUI()
        {
            var e = Event.current;
            if (e.commandName == "UndoRedoPerformed")
            {
                SceneView.RepaintAll();
                return;
            }

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

                //filter targets
                var editableTargets = MTEContext.Targets.Where(CanPaintVertexColor).ToList();

                // collect modifiy group
                modifyGroups.Clear();
                foreach (var target in editableTargets)
                {
                    var meshFilter = target.GetComponent<MeshFilter>();
                    var meshCollider = target.GetComponent<MeshCollider>();
                    var mesh = meshFilter.sharedMesh;

                    var hitPointLocal = target.transform.InverseTransformPoint(raycastHit.point);
                    List<int> vIndex;
                    List<float> vDistance;
                    VertexMap.GetAffectedVertex(target, hitPointLocal, BrushSizeInU3D,
                        out vIndex, out vDistance);

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
                    using (new Undo.UndoTransaction("Paint Vertex Color"))
                    {
                        foreach (var target in editableTargets)
                        {
                            if (!target)
                            {
                                return;
                            }
                            var mesh = target.GetComponent<MeshFilter>().sharedMesh;
                            var originalColors = mesh.colors;
                            var vertexColor = target.GetComponent<VertexColorInitializer>().vertexColor;
                            UndoRedoManager.Instance().Push(a =>
                            {
                                mesh.ModifyVertexColors(vertexColor, a);
                                EditorUtility.SetDirty(vertexColor);
                            }, originalColors, "Paint Vertex Color");
                        }
                    }
                }

                // execute the modification
                if (modifyGroups.Count != 0)
                {
                    for (int i = 0; i < modifyGroups.Count; i++)
                    {
                        var modifyGroup = modifyGroups[i];
                        var gameObject = modifyGroup.gameObject;
                        bool validTargets = editableTargets.Contains(gameObject);
                        if (!validTargets) continue;

                        var mesh = modifyGroup.mesh;
                        var vertexColor = modifyGroup.gameObject.GetComponent<VertexColorInitializer>().vertexColor;
                        var vIndex = modifyGroup.vIndex;
                        var vDistance = modifyGroup.vDistance;
                        PaintVertexColor(mesh, vertexColor, vIndex, vDistance);
                    }
                }

                // auto save when mouse up
                if (e.type == EventType.MouseUp && e.button == 0)
                {
                    foreach (var target in editableTargets)
                    {
                        var vertexColor = target.GetComponent<VertexColorInitializer>().vertexColor;
                        EditorUtility.SetDirty(vertexColor);
                    }
                }
            }

            SceneView.RepaintAll();
        }

        /// <summary>
        /// Paint vertex color
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="vertexColor"></param>
        /// <param name="vIndex">indexes of selecting vertexes</param>
        /// <param name="vDistance">distances of selecting vertexes to the center</param>
        public void PaintVertexColor(Mesh mesh, VertexColor vertexColor, List<int> vIndex, List<float> vDistance)
        {
            var new_vertexColor = (Color[]) vertexColor.colors.Clone();
            float currentBrushSize = BrushSizeInU3D;

            Color newColor;
            if(BrushIndex == 0)
            {
                if(Mathf.Abs(Intensity - 1.0f) > 0.001f)
                {
                    newColor = TheColor*Intensity;
                }
                else
                {
                    newColor = TheColor;
                }
            }
            else
            {
                newColor = Color.white;
            }

            //modify vertex colors
            for (var i = 0; i < vIndex.Count; ++i)
            {
                var index = vIndex[i];
                var distance = vDistance[i];
                var falloff = 1 - Mathf.Clamp01(distance / currentBrushSize);//TODO add falloff curve on request
                var k = Speed*falloff;

                if (singleChannelOnly)
                {
                    var oldColorChannel = new_vertexColor[index][theChannel];
                    var newColorChannel = newColor[theChannel];
                    new_vertexColor[index][theChannel] = Mathf.Lerp(oldColorChannel, newColorChannel, k);
                }
                else
                {
                    new_vertexColor[index] = Color.Lerp(new_vertexColor[index], newColor, k);
                }
            }

            vertexColor.colors = new_vertexColor;
            mesh.colors = new_vertexColor;
        }

        #region Tools

        public static bool CanAttachVertexColor()
        {
            var gameObject = Selection.activeGameObject;
            if (gameObject == null)
            {
                return false;
            }
            // check mesh
            var meshFilter = gameObject.GetComponent<MeshFilter>();
            if (meshFilter == null || meshFilter.sharedMesh == null)
            {
                return false;
            }
            var mesh = meshFilter.sharedMesh;

            // vertex color component
            var mci = gameObject.GetComponent<VertexColorInitializer>();
            if (mci == null)
            {
                return true;
            }

            if (mci.vertexColor == null)
            {
                return true;
            }

            if (mci.vertexColor.colors.Length != mesh.vertexCount)
            {
                return true;
            }

            return false;
        }

        private static void AttachVertexColor()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                StringTable.Get(C.Warning),
                StringTable.Get(C.Warning_Confirm),
                StringTable.Get(C.Yes), StringTable.Get(C.No));
            if (confirmed)
            {
                AttachVertexColor(Selection.activeGameObject);
            }
        }

        public static void AttachVertexColor(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }

            try
            {
                // check mesh
                var meshFilter = gameObject.GetComponent<MeshFilter>();
                if (meshFilter == null || meshFilter.sharedMesh == null)
                {
                    throw new InvalidOperationException("The gameObject doesn't contain a mesh.");
                }
                var mesh = meshFilter.sharedMesh;

                // vertex color component
                var mci = gameObject.GetComponent<VertexColorInitializer>();
                if (mci == null)
                {
                    mci = gameObject.AddComponent<VertexColorInitializer>();
                }
                if (mci.vertexColor == null)
                {
                    var meshRelativePath = AssetDatabase.GetAssetPath(mesh);
                    var meshPath = Utility.GetSysPath(meshRelativePath);
                    var meshDirPath = PathEx.UnifySlash(System.IO.Path.GetDirectoryName(meshPath));
                    var meshDirRelativePath = Utility.GetUnityPath(meshDirPath);

                    var vertexColor = ScriptableObject.CreateInstance<VertexColor>();
                    vertexColor.colors = Enumerable.Repeat(new Color(1, 1, 1, 1), mesh.vertexCount).ToArray();
                    mci.vertexColor = vertexColor;
                    AssetDatabase.CreateAsset(vertexColor, meshDirRelativePath + "\\VertexColor.asset");
                }
                if (mci.vertexColor.colors == null || mci.vertexColor.colors.Length != mesh.vertexCount)
                {
                    mci.vertexColor.colors = Enumerable.Repeat(new Color(1, 1, 1, 1), mesh.vertexCount).ToArray();
                }

                // material
                var meshRenderer = gameObject.GetComponent<MeshRenderer>();
                if (meshRenderer == null)
                {
                    throw new System.InvalidOperationException("[MTE] The gameObject doesn't have a MeshRenderer.");
                }
                var material = meshRenderer.sharedMaterial;
                if (material == null)
                {
                    throw new System.InvalidOperationException("[MTE] The gameObject doesn't have a Material.");
                }
                material.shader = Shader.Find("MTE/VertexColored/ColorOnly");
            }
            catch (System.Exception e)
            {
                Debug.LogErrorFormat("[MTE] failed to attach vertex color to the gameObject. Error: {0}", e.ToString());

                var mci = gameObject.GetComponent<VertexColorInitializer>();
                if (mci != null)
                {
                    if (mci.vertexColor != null)
                    {
                        UnityEngine.Object.DestroyImmediate(mci.vertexColor, true);
                    }
                    UnityEngine.Object.DestroyImmediate(mci);
                }
            }
        }

        public static bool CanPaintVertexColor(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return false;
            }
            // check mesh
            var meshFilter = gameObject.GetComponent<MeshFilter>();
            if (meshFilter == null || meshFilter.sharedMesh == null)
            {
                return false;
            }
            var mesh = meshFilter.sharedMesh;

            // vertex color component
            var mci = gameObject.GetComponent<VertexColorInitializer>();
            if (mci == null)
            {
                return false;
            }

            if (mci.vertexColor == null)
            {
                MTEDebug.LogWarningFormat("MTE will not edit the mesh, because the vertex color is not set.", mesh.name);
                return false;
            }

            if (mci.vertexColor.colors.Length != mesh.vertexCount)
            {
                MTEDebug.LogWarningFormat("MTE will not edit the mesh, because the vertex color count is not equal to the vertex count of the mesh<{0}>.", mesh.name);
                return false;
            }

            return true;
        }


        private void FillTool()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                StringTable.Get(C.Warning),
                StringTable.Get(C.Warning_Confirm),
                StringTable.Get(C.Yes), StringTable.Get(C.No));
            if (confirmed)
            {
                var editableTargets = MTEContext.Targets.Where(CanPaintVertexColor).ToList();

                using (new Undo.UndoTransaction("Paint Vertex Color: Fill"))
                {
                    foreach (var target in editableTargets)
                    {
                        if (!target)
                        {
                            continue;
                        }
                        var mesh = target.GetComponent<MeshFilter>().sharedMesh;
                        var originalColors = mesh.colors;
                        var vci = target.GetComponent<VertexColorInitializer>();
                        if (vci == null) continue;
                        var vertexColor = vci.vertexColor;
                        if (vertexColor == null) continue;
                        if (vertexColor.colors.Length != originalColors.Length) continue;
                        UndoRedoManager.Instance().Push(a =>
                        {
                            mesh.ModifyVertexColors(vertexColor, a);
                            EditorUtility.SetDirty(vertexColor);
                        }, originalColors, "Paint Vertex Color: Fill");

                        var newColors = Enumerable.Repeat(TheColor, vertexColor.colors.Length).ToArray();
                        mesh.colors = newColors;
                        vertexColor.colors = newColors;

                        EditorUtility.SetDirty(vertexColor);
                    }
                }
                MTEEditorWindow.Instance.Focus();
            }
        }

        private void RandomizeTool()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                StringTable.Get(C.Warning),
                StringTable.Get(C.Warning_Confirm),
                StringTable.Get(C.Yes), StringTable.Get(C.No));
            if (confirmed)
            {
                var editableTargets = MTEContext.Targets.Where(CanPaintVertexColor).ToList();

                using (new Undo.UndoTransaction("Paint Vertex Color: Randomize"))
                {
                    foreach (var target in editableTargets)
                    {
                        if (!target)
                        {
                            continue;
                        }
                        var mesh = target.GetComponent<MeshFilter>().sharedMesh;
                        var originalColors = mesh.colors;
                        var vci = target.GetComponent<VertexColorInitializer>();
                        if (vci == null) continue;
                        var vertexColor = vci.vertexColor;
                        if (vertexColor == null) continue;
                        if (vertexColor.colors.Length != originalColors.Length) continue;
                        UndoRedoManager.Instance().Push(a =>
                        {
                            mesh.ModifyVertexColors(vertexColor, a);
                            EditorUtility.SetDirty(vertexColor);
                        }, originalColors, "Paint Vertex Color: Randomize");

                        Color[] newColors;
                        {
                            newColors = Enumerable
                            .Repeat(0, originalColors.Length)
                            .Select(c => new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value))
                            .ToArray();
                        }
                        mesh.colors = newColors;
                        vertexColor.colors = newColors;

                        EditorUtility.SetDirty(vertexColor);
                    }
                }
                MTEEditorWindow.Instance.Focus();
            }
        }

        #endregion

        #region GUI

        private readonly string[] RGBA = {"R", "G", "B", "A"};
        const int LabelWidth = 90;

        #endregion
    }
}