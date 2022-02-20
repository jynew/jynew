using MTE.Undo;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace MTE
{
    internal class FlowPainter : IEditor
    {
        public int Id { get; } = 8;

        public bool Enabled { get; set; } = true;

        public string Name { get; } = "FlowPainter";

        public Texture Icon { get; } = MTEStyles.PaintFlowToolIcon;

        public bool WantMouseMove { get; } = false;

        public bool WillEditMesh { get; } = false;

        public enum PaintMode
        {
            Fixed,
            Movement,
            PinchInflate,
            Vortex,
        }

        #region Parameters

        #region Constant
        // default
        const PaintMode DefaultMode = PaintMode.Fixed;
        const int DefaultBrushIndex = 0;
        const float DefaultBrushSize = 1f;
        const float DefaultStrength = 0.5f;
        const float DefaultSpeed = 0.05f;
        const float DefaultDirection = 0f;
        const bool DefaultPinching = true;
        const bool DefaultVortexRotationClockWise = true;

        // min/max
        const float MinBrushSize = 0.1f;
        const float MaxBrushSize = 50f;
        const float MinSpeed = 0.01f;
        const float MaxSpeed = 1f;
        #endregion

        private PaintMode mode;
        private int brushIndex;
        private float strength;
        private float direction;
        private float brushSize;
        private float speed;
        private bool pinching;
        private bool vortexRotationClockWise;

        public PaintMode Mode
        {
            get { return mode; }
            set
            {
                if(mode != value)
                {
                    EditorPrefs.SetInt("MTE_FlowPainter.mode", (int)value);
                    mode = value;
                }
            }
        }

        /// <summary>
        /// Brush index
        /// </summary>
        public int BrushIndex
        {
            get { return brushIndex; }
            set
            {
                if (brushIndex != value)
                {
                    //Update preview
                    //if (previewObj != null)
                    //{
                    //    SetPreviewMaskTexture(value);
                    //}
                    EditorPrefs.SetInt("MTE_FlowPainter.brushIndex", value);
                    brushIndex = value;
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

                    EditorPrefs.SetFloat("MTE_FlowPainter.brushSize", value);
                    //if (previewObj != null)
                    //{
                    //    SetPreviewSize(BrushSizeInU3D / 2);
                    //}
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
                    EditorPrefs.SetFloat("MTE_FlowPainter.speed", value);
                }
            }
        }

        /// <summary>
        /// Flow direction, angle to north(+u)
        /// </summary>
        public float Direction
        {
            get
            {
                return direction;
            }

            set
            {
                value = Mathf.Clamp(value, 0, 2 * Mathf.PI);
                if (!MathEx.AmostEqual(value, direction))
                {
                    EditorPrefs.SetFloat("MTE_FlowPainter.direction", direction);
                    direction = value;
                }
            }
        }

        public float Strength
        {
            get { return strength; }
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (value != strength)
                {
                    EditorPrefs.SetFloat("MTE_FlowPainter.strength", value);
                    strength = value;
                }
            }
        }

        /// <summary>
        /// the color
        /// </summary>
        public Color TheColor
        {
            get
            {
                var r = 0.5f - Strength * Mathf.Cos(direction) * 0.5f;
                var g = 0.5f + Strength * Mathf.Sin(direction) * 0.5f;
                return new Color(r, g, 0);
            }
        }

        public bool Pinching
        {
            get { return pinching; }
            set
            {
                if (value != pinching)
                {
                    pinching = value;
                    EditorPrefs.SetBool("MTE_FlowPainter.pinching", value);
                }
            }
        }

        public bool VortexRotationClockWise
        {
            get { return vortexRotationClockWise; }
            set
            {
                if (value != vortexRotationClockWise)
                {
                    vortexRotationClockWise = value;
                    EditorPrefs.SetBool("MTE_FlowPainter.vortexRotationClockWise", value);
                }
            }
        }

        #endregion

        public FlowPainter()
        {
            MTEContext.EnableEvent += (sender, args) =>
            {
                if (MTEContext.editor == this)
                {
                    LoadSavedParamter();
                    //LoadPreview();
                }
            };

            MTEContext.EditTypeChangedEvent += (sender, args) =>
            {
                if (MTEContext.editor == this)
                {
                    LoadSavedParamter();
                    //LoadPreview();
                }
                else
                {
                    //UnLoadPreview();
                }
            };

            //MTEContext.DisableEvent += (sender, args) =>
            //{
            //    UnLoadPreview();
            //};

            // Load default parameters
            brushIndex = DefaultBrushIndex;
            brushSize = DefaultBrushSize;
            speed = DefaultSpeed;
        }

        private void LoadSavedParamter()
        {
            // Load parameters from the EditorPrefs
            mode = (PaintMode)EditorPrefs.GetInt("MTE_FlowPainter.mode", (int)DefaultMode);
            brushIndex = EditorPrefs.GetInt("MTE_FlowPainter.brushIndex", DefaultBrushIndex);
            brushSize = EditorPrefs.GetFloat("MTE_FlowPainter.brushSize", DefaultBrushSize);
            strength = EditorPrefs.GetFloat("MTE_FlowPainter.strength", DefaultStrength);
            speed = EditorPrefs.GetFloat("MTE_FlowPainter.speed", DefaultSpeed);

            direction = EditorPrefs.GetFloat("MTE_FlowPainter.direction", DefaultDirection);
            pinching = EditorPrefs.GetBool("MTE_FlowPainter.pinching", DefaultPinching);
            vortexRotationClockWise = EditorPrefs.GetBool("MTE_FlowPainter.vortexRotationClockWise", DefaultVortexRotationClockWise);
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
                }),
                new Hotkey(this, KeyCode.Space, () =>
                {
                    if (Mode == PaintMode.PinchInflate)
                    {
                        //toggle Pinching/Inflate
                        Pinching = !Pinching;
                        MTEEditorWindow.Instance.Repaint();
                    }
                    else if (Mode == PaintMode.Vortex)
                    {
                        //toggle clockwise/counter-clockwise vortex
                        VortexRotationClockWise = !VortexRotationClockWise;
                        MTEEditorWindow.Instance.Repaint();
                    }
                })
            };
        }

        public string Header { get { return StringTable.Get(C.PaintFlow_Header); } }
        public string Description { get { return StringTable.Get(C.PaintFlow_Description); } }

        bool IsGUILoaded = false;
        #region GUI contents
        GUIContent[] ModeTextContents;
        #endregion

        Vector2 po;
        public void DoArgsGUI()
        {
            if (!IsGUILoaded)
            {
                ModeTextContents = new []
                    {
                        new GUIContent(MTEStyles.flowToolTexture_Fixed),
                        new GUIContent(MTEStyles.flowToolTexture_Movement),
                        new GUIContent(MTEStyles.flowToolTexture_Pinch),
                        new GUIContent(MTEStyles.flowToolTexture_Vortex),
                    };
                IsGUILoaded = true;
            }

            // Settings
            if (!Settings.CompactGUI)
            {
                GUILayout.Label(StringTable.Get(C.Mode), MTEStyles.SubHeader);
            }
            Mode = (PaintMode)GUILayout.Toolbar((int)Mode, ModeTextContents, Settings.CompactGUI ? GUILayout.Height(32) : GUILayout.Height(64));

            if(mode == PaintMode.Fixed)
            {
                if (!Settings.CompactGUI)
                {
                    EditorGUILayout.HelpBox(StringTable.Get(C.Info_ModeDescription_Fixed), MessageType.Info);
                    GUILayout.Label(StringTable.Get(C.Direction), MTEStyles.SubHeader);
                }
                EditorGUILayout.BeginHorizontal("box", GUILayout.Height(130));
                {
                    Rect aRect = GUILayoutUtility.GetRect(128, 128, 128, 128, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
                    po = aRect.center + new Vector2(Mathf.Cos(direction), -Mathf.Sin(direction)) * strength * 55;//55 is the outer radius of *flowDirectionSelectorTexture*
                    GUI.DrawTexture(aRect, MTEStyles.flowDirectionSelectorTexture);
                    if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag)
                    {
                        if (aRect.Contains(Event.current.mousePosition))
                        {
                            po = Event.current.mousePosition;
                            var dir = (po - aRect.center);
                            Strength = dir.magnitude / 55;
                            Direction = MathEx.AngleFromTo(dir, Vector2.right, new Vector3(0, 0, 1)) * Mathf.Deg2Rad;
                            MTEEditorWindow.Instance.Repaint();
                            SceneView.RepaintAll();
                        }
                    }
                    if ((aRect.center - po).sqrMagnitude > 0.01f)
                    {
                        Handles.BeginGUI();
                        GUIEx.DrawLine(new Vector3(aRect.center.x, aRect.center.y), new Vector3(po.x, po.y), 2);
                        EditorGUI.DrawRect(new Rect(po.x - 2.5f, po.y - 2.5f, 5, 5), Color.blue);
                        Handles.EndGUI();
                    }
                    EditorGUILayout.BeginVertical();
                    {
                        GUILayout.FlexibleSpace();
                        var old = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth = 60;
                        Strength = EditorGUILayout.FloatField(StringTable.Get(C.Strength), Strength);
                        EditorGUILayout.Space();
                        var directionAngle = EditorGUILayout.FloatField(StringTable.Get(C.AngleInDegrees), Direction * Mathf.Rad2Deg);
                        Direction = directionAngle * Mathf.Deg2Rad;
                        EditorGUIUtility.labelWidth = old;
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginVertical("box");
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    bool NW = GUILayout.Button("↖", GUILayout.Width(32), GUILayout.Height(32));
                    bool N = GUILayout.Button("↑", GUILayout.Width(32), GUILayout.Height(32));
                    bool NE = GUILayout.Button("↗", GUILayout.Width(32), GUILayout.Height(32));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    bool W = GUILayout.Button("←", GUILayout.Width(32), GUILayout.Height(32));
                    bool NOMOVE = GUILayout.Button("C", GUILayout.Width(32), GUILayout.Height(32));
                    bool E = GUILayout.Button("→", GUILayout.Width(32), GUILayout.Height(32));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    bool SW = GUILayout.Button("↙", GUILayout.Width(32), GUILayout.Height(32));
                    bool S = GUILayout.Button("↓", GUILayout.Width(32), GUILayout.Height(32));
                    bool SE = GUILayout.Button("↘", GUILayout.Width(32), GUILayout.Height(32));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    if (NW) { Strength = 1; Direction = 0.75f * Mathf.PI; }
                    if (N) { Strength = 1; Direction = 0.5f * Mathf.PI; }
                    if (NE) { Strength = 1; Direction = 0.25f * Mathf.PI; }

                    if (W) { Strength = 1; Direction = Mathf.PI; }
                    if (NOMOVE) { Strength = 0; Direction = -1; }
                    if (E) { Strength = 1; Direction = 0; }

                    if (SW) { Strength = 1; Direction = 1.25f * Mathf.PI; }
                    if (S) { Strength = 1; Direction = 1.5f * Mathf.PI; }
                    if (SE) { Strength = 1; Direction = 1.75f * Mathf.PI; }
                }
                EditorGUILayout.EndVertical();

            }
            else if (mode == PaintMode.Movement)
            {
                if (!Settings.CompactGUI)
                {
                    EditorGUILayout.HelpBox(StringTable.Get(C.Info_ModeDescription_Movement), MessageType.Info);
                }
                Strength = EditorGUILayoutEx.Slider(StringTable.Get(C.Strength), Strength, 0, 1);
            }
            else if (mode == PaintMode.PinchInflate)
            {
                if (!Settings.CompactGUI)
                {
                    EditorGUILayout.HelpBox(StringTable.Get(C.Info_ModeDescription_Pinch), MessageType.Info);
                }
                Strength = EditorGUILayoutEx.Slider(StringTable.Get(C.Strength), Strength, 0, 1);
            }
            else if (mode == PaintMode.Vortex)
            {
                if (!Settings.CompactGUI)
                {
                    EditorGUILayout.HelpBox(StringTable.Get(C.Info_ModeDescription_Vortex), MessageType.Info);
                }
                Strength = EditorGUILayoutEx.Slider(StringTable.Get(C.Strength), Strength, 0, 1);
            }

            // Shared settings
            if (!Settings.CompactGUI)
            {
                GUILayout.Label(StringTable.Get(C.Settings), MTEStyles.SubHeader);
            }
            BrushSize = EditorGUILayoutEx.Slider(StringTable.Get(C.Size), "-", "+", BrushSize, MinBrushSize, MaxBrushSize);
            Speed = EditorGUILayoutEx.Slider(StringTable.Get(C.Speed), "[", "]", Speed, MinSpeed, MaxSpeed);

            // Tools
            if (!Settings.CompactGUI)
            {
                EditorGUILayout.Space();
                GUILayout.Label(StringTable.Get(C.Tools), MTEStyles.SubHeader);
            }
            EditorGUILayout.BeginVertical();
            {
                if(Mode == PaintMode.Fixed)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button(StringTable.Get(C.CreateFlowMap), GUILayout.Width(100), GUILayout.Height(40)))
                        {
                            CreateFlowMap(TheColor);
                        }
                        GUILayout.Space(20);
                        EditorGUILayout.LabelField(StringTable.Get(C.Info_ToolDescription_CreateFlowMap), MTEStyles.labelFieldWordwrap);
                        GUI.enabled = true;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            EditorGUILayout.HelpBox(StringTable.Get(C.Info_WillBeSavedInstantly),
                MessageType.Info, true);
        }

        internal struct TextureModifyGroup
        {
            public GameObject gameObject;
            public Texture2D texture;
            public Rect texelRect;
            public Vector2 center;

            public TextureModifyGroup(GameObject gameObject, Texture2D texture, Rect texelRect, Vector2 center)
            {
                this.gameObject = gameObject;
                this.texture = texture;
                this.texelRect = texelRect;
                this.center = center;
            }
        }

        // buffers of editing helpers
        private readonly List<TextureModifyGroup> modifyGroups = new List<TextureModifyGroup>(4);
        private readonly List<Color[]> modifyingSections = new List<Color[]>(2);

        Vector2 mouseDelta;

        public void OnSceneGUI()
        {
            var e = Event.current;
            mouseDelta = e.delta;//used by PaintMode.Movement

            if (e.commandName == "UndoRedoPerformed")
            {
                SceneView.RepaintAll();
                return;
            }

            if (!(EditorWindow.mouseOverWindow is SceneView))
            {
                return;
            }

            if (Mode == PaintMode.Fixed)
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
                    if (strength > 0.01f)
                    {
                        Handles.ArrowHandleCap(0, hit.point, Quaternion.Euler(0, -(Direction - 0.5f * Mathf.PI) * Mathf.Rad2Deg, 0), BrushSizeInU3D + Strength * Settings.PointSize, EventType.Repaint);
                    }
                    else
                    {
                        Handles.color = Color.red;
                        Handles.SphereHandleCap(0, hit.point, Quaternion.identity, Settings.PointSize, EventType.Repaint);
                    }
                }
            }

            // do nothing when mouse middle/right button, alt key is pressed
            if (e.button != 0 || e.alt)
                return;

            if(Mode == PaintMode.Fixed)
            {
                // hold control key and scroll wheel to change flow direction
                if (e.control && !e.isKey && e.type == EventType.ScrollWheel)
                {
                    float oldDirection = Direction;
                    float direction = oldDirection;
                    ChangeDirection(e.delta.y, ref direction);
                    if (Mathf.Abs(direction - oldDirection) > Mathf.Epsilon)
                    {
                        MTEEditorWindow.Instance.Repaint();
                        Direction = direction;
                    }
                    e.Use();
                }
            }

            if(Mode == PaintMode.Movement)
            {
                this.delta[Time.frameCount % smoothing] = new Vector3(-this.mouseDelta.x, 0f, -this.mouseDelta.y);
                this.vector = Vector3.zero;
                foreach (Vector3 b in this.delta)
                {
                    this.vector += b;
                }
                this.vector /= (float)smoothing;
                this.vector.y = 0f;
                this.vector *= Time.fixedDeltaTime;
                this.vector.x = this.vector.x * 0.5f + 0.5f;
                this.vector.z = this.vector.z * 0.5f + 0.5f;
            }

            HandleUtility.AddDefaultControl(0);
            var ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit raycastHit;
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

                var currentBrushSize = BrushSizeInU3D;
                if (Settings.ShowBrushRect)
                {
                    Utility.ShowBrushRect(raycastHit.point, currentBrushSize);
                }

                var hitPoint = raycastHit.point;

                if (Settings.FlashAffectedVertex)
                {
                    Handles.color = Utility.GetFlashingColor();
                }
                else
                {
                    Handles.color = Color.green;
                }
                Handles.CircleHandleCap(0, hitPoint, Quaternion.LookRotation(Vector3.up), BrushSizeInU3D/2, EventType.Repaint);

                // collect modifiy group
                modifyGroups.Clear();
                foreach (var target in MTEContext.Targets)
                {
                    // check if we can paint on target
                    var meshRenderer = target.GetComponent<MeshRenderer>();
                    if (meshRenderer == null) continue;
                    var meshFilter = target.GetComponent<MeshFilter>();
                    if (meshFilter == null) continue;
                    var mesh = meshFilter.sharedMesh;
                    if (mesh == null) continue;
                    var material = meshRenderer.sharedMaterial;
                    if (!material.HasProperty("_FlowMap"))
                    {
                        continue;
                    }

                    Vector2 textureUVMin;//min texture uv that is to be modified
                    Vector2 textureUVMax;//max texture uv that is to be modified
                    Vector2 centerUV;
                    {
                        // check if the brush rect intersects with the `Mesh.bounds` of this target
                        var hitPointLocal = target.transform.InverseTransformPoint(hitPoint);//convert hit point from world space to target mesh space

                        Bounds brushBounds = new Bounds(center: new Vector3(hitPointLocal.x, 0, hitPointLocal.z), size: new Vector3(currentBrushSize, 99999, currentBrushSize));
                        Bounds meshBounds = mesh.bounds;//TODO rename this

                        centerUV = new Vector2(
                            MathEx.NormalizeTo01(meshBounds.min.x, meshBounds.max.x, hitPointLocal.x),
                            MathEx.NormalizeTo01(meshBounds.min.z, meshBounds.max.z, hitPointLocal.z));//FIXME should we use a coordinate in world space?

                        Bounds paintingBounds;
                        var intersected = meshBounds.Intersect(brushBounds, out paintingBounds);
                        if (!intersected) continue;

                        Vector2 paintingBounds2D_min = new Vector2(paintingBounds.min.x, paintingBounds.min.z);
                        Vector2 paintingBounds2D_max = new Vector2(paintingBounds.max.x, paintingBounds.max.z);
                        Vector2 meshRendererBounds2D_min = new Vector2(meshBounds.min.x, meshBounds.min.z);
                        Vector2 meshRendererBounds2D_max = new Vector2(meshBounds.max.x, meshBounds.max.z);

                        //calculate which part of texture should be modified
                        textureUVMin = MathEx.NormalizeTo01(rangeMin: meshRendererBounds2D_min, rangeMax: meshRendererBounds2D_max, value: paintingBounds2D_min);
                        textureUVMax = MathEx.NormalizeTo01(rangeMin: meshRendererBounds2D_min, rangeMax: meshRendererBounds2D_max, value: paintingBounds2D_max);

                        if (Settings.DebugMode)
                        {
                            Handles.color = Color.blue;
                            HandlesEx.DrawRectangle(paintingBounds2D_min, paintingBounds2D_max);
                            Handles.color = new Color(255, 128, 166);
                            HandlesEx.DrawRectangle(meshRendererBounds2D_min, meshRendererBounds2D_max);
                        }
                    }

                    if (e.button == 0 && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag))
                    {
                        Texture2D targetTexture = null;
                        if (material.GetTexture("_FlowMap") != null)
                        {
                            targetTexture = (Texture2D)material.GetTexture("_FlowMap");
                        }
                        else
                        {
                            MTEDebug.LogWarningFormat("\"_FlowMap\" is not assigned or existing in material<{0}>.", material.name);
                            return;
                        }

                        //get modifying texel rect of the control texture
                        var x = textureUVMin.x * targetTexture.width;
                        var y = textureUVMin.y * targetTexture.height;
                        var width = (textureUVMax.x - textureUVMin.x) * targetTexture.width;
                        var height = (textureUVMax.y - textureUVMin.y) * targetTexture.height;
                        var texelRect = new Rect(x, y, width, height);
                        Vector2 center = new Vector2(centerUV.x * targetTexture.width, centerUV.y * targetTexture.height);
                        modifyGroups.Add(new TextureModifyGroup(target, targetTexture, texelRect, center));
                    }

                    if (Settings.DebugMode)
                    {
                        Handles.BeginGUI();
                        GUILayout.Button(string.Format("center uv: ({0},{1})", centerUV.x, centerUV.y));
                        Handles.EndGUI();
                    }

                }


                //record undo operation for targets that to be modified
                if (e.type == EventType.MouseDown)
                {
                    using (new Undo.UndoTransaction("Paint Flow"))
                    {
                        foreach (var target in MTEContext.Targets)
                        {
                            var material = target.GetComponent<MeshRenderer>().sharedMaterial;
                            if (material.HasProperty("_FlowMap"))
                            {
                                Texture2D texture = (Texture2D)material.GetTexture("_FlowMap");
                                if (texture != null)
                                {
                                    var originalColors = texture.GetPixels();
                                    UndoRedoManager.Instance().Push(a =>
                                    {
                                        texture.ModifyPixels(a);
                                        texture.Apply();
                                        Save(texture);
                                    }, originalColors, "Paint flow map");
                                }
                            }
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
                        var material = gameObject.GetComponent<MeshRenderer>().sharedMaterial;
                        if (material.HasProperty("_FlowMap"))
                        {
                            Utility.SetTextureReadable(material.GetTexture("_FlowMap"));
                            if (mode == PaintMode.Fixed)
                            {
                                PaintTextureFixed(modifyGroup);
                            }
                            else if(mode == PaintMode.Movement)
                            {
                                PaintTextureMovement(modifyGroup.texture, modifyGroup.texelRect);
                            }
                            else if (mode == PaintMode.PinchInflate)
                            {
                                PaintTexturePinchInflate(modifyGroup.texture, modifyGroup.texelRect);
                            }
                            else if (mode == PaintMode.Vortex)
                            {
                                PaintTextureVortex(modifyGroup.texture, modifyGroup.texelRect);
                            }
                        }

                    }
                }

                // auto save when mouse up
                if (e.type == EventType.MouseUp && e.button == 0)
                {
                    foreach (var target in MTEContext.Targets)
                    {
                        var material = target.GetComponent<MeshRenderer>().sharedMaterial;
                        if (material.HasProperty("_FlowMap"))
                        {
                            Texture2D texture = (Texture2D)material.GetTexture("_FlowMap");
                            if (texture != null)
                            {
                                Save(texture);
                            }
                        }
                    }
                }

            }

            SceneView.RepaintAll();
        }

        private static void Save(Texture2D texture)
        {
            if (texture == null)
            {
                throw new System.ArgumentNullException("texture");
            }
            var path = AssetDatabase.GetAssetPath(texture);
            var bytes = texture.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
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

        private static void CreateFlowMap(Color flowColor)
        {
            var texture = new Texture2D(512, 512, TextureFormat.ARGB32, true);
            var colors = new Color[512 * 512];
            for (var t = 0; t < colors.Length; t++)
            {
                colors[t] = flowColor;
            }
            texture.SetPixels(colors);
            var relativePath = "Assets/FlowMap.png";
            System.IO.File.WriteAllBytes(relativePath, texture.EncodeToPNG());
            AssetDatabase.ImportAsset(relativePath, ImportAssetOptions.ForceUpdate);
            var textureImporter = (TextureImporter)AssetImporter.GetAtPath(relativePath);
            textureImporter.isReadable = true;
            textureImporter.SaveAndReimport();
            texture = AssetDatabase.LoadAssetAtPath<Texture>(relativePath) as Texture2D;
            Selection.activeObject = texture;
        }

        #region GUI

        const int LabelWidth = 90;

        #endregion

        #region Preview (not used)
        public GameObject previewObj;

        internal void LoadPreview()
        {
            UnLoadPreview();
            previewObj = new GameObject("MTEPreview");
            previewObj.SetActive(false);
            var projector = previewObj.AddComponent<Projector>();
            previewObj.hideFlags = HideFlags.HideAndDontSave;
            projector.material = new Material(Shader.Find("Hidden/MTE/PaintTexturePreview"));
            projector.orthographic = true;
            projector.nearClipPlane = -1000;
            projector.farClipPlane = 1000;
            projector.transform.Rotate(90, 0, 0);

            SetPreviewTexture(Vector2.one, MTEStyles.flowPainterMaskTextures[BrushIndex], null);
            SetPreviewSize(BrushSizeInU3D / 2);
            SetPreviewMaskTexture(BrushIndex);
        }

        internal void UnLoadPreview()
        {
            if (previewObj != null)
            {
                UnityEngine.Object.DestroyImmediate(previewObj);
            }
        }

        private void SetPreviewTexture(Vector2 textureScale, Texture texture, Texture normalTexture)
        {
            var projector = previewObj.GetComponent<Projector>();
            projector.material.SetTexture("_MainTex", texture);
            projector.material.SetTextureScale("_MainTex", textureScale);
            projector.material.SetTexture("_NormalTex", normalTexture);
            SceneView.RepaintAll();
        }

        private void SetPreviewMaskTexture(int maskIndex)
        {
            var projector = previewObj.GetComponent<Projector>();
            projector.material.SetTexture("_MaskTex", MTEStyles.flowPainterMaskTextures[maskIndex]);
            projector.material.SetTextureScale("_MaskTex", Vector2.one);
            SceneView.RepaintAll();
        }

        private void SetPreviewSize(float value)
        {
            var projector = previewObj.GetComponent<Projector>();
            projector.orthographicSize = value;
            SceneView.RepaintAll();
        }
        #endregion

        private const int smoothing = 10;
        private Vector3[] delta = new Vector3[smoothing];
        private Vector3 vector;
        private AnimationCurve falloff = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

        private void PaintTextureFixed(TextureModifyGroup modifyGroup)
        {
            Texture2D texture = modifyGroup.texture;
            Rect texelRect = modifyGroup.texelRect;

            // clear modifying sections
            modifyingSections.Clear();

            // get accurate texel rect
            int x = Mathf.Clamp(Mathf.RoundToInt(texelRect.x), 0, texture.width);
            int y = Mathf.Clamp(Mathf.RoundToInt(texelRect.y + 0.5f), 0, texture.height);
            int width = Mathf.Clamp(Mathf.RoundToInt(texelRect.width), 1, texture.width - x);
            int height = Mathf.Clamp(Mathf.RoundToInt(texelRect.height), 1, texture.height - y);

            // modify target
            var offset = texelRect.position;
            var replaced = texture.GetPixels(x, y, width, height, 0);
            var r = texelRect.width * 0.5f;
            var center = modifyGroup.center;

            // blend the pixel section
            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    var pixelIndex = i * width + j;
                    var point = new Vector2(j, i);

                    var distanceToCenter = Vector2.Distance(offset + point, center);
                    if (distanceToCenter > r)
                    {
                        continue;
                    }

                    replaced[pixelIndex] = Color.Lerp(replaced[pixelIndex], TheColor, Speed);
                }
            }

            // modify the control texture
            texture.SetPixels(x, y, width, height, replaced);
            texture.Apply();
        }

        private void PaintTextureMovement(Texture2D texture, Rect texelRect)
        {
            // check parameters
            if (texture == null)
            {
                throw new System.ArgumentNullException("texture");
            }

            // clear modifying sections
            modifyingSections.Clear();
            int x = (int)texelRect.x;
            int y = (int)texelRect.y;
            int width = (int)texelRect.width;
            int height = (int)texelRect.height;

            // modify target
            var replaced = texture.GetPixels(x, y, width, height, 0);
            var r = width*0.5f;
            // blend the pixel section
            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    var pixelIndex = i * width + j;

                    var distanceToCenter = Mathf.Sqrt((i - r) * (i - r) + (j - r) * (j - r));
                    if(distanceToCenter > r)
                    {
                        continue;
                    }
                    var k = distanceToCenter / r;
                    float t = Mathf.Clamp01(this.falloff.Evaluate(k)) * Strength;

                    var oldColor = replaced[pixelIndex];
                    var newColor = new Color(
                         Mathf.Lerp(oldColor.r, this.vector.x, t),
                         Mathf.Lerp(oldColor.g, this.vector.z, t),
                         oldColor.b);

                    replaced[pixelIndex] = Color.Lerp(oldColor, newColor, Speed);
                }
            }

            // modify the control texture
            texture.SetPixels(x, y, width, height, replaced);
            texture.Apply();
        }

        private void PaintTexturePinchInflate(Texture2D texture, Rect texelRect)
        {
            // check parameters
            if (texture == null)
            {
                throw new System.ArgumentNullException("texture");
            }

            // clear modifying sections
            modifyingSections.Clear();
            int x = (int)texelRect.x;
            int y = (int)texelRect.y;
            int width = (int)texelRect.width;
            int height = (int)texelRect.height;

            // modify target
            var replaced = texture.GetPixels(x, y, width, height, 0);
            var r = width * 0.5f;
            var center = new Vector2(r, r);

            var dir = Vector2.zero;

            // blend the pixel section
            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    var pixelIndex = i * width + j;
                    var point = new Vector2(j, i);
                    var distanceToCenter = Vector2.Distance(point, center);
                    if (distanceToCenter > r)
                    {
                        continue;
                    }
                    var k = distanceToCenter / r;
                    float t = Mathf.Clamp01(this.falloff.Evaluate(k)) * Strength;

                    if (this.Pinching)
                    {
                        dir = (center - point) / r;
                    }
                    else
                    {
                        dir = (point - center) / r;
                    }
                    dir *= 2 * (Mathf.Cos(Mathf.PI * k) * 0.5f + 0.5f) * Strength;

                    var oldColor = replaced[pixelIndex];
                    var newColor = new Color(
                         Mathf.Lerp(oldColor.r, dir.x * 0.5f + 0.5f, t),
                         Mathf.Lerp(oldColor.g, -dir.y * 0.5f + 0.5f, t),
                         oldColor.b);

                    replaced[pixelIndex] = Color.Lerp(oldColor, newColor, Speed);
                }
            }

            // modify the control texture
            texture.SetPixels(x, y, width, height, replaced);
            texture.Apply();
        }

        private void PaintTextureVortex(Texture2D texture, Rect texelRect)
        {
            // check parameters
            if (texture == null)
            {
                throw new System.ArgumentNullException("texture");
            }

            // clear modifying sections
            modifyingSections.Clear();
            int x = (int)texelRect.x;
            int y = (int)texelRect.y;
            int width = (int)texelRect.width;
            int height = (int)texelRect.height;

            // modify target
            var replaced = texture.GetPixels(x, y, width, height, 0);
            var r = width * 0.5f;
            var center = new Vector2(r, r);

            var dir = Vector2.zero;

            // blend the pixel section
            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    var pixelIndex = i * width + j;
                    var point = new Vector2(j, i);
                    var distanceToCenter = Vector2.Distance(point, center);
                    if (distanceToCenter > r)
                    {
                        continue;
                    }
                    var k = distanceToCenter / r;
                    float t = Mathf.Clamp01(this.falloff.Evaluate(k)) * Strength;

                    if (this.VortexRotationClockWise)
                    {
                        dir = (center - point) / r;
                    }
                    else
                    {
                        dir = (point - center) / r;
                    }
                    dir *= 2 * (Mathf.Cos(Mathf.PI * k) * 0.5f + 0.5f) * Strength;

                    var oldColor = replaced[pixelIndex];
                    var newColor = new Color(
                         Mathf.Lerp(oldColor.r, dir.y * 0.5f + 0.5f, t),
                         Mathf.Lerp(oldColor.g, dir.x * 0.5f + 0.5f, t),
                         oldColor.b);

                    replaced[pixelIndex] = Color.Lerp(oldColor, newColor, Speed);
                }
            }

            // modify the control texture
            texture.SetPixels(x, y, width, height, replaced);
            texture.Apply();
        }
    }
}