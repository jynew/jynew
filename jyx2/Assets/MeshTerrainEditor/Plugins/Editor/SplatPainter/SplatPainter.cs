using System;
using System.Collections.Generic;
using System.IO;
using MTE.Undo;
using UnityEditor;
using UnityEngine;

namespace MTE
{
    /// <summary>
    /// Splat texture painter
    /// </summary>
    /// <remarks>
    /// naming convention:
    ///     control textures, "_Control" and "_ControlExtra"
    ///     splat textures, "_Splat0/1/2/3/4/5/6/7"
    /// Only the last few splat textures can be null.
    /// Painting textures will normalize all control textures' color rects.
    /// </remarks>
    internal class SplatPainter : IEditor
    {
        private static readonly GUIContent[] EditorFilterModeContents =
        {
            new GUIContent(StringTable.Get(C.SplatPainter_Mode_Filtered),
                StringTable.Get(C.SplatPainter_Mode_FilteredDescription)),
            new GUIContent(StringTable.Get(C.SplatPainter_Mode_Selected),
                StringTable.Get(C.SplatPainter_Mode_SelectedDescription)),
        };

        public int Id { get; } = 4;

        public bool Enabled { get; set; } = true;

        public string Name { get; } = "SplatPainter";

        public Texture Icon { get; } =
            EditorGUIUtility.IconContent("TerrainInspector.TerrainToolSplat").image;

        public bool WantMouseMove { get; } = true;

        public bool WillEditMesh { get; } = false;

        #region Parameters

        #region Constant
        // default
        const EditorFilterMode DefaultPainterMode
            = EditorFilterMode.FilteredGameObjects;
        const float DefaultBrushSize = 1;
        const float DefaultBrushFlow = 0.5f;
        const float DefaultBrushAlpha = 0.5f;
        // min/max
        const float MinBrushSize = 0.1f;
        const float MaxBrushSize = 10f;
        const float MinBrushFlow = 0.01f;
        const float MaxBrushFlow = 1f;
        const float MinBrushAlpha = 0.5f;
        const float MaxBrushAlpha = 0.5f;
        const int MaxHotkeyNumberForTexture = 8;
        #endregion

        public int brushIndex;
        public float brushSize;
        public float brushFlow;
        private int selectedTextureIndex;
        private EditorFilterMode painterMode;

        private EditorFilterMode PainterMode
        {
            get { return this.painterMode; }
            set
            {
                if (value != this.painterMode)
                {
                    EditorPrefs.SetInt("MTE_SplatPainter.painterMode", (int)value);
                    this.painterMode = value;
                }
            }
        }

        /// <summary>
        /// Index of selected texture in the texture list; not the layer index.
        /// </summary>
        public int SelectedTextureIndex
        {
            get { return this.selectedTextureIndex; }
            set
            {
                var textureListCount = TextureList.Count;
                if (value < textureListCount)
                {
                    this.selectedTextureIndex = value;
                }
            }
        }

        /// <summary>
        /// Index of selected brush
        /// </summary>
        public int BrushIndex
        {
            get { return brushIndex; }
            set
            {
                if (brushIndex != value)
                {
                    preview.SetPreviewMaskTexture(value);

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

                    EditorPrefs.SetFloat("MTE_SplatPainter.brushSize", value);
                    if (PainterMode == EditorFilterMode.FilteredGameObjects)
                    {
                        preview.SetPreviewSize(BrushSizeInU3D/2);
                    }
                    else
                    {
                        //preview size for SelectedGameObject mode are set in OnSceneGUI
                    }
                }
            }
        }

        //real brush size
        private float BrushSizeInU3D { get { return BrushSize * Settings.BrushUnit; } }

        /// <summary>
        /// Brush flow
        /// </summary>
        public float BrushFlow
        {
            get { return brushFlow; }
            set
            {
                value = Mathf.Clamp(value, MinBrushFlow, MaxBrushFlow);
                if (Mathf.Abs(brushFlow - value) > 0.0001f)
                {
                    brushFlow = value;
                    EditorPrefs.SetFloat("MTE_SplatPainter.brushFlow", value);
                }
            }
        }

        #endregion

        public SplatPainter()
        {
            MTEContext.EnableEvent += (sender, args) =>
            {
                if (MTEContext.editor == this)
                {
                    LoadSavedParamter();
                    LoadTextureList();
                    if (PainterMode == EditorFilterMode.SelectedGameObject)
                    {
                        BuildEditingInfoForLegacyMode(Selection.activeGameObject);
                    }
                    if (TextureList.Count != 0)
                    {
                        if (SelectedTextureIndex < 0)
                        {
                            SelectedTextureIndex = 0;
                        }
                        LoadPreview();
                    }
                }
            };

            MTEContext.EditTypeChangedEvent += (sender, args) =>
            {
                if (MTEContext.editor == this)
                {
                    LoadSavedParamter();
                    LoadTextureList();
                    if (PainterMode == EditorFilterMode.SelectedGameObject)
                    {
                        BuildEditingInfoForLegacyMode(Selection.activeGameObject);
                    }
                    if (TextureList.Count != 0)
                    {
                        if (SelectedTextureIndex < 0 || SelectedTextureIndex > TextureList.Count - 1)
                        {
                            SelectedTextureIndex = 0;
                        }
                        LoadPreview();
                    }
                }
                else
                {
                    if (preview != null)
                    {
                        preview.UnLoadPreview();
                    }
                }
            };

            MTEContext.SelectionChangedEvent += (sender, args) =>
            {
                if (MTEContext.editor == this)
                {
                    if (args.SelectedGameObject)
                    {
                        if (PainterMode == EditorFilterMode.SelectedGameObject)
                        {
                            BuildEditingInfoForLegacyMode(args.SelectedGameObject);
                        }
                    }
                }
            };

            MTEContext.TextureChangedEvent += (sender, args) =>
            {
                if (MTEContext.editor == this)
                {
                    LoadTextureList();
                    if (PainterMode == EditorFilterMode.SelectedGameObject)
                    {
                        BuildEditingInfoForLegacyMode(Selection.activeGameObject);
                    }
                }
            };

            MTEContext.DisableEvent += (sender, args) =>
            {
                if (preview != null)
                {
                    preview.UnLoadPreview();
                }
            };

            MTEContext.EditTargetsLoadedEvent += (sender, args) =>
            {
                if (MTEContext.editor == this)
                {
                    LoadTextureList();
                }
            };

            // Load default parameters
            painterMode = DefaultPainterMode;
            brushSize = DefaultBrushSize;
            brushFlow = DefaultBrushFlow;
        }

        private void LoadPreview()
        {
            var texture = TextureList[SelectedTextureIndex];
            preview.LoadPreview(texture, BrushSizeInU3D, BrushIndex);
        }

        private void LoadSavedParamter()
        {
            // Load parameters from the EditorPrefs
            painterMode = (EditorFilterMode)EditorPrefs.GetInt(
                "MTE_SplatPainter.painterMode", (int)DefaultPainterMode);
            brushSize = EditorPrefs.GetFloat("MTE_SplatPainter.brushSize", DefaultBrushSize);
            brushFlow = EditorPrefs.GetFloat("MTE_SplatPainter.brushFlow", DefaultBrushFlow);
        }
        
        private GameObject targetGameObject { get; set; }
        private Mesh targetMesh { get; set; }
        private Material targetMaterial { get; set; }
        private Texture2D[] controlTextures { get; } = new Texture2D[2] {null, null};
        private void BuildEditingInfoForLegacyMode(GameObject gameObject)
        {
            //reset
            this.TextureList.Clear();
            this.controlTextures[0] = null;
            this.controlTextures[1] = null;
            this.targetGameObject = null;
            this.targetMaterial = null;
            this.targetMesh = null;

            //check gameObject
            if (!gameObject)
            {
                return;
            }
            if (PainterMode != EditorFilterMode.SelectedGameObject)
            {
                return;
            }
            var meshFilter = gameObject.GetComponent<MeshFilter>();
            if (!meshFilter)
            {
                return;
            }
            var meshRenderer = gameObject.GetComponent<MeshRenderer>();
            if (!meshRenderer)
            {
                return;
            }
            var material = meshRenderer.sharedMaterial;
            if (!meshRenderer)
            {
                return;
            }
            if (MTEShaders.IsMTETextureArrayShader(material.shader))
            {
                return;
            }

            //collect targets info
            this.targetGameObject = gameObject;
            this.targetMaterial = material;
            this.targetMesh = meshFilter.sharedMesh;
            // splat textures
            LoadTextureList();
            LoadControlTextures();
            if (controlTextures[0] == null)
            {
                return;
            }

            // Preview
            if (TextureList.Count != 0)
            {
                if (SelectedTextureIndex < 0 || SelectedTextureIndex > TextureList.Count - 1)
                {
                    SelectedTextureIndex = 0;
                }
                LoadPreview();
            }
        }

        public string Header { get { return StringTable.Get(C.SplatPainter_Header); } }
        public string Description { get { return StringTable.Get(C.SplatPainter_Description); } }
        
        private static class Styles
        {
            public static string NoGameObjectSelectedHintText;

            private static bool unloaded= true;

            public static void Init()
            {
                if (!unloaded) return;
                NoGameObjectSelectedHintText
                    = StringTable.Get(C.Info_PleaseSelectAGameObjectWithVaildMesh);
                unloaded = false;
            }
        }

        public void DoArgsGUI()
        {
            Styles.Init();

            EditorGUI.BeginChangeCheck();
            this.PainterMode = (EditorFilterMode)GUILayout.Toolbar(
                (int)this.PainterMode, EditorFilterModeContents);
            if (EditorGUI.EndChangeCheck()
                && PainterMode == EditorFilterMode.SelectedGameObject)
            {
                BuildEditingInfoForLegacyMode(Selection.activeGameObject);
            }
            if (PainterMode == EditorFilterMode.SelectedGameObject
                && Selection.activeGameObject == null)
            {
                EditorGUILayout.HelpBox(Styles.NoGameObjectSelectedHintText, MessageType.Warning);
                return;
            }

            BrushIndex = Utility.ShowBrushes(BrushIndex);

            // Splat-textures
            if (!Settings.CompactGUI)
            {
                GUILayout.Label(StringTable.Get(C.Textures), MTEStyles.SubHeader);
            }
            EditorGUILayout.BeginVertical("box");
            {
                var textureListCount = TextureList.Count;
                if (textureListCount == 0)
                {
                    if (PainterMode == EditorFilterMode.FilteredGameObjects)
                    {
                        EditorGUILayout.LabelField(
                            StringTable.Get(C.Info_SplatPainter_NoSplatTextureFound),
                            GUILayout.Height(64));
                    }
                    else
                    {
                        EditorGUILayout.LabelField(
                            StringTable.Get(C.Info_SplatPainter_NoSplatTextureFoundOnSelectedObject),
                            GUILayout.Height(64));
                    }
                }
                else
                {
                    for (int i = 0; i < textureListCount; i += 4)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            var oldBgColor = GUI.backgroundColor;
                            for (int j = 0; j < 4; j++)
                            {
                                if (i + j >= textureListCount) break;

                                EditorGUILayout.BeginVertical();
                                var texture = TextureList[i + j];
                                bool toggleOn = SelectedTextureIndex == i + j;
                                if (toggleOn)
                                {
                                    GUI.backgroundColor = new Color(62 / 255.0f, 125 / 255.0f, 231 / 255.0f);
                                }

                                GUIContent toggleContent;
                                if (i + j + 1 <= MaxHotkeyNumberForTexture)
                                {
                                    toggleContent = new GUIContent(texture,
                                        StringTable.Get(C.Hotkey) + ':' + StringTable.Get(C.NumPad) + (i + j + 1));
                                }
                                else
                                {
                                    toggleContent = new GUIContent(texture);
                                }

                                var new_toggleOn = GUILayout.Toggle(toggleOn,
                                    toggleContent, GUI.skin.button,
                                    GUILayout.Width(64), GUILayout.Height(64));
                                GUI.backgroundColor = oldBgColor;
                                if (new_toggleOn && !toggleOn)
                                {
                                    SelectedTextureIndex = i + j;
                                    // reload the preview
                                    if (PainterMode == EditorFilterMode.SelectedGameObject)
                                    {
                                        preview.LoadPreviewFromObject(texture, BrushSizeInU3D, BrushIndex, targetGameObject);

                                    }
                                    else
                                    {
                                        preview.LoadPreview(texture, BrushSizeInU3D, BrushIndex);
                                    }
                                }
                                EditorGUILayout.EndVertical();
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            EditorGUILayout.EndVertical();

            //Settings
            if (!Settings.CompactGUI)
            {
                EditorGUILayout.Space();
                GUILayout.Label(StringTable.Get(C.Settings), MTEStyles.SubHeader);
            }
            BrushSize = EditorGUILayoutEx.Slider(StringTable.Get(C.Size), "-", "+", BrushSize, MinBrushSize, MaxBrushSize);
            BrushFlow = EditorGUILayoutEx.SliderLog10(StringTable.Get(C.Flow), "[", "]", BrushFlow, MinBrushFlow, MaxBrushFlow);

            GUILayout.FlexibleSpace();
            EditorGUILayout.HelpBox(StringTable.Get(C.Info_WillBeSavedInstantly),
                MessageType.Info, true);
        }
        
        public HashSet<Hotkey> DefineHotkeys()
        {
            var hashSet = new HashSet<Hotkey>
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
                    BrushFlow -= 0.01f;
                    MTEEditorWindow.Instance.Repaint();
                }),
                new Hotkey(this, KeyCode.RightBracket, () =>
                {
                    BrushFlow += 0.01f;
                    MTEEditorWindow.Instance.Repaint();
                }),
            };

            for (int i = 0; i < MaxHotkeyNumberForTexture; i++)
            {
                int index = i;
                var hotkey = new Hotkey(this, KeyCode.Keypad0+index+1, () =>
                {
                    SelectedTextureIndex = index;
                    // reload the preview
                    if (PainterMode == EditorFilterMode.SelectedGameObject)
                    {
                        preview.LoadPreviewFromObject(TextureList[SelectedTextureIndex], BrushSizeInU3D,
                            BrushIndex, targetGameObject);
                    }
                    else
                    {
                        preview.LoadPreview(TextureList[SelectedTextureIndex], BrushSizeInU3D,
                            BrushIndex);
                    }
                    MTEEditorWindow.Instance.Repaint();
                });
                hashSet.Add(hotkey);
            }

            return hashSet;
        }

        // buffers of editing helpers
        private readonly List<TextureModifyGroup> modifyGroups = new List<TextureModifyGroup>(4);
        private float[] BrushStrength = new float[1024 * 1024];//buffer for brush blending to forbid re-allocate big array every frame when painting.
        private readonly List<Color[]> modifyingSections = new List<Color[]>(2);

        private UndoTransaction currentUndoTransaction;

        public void OnSceneGUI()
        {
            var e = Event.current;

            if (preview == null || !preview.IsReady || TextureList.Count == 0)
            {
                return;
            }

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
            var ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit raycastHit;

            if (PainterMode == EditorFilterMode.SelectedGameObject)
            {
                if (!targetGameObject || !targetMaterial || !targetMesh)
                {
                    return;
                }

                if (!Physics.Raycast(ray, out raycastHit, Mathf.Infinity, ~targetGameObject.layer))
                {
                    return;
                }
                
                var currentBrushSize = BrushSizeInU3D/2;

                if (Settings.ShowBrushRect)
                {
                    Utility.ShowBrushRect(raycastHit.point, currentBrushSize);
                }

                var controlIndex = SelectedTextureIndex / 4;
                var controlTexture = controlTextures[controlIndex];
                var controlWidth = controlTexture.width;
                var controlHeight = controlTexture.height;
                var meshSize = targetGameObject.GetComponent<MeshRenderer>().bounds.size.x;
                var brushSizeInTexel = (int) Mathf.Round(BrushSizeInU3D/meshSize*controlWidth);
                preview.SetNormalizedBrushSize(BrushSizeInU3D/meshSize);
                preview.SetNormalizedBrushCenter(raycastHit.textureCoord);
                preview.SetPreviewSize(BrushSizeInU3D/2);
                preview.MoveTo(raycastHit.point);
                SceneView.RepaintAll();

                if ((e.type == EventType.MouseDrag && e.alt == false && e.shift == false && e.button == 0) ||
                    (e.type == EventType.MouseDown && e.shift == false && e.alt == false && e.button == 0))
                {
                    // 1. Collect all sections to be modified
                    var sections = new List<Color[]>();

                    var texelUV = raycastHit.textureCoord;
                    var pX = Mathf.FloorToInt(texelUV.x * controlWidth);
                    var pY = Mathf.FloorToInt(texelUV.y * controlHeight);
                    var x = Mathf.Clamp(pX - brushSizeInTexel / 2, 0, controlWidth - 1);
                    var y = Mathf.Clamp(pY - brushSizeInTexel / 2, 0, controlHeight - 1);
                    var width = Mathf.Clamp((pX + brushSizeInTexel / 2), 0, controlWidth) - x;
                    var height = Mathf.Clamp((pY + brushSizeInTexel / 2), 0, controlHeight) - y;

                    for (var i = 0; i < controlTextures.Length; i++)
                    {
                        var texture = controlTextures[i];
                        if (texture == null) continue;
                        sections.Add(texture.GetPixels(x, y, width, height, 0));
                    }

                    // 2. Modify target
                    var replaced = sections[controlIndex];
                    var maskTexture = (Texture2D) MTEStyles.brushTextures[BrushIndex];
                    BrushStrength = new float[brushSizeInTexel * brushSizeInTexel];
                    for (var i = 0; i < brushSizeInTexel; i++)
                    {
                        for (var j = 0; j < brushSizeInTexel; j++)
                        {
                            BrushStrength[j * brushSizeInTexel + i] =
                                maskTexture.GetPixelBilinear(((float) i) / brushSizeInTexel,
                                    ((float) j) / brushSizeInTexel).a;
                        }
                    }

                    var controlColor = new Color();
                    controlColor[SelectedTextureIndex % 4] = 1.0f;
                    for (var i = 0; i < height; i++)
                    {
                        for (var j = 0; j < width; j++)
                        {
                            var index = (i * width) + j;
                            var Stronger =
                                BrushStrength[
                                    Mathf.Clamp((y + i) - (pY - brushSizeInTexel / 2), 0,
                                        brushSizeInTexel - 1) *
                                    brushSizeInTexel +
                                    Mathf.Clamp((x + j) - (pX - brushSizeInTexel / 2), 0,
                                        brushSizeInTexel - 1)] *
                                BrushFlow;
                            replaced[index] = Color.Lerp(replaced[index], controlColor, Stronger);
                        }
                    }

                    if (e.type == EventType.MouseDown)
                    {
                        using (new UndoTransaction())
                        {
                            var material = targetMaterial;
                            if (material.HasProperty("_Control"))
                            {
                                Texture2D texture = (Texture2D) material.GetTexture("_Control");
                                if (texture != null)
                                {
                                    var originalColors = texture.GetPixels();
                                    UndoRedoManager.Instance().Push(a =>
                                    {
                                        texture.ModifyPixels(a);
                                        texture.Apply();
                                        Save(texture);
                                    }, originalColors, "Paint control texture");
                                }
                            }

                            if (material.HasProperty("_ControlExtra"))
                            {
                                Texture2D texture = (Texture2D) material.GetTexture("_ControlExtra");
                                if (texture != null)
                                {
                                    var originalColors = texture.GetPixels();
                                    UndoRedoManager.Instance().Push(a =>
                                    {
                                        texture.ModifyPixels(a);
                                        texture.Apply();
                                        Save(texture);
                                    }, originalColors, "Paint control texture");
                                }
                            }
                        }
                    }

                    controlTexture.SetPixels(x, y, width, height, replaced);
                    controlTexture.Apply();

                    // 3. Normalize other control textures
                    NormalizeWeightsLegacy(sections);
                    for (var i = 0; i < controlTextures.Length; i++)
                    {
                        var texture = controlTextures[i];
                        if (texture == null)
                        {
                            continue;
                        }

                        if (texture == controlTexture)
                        {
                            continue;
                        }

                        texture.SetPixels(x, y, width, height, sections[i]);
                        texture.Apply();
                    }
                }
                else if (e.type == EventType.MouseUp && e.alt == false && e.button == 0)
                {
                    foreach (var texture in controlTextures)
                    {
                        if (texture)
                        {
                            Save(texture);
                        }
                    }
                }
            }
            else
            {
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
                    var currentBrushSize = BrushSizeInU3D;

                    if (Settings.ShowBrushRect)
                    {
                        Utility.ShowBrushRect(raycastHit.point, currentBrushSize/2);
                    }

                    var hitPoint = raycastHit.point;
                    preview.MoveTo(hitPoint);

                    float meshSize = 1.0f;

                    // collect modify group
                    modifyGroups.Clear();
                    foreach (var target in MTEContext.Targets)
                    {
                        //MTEDebug.Log("Check if we can paint on target.");
                        var meshRenderer = target.GetComponent<MeshRenderer>();
                        if (meshRenderer == null) continue;
                        var meshFilter = target.GetComponent<MeshFilter>();
                        if (meshFilter == null) continue;
                        var mesh = meshFilter.sharedMesh;
                        if (mesh == null) continue;

                        Vector2 textureUVMin;//min texture uv that is to be modified
                        Vector2 textureUVMax;//max texture uv that is to be modified
                        Vector2 brushUVMin;//min brush mask uv that will be used
                        Vector2 brushUVMax;//max brush mask uv that will be used
                        {
                            //MTEDebug.Log("Start: Check if they intersect with each other.");
                            // check if the brush rect intersects with the `Mesh.bounds` of this target
                            var hitPointLocal = target.transform.InverseTransformPoint(hitPoint);//convert hit point from world space to target mesh space

                            Bounds brushBounds = new Bounds(center: new Vector3(hitPointLocal.x, 0, hitPointLocal.z), size: new Vector3(currentBrushSize, 99999, currentBrushSize));
                            Bounds meshBounds = mesh.bounds;//TODO rename this

                            Bounds paintingBounds;
                            var intersected = meshBounds.Intersect(brushBounds, out paintingBounds);
                            if(!intersected) continue;

                            Vector2 paintingBounds2D_min = new Vector2(paintingBounds.min.x, paintingBounds.min.z);
                            Vector2 paintingBounds2D_max = new Vector2(paintingBounds.max.x, paintingBounds.max.z);

                            //calculate which part of control texture should be modified
                            Vector2 meshRendererBounds2D_min = new Vector2(meshBounds.min.x, meshBounds.min.z);
                            Vector2 meshRendererBounds2D_max = new Vector2(meshBounds.max.x, meshBounds.max.z);
                            textureUVMin = MathEx.NormalizeTo01(rangeMin: meshRendererBounds2D_min, rangeMax: meshRendererBounds2D_max, value: paintingBounds2D_min);
                            textureUVMax = MathEx.NormalizeTo01(rangeMin: meshRendererBounds2D_min, rangeMax: meshRendererBounds2D_max, value: paintingBounds2D_max);

                            if (target.transform == raycastHit.transform)
                            {
                                meshSize = meshBounds.size.x;
                            }

                            //calculate which part of brush mask texture should be used
                            Vector2 brushBounds2D_min = new Vector2(brushBounds.min.x, brushBounds.min.z);
                            Vector2 brushBounds2D_max = new Vector2(brushBounds.max.x, brushBounds.max.z);
                            brushUVMin = MathEx.NormalizeTo01(rangeMin: brushBounds2D_min, rangeMax: brushBounds2D_max, value: paintingBounds2D_min);
                            brushUVMax = MathEx.NormalizeTo01(rangeMin: brushBounds2D_min, rangeMax: brushBounds2D_max, value: paintingBounds2D_max);

                            if (Settings.DebugMode)
                            {
                                Handles.color = Color.blue;
                                HandlesEx.DrawRectangle(paintingBounds2D_min, paintingBounds2D_max);
                                Handles.color = new Color(255, 128, 166);
                                HandlesEx.DrawRectangle(meshRendererBounds2D_min, meshRendererBounds2D_max);
                                Handles.color = Color.green;
                                HandlesEx.DrawRectangle(brushBounds2D_min, brushBounds2D_max);
                            }
                            //MTEDebug.Log("End: Check if they intersect with each other.");
                        }

                        if (e.button == 0 && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag))
                        {
                            //MTEDebug.Log("Start handling mouse down.");
                            // find the splat-texture in the material, get the X (splatIndex) from `_SplatX`
                            var selectedTexture = TextureList[SelectedTextureIndex];
                            var material = meshRenderer.sharedMaterial;
                            if (material == null)
                            {
                                MTEDebug.LogError("Failed to find material on target GameObject's MeshRenderer. " +
                                                  "The first material on the MeshRenderer should be editable by MTE.");
                                return;
                            }

                            //MTEDebug.Log("Finding the selected texture in the material.");
                            var splatIndex = material.FindSplatTexture(selectedTexture);
                            if (splatIndex < 0)
                            {
                                continue;
                            }

                            //MTEDebug.Log("get number of splat-textures in the material.");
                            int splatTotal = GetSplatTextureCount(material);

                            //MTEDebug.Log("check control textures.");

                            // check control textures
                            var controlTexture0_ = material.GetTexture("_Control");
                            Texture2D controlTexture0 = null, controlTexture1 = null;
                            if (controlTexture0_ != null)
                            {
                                controlTexture0 = (Texture2D)controlTexture0_;
                            }
                            else
                            {
                                throw new InvalidOperationException(string.Format("[MTE] \"_Control\" is not assigned or existing in material<{0}>.", material.name));
                            }

                            if (material.HasProperty("_ControlExtra"))
                            {
                                var controlTexture1_ = material.GetTexture("_ControlExtra");
                                if (controlTexture1_ == null)
                                {
                                    throw new InvalidOperationException(string.Format("[MTE] \"_ControlExtra\" is not assigned or existing in material<{0}>.", material.name));
                                }
                                controlTexture1 = (Texture2D)controlTexture1_;
                            }

                            // check which control texture is to be modified
                            Texture2D controlTexture = controlTexture0;
                            if (splatIndex >= 4)
                            {
                                controlTexture = controlTexture1;
                            }
                            System.Diagnostics.Debug.Assert(controlTexture != null, "controlTexture != null");

                            //get modifying texel rect of the control texture
                            int x = (int)Mathf.Clamp(textureUVMin.x * (controlTexture.width - 1), 0, controlTexture.width - 1);
                            int y = (int)Mathf.Clamp(textureUVMin.y * (controlTexture.height - 1), 0, controlTexture.height - 1);
                            int width = Mathf.Clamp(Mathf.FloorToInt(textureUVMax.x * controlTexture.width) - x, 0, controlTexture.width - x);
                            int height = Mathf.Clamp(Mathf.FloorToInt(textureUVMax.y * controlTexture.height) - y, 0, controlTexture.height - y);

                            var texelRect = new Rect(x, y, width, height);
                            modifyGroups.Add(new TextureModifyGroup(target, splatIndex, splatTotal, controlTexture0, controlTexture1, texelRect, brushUVMin, brushUVMax));

                            //MTEDebug.Log("End handling mouse down.");
                        }
                    }

                    preview.SetNormalizedBrushSize(BrushSizeInU3D/meshSize);
                    preview.SetNormalizedBrushCenter(raycastHit.textureCoord);

                    //record undo operation for targets that to be modified
                    if (e.button == 0 && e.type == EventType.MouseDown)
                    {
                        currentUndoTransaction = new UndoTransaction("Paint Texture");
                    }
                    
                    if (currentUndoTransaction != null && 
                       e.button == 0 && e.type==EventType.MouseDown)
                    {
                        foreach (var modifyGroup in modifyGroups)
                        {
                            var gameObject = modifyGroup.gameObject;
                            var material = gameObject.GetComponent<MeshRenderer>().sharedMaterial;
                            if (material.HasProperty("_Control"))
                            {
                                Texture2D texture = (Texture2D)material.GetTexture("_Control");
                                if (texture != null)
                                {
                                    var originalColors = texture.GetPixels();
                                    UndoRedoManager.Instance().Push(a =>
                                    {
                                        texture.ModifyPixels(a);
                                        texture.Apply();
                                        Save(texture);
                                    }, originalColors, "Paint control texture");
                                }
                            }
                            if (material.HasProperty("_ControlExtra"))
                            {
                                Texture2D texture = (Texture2D) material.GetTexture("_ControlExtra");
                                if (texture != null)
                                {
                                    var originalColors = texture.GetPixels();
                                    UndoRedoManager.Instance().Push(a =>
                                    {
                                        texture.ModifyPixels(a);
                                        texture.Apply();
                                        Save(texture);
                                    }, originalColors, "Paint control texture");
                                }
                            }
                        }
                    }

                    if (e.button == 0 && e.type == EventType.MouseUp)
                    {
                        Debug.Assert(currentUndoTransaction != null);
                        currentUndoTransaction.Dispose();
                    }

                    // execute the modification
                    if (modifyGroups.Count != 0)
                    {
                        for (int i = 0; i < modifyGroups.Count; i++)
                        {
                            var modifyGroup = modifyGroups[i];
                            var gameObject = modifyGroup.gameObject;
                            var material = gameObject.GetComponent<MeshRenderer>().sharedMaterial;
                            Utility.SetTextureReadable(material.GetTexture("_Control"));
                            if (material.HasProperty("_ControlExtra"))
                            {
                                Utility.SetTextureReadable(material.GetTexture("_ControlExtra"));
                            }
                            PaintTexture(modifyGroup.controlTexture0, modifyGroup.controlTexture1, modifyGroup.splatIndex, modifyGroup.splatTotal, modifyGroup.texelRect, modifyGroup.minUV, modifyGroup.maxUV);

                        }
                    }

                    // auto save when mouse up
                    if (e.type == EventType.MouseUp && e.button == 0)
                    {
                        foreach (var texture2D in DirtyTextureSet)
                        {
                            Save(texture2D);
                        }
                        DirtyTextureSet.Clear();
                    }
                }
            }

            SceneView.RepaintAll();
        }

        private static int GetSplatTextureCount(Material material)
        {
            var hasPackedSplat012 = material.HasProperty("_PackedSplat0")
                                     && material.HasProperty("_PackedSplat1")
                                     && material.HasProperty("_PackedSplat2");
            var hasPackedSplat345 = material.HasProperty("_PackedSplat3")
                                     && material.HasProperty("_PackedSplat4")
                                     && material.HasProperty("_PackedSplat5");
            if(hasPackedSplat012)
            {
                if (hasPackedSplat345)
                {
                    return 8;
                }
                return 4;
            }

            int splatTotal;
            if (material.HasProperty("_Splat7"))
            {
                splatTotal = 8;
            }
            else if (material.HasProperty("_Splat6"))
            {
                splatTotal = 7;
            }
            else if (material.HasProperty("_Splat5"))
            {
                splatTotal = 6;
            }
            else if (material.HasProperty("_Splat4"))
            {
                splatTotal = 5;
            }
            else if (material.HasProperty("_Splat3"))
            {
                splatTotal = 4;
            }
            else if (material.HasProperty("_Splat2"))
            {
                splatTotal = 3;
            }
            else if (material.HasProperty("_Splat1"))
            {
                splatTotal = 2;
            }
            else
            {
                throw new InvalidShaderException(
                    "[MTE] Cannot find property _Splat1/2/3/4/5/6/7 or _PackedSplat0/1/2/3/4/5 in shader.");
            }

            return splatTotal;
        }

        private void PaintTexture(Texture2D controlTexture0, Texture2D controlTexture1, int splatIndex, int splatTotal, Rect texelRect, Vector2 minUV, Vector2 maxUV)
        {
            // check parameters
            if (controlTexture0 == null)
            {
                throw new ArgumentNullException("controlTexture0");
            }
            if (splatIndex > 3 && controlTexture1 == null)
            {
                throw new ArgumentException("[MTE] splatIndex is 4/5/6/7 but controlTexture1 is null.", "controlTexture1");
            }
            if (splatIndex < 0 || splatIndex > 7)
            {
                throw new ArgumentOutOfRangeException("splatIndex", splatIndex, "splatIndex should be 0/1/2/3/4/5/6/7.");
            }

            // collect the pixel sections to modify
            modifyingSections.Clear();
            int x = (int)texelRect.x;
            int y = (int)texelRect.y;
            int width = (int)texelRect.width;
            int height = (int)texelRect.height;
            modifyingSections.Add(controlTexture0.GetPixels(x, y, width, height, 0));
            if (controlTexture1 != null)
            {
                modifyingSections.Add(controlTexture1.GetPixels(x, y, width, height, 0));
            }

            // sample brush strength from the mask texture
            var maskTexture = (Texture2D) MTEStyles.brushTextures[BrushIndex];
            if (BrushStrength.Length < width*height)//enlarge buffer if it is not big enough
            {
                BrushStrength = new float[width * height];
            }
            var unitUV_u = (maxUV.x - minUV.x)/(width-1);
            if (width == 1)
            {
                unitUV_u = maxUV.x - minUV.x;
            }
            var unitUV_v = (maxUV.y - minUV.y)/(height-1);
            if (height == 1)
            {
                unitUV_v = maxUV.y - minUV.y;
            }
            for (var i = 0; i < height; i++)
            {
                float v = minUV.y + i * unitUV_v;
                for (var j = 0; j < width; j++)
                {
                    var pixelIndex = i * width + j;
                    float u = minUV.x + j * unitUV_u;
                    BrushStrength[pixelIndex] = maskTexture.GetPixelBilinear(u, v).a;
                }
            }

            // blend the pixel section
            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    var pixelIndex = i * width + j;
                    var factor = BrushStrength[pixelIndex] * BrushFlow;
                    var oldWeight = GetWeight(modifyingSections, j, i, width, splatIndex);
                    var newWeight = Mathf.Lerp(oldWeight, 1, factor);
                    SetWeight(modifyingSections, j, i, width, splatIndex, newWeight);
                    NormalizeWeights(j, i, width, splatIndex, splatTotal);
                }
            }

            // modify the control texture
            if(splatTotal >= 5)
            {
                controlTexture0.SetPixels(x, y, width, height, modifyingSections[0]);
                controlTexture0.Apply();
                System.Diagnostics.Debug.Assert(controlTexture1 != null, nameof(controlTexture1) + " != null");
                controlTexture1.SetPixels(x, y, width, height, modifyingSections[1]);
                controlTexture1.Apply();
                DirtyTextureSet.Add(controlTexture0);
                DirtyTextureSet.Add(controlTexture1);
            }
            else
            {
                controlTexture0.SetPixels(x, y, width, height, modifyingSections[0]);
                controlTexture0.Apply();
                DirtyTextureSet.Add(controlTexture0);
            }
        }

        private static float GetWeight(List<Color[]> colorData, int x, int y, int width, int splatIndex)
        {
            var colors = colorData[splatIndex / 4];
            var weight = colors[y * width + x][splatIndex % 4];
            return weight;
        }

        private static void SetWeight(List<Color[]> colorData, int x, int y, int width, int splatIndex, float weight)
        {
            var colors = colorData[splatIndex / 4];
            var color = colors[y * width + x];
            color[splatIndex % 4] = weight;
            colors[y * width + x] = color;
        }

        private void NormalizeWeights(int x, int y, int width, int splatIndex, int splatTotal)
        {
            float newWeight = GetWeight(modifyingSections, x, y, width, splatIndex);
            float otherWeights = 0;
            for (int i = 0; i < splatTotal; i++)
            {
                if (i != splatIndex)
                {
                    otherWeights += GetWeight(modifyingSections, x, y, width, i);
                }
            }
            if (otherWeights >= 0.01)
            {
                float k = (1 - newWeight) / otherWeights;
                for (int i = 0; i < splatTotal; i++)
                {
                    if (i != splatIndex)
                    {
                        var weight = k * GetWeight(modifyingSections, x, y, width, i);
                        SetWeight(modifyingSections, x, y, width, i, weight);
                    }
                }
            }
            else
            {
                for (int i = 0; i < splatTotal; i++)
                {
                    var weight = (i == splatIndex) ? 1 : 0;
                    SetWeight(modifyingSections, x, y, width, i, weight);
                }
            }
        }
        
        private void NormalizeWeightsLegacy(List<Color[]> sections)
        {
            var colorCount = sections[0].Length;
            for (var i = 0; i < colorCount; i++)
            {
                var total = 0f;
                for (var j = 0; j < sections.Count; j++)
                {
                    var color = sections[j][i];
                    total += color[0] + color[1] + color[2] + color[3];
                    if(j == SelectedTextureIndex/4)
                    {
                        total -= color[SelectedTextureIndex%4];
                    }
                }
                if(total > 0.01)
                {
                    var a = sections[SelectedTextureIndex/4][i][SelectedTextureIndex%4];
                    var k = (1 - a)/total;

                    for (var j = 0; j < sections.Count; j++)
                    {
                        for (var l = 0; l < 4; l++)
                        {
                            if(!(j == SelectedTextureIndex/4 && l == SelectedTextureIndex%4))
                            {
                                sections[j][i][l] *= k;
                            }
                        }
                    }
                }
                else
                {
                    for (var j = 0; j < sections.Count; j++)
                    {
                        sections[j][i][SelectedTextureIndex%4] = (j != SelectedTextureIndex/4) ? 0 : 1;
                    }
                }
            }
        }

        public static readonly HashSet<Texture2D> DirtyTextureSet = new HashSet<Texture2D>();

        private static void Save(Texture2D texture)
        {
            if(texture == null)
            {
                throw new ArgumentNullException("texture");
            }
            var path = AssetDatabase.GetAssetPath(texture);
            var bytes = texture.EncodeToPNG();
            if(bytes == null || bytes.Length == 0)
            {
                throw new Exception("[MTE] Failed to save texture to png file.");
            }
            File.WriteAllBytes(path, bytes);
            MTEDebug.LogFormat("Texture<{0}> saved to <{1}>.", texture.name, path);
        }

        private Preview preview = new Preview(isArray: false);

        //Don't modify this field, it's used by MTE editors internally
        public List<Texture> TextureList = new List<Texture>(16);

        /// <summary>
        /// load all splat textures form targets
        /// </summary>
        public void LoadTextureList()
        {
            TextureList.Clear();
            if (painterMode == EditorFilterMode.SelectedGameObject)
            {
                MTEDebug.Log("Loading layer textures on selected GameObject...");
                LoadTargetTextures(targetGameObject);
            }
            else
            {
                MTEDebug.Log("Loading layer textures on target GameObject(s)...");
                foreach (var target in MTEContext.Targets)
                {
                    LoadTargetTextures(target);
                }
            }

            // make collected splat textures readable
            Utility.SetTextureReadable(TextureList, true);
            MTEDebug.LogFormat("{0} layer textures loaded.", TextureList.Count);
        }

        private void LoadTargetTextures(GameObject target)
        {
            if (!target)
            {
                return;
            }
            var meshRenderer = target.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
            {
                return;
            }
            var material = meshRenderer.sharedMaterial;

            if (!material)
            {
                return;
            }

            if (!CheckIfMaterialAssetPathAvailable(material))
            {
                return;
            }

            Shader shader = material.shader;
            if (shader == null)
            {
                MTEDebug.LogWarning(string.Format("The material<{0}> isn't using a valid shader!", material.name));
                return;
            }

            //regular shaders: find textures from shader properties
            var propertyCount = ShaderUtil.GetPropertyCount(shader);
            for (int j = 0; j < propertyCount; j++)
            {
                if (ShaderUtil.GetPropertyType(shader, j) == ShaderUtil.ShaderPropertyType.TexEnv)
                {
                    var propertyName = ShaderUtil.GetPropertyName(shader, j); //propertyName should be _Splat0/1/2/3/4
                    if (propertyName.StartsWith("_Splat"))
                    {
                        var texture = material.GetTexture(propertyName);
                        if (texture is Texture2DArray)
                        {
                            continue;
                        }
                        if (texture != null && !TextureList.Contains(texture))
                        {
                            TextureList.Add(texture);
                        }
                    }
                }
            }

            //packed shaders: find textures from related parameter asset
            var parameters = material.LoadPackedShaderGUIParameters();
            if (parameters != null)
            {
                foreach (var texture in parameters.SplatTextures)
                {
                    if (texture != null && !TextureList.Contains(texture))
                    {
                        TextureList.Add(texture);
                    }
                }
            }
        }

        private void LoadControlTextures()
        {
            if (!targetMaterial)
            {
                return;
            }

            var material = targetMaterial;
            Texture controlTexture0_ = null;
            if (material.HasProperty("_Control"))
            {
                controlTexture0_ = material.GetTexture("_Control");
            }
            if (controlTexture0_ != null)
            {
                controlTextures[0] = (Texture2D)controlTexture0_;
            }
            else
            {
                MTEDebug.LogWarning(
                    $"[MTE] \"_Control\" is not assigned or existing in material<{material.name}>.");
            }

            if (material.HasProperty("_ControlExtra"))
            {
                var controlTexture1_ = material.GetTexture("_ControlExtra");
                if (controlTexture1_ == null)
                {
                    MTEDebug.LogWarning(
                        $"[MTE] \"_ControlExtra\" is not assigned or existing in material<{material.name}>.");
                }
                else
                {
                    controlTextures[1] = (Texture2D)controlTexture1_;
                }
            }
        }

        private static bool CheckIfMaterialAssetPathAvailable(Material material)
        {
            var relativePathOfMaterial = AssetDatabase.GetAssetPath(material);
            if (relativePathOfMaterial.StartsWith("Resources"))
            {//built-in material
                return false;
            }
            return true;
        }
    }
}