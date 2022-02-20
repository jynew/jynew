using System.Collections.Generic;
using System.IO;
using System.Linq;
using MTE.Undo;
using UnityEditor;
using UnityEngine;
using System.Runtime.CompilerServices;
using static MTE.TextureArrayShaderPropertyNames;
using System.Runtime.InteropServices;

namespace MTE
{
    /// <summary>
    /// Texture-array-based Mesh-Terrain texture editor.
    /// </summary>
    internal class TextureArrayPainter : IEditor
    {
        public int Id { get; } = 10;

        public bool Enabled { get; set; } = true;

        public string Name { get; } = nameof(TextureArrayPainter);

        public Texture Icon { get; } =
            EditorGUIUtility.IconContent("TerrainInspector.TerrainToolSplat").image;

        public string Header { get { return StringTable.Get(C.TextureArrayPainter_Header); } }

        public string Description { get { return StringTable.Get(C.TextureArrayPainter_Description); } }

        public bool WantMouseMove { get; } = true;

        public bool WillEditMesh { get; } = false;


        #region Parameters

        #region Constant
        // default
        const EditorFilterMode DefaultPainterMode
            = EditorFilterMode.FilteredGameObjects;
        const float DefaultBrushSize = 1;
        const float DefaultBrushFlow = 0.5f;
        // min/max
        const float MinBrushSize = 0.1f;
        const float MaxBrushSize = 10f;
        const float MinBrushFlow = 0.01f;
        const float MaxBrushFlow = 1f;
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

                    EditorPrefs.SetFloat("MTE_TextureArrayPainter.brushSize", value);
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
                    EditorPrefs.SetFloat("MTE_TextureArrayPainter.brushFlow", value);
                }
            }
        }
        #endregion

        #region UI
        private static readonly GUIContent[] EditorFilterModeContents =
        {
            new GUIContent(StringTable.Get(C.SplatPainter_Mode_Filtered),
                StringTable.Get(C.SplatPainter_Mode_FilteredDescription)),
            new GUIContent(StringTable.Get(C.SplatPainter_Mode_Selected),
                StringTable.Get(C.SplatPainter_Mode_SelectedDescription)),
        };
        #endregion
        
        public TextureArrayPainter()
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
                if (args.SelectedGameObject)
                {
                    if (PainterMode == EditorFilterMode.SelectedGameObject)
                    {
                        BuildEditingInfoForLegacyMode(args.SelectedGameObject);
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
            painterMode = (EditorFilterMode)EditorPrefs.GetInt(
                "MTE_TextureArrayPainter.painterMode", (int)DefaultPainterMode);
            brushSize = EditorPrefs.GetFloat("MTE_TextureArrayPainter.brushSize", DefaultBrushSize);
            brushFlow = EditorPrefs.GetFloat("MTE_TextureArrayPainter.brushFlow", DefaultBrushFlow);
        }

        private GameObject targetGameObject { get; set; }
        private Mesh targetMesh { get; set; }
        private Material targetMaterial { get; set; }
        private Texture2D[] controlTextures { get; } = new Texture2D[3] {null, null, null};
        private void BuildEditingInfoForLegacyMode(GameObject gameObject)
        {
            //reset
            this.TextureList.Clear();
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
            if (!material)
            {
                return;
            }
            if (!MTEShaders.IsMTETextureArrayShader(material.shader))
            {
                return;
            }

            //collect targets info
            this.targetGameObject = gameObject;
            this.targetMaterial = material;
            this.targetMesh = meshFilter.sharedMesh;
            // Texture
            LoadTextureList();
            LoadControlTextures();
            // Preview
            if (TextureList.Count != 0)
            {
                if (SelectedTextureIndex < 0 || SelectedTextureIndex > TextureList.Count - 1)
                {
                    SelectedTextureIndex = 0;
                }
                preview.LoadPreview(TextureList[SelectedTextureIndex],
                    BrushSizeInU3D,
                    BrushIndex);
            }
        }
        
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
            if (EditorGUI.EndChangeCheck())
            {
                LoadTextureList();
                if (PainterMode == EditorFilterMode.SelectedGameObject)
                {
                    BuildEditingInfoForLegacyMode(Selection.activeGameObject);
                }
                LoadPreview();
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
                        //TODO use texture-array version message
                    }
                    else
                    {
                        EditorGUILayout.LabelField(
                            StringTable.Get(C.Info_TextureArrayPainter_NoSplatTextureFoundOnSelectedObject),
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

            //Tools
            if (!Settings.CompactGUI)
            {
                EditorGUILayout.Space();
                GUILayout.Label(StringTable.Get(C.Tools), MTEStyles.SubHeader);
            }
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button(StringTable.Get(C.CreateTextureArraySettings),
                        GUILayout.Width(100), GUILayout.Height(40)))
                    {
                        EditorApplication.ExecuteMenuItem(
                            $"Assets/Create/Mesh Terrain Editor/{nameof(TextureArraySettings)}");
                    }
                    GUILayout.Space(20);
                    EditorGUILayout.LabelField(
                        StringTable.Get(C.Info_ToolDescription_CreateTextureArraySettings),
                        MTEStyles.labelFieldWordwrap);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

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
                    LoadPreview();
                });
                hashSet.Add(hotkey);
            }

            return hashSet;
        }

        // buffers of editing helpers
        private readonly List<TextureModifyGroup> modifyGroups = new List<TextureModifyGroup>(4);
        private float[] BrushStrength = new float[1024 * 1024];//buffer for brush blending to forbid re-allocate big array every frame when painting.
        private readonly List<Color[]> modifyingSections = new List<Color[]>(3);
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

                if (targetGameObject != raycastHit.transform.gameObject)
                {
                    return;
                }
            
                var currentBrushSize = BrushSizeInU3D/2;

                if (Settings.ShowBrushRect)
                {
                    Utility.ShowBrushRect(raycastHit.point, currentBrushSize);
                }

                var controlIndex = SelectedTextureIndex / 4;
                Debug.Assert(0 <= controlIndex && controlIndex <= 3);
                var controlTexture = controlTextures[controlIndex];
                if (controlTexture == null)
                {
                    throw new MTEEditException("The control texture at index {controlIndex} is null.");
                }
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

                    var pixelUV = raycastHit.textureCoord;
                    var pX = Mathf.FloorToInt(pixelUV.x * controlWidth);
                    var pY = Mathf.FloorToInt(pixelUV.y * controlHeight);
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
                            if (material.HasProperty(ControlTexturePropertyNames[0]))
                            {
                                Texture2D texture = (Texture2D) material.GetTexture(ControlTexturePropertyNames[0]);
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

                            if (material.HasProperty(ControlTexturePropertyNames[1]))
                            {
                                Texture2D texture = (Texture2D) material.GetTexture(ControlTexturePropertyNames[1]);
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

                            if (material.HasProperty(ControlTexturePropertyNames[2]))
                            {
                                Texture2D texture = (Texture2D) material.GetTexture(ControlTexturePropertyNames[2]);
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
                            //Convention: for texture array, the selected texture index is the layer index
                            Texture2DArray textureArray =
                                material.GetTexture(AlbedoArrayPropertyName) as Texture2DArray;
                            var layerIndex = TextureArrayManager.Instance.GetTextureSliceIndex(textureArray,
                                selectedTexture);
                            if (layerIndex < 0)
                            {
                                continue;
                            }

                            //MTEDebug.Log("get number of layer-textures in the material.");
                            int splatTotal = GetLayerTextureNumber(material);

                            //MTEDebug.Log("check control textures.");

                            // check control textures
                            var controlTexture0_ = material.GetTexture(ControlTexturePropertyNames[0]);
                            Texture2D controlTexture0 = null, controlTexture1 = null, controlTexture2 = null;
                            if (controlTexture0_ != null)
                            {
                                controlTexture0 = (Texture2D)controlTexture0_;
                            }
                            else
                            {
                                throw new MTEEditException($"{ControlTexturePropertyNames[0]} is" +
                                    $" not assigned or existing in material<{material.name}>.");
                            }

                            if (material.HasProperty(ControlTexturePropertyNames[1]))
                            {
                                var controlTexture1_ = material.GetTexture(ControlTexturePropertyNames[1]);
                                if (controlTexture1_ == null)
                                {
                                    throw new MTEEditException($"{ControlTexturePropertyNames[1]} " +
                                        $"is not assigned or existing in material<{material.name}>.");
                                }
                                controlTexture1 = (Texture2D)controlTexture1_;
                            }

                            if (material.HasProperty(ControlTexturePropertyNames[2]))
                            {
                                var controlTexture2_ = material.GetTexture(ControlTexturePropertyNames[2]);
                                if (controlTexture2_ == null)
                                {
                                    throw new MTEEditException($"{ControlTexturePropertyNames[2]} " +
                                        $"is not assigned or existing in material<{material.name}>.");
                                }
                                controlTexture2 = (Texture2D)controlTexture2_;
                            }

                            // check which control texture is to be modified
                            Texture2D controlTexture = controlTexture0;
                            if (layerIndex >= 4)
                            {
                                controlTexture = controlTexture1;
                            }
                            if (layerIndex >= 8)
                            {
                                controlTexture = controlTexture2;
                            }
                            if (controlTexture1 != null)
                            {
                                if (controlTexture0.width != controlTexture1.width)
                                {
                                    throw new MTEEditException(
                                        $"Size of {controlTexture0.name} is different from other control textures." +
                                        "Make sure all control textures is the same size.");
                                }
                            }
                            if (controlTexture2 != null)
                            {
                                if (controlTexture0.width != controlTexture2.width)
                                {
                                    throw new MTEEditException(
                                        $"Size of {controlTexture2.name} is different from other control textures." +
                                        "Make sure all control textures is the same size.");
                                }
                            }
                            System.Diagnostics.Debug.Assert(controlTexture != null, "controlTexture != null");

                            //get modifying texel rect of the control texture
                            int x = (int)Mathf.Clamp(textureUVMin.x * (controlTexture.width - 1), 0, controlTexture.width - 1);
                            int y = (int)Mathf.Clamp(textureUVMin.y * (controlTexture.height - 1), 0, controlTexture.height - 1);
                            int width = Mathf.Clamp(Mathf.FloorToInt(textureUVMax.x * controlTexture.width) - x, 0, controlTexture.width - x);
                            int height = Mathf.Clamp(Mathf.FloorToInt(textureUVMax.y * controlTexture.height) - y, 0, controlTexture.height - y);

                            var texelRect = new Rect(x, y, width, height);
                            modifyGroups.Add(new TextureModifyGroup(target, layerIndex, splatTotal,
                                controlTexture0, controlTexture1, controlTexture2,
                                texelRect, brushUVMin, brushUVMax));

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
                       e.button == 0 && e.type== EventType.MouseDown)
                    {
                        //record values before modification for undo
                        foreach (var modifyGroup in modifyGroups)
                        {
                            var gameObject = modifyGroup.gameObject;
                            var material = gameObject.GetComponent<MeshRenderer>().sharedMaterial;
                            if (material.HasProperty(ControlTexturePropertyNames[0]))
                            {
                                Texture2D texture = (Texture2D)material.GetTexture(ControlTexturePropertyNames[0]);
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
                            if (material.HasProperty(ControlTexturePropertyNames[1]))
                            {
                                Texture2D texture = (Texture2D) material.GetTexture(ControlTexturePropertyNames[1]);
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
                            if (material.HasProperty(ControlTexturePropertyNames[2]))
                            {
                                Texture2D texture = (Texture2D) material.GetTexture(ControlTexturePropertyNames[2]);
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
                            //set all control textures readable, just in case
                            Utility.SetTextureReadable(material.GetTexture(ControlTexturePropertyNames[0]));
                            if (material.HasProperty(ControlTexturePropertyNames[1]))
                            {
                                Utility.SetTextureReadable(material.GetTexture(ControlTexturePropertyNames[1]));
                            }
                            if (material.HasProperty(ControlTexturePropertyNames[2]))
                            {
                                Utility.SetTextureReadable(material.GetTexture(ControlTexturePropertyNames[2]));
                            }
                            PaintTexture(modifyGroup.controlTexture0,
                                modifyGroup.controlTexture1,
                                modifyGroup.controlTexture2,
                                modifyGroup.splatIndex,
                                modifyGroup.splatTotal,
                                modifyGroup.texelRect,
                                modifyGroup.minUV, modifyGroup.maxUV);
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

        private void PaintTexture(Texture2D controlTexture0, Texture2D controlTexture1,
             Texture2D controlTexture2, int splatIndex, int splatTotal, Rect texelRect,
             Vector2 minUV, Vector2 maxUV)
        {
            // check parameters
            if (controlTexture0 == null)
            {
                throw new System.ArgumentException(
                    $"[MTE] {nameof(controlTexture0)} is null.",
                    nameof(controlTexture0));
            }
            if (splatIndex > 3 && controlTexture1 == null)
            {
                throw new System.ArgumentException(
                    $"[MTE] splatIndex is 4/5/6/7 but {nameof(controlTexture1)} is null.",
                    nameof(controlTexture1));
            }
            if (splatIndex > 7 && controlTexture1 == null)
            {
                throw new System.ArgumentException(
                    $"[MTE] splatIndex is 8/9/10/11 but {nameof(controlTexture2)} is null.",
                    nameof(controlTexture2));
            }
            if (splatIndex < 0 || splatIndex > 11)
            {
                throw new System.ArgumentOutOfRangeException("splatIndex", splatIndex,
                    "[MTE] splatIndex should be [0, 11].");
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
            else
            {
                modifyingSections.Add(new Color[1]);
            }
            if (controlTexture2 != null)
            {
                modifyingSections.Add(controlTexture2.GetPixels(x, y, width, height, 0));
            }
            else
            {
                modifyingSections.Add(new Color[1]);
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
            Utility.BlendPixelSections(BrushFlow, BrushStrength, modifyingSections,
                splatIndex, splatTotal, height, width);

            // modify the control texture
            if (splatTotal >= 8)
            {
                controlTexture0.SetPixels(x, y, width, height, modifyingSections[0]);
                controlTexture0.Apply();
                controlTexture1.SetPixels(x, y, width, height, modifyingSections[1]);
                controlTexture1.Apply();
                controlTexture2.SetPixels(x, y, width, height, modifyingSections[2]);
                controlTexture2.Apply();
                DirtyTextureSet.Add(controlTexture0);
                DirtyTextureSet.Add(controlTexture1);
                DirtyTextureSet.Add(controlTexture2);
            }
            else if(splatTotal >= 5)
            {
                controlTexture0.SetPixels(x, y, width, height, modifyingSections[0]);
                controlTexture0.Apply();
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
                throw new System.ArgumentNullException("texture");
            }
            var path = AssetDatabase.GetAssetPath(texture);
            var bytes = texture.EncodeToPNG();
            if(bytes == null || bytes.Length == 0)
            {
                throw new System.Exception("[MTE] Failed to save texture to png file.");
            }
            File.WriteAllBytes(path, bytes);
            MTEDebug.LogFormat("Texture<{0}> saved to <{1}>.", texture.name, path);
        }

        private Preview preview = new Preview(isArray: true);

        //Don't modify this field, it's used by MTE editors internally
        public List<Texture> TextureList = new List<Texture>(16);

        /// <summary>
        /// load all splat textures form targets
        /// </summary>
        private void LoadTextureList()
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
                MTEDebug.LogWarning($"Material<{material.name}> doesn't use a valid shader!");
                return;
            }

            if (!MTEShaders.IsMTETextureArrayShader(shader))
            {
                MTEDebug.LogWarning(
                    $"Material<{material.name}> doesn't use a MTE TextureArray shader!");
                return;
            }

            var propertyCount = ShaderUtil.GetPropertyCount(shader);
            for (int j = 0; j < propertyCount; j++)
            {
                if (ShaderUtil.GetPropertyType(shader, j) != ShaderUtil.ShaderPropertyType.TexEnv)
                {
                    continue;
                }

                var propertyName = ShaderUtil.GetPropertyName(shader, j);
                if (propertyName != AlbedoArrayPropertyName)
                {
                    continue;
                }

                var textureArray = material.GetTexture(AlbedoArrayPropertyName) as Texture2DArray;
                if (textureArray == null || textureArray.depth <= 0)
                {
                    continue;
                }

                TextureArrayManager.Instance.AddOrUpdate(textureArray);
                TextureArrayManager.Instance.GetTextures(textureArray, out var textures);
                foreach (var texture in textures)
                {
                    if (!TextureList.Contains(texture))
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

            int width = -1, height = -1;
            for (int i = 0; i < ControlTexturePropertyNames.Length; i++)
            {
                var controlPropertyName = ControlTexturePropertyNames[i];
                var controlTexture = targetMaterial.GetTexture(controlPropertyName) as Texture2D;
                if (i == 0 && controlTexture == null )
                {
                    MTEDebug.LogWarning(
                        $"[MTE] \"{controlPropertyName}\" is not assigned" +
                        $" or existing in material<{targetMaterial.name}>.");
                }
                var controlTextureWidth = controlTexture.width;
                var controlTextureHeight = controlTexture.height;
                if (controlTextureWidth != controlTextureHeight)
                {
                    throw new MTEEditException($"{controlPropertyName} texture is not square.");
                }

                if (width < 0 && height < 0)
                {
                    width = controlTextureWidth;
                    height = controlTextureHeight;
                }
                if (width != controlTextureWidth || height != controlTextureHeight)
                {
                    throw new MTEEditException(
                        $"Size of {controlPropertyName} is different from others.");
                }

                controlTextures[i] = controlTexture;
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
        
        private static int GetLayerTextureNumber(Material material)
        {
            if (material.IsKeywordEnabled(KeyWords.HasWeightMap2))
            {
                return 12;
            }
            if (material.IsKeywordEnabled(KeyWords.HasWeightMap1))
            {
                return 8;
            }
            return 4;
        }
    }
}