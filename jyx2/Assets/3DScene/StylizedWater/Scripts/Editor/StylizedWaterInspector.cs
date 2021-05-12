// Stylized Water Shader by Staggart Creations http://u3d.as/A2R
// Online documentation can be found at http://staggart.xyz

using UnityEngine;
using UnityEditor.AnimatedValues;
using UnityEditor;


namespace StylizedWater
{
    [CustomEditor(typeof(StylizedWater))]
    public class StylizedWaterInspector : Editor
    {
        //Non serialized, local
        StylizedWater stylizedWater;

        new SerializedObject serializedObject;

        //Variables
        #region Shader parameters
        private Shader shader;
        string shaderName = null;
        SerializedProperty shaderIndex;

        //Colors
        SerializedProperty waterColor;
        SerializedProperty waterShallowColor;
        SerializedProperty depth;
        SerializedProperty colorGradient;
        SerializedProperty enableGradient;
        SerializedProperty fresnelColor;
        SerializedProperty fresnel;
        SerializedProperty rimColor;
        SerializedProperty waveTint;

        //Surface
        SerializedProperty transparency;
        SerializedProperty edgeFade;
        SerializedProperty worldSpaceTiling;
        public string[] tilingMethodNames;
        SerializedProperty glossiness;
        SerializedProperty refractionAmount;

        //Normals
        SerializedProperty enableNormalMap;
        SerializedProperty normalStrength;
        SerializedProperty enableMacroNormals;
        SerializedProperty macroNormalsDistance;
        SerializedProperty normalTiling;

        //Intersection
        SerializedProperty intersectionSolver;
        SerializedProperty rimSize;
        SerializedProperty rimFalloff;
        SerializedProperty rimTiling;
        SerializedProperty rimDistortion;

        //Foam
        SerializedProperty foamSolver;
        SerializedProperty foamOpacity;
        SerializedProperty foamSize;
        SerializedProperty foamTiling;
        SerializedProperty foamDistortion;
        SerializedProperty foamSpeed;

        //Waves
        SerializedProperty waveSpeed;
        SerializedProperty waveStrength;
        SerializedProperty waveSize;
        SerializedProperty enableSecondaryWaves;
        SerializedProperty waveFoam;
        SerializedProperty waveDirectionXZ;
        private float waveDirectionX;
        private float waveDirectionZ;

        //Other
        //SerializedProperty tessellation;
        #endregion

        #region Rendering parameters
        //SerializedProperty renderQueue;
        SerializedProperty enableVertexColors;
        SerializedProperty enableMaskRendering;
        SerializedProperty swsMaskRenderLayers;

        //Lighting
        SerializedProperty lightingMethod;
        SerializedProperty enableShadows;
        SerializedProperty shadowCaster;

        #endregion

        #region Texture parameters
        SerializedProperty useCompression;

        SerializedProperty useCustomIntersection;
        SerializedProperty useCustomNormals;
        SerializedProperty useCustomHeightmap;

        SerializedProperty intersectionStyle;
        SerializedProperty waveStyle;
        SerializedProperty waveHeightmapStyle;
        #endregion

        #region Local variables
        //Input textures
        SerializedProperty customIntersection;
        SerializedProperty customHeightmap;
        SerializedProperty customNormal;

        GameObject selected;
        public bool isMobileAdvanced;
        public bool isMobileBasic;
        #endregion

        #region Reflection
        //SerializedProperty refractionSolver;
        SerializedProperty reflectionRefraction;
        // SerializedProperty refractionRes;


        SerializedProperty enableReflectionBlur;
        SerializedProperty reflectionBlurLength;
        SerializedProperty reflectionBlurPasses;
        SerializedProperty useReflection;
        SerializedProperty clipPlaneOffset;

        public string[] reslist = new string[] { "64x64", "128x128", "256x256", "512x512", "1024x1024", "2048x2048" };
        SerializedProperty reflectLayers;
        SerializedProperty reflectionRes;
        SerializedProperty reflectionStrength;
        SerializedProperty reflectionFresnel;
        #endregion

        #region Meta
        //Section toggles
        private SerializedProperty showColors;
        private SerializedProperty showLighting;
        private SerializedProperty showSurface;
        private SerializedProperty showNormals;
        private SerializedProperty showReflection;
        private SerializedProperty showIntersection;
        private SerializedProperty showFoam;
        private SerializedProperty showDepth;
        private SerializedProperty showWaves;
        private SerializedProperty showAdvanced;

        //Section anims
        private float animSpeed = 4f;
        AnimBool m_showHelp;
        AnimBool m_showColors;
        AnimBool m_showLighting;
        AnimBool m_showSurface;
        AnimBool m_showNormals;
        AnimBool m_showReflection;
        AnimBool m_showIntersection;
        AnimBool m_showFoam;
        AnimBool m_showDepth;
        AnimBool m_showWaves;
        AnimBool m_showAdvanced;

        //Help toggles
        private bool showHelp;
        private bool showHelpColors;
        private bool showHelpLighting;
        private bool showHelpSurface;
        private bool showHelpNormals;
        private bool showHelpReflection;
        private bool showHelpIntersection;
        private bool showHelpFoam;
        private bool showHelpDepth;
        private bool showHelpWaves;
        private bool showHelpAdvanced;

        private SerializedProperty isWaterLayer;
        private SerializedProperty hasShaderParams;
        private Shader currentShader;
#if !UNITY_5_5_OR_NEWER
        private SerializedProperty hideWireframe;
#endif
        private SerializedProperty hideMaterialInspector;
        private SerializedProperty hideMeshRenderer;
        private SerializedProperty realtimeEditing;

        private bool isReady = false;

        //Camera check
        private bool isOrtho;
        #endregion

        #region Third-party

        #endregion

        void OnEnable()
        {
            //Prevent OnInspectorGUI from running before these functions have been called
            isReady = false;

            selected = Selection.activeGameObject;

            if (!selected) return;

            if (!stylizedWater) stylizedWater = selected.GetComponent<StylizedWater>();

            serializedObject = new SerializedObject(stylizedWater);

            stylizedWater.Init();

            GetProperties();

            isWaterLayer.boolValue = (selected.gameObject.layer == 4) ? true : false;

            Undo.undoRedoPerformed += ApplyChanges;

            EditorUtility.SetDirty(target);

            if (SceneView.currentDrawingSceneView != null) isOrtho = SceneView.currentDrawingSceneView.orthographic;

            #region Foldouts
            m_showHelp = new AnimBool(true);
            m_showHelp.valueChanged.AddListener(Repaint);
            m_showHelp.speed = animSpeed;

            m_showColors = new AnimBool(true);
            m_showColors.valueChanged.AddListener(Repaint);
            m_showColors.speed = animSpeed;

            m_showLighting = new AnimBool(true);
            m_showLighting.valueChanged.AddListener(Repaint);
            m_showLighting.speed = animSpeed;

            m_showSurface = new AnimBool(true);
            m_showSurface.valueChanged.AddListener(Repaint);
            m_showSurface.speed = animSpeed;

            m_showNormals = new AnimBool(true);
            m_showNormals.valueChanged.AddListener(Repaint);
            m_showNormals.speed = animSpeed;

            m_showReflection = new AnimBool(true);
            m_showReflection.valueChanged.AddListener(Repaint);
            m_showReflection.speed = animSpeed;

            m_showIntersection = new AnimBool(true);
            m_showIntersection.valueChanged.AddListener(Repaint);
            m_showIntersection.speed = animSpeed;

            m_showFoam = new AnimBool(true);
            m_showFoam.valueChanged.AddListener(Repaint);
            m_showFoam.speed = animSpeed;

            m_showDepth = new AnimBool(true);
            m_showDepth.valueChanged.AddListener(Repaint);
            m_showDepth.speed = animSpeed;

            m_showWaves = new AnimBool(true);
            m_showWaves.valueChanged.AddListener(Repaint);
            m_showWaves.speed = animSpeed;

            m_showAdvanced = new AnimBool(true);
            m_showAdvanced.valueChanged.AddListener(Repaint);
            m_showAdvanced.speed = animSpeed;
            #endregion

            isReady = true;
        }

        public override void OnInspectorGUI()
        {
            if (!isReady) return;

            //Prevent null values when component is added
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            //if (stylizedWater.hasMaterial) Undo.RecordObject(stylizedWater.material, "Material");
            Undo.RecordObject(target, "Component");

            /*-------------------*/
            DrawAllFields();
            /*-------------------*/

            //Apply values
            if (EditorGUI.EndChangeCheck())
            {
                ApplyChanges();
            }

        }

        void ApplyChanges()
        {
            if (serializedObject.targetObject) serializedObject.ApplyModifiedProperties();
            stylizedWater.SetProperties();

            //Changes applied, grab new values
            GetProperties();

            //EditorUtility.SetDirty(serializedObject.targetObject);
        }

        //Get shader and Substance values
        void GetProperties()
        {
            //During runtime, nothing may be selected
            if (!selected) return;

            //Get all shader and Substance properties
            stylizedWater.GetProperties();

            hasShaderParams = serializedObject.FindProperty("hasShaderParams");
            //Inspector
#if !UNITY_5_5_OR_NEWER
            hideWireframe = serializedObject.FindProperty("hideWireframe");
#endif
            hideMaterialInspector = serializedObject.FindProperty("hideMaterialInspector");
            hideMeshRenderer = serializedObject.FindProperty("hideMeshRenderer");

            //Rendering
            //renderQueue = serializedObject.FindProperty("renderQueue");
            enableVertexColors = serializedObject.FindProperty("enableVertexColors");
            swsMaskRenderLayers = serializedObject.FindProperty("swsMaskRenderLayers");
            enableMaskRendering = serializedObject.FindProperty("enableMaskRendering");
            shadowCaster = serializedObject.FindProperty("shadowCaster");

            //Shader meta
            shader = stylizedWater.shader;
            shaderIndex = serializedObject.FindProperty("shaderIndex");

            useCompression = serializedObject.FindProperty("useCompression");
            isWaterLayer = serializedObject.FindProperty("isWaterLayer");
            realtimeEditing = serializedObject.FindProperty("realtimeEditing");

            shaderName = stylizedWater.shaderName;

            isMobileAdvanced = stylizedWater.isMobileAdvanced;
            isMobileBasic = stylizedWater.isMobileBasic;

            //Meta
            showLighting = serializedObject.FindProperty("showLighting");
            showColors = serializedObject.FindProperty("showColors");
            showSurface = serializedObject.FindProperty("showSurface");
            showNormals = serializedObject.FindProperty("showNormals");
            showIntersection = serializedObject.FindProperty("showIntersection");
            showFoam = serializedObject.FindProperty("showHighlights");
            showDepth = serializedObject.FindProperty("showDepth");
            showWaves = serializedObject.FindProperty("showWaves");
            showAdvanced = serializedObject.FindProperty("showAdvanced");

            //If the shader is not an SWS shader, abort
            if (!shaderName.Contains("StylizedWater")) return;

            //Lighting
            lightingMethod = serializedObject.FindProperty("lightingMethod");
            enableShadows = serializedObject.FindProperty("enableShadows");

            //Color
            enableGradient = serializedObject.FindProperty("enableGradient");
            colorGradient = serializedObject.FindProperty("colorGradient");

            //Reflection
            showReflection = serializedObject.FindProperty("showReflection");
            enableReflectionBlur = serializedObject.FindProperty("enableReflectionBlur");
            reflectionBlurLength = serializedObject.FindProperty("reflectionBlurLength");
            reflectionBlurPasses = serializedObject.FindProperty("reflectionBlurPasses");

            useReflection = serializedObject.FindProperty("useReflection");
            clipPlaneOffset = serializedObject.FindProperty("clipPlaneOffset");
            reflectionRes = serializedObject.FindProperty("reflectionRes");
            reflectLayers = serializedObject.FindProperty("reflectLayers");
            reflectionStrength = serializedObject.FindProperty("reflectionStrength");
            reflectionRefraction = serializedObject.FindProperty("reflectionRefraction");
            reflectionFresnel = serializedObject.FindProperty("reflectionFresnel");

            #region Shader parameters
            //Color
            waterColor = serializedObject.FindProperty("waterColor");
            waterShallowColor = serializedObject.FindProperty("waterShallowColor");
            depth = serializedObject.FindProperty("depth");
            fresnelColor = serializedObject.FindProperty("fresnelColor");
            fresnel = serializedObject.FindProperty("fresnel");
            rimColor = serializedObject.FindProperty("rimColor");
            waveTint = serializedObject.FindProperty("waveTint");

            //Surface
            worldSpaceTiling = serializedObject.FindProperty("worldSpaceTiling");
            transparency = serializedObject.FindProperty("transparency");
            edgeFade = serializedObject.FindProperty("edgeFade");
            glossiness = serializedObject.FindProperty("glossiness");
            //refractionSolver = serializedObject.FindProperty("refractionSolver");
            refractionAmount = serializedObject.FindProperty("refractionAmount");
            //refractionRes = serializedObject.FindProperty("refractRes");

            //Normals
            enableNormalMap = serializedObject.FindProperty("enableNormalMap");
            normalStrength = serializedObject.FindProperty("normalStrength");
            enableMacroNormals = serializedObject.FindProperty("enableMacroNormals");
            macroNormalsDistance = serializedObject.FindProperty("macroNormalsDistance");
            normalTiling = serializedObject.FindProperty("normalTiling");

            //Foam
            waveFoam = serializedObject.FindProperty("waveFoam");
            foamSolver = serializedObject.FindProperty("foamSolver");
            foamOpacity = serializedObject.FindProperty("foamOpacity");
            foamSize = serializedObject.FindProperty("foamSize");
            foamTiling = serializedObject.FindProperty("foamTiling");
            foamDistortion = serializedObject.FindProperty("foamDistortion");
            foamSpeed = serializedObject.FindProperty("foamSpeed");

            //Intersection
            intersectionSolver = serializedObject.FindProperty("intersectionSolver");
            rimSize = serializedObject.FindProperty("rimSize");
            rimFalloff = serializedObject.FindProperty("rimFalloff");
            rimTiling = serializedObject.FindProperty("rimTiling");
            rimDistortion = serializedObject.FindProperty("rimDistortion");

            //Waves
            waveSpeed = serializedObject.FindProperty("waveSpeed");
            waveStrength = serializedObject.FindProperty("waveStrength");
            waveSize = serializedObject.FindProperty("waveSize");
            waveDirectionXZ = serializedObject.FindProperty("waveDirectionXZ");
            waveDirectionX = waveDirectionXZ.vector4Value.x;
            waveDirectionZ = waveDirectionXZ.vector4Value.z;
            enableSecondaryWaves = serializedObject.FindProperty("enableSecondaryWaves");
            #endregion

            #region Texture parameters
            intersectionStyle = serializedObject.FindProperty("intersectionStyle");
            waveStyle = serializedObject.FindProperty("waveStyle");
            waveHeightmapStyle = serializedObject.FindProperty("waveHeightmapStyle");

            useCustomNormals = serializedObject.FindProperty("useCustomNormals");
            useCustomHeightmap = serializedObject.FindProperty("useCustomHeightmap");
            useCustomIntersection = serializedObject.FindProperty("useCustomIntersection");

            //Substance input textures
            customIntersection = serializedObject.FindProperty("customIntersection");
            customNormal = serializedObject.FindProperty("customNormal");
            customHeightmap = serializedObject.FindProperty("customHeightmap");

            #endregion

        }

        private void BakeShaderMap()
        {
            stylizedWater.GetShaderMap(useCompression.boolValue, useCustomIntersection.boolValue, useCustomHeightmap.boolValue);
        }

        private void BakeNormalMap()
        {
            stylizedWater.GetNormalMap(useCompression.boolValue, useCustomNormals.boolValue);
        }

        //Draw inspector fields
        void DrawAllFields()
        {
            DoHeader();

            //If not a valid shader
            if (!stylizedWater.hasMaterial || !shaderName.Contains("StylizedWater"))
            {
                EditorGUILayout.HelpBox("Please assign a material with a \"StylizedWater\" shader to this object.", MessageType.Error);
                return;
            }

            DoInfo();

            DoColors();
            DoLighting();
            DoSurface();
            if (!isMobileBasic) DoNormals();
            DoFoam();
            DoInterSection();
            DoWaves();
            //Desktop only
            if (!isMobileBasic && !isMobileAdvanced) DoReflection();
            DoAdvanced();

            DoFooter();
        }

        #region Fields
        void DoHeader()
        {
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Version " + StylizedWaterCore.INSTALLED_VERSION, new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                {
                    alignment = TextAnchor.MiddleLeft,
                    wordWrap = true,
                    fontSize = 12,
                });

                showHelp = GUILayout.Toggle(showHelp, "Toggle help", HelpButton);
                m_showHelp.target = showHelp;
            }
            EditorGUILayout.EndHorizontal();


            if (EditorGUILayout.BeginFadeGroup(m_showHelp.faded))
            {
                EditorGUILayout.Space();

                EditorGUILayout.HelpBox("\n\nThis GUI allows you to customize the water material and bake textures containing an intersection, wave and heightmap style per material\n\nBaked textures are saved in a \"/Textures\" folder next to the Material file.\n\nPress the buttons on the right to toggle parameter descriptions.\n\n", MessageType.Info);

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("<b><size=12>Open online documentation</size></b>\n<i>Usage instructions and troubleshooting</i>", RichTextButton))
                {
                    Application.OpenURL(StylizedWaterCore.DOC_URL);
                }
                if (GUILayout.Button("<b><size=12>Check for update</size></b>\n<i>Download new version</i>", RichTextButton))
                {
                    StylizedWaterCore.VersionChecking.GetLatestVersionPopup();
                }
                EditorGUILayout.EndHorizontal();

            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.Space();

        }

        void DoInfo()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox); //Box
            {

                EditorGUILayout.BeginHorizontal(); //Colums
                {
                    //Icon
                    GUILayout.Label(SWSIconContent, SWSIconStyle);

                    EditorGUILayout.BeginVertical(); //Right colum
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.PrefixLabel("Material");
                            GUILayout.Label(stylizedWater.material.name);
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUI.BeginDisabledGroup(stylizedWater.isMobilePlatform);
                        shaderIndex.intValue = EditorGUILayout.Popup("Shader", (int)shaderIndex.intValue, stylizedWater.shaderNames, new GUILayoutOption[0]);
                        EditorGUI.EndDisabledGroup();

                        if (shader != currentShader)
                        {
                            //Shader changed, current values of material are unknown
                            //Also executed OnEnable
                            hasShaderParams.boolValue = false;
                            currentShader = shader;
                            GetProperties();
                        }
                        //EditorGUILayout.PropertyField(renderQueue, new GUIContent("Render queue"));

                    }
                    EditorGUILayout.EndVertical(); //Right clum
                }
                EditorGUILayout.EndHorizontal(); //Colums
            }
            EditorGUILayout.EndVertical(); //Box

            if (stylizedWater.isMobilePlatform && shaderName.Contains("Desktop"))
            {
                EditorGUILayout.HelpBox("You are using a desktop shader on a mobile platform, which is not supported.\n\nThis will be automatically corrected.", MessageType.Error);
            }

            //StandaloneOSXIntel was removed in 2017.3, throws a warning on MacOS
#if !UNITY_2017_3_OR_NEWER
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSXIntel && shaderName.Contains("DX11"))
#else
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSX && shaderName.Contains("DX11"))
#endif
            {
                EditorGUILayout.HelpBox("The tesellation shader is not supported on macOS.\n\nSwitch back to the standard Desktop shader.", MessageType.Error);
            }

            if (isOrtho)
            {
                EditorGUILayout.HelpBox("When the scene view is set to orthographic, the water may not appear correct.\n\nIn the game view this is not the case", MessageType.Warning);
            }

            EditorGUILayout.Space();

        }

        void DoColors()
        {
            //Head
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button((showColors.boolValue) ? charFoldout : charCollapsed, CollapseButton))
            {
                SwitchSection(showColors);
            }
            if (GUILayout.Button("Colors", GroupFoldout))
            {
                SwitchSection(showColors);
            }
            showHelpColors = GUILayout.Toggle((showColors.boolValue) ? showHelpColors : false, HelpIcon, SectionHelpButtonStyle);
            m_showColors.target = showColors.boolValue;
            EditorGUILayout.EndHorizontal();

            //Body
            if (EditorGUILayout.BeginFadeGroup(m_showColors.faded))
            {
                EditorGUILayout.Space();

                //Desktop only
                if (!isMobileBasic && !isMobileAdvanced)
                {

                    EditorGUILayout.PropertyField(enableGradient, new GUIContent("Use gradient"));
                    if (showHelpColors) EditorGUILayout.HelpBox("Samples the water and shallow water colors from a gradient.\n\nAlpha controls transparency.", MessageType.Info);

                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();
                    if (enableGradient.boolValue)
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(colorGradient, new GUIContent("Color"));

                        if (EditorGUI.EndChangeCheck() && realtimeEditing.boolValue)
                        {
                            stylizedWater.GetGradientTex();
                        }

                        if (!realtimeEditing.boolValue)
                        {
                            if (GUILayout.Button("Apply"))
                            {
                                stylizedWater.GetGradientTex();
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                }
                else
                {
                    enableGradient.boolValue = false;
                }

                if (enableGradient.boolValue == false)
                {
                    EditorGUILayout.PropertyField(waterColor, new GUIContent("Deep"));

                    EditorGUILayout.PropertyField(waterShallowColor, new GUIContent("Shallow"));
                    if (showHelpColors) EditorGUILayout.HelpBox("The color where the water is shallow, alpha channel controls the opacity.\n\nThe Depth parameter has an influence on this effect.", MessageType.Info);
                }

                EditorGUILayout.PropertyField(depth, new GUIContent("Depth"));
                if (showHelpColors) EditorGUILayout.HelpBox("Sets the depth of the water.", MessageType.Info);

                //Desktop only
                if (!isMobileAdvanced && !isMobileBasic)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(fresnelColor, new GUIContent("Horizon"));
                    if (showHelpColors) EditorGUILayout.HelpBox("The color's alpha channel controls the intensity", MessageType.Info);
                    EditorGUILayout.PropertyField(fresnel, new GUIContent("Horizon distance"));
                    if (showHelpColors) EditorGUILayout.HelpBox("Controls the distance of the fresnel effect. Higher values push it further towards the horizon.", MessageType.Info);
                }

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(rimColor, new GUIContent("Intersection"));
                if (showHelpColors) EditorGUILayout.HelpBox("The color's alpha channel controls the intensity", MessageType.Info);

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(waveTint, new GUIContent("Wave tint"));
                if (showHelpColors) EditorGUILayout.HelpBox("Brightens or darkerns the water, based on the chosen heightmap in the Waves options section.", MessageType.Info);


            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.Space();
        }

        void DoLighting()
        {
            //Head
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button((showLighting.boolValue) ? charFoldout : charCollapsed, CollapseButton))
            {
                SwitchSection(showLighting);
            }
            if (GUILayout.Button("Lighting", GroupFoldout))
            {
                SwitchSection(showLighting);
            }
            showHelpLighting = GUILayout.Toggle((showLighting.boolValue) ? showHelpLighting : false, HelpIcon, SectionHelpButtonStyle);
            m_showLighting.target = showLighting.boolValue;
            EditorGUILayout.EndHorizontal();

            //Body
            if (EditorGUILayout.BeginFadeGroup(m_showLighting.faded))
            {
                EditorGUILayout.Space();

                lightingMethod.intValue = EditorGUILayout.Popup("Method", (int)lightingMethod.intValue, stylizedWater.lightingMethodNames, new GUILayoutOption[0]);

                switch (lightingMethod.intValue)
                {
                    case 0:
                        EditorGUILayout.HelpBox("Material receives no light, purely shows the water's color", MessageType.None);
                        break;
                    case 1:
                        EditorGUILayout.HelpBox("Material receives color from Directional Light and Ambient Color", MessageType.None);
                        break;
                    case 2:
                        EditorGUILayout.HelpBox("Material uses all Unity lighting features", MessageType.None);
                        break;
                }
                if (showHelpLighting) EditorGUILayout.HelpBox("Advanced lighting enables all lighting features such as GI, glossiness, point lights and skybox reflection. Turning this off can be a huge performance saver on low-end devices\n\nSimple lighting uses a custom lighting model with ambient light color and sun reflection.", MessageType.Info);

                if (!isMobileBasic && !isMobileAdvanced)
                {

                    EditorGUILayout.PropertyField(enableShadows, new GUIContent("Shadows"));
                    if (showHelpLighting) EditorGUILayout.HelpBox("Shows the shadows cast beneath the water surface, regardless of transparency.", MessageType.Info);


                    if (enableShadows.boolValue)
                    {
                        EditorGUILayout.PropertyField(shadowCaster, new GUIContent("Directonal Light"));

                        if (shadowCaster.objectReferenceValue == null)
                        {
                            EditorGUILayout.HelpBox("This field should not be empty, water will not render correctly otherwise", MessageType.Warning);
                        }
                    }

                }

                EditorGUILayout.Space();

            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.Space();
        }

        void DoSurface()
        {
            //Head
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button((showSurface.boolValue) ? charFoldout : charCollapsed, CollapseButton))
            {
                SwitchSection(showSurface);
            }
            if (GUILayout.Button("Surface", GroupFoldout))
            {
                SwitchSection(showSurface);
            }
            showHelpSurface = GUILayout.Toggle((showSurface.boolValue) ? showHelpSurface : false, HelpIcon, SectionHelpButtonStyle);
            m_showSurface.target = showSurface.boolValue;
            EditorGUILayout.EndHorizontal();

            //Body
            if (EditorGUILayout.BeginFadeGroup(m_showSurface.faded))
            {
                EditorGUILayout.Space();

                /*
                if (!isMobileAdvanced && !isMobileBasic)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(pixelate, new GUIContent("Pixelate"));
                    EditorGUILayout.LabelField("(Expiremental)");
                    EditorGUILayout.EndHorizontal();
                    if (showHelpSurface) EditorGUILayout.HelpBox("Pixelizes the water, this still has some visual issues...", MessageType.Info);
                }
                */

                worldSpaceTiling.floatValue = EditorGUILayout.Popup("Tiling method", (int)worldSpaceTiling.floatValue, stylizedWater.tilingMethodNames, new GUILayoutOption[0]);

                //EditorGUILayout.PropertyField(worldSpaceTiling, new GUIContent("World-space tiling"));
                if (showHelpSurface) EditorGUILayout.HelpBox("Rather than using the mesh's UV, you can opt to use world-space coordinates. Which are independent from the object's scale and position.", MessageType.Info);

                EditorGUILayout.PropertyField(transparency, new GUIContent("Transparency"));
                if (showHelpSurface) EditorGUILayout.HelpBox("Opacity is also read from the Green vertex color channel, and thus can be painted using third-party tools.", MessageType.Info);

                EditorGUILayout.PropertyField(glossiness, new GUIContent("Glossiness"));
                if (showHelpSurface) EditorGUILayout.HelpBox("Determine how glossy the material is. Higher values show more light reflection.", MessageType.Info);

                if (!isMobileBasic)
                {
                    EditorGUILayout.PropertyField(edgeFade, new GUIContent("Edge fade"));
                    if (showHelpIntersection) EditorGUILayout.HelpBox("Adds an inward offset to the effect from the intersecting object.", MessageType.Info);

                }

                //Desktop only
                if (!isMobileAdvanced && !isMobileBasic)
                {
                    /*
                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Refraction", EditorStyles.boldLabel);
                    refractionSolver.intValue = EditorGUILayout.Popup("Configuration", (int)refractionSolver.intValue, stylizedWater.refractionSolverNames, new GUILayoutOption[0]);
                    
                    if(refractionSolver.intValue == 1)
                    {
                        refractionRes.intValue = EditorGUILayout.Popup("Resolution", refractionRes.intValue, stylizedWater.resolutionNames, new GUILayoutOption[0]);
                    }
                    */
                    EditorGUILayout.PropertyField(refractionAmount, new GUIContent("Refraction"));
                    if (showHelpSurface) EditorGUILayout.HelpBox("Bends the light passing through the water surface, creating a visual distortion.", MessageType.Info);


                    // EditorGUILayout.PropertyField(tessellation, new GUIContent("Tessellation"));
                    //if (showHelpSurface) EditorGUILayout.HelpBox("Tessellation creates additional mesh vertices close to the camera, resulting in more detailed wave animations.", MessageType.Info);
                }

            }
            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.Space();
        }

        void DoNormals()
        {
            //Head
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button((showNormals.boolValue) ? charFoldout : charCollapsed, CollapseButton))
            {
                SwitchSection(showNormals);
            }
            if (GUILayout.Button("Normals", GroupFoldout))
            {
                SwitchSection(showNormals);
            }
            showHelpNormals = GUILayout.Toggle((showNormals.boolValue) ? showHelpNormals : false, HelpIcon, SectionHelpButtonStyle);
            m_showNormals.target = showNormals.boolValue;
            EditorGUILayout.EndHorizontal();

            //Body
            if (EditorGUILayout.BeginFadeGroup(m_showNormals.faded))
            {
                EditorGUILayout.Space();

                if (isMobileAdvanced)
                {
                    EditorGUILayout.PropertyField(enableNormalMap, new GUIContent("Enabled"));
                    if (showHelpNormals) EditorGUILayout.HelpBox("Disabling the normals sees a performance increase, but at the cost of the sun reflection", MessageType.Info);

                    EditorGUILayout.Space();
                }


                EditorGUI.BeginDisabledGroup(!enableNormalMap.boolValue);
                {

                    EditorGUILayout.BeginHorizontal();

                    if (realtimeEditing.boolValue)
                    {
                        EditorGUI.BeginChangeCheck();
                    }
                    waveStyle.intValue = EditorGUILayout.Popup("Style", (int)waveStyle.intValue, stylizedWater.waveStyleNames, new GUILayoutOption[0]);

                    if (realtimeEditing.boolValue)
                    {
                        if (EditorGUI.EndChangeCheck())
                        {
                            BakeShaderMap();
                            BakeNormalMap();
                        }
                    }

                    if (!realtimeEditing.boolValue)
                    {
                        EditorGUI.BeginDisabledGroup(useCustomNormals.boolValue && customNormal.objectReferenceValue == null);
                        if (GUILayout.Button("Apply", EditorStyles.miniButton))
                        {
                            BakeShaderMap();
                            BakeNormalMap();
                        }
                        EditorGUI.EndDisabledGroup();
                    }
                    EditorGUILayout.EndHorizontal();
                    if (showHelpNormals) EditorGUILayout.HelpBox("Choose from one of the built-in normal maps, or pick a custom one.", MessageType.Info);

                    useCustomNormals.boolValue = (waveStyle.intValue == stylizedWater.waveStyleNames.Length - 1) ? true : false;

                    if (useCustomNormals.boolValue)
                    {
                        EditorGUILayout.PropertyField(customNormal, new GUIContent("Normal map"));

                        if (customNormal.objectReferenceValue == null)
                        {
                            EditorGUILayout.HelpBox("Field cannot be empty", MessageType.Warning);
                        }
                    }

                    EditorGUILayout.Space();

                    EditorGUILayout.PropertyField(normalStrength, new GUIContent("Strength"));
                    if (showHelpNormals) EditorGUILayout.HelpBox("The normal are also used for the reflection highlight, so the normal strength also has an effect on it.", MessageType.Info);

                    if (!isMobileAdvanced && !isMobileBasic)
                    {
                        EditorGUILayout.PropertyField(enableMacroNormals, new GUIContent("Tiling reduction"));
                        if (showHelpNormals) EditorGUILayout.HelpBox("Blend in a additional normal map, 10x the tiling size at a far away distance.", MessageType.Info);

                        if (enableMacroNormals.boolValue)
                            EditorGUILayout.PropertyField(macroNormalsDistance, new GUIContent("Blend distance"));
                    }

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Tiling");
                    if (GUILayout.Button("<<", EditorStyles.miniButtonLeft))
                    {
                        normalTiling.floatValue -= .1f;
                    }
                    if (GUILayout.Button("<", EditorStyles.miniButtonMid))
                    {
                        normalTiling.floatValue -= .05f;
                    }
                    EditorGUILayout.PropertyField(normalTiling, new GUIContent(""), GUILayout.MaxWidth(45));
                    if (GUILayout.Button(">", EditorStyles.miniButtonMid))
                    {
                        normalTiling.floatValue += .05f;
                    }
                    if (GUILayout.Button(">>", EditorStyles.miniButtonRight))
                    {
                        normalTiling.floatValue += .1f;
                    }
                    EditorGUILayout.EndHorizontal();
                    if (normalTiling.floatValue == 0)
                    {
                        EditorGUILayout.HelpBox("Tiling value should not be 0", MessageType.Warning);
                    }
                    if (showHelpNormals) EditorGUILayout.HelpBox("Sets the size of the normals at which it repeats.", MessageType.Info);

                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.Space();
        }

        void DoFoam()
        {
            //Head
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button((showFoam.boolValue) ? charFoldout : charCollapsed, CollapseButton))
            {
                SwitchSection(showFoam);
            }
            if (GUILayout.Button("Foam", GroupFoldout))
            {
                SwitchSection(showFoam);
            }
            showHelpFoam = GUILayout.Toggle((showFoam.boolValue) ? showHelpFoam : false, HelpIcon, SectionHelpButtonStyle);
            m_showFoam.target = showFoam.boolValue;
            EditorGUILayout.EndHorizontal();

            //Body
            if (EditorGUILayout.BeginFadeGroup(m_showFoam.faded))
            {
                EditorGUILayout.Space();

                //Desktop and Mobile Advanced only
                if (!isMobileBasic)
                {

                    EditorGUI.BeginDisabledGroup(intersectionStyle.intValue == 0);

                    foamSolver.intValue = EditorGUILayout.Popup("Source", (int)foamSolver.intValue, stylizedWater.foamSolverNames, new GUILayoutOption[0]);

                    if (showHelpFoam) EditorGUILayout.HelpBox("Instead of sampling the highlight from the wave normal map, use the intersection texture.", MessageType.Info);
                    EditorGUI.EndDisabledGroup();
                    if (intersectionStyle.intValue == 0)
                    {
                        EditorGUILayout.HelpBox("Intersection style is set to \"None\", using normal map.", MessageType.Info);
                        foamSolver.intValue = 0;
                    }
                    if (useCustomNormals.boolValue)
                    {
                        EditorGUILayout.HelpBox("When using a custom normal map, the intersection texture must be used", MessageType.Info);
                        foamSolver.intValue = 1;
                    }

                }
                else
                {
                    foamSolver.intValue = 1;
                }

                EditorGUILayout.PropertyField(foamOpacity, new GUIContent("Opacity"));
                if (showHelpFoam) EditorGUILayout.HelpBox("Controls the intensity of the effect.", MessageType.Info);


                if (foamSolver.intValue == 0)
                {
                    EditorGUILayout.PropertyField(foamSize, new GUIContent("Size"));

                }

                //Desktop and Mobile Advanced only
                if (!isMobileBasic)
                {
                    EditorGUILayout.PropertyField(waveFoam, new GUIContent("Wave mask"));
                    if (showHelpFoam) EditorGUILayout.HelpBox("Shows the foam based on the heightmap", MessageType.Info);

                    //Desktop only
                    if (!isMobileAdvanced && !isMobileBasic)
                    {
                        EditorGUILayout.PropertyField(foamDistortion, new GUIContent("Distortion"));
                        if (showHelpFoam) EditorGUILayout.HelpBox("Distorts the foam based on the heightmap", MessageType.Info);
                    }


                    EditorGUILayout.PropertyField(foamSpeed, new GUIContent("Speed"));
                    if (showHelpFoam) EditorGUILayout.HelpBox("Move the foam in the wave direction", MessageType.Info);

                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Tiling");
                if (GUILayout.Button("<<", EditorStyles.miniButtonLeft))
                {
                    foamTiling.floatValue -= .1f;
                }
                if (GUILayout.Button("<", EditorStyles.miniButtonMid))
                {
                    foamTiling.floatValue -= .01f;
                }
                EditorGUILayout.PropertyField(foamTiling, new GUIContent(""), GUILayout.MaxWidth(45));
                if (GUILayout.Button(">", EditorStyles.miniButtonMid))
                {
                    foamTiling.floatValue += .01f;
                }
                if (GUILayout.Button(">>", EditorStyles.miniButtonRight))
                {
                    foamTiling.floatValue += .1f;
                }
                EditorGUILayout.EndHorizontal();
                if (foamTiling.floatValue == 0)
                {
                    EditorGUILayout.HelpBox("Tiling value should not be 0", MessageType.Warning);
                }
                if (showHelpFoam) EditorGUILayout.HelpBox("Controls the tiling size of the foam texture.", MessageType.Info);
            }

            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.Space();
        }

        void DoInterSection()
        {
            //Head
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button((showIntersection.boolValue) ? charFoldout : charCollapsed, CollapseButton))
            {
                SwitchSection(showIntersection);
            }
            if (GUILayout.Button("Intersection", GroupFoldout))
            {
                SwitchSection(showIntersection);
            }
            showHelpIntersection = GUILayout.Toggle((showIntersection.boolValue) ? showHelpIntersection : false, HelpIcon, SectionHelpButtonStyle);
            m_showIntersection.target = showIntersection.boolValue;
            EditorGUILayout.EndHorizontal();

            //Body
            if (EditorGUILayout.BeginFadeGroup(m_showIntersection.faded))
            {
                EditorGUILayout.Space();

                //if (showHelpIntersection) EditorGUILayout.HelpBox("If the intersection effect doesn't appear, be sure to add the \"EnableDepthBuffer\" script to your camera.", MessageType.Warning);

                EditorGUILayout.BeginHorizontal();
                var m_intersectionStyle = intersectionStyle.intValue;

                intersectionStyle.intValue = EditorGUILayout.Popup("Style", (int)intersectionStyle.intValue, stylizedWater.intersectionStyleNames, new GUILayoutOption[0]);
                if (realtimeEditing.boolValue && m_intersectionStyle != intersectionStyle.intValue)
                {
                    BakeShaderMap();
                }

                if (!realtimeEditing.boolValue)
                {
                    EditorGUI.BeginDisabledGroup(useCustomIntersection.boolValue && customIntersection.objectReferenceValue == null);
                    if (GUILayout.Button("Apply", EditorStyles.miniButton))
                    {
                        BakeShaderMap();
                    }
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndHorizontal();
                if (showHelpIntersection) EditorGUILayout.HelpBox("Pick one of the built-in intersection texture styles.", MessageType.Info);

                useCustomIntersection.boolValue = (intersectionStyle.intValue == stylizedWater.intersectionStyleNames.Length - 1) ? true : false;

                if (useCustomIntersection.boolValue)
                {
                    EditorGUILayout.PropertyField(customIntersection, new GUIContent("Grayscale texture"));
                    if (customIntersection.objectReferenceValue == null)
                    {
                        EditorGUILayout.HelpBox("Texture field may not be empty", MessageType.Warning);
                    }
                    if (showHelpIntersection) EditorGUILayout.HelpBox("Choose a black/white texture, it will automatically be packed into the \"shadermap\" texture channel.", MessageType.Info);

                }

                EditorGUILayout.Space();

                intersectionSolver.intValue = EditorGUILayout.Popup("Solver", intersectionSolver.intValue, stylizedWater.intersectionSolverNames, new GUILayoutOption[0]);
                if (showHelpIntersection) EditorGUILayout.HelpBox("Using depth differences or sample the mesh's red vertex color channel.\n\nThis can be applied using third-party vertex painting tools.", MessageType.Info);

                if (intersectionSolver.intValue == 1 && enableVertexColors.boolValue == false)
                {
                    EditorGUILayout.HelpBox("The \"Enable vertex colors\" option must be enabled in the Advanced tab", MessageType.Warning);
                }

                EditorGUILayout.PropertyField(rimSize, new GUIContent("Size"));
                if (showHelpIntersection) EditorGUILayout.HelpBox("Increase the size of the intersection effect.", MessageType.Info);

                EditorGUILayout.PropertyField(rimFalloff, new GUIContent("Falloff"));
                if (showHelpIntersection) EditorGUILayout.HelpBox("Controls how strongly the effect should taper off.", MessageType.Info);

                //Desktop
                if (!isMobileAdvanced && !isMobileBasic)
                {
                    EditorGUILayout.PropertyField(rimDistortion, new GUIContent("Distortion"));
                    if (showHelpIntersection) EditorGUILayout.HelpBox("Distorts the secondary intersection layer by the heightmap", MessageType.Info);
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Tiling");
                if (GUILayout.Button("<<", EditorStyles.miniButtonLeft))
                {
                    rimTiling.floatValue -= .1f;
                }
                if (GUILayout.Button("<", EditorStyles.miniButtonMid))
                {
                    rimTiling.floatValue -= .01f;
                }
                EditorGUILayout.PropertyField(rimTiling, new GUIContent(""), GUILayout.MaxWidth(45));
                if (GUILayout.Button(">", EditorStyles.miniButtonMid))
                {
                    rimTiling.floatValue += .01f;
                }
                if (GUILayout.Button(">>", EditorStyles.miniButtonRight))
                {
                    rimTiling.floatValue += .1f;
                }
                EditorGUILayout.EndHorizontal();
                if (rimTiling.floatValue == 0)
                {
                    EditorGUILayout.HelpBox("Tiling value should not be 0", MessageType.Warning);
                }
                if (showHelpIntersection) EditorGUILayout.HelpBox("Controls the tiling size of the intersection texture.\n\nThis is also affected by the \"Tiling\" value in the Surface options section.", MessageType.Info);

            }

            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.Space();
        }

        void DoReflection()
        {
            //Head
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button((showReflection.boolValue) ? charFoldout : charCollapsed, CollapseButton))
            {
                SwitchSection(showReflection);
            }
            if (GUILayout.Button("Planar reflections", GroupFoldout))
            {
                SwitchSection(showReflection);
            }
            showHelpReflection = GUILayout.Toggle((showReflection.boolValue) ? showHelpReflection : false, HelpIcon, SectionHelpButtonStyle);
            m_showReflection.target = showReflection.boolValue;
            EditorGUILayout.EndHorizontal();

            //Body
            if (EditorGUILayout.BeginFadeGroup(m_showReflection.faded))
            {
                EditorGUILayout.Space();

                if (stylizedWater.usingSinglePassRendering)
                {
                    EditorGUILayout.HelpBox("Reflections are not supported in Single-Pass Stereo Rendering", MessageType.Error);
                    EditorGUILayout.Space();
                }
                else
                {

                    if (showHelpReflection) EditorGUILayout.HelpBox("This feature creates a secondary camera, to render a reflection.\n\nThis can have a huge performance impact, be sure to make use of the layer culling option.", MessageType.Warning);

                    useReflection.boolValue = EditorGUILayout.Toggle("Enable", useReflection.boolValue);

                    if (useReflection.boolValue)
                    {
                        EditorGUILayout.Space();

                        EditorGUILayout.PropertyField(reflectLayers);
                        if (showHelpReflection) EditorGUILayout.HelpBox("Choose which layers should be reflected. The \"Water\" layer is always ignored.", MessageType.Info);

                        reflectionRes.intValue = EditorGUILayout.Popup("Resolution", reflectionRes.intValue, stylizedWater.resolutionNames, new GUILayoutOption[0]);

                        EditorGUILayout.PropertyField(reflectionStrength, new GUIContent("Strength"));
                        if (showHelpReflection) EditorGUILayout.HelpBox("The amount of reflection shown", MessageType.Info);
                        EditorGUILayout.PropertyField(reflectionRefraction, new GUIContent("Distortion"));
                        if (showHelpReflection) EditorGUILayout.HelpBox("Bends the reflected image through the waves", MessageType.Info);
                        EditorGUILayout.PropertyField(reflectionFresnel, new GUIContent("Fresnel"));
                        if (showHelpReflection) EditorGUILayout.HelpBox("Determine how much reflection should be shown at glazing angles", MessageType.Info);
                        EditorGUILayout.PropertyField(clipPlaneOffset, new GUIContent("Offset"));
                        if (showHelpReflection) EditorGUILayout.HelpBox("Normally the reflection is clipped at the water plane, but you can add an offset in case a seam shows", MessageType.Info);

                        EditorGUILayout.Space();

                        EditorGUILayout.PropertyField(enableReflectionBlur);
                        if (showHelpReflection) EditorGUILayout.HelpBox("Blur the reflection in a vertical manner. It is recommended to lower the reflection resolution for the best result.", MessageType.Info);

                        if (enableReflectionBlur.boolValue)
                        {
                            EditorGUILayout.PropertyField(reflectionBlurLength, new GUIContent("Length"));
                            EditorGUILayout.PropertyField(reflectionBlurPasses, new GUIContent("Iterations"));
                            if (showHelpReflection) EditorGUILayout.HelpBox("The amount of time the blurred image is blurred again. A higher iteration count results in a better image, but at a higher rendering cost.", MessageType.Info);
                        }

                    }
                }//VR
            }

            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.Space();
        }

        void DoWaves()
        {
            //Head
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button((showWaves.boolValue) ? charFoldout : charCollapsed, CollapseButton))
            {
                SwitchSection(showWaves);
            }
            if (GUILayout.Button("Waves", GroupFoldout))
            {
                SwitchSection(showWaves);
            }
            showHelpWaves = GUILayout.Toggle((showWaves.boolValue) ? showHelpWaves : false, HelpIcon, SectionHelpButtonStyle);
            m_showWaves.target = showWaves.boolValue;

            EditorGUILayout.EndHorizontal();

            //Body
            if (EditorGUILayout.BeginFadeGroup(m_showWaves.faded))
            {
                EditorGUILayout.Space();

                if (realtimeEditing.boolValue)
                {
                    EditorGUI.BeginChangeCheck();
                }

                EditorGUILayout.BeginHorizontal();

                waveHeightmapStyle.intValue = EditorGUILayout.Popup("Heightmap style", (int)waveHeightmapStyle.intValue, stylizedWater.waveHeightmapNames, new GUILayoutOption[0]);

                if (realtimeEditing.boolValue)
                {
                    if (EditorGUI.EndChangeCheck()) BakeShaderMap();
                }
                else
                {
                    EditorGUI.BeginDisabledGroup(useCustomHeightmap.boolValue && customHeightmap.objectReferenceValue == null);
                    if (GUILayout.Button("Apply", EditorStyles.miniButton))
                    {
                        BakeShaderMap();
                    }
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndHorizontal();
                if (showHelpWaves) EditorGUILayout.HelpBox("Choose from one of the built-in height maps.", MessageType.Info);

                useCustomHeightmap.boolValue = (waveHeightmapStyle.intValue == stylizedWater.waveHeightmapNames.Length - 1) ? true : false;

                if (useCustomHeightmap.boolValue)
                {
                    EditorGUILayout.PropertyField(customHeightmap, new GUIContent("Heightmap"));
                    if (customHeightmap.objectReferenceValue == null)
                    {
                        EditorGUILayout.HelpBox("Texture field may not be empty", MessageType.Warning);
                    }
                }

                //Desktop only
                if (!isMobileAdvanced && !isMobileBasic)
                {
                    EditorGUILayout.PropertyField(enableSecondaryWaves, new GUIContent("Additional layer"));
                }

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(waveSpeed, new GUIContent("Speed"));
                if (showHelpWaves) EditorGUILayout.HelpBox("Overal speed of the wave animations.", MessageType.Info);


                waveDirectionX = EditorGUILayout.Slider("Direction X", waveDirectionX, -1f, 1f);
                waveDirectionZ = EditorGUILayout.Slider("Direction Z", waveDirectionZ, -1f, 1f);

                waveDirectionXZ.vector4Value = new Vector4(waveDirectionX, 0f, waveDirectionZ, 0f);

                EditorGUILayout.PropertyField(waveStrength, new GUIContent("Height"));
                if (showHelpWaves) EditorGUILayout.HelpBox("height of the waves, this can also be controlled through the Y-axis scale of the object, at least for planes.", MessageType.Info);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Tiling");
                if (GUILayout.Button("<<", EditorStyles.miniButtonLeft))
                {
                    waveSize.floatValue -= .1f;
                }
                if (GUILayout.Button("<", EditorStyles.miniButtonMid))
                {
                    waveSize.floatValue -= .01f;
                }
                EditorGUILayout.PropertyField(waveSize, new GUIContent(""), GUILayout.MaxWidth(45));
                if (GUILayout.Button(">", EditorStyles.miniButtonMid))
                {
                    waveSize.floatValue += .01f;
                }
                if (GUILayout.Button(">>", EditorStyles.miniButtonRight))
                {
                    waveSize.floatValue += .1f;
                }
                EditorGUILayout.EndHorizontal();
                if (waveSize.floatValue == 0)
                {
                    EditorGUILayout.HelpBox("Tiling value should not be 0", MessageType.Warning);
                }
                if (showHelpWaves) EditorGUILayout.HelpBox("Controls the tiling size of the wave animation.\n\nThis is also affected by the \"Tiling\" value in the Surface options section.", MessageType.Info);

            }

            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.Space();
        }

        void DoAdvanced()
        {
            //Head
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button((showAdvanced.boolValue) ? charFoldout : charCollapsed, CollapseButton))
            {
                SwitchSection(showAdvanced);
            }
            if (GUILayout.Button("Advanced", GroupFoldout))
            {
                SwitchSection(showAdvanced);
            }
            showHelpAdvanced = GUILayout.Toggle((showAdvanced.boolValue) ? showHelpAdvanced : false, HelpIcon, SectionHelpButtonStyle);
            m_showAdvanced.target = showAdvanced.boolValue;
            EditorGUILayout.EndHorizontal();

            //Body
            if (EditorGUILayout.BeginFadeGroup(m_showAdvanced.faded))
            {
                EditorGUILayout.Space();
                //Disabled until fixed
                //EditorGUILayout.PropertyField(realtimeEditing, new GUIContent("Real-time changes"));
                //if (showHelpAdvanced) EditorGUILayout.HelpBox("Bakes new textures when related values are changed. This is a tad slower", MessageType.Info);

                EditorGUILayout.LabelField("Masking", EditorStyles.boldLabel);

                if (!isMobileBasic && !isMobileAdvanced)
                {
                    EditorGUI.BeginDisabledGroup(!enableMaskRendering.boolValue);
                    EditorGUILayout.PropertyField(enableMaskRendering, new GUIContent("Enable renderer (work in progress)"));
                    EditorGUI.EndDisabledGroup();
                    if (showHelpAdvanced) EditorGUILayout.HelpBox("When enabled, you'll be able to render certain objects to an intersection or opacity mask.\n\nIt's important to view the documentation about the set up.", MessageType.Info);
                    if (enableMaskRendering.boolValue)
                    {
                        EditorGUILayout.PropertyField(swsMaskRenderLayers, new GUIContent("Render layer"));
                        if (showHelpAdvanced) EditorGUILayout.HelpBox("Objects on these layers will be rendered into the mask buffer.", MessageType.Info);
                    }
                }

                EditorGUILayout.PropertyField(enableVertexColors, new GUIContent("Enable vertex colors"));
                if (showHelpAdvanced) EditorGUILayout.HelpBox("When enabled, vertex colors will be sampled for masking:\n\nRed: Intersection\nGreen: Opacity", MessageType.Info);


                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Hide", EditorStyles.boldLabel);
#if !UNITY_5_5_OR_NEWER
                EditorGUILayout.PropertyField(hideWireframe, new GUIContent("Wireframe"));
                if (showHelpAdvanced) EditorGUILayout.HelpBox("Hides the wireframe of the selected object, for easier visual tuning.", MessageType.Info);
#endif
                EditorGUILayout.PropertyField(hideMaterialInspector, new GUIContent("Material inspector"));
                if (showHelpAdvanced) EditorGUILayout.HelpBox("Hides material inspector, for less clutter", MessageType.Info);

                EditorGUILayout.PropertyField(hideMeshRenderer, new GUIContent("Mesh Renderer"));
                if (showHelpAdvanced) EditorGUILayout.HelpBox("Hides material inspector, for less clutter", MessageType.Info);

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Textures", EditorStyles.boldLabel);
                var m_useCompression = useCompression.boolValue;
                EditorGUILayout.PropertyField(useCompression, new GUIContent("Compress"));
                if (showHelpAdvanced) EditorGUILayout.HelpBox("Bake the textures using compression, trading in quality for a smaller file size.\n\nPVRTC compression is used for mobile, DXT5 is used on other platforms.", MessageType.Info);

#if VEGETATION_STUDIO
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Vegetation Studio", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PrefixLabel(new GUIContent("Sea level"));
                    if (GUILayout.Button("Set"))
                    {
                        stylizedWater.SetVegetationStudioWaterLevel();
                    }
                }
                EditorGUILayout.EndHorizontal();
                if (showHelpAdvanced) EditorGUILayout.HelpBox("Use this object's Y-position to set the global water level for all Vegetation Systems", MessageType.Info);
#endif

                if (m_useCompression != useCompression.boolValue)
                {
                    BakeNormalMap();
                    BakeShaderMap();
                }

                EditorGUILayout.Space();

                if (realtimeEditing.boolValue == false)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Apply all settings", GUILayout.MaxHeight(35)))
                    {
                        BakeShaderMap();
                        BakeNormalMap();
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndFadeGroup();
        }

        void DoFooter()
        {
            EditorGUILayout.Space();

            GUILayout.Label("- Staggart Creations -", new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                fontSize = 12
            });

            if (StylizedWaterCore.IsUpToDate == false)
            {
                Color defaultColor = GUI.contentColor;
                GUI.contentColor = Color.green;
                if (GUILayout.Button(new GUIContent("Update package"), UpdateText))
                {
                    StylizedWaterCore.OpenStorePage();
                }
                GUI.contentColor = defaultColor;
            }

            EditorGUILayout.Space();

        }
        #endregion

        private void SwitchSection(SerializedProperty showSectionValue)
        {
            showLighting.boolValue = (showSectionValue == showLighting) ? !showLighting.boolValue : false;
            showColors.boolValue = (showSectionValue == showColors) ? !showColors.boolValue : false;
            showSurface.boolValue = (showSectionValue == showSurface) ? !showSurface.boolValue : false;
            showNormals.boolValue = (showSectionValue == showNormals) ? !showNormals.boolValue : false;
            showReflection.boolValue = (showSectionValue == showReflection) ? !showReflection.boolValue : false;
            showIntersection.boolValue = (showSectionValue == showIntersection) ? !showIntersection.boolValue : false;
            showFoam.boolValue = (showSectionValue == showFoam) ? !showFoam.boolValue : false;
            showDepth.boolValue = (showSectionValue == showDepth) ? !showDepth.boolValue : false;
            showWaves.boolValue = (showSectionValue == showWaves) ? !showWaves.boolValue : false;
            showAdvanced.boolValue = (showSectionValue == showAdvanced) ? !showAdvanced.boolValue : false;
        }

        #region Styles
        private static string charFoldout = "−";
        private static string charCollapsed = "≡";
        private static GUIStyle _ParameterGroup;
        public static GUIStyle ParameterGroup
        {
            get
            {
                if (_ParameterGroup == null)
                {
                    _ParameterGroup = new GUIStyle(EditorStyles.helpBox)
                    {

                    };
                }

                return _ParameterGroup;
            }
        }

        private static GUIStyle _RichTextButton;
        public static GUIStyle RichTextButton
        {
            get
            {
                if (_RichTextButton == null)
                {
                    _RichTextButton = new GUIStyle(GUI.skin.button)
                    {
                        alignment = TextAnchor.MiddleLeft,
                        stretchWidth = true,
                        richText = true,
                        wordWrap = true,
                        padding = new RectOffset()
                        {
                            left = 14,
                            right = 14,
                            top = 8,
                            bottom = 8
                        }
                    };
                }

                return _RichTextButton;
            }
        }

        private static GUIContent _SWSIconContent;
        public static GUIContent SWSIconContent
        {
            get
            {
                if (_SWSIconContent == null)
                {
                    _SWSIconContent = new GUIContent()
                    {
                        image = Resources.Load("sws_icon") as Texture,

                    };
                }

                return _SWSIconContent;
            }
        }

        private static GUIStyle _SWSIconStyle;
        public static GUIStyle SWSIconStyle
        {
            get
            {
                if (_SWSIconStyle == null)
                {
                    _SWSIconStyle = new GUIStyle()
                    {
                        fixedHeight = 40f,
                        fixedWidth = 40f,
                        margin = new RectOffset()
                        {
                            right = 10
                        }
                    };
                }

                return _SWSIconStyle;
            }
        }

        private static GUIStyle _HelpButton;
        public static GUIStyle HelpButton
        {
            get
            {
                if (_HelpButton == null)
                {
                    _HelpButton = new GUIStyle(GUI.skin.button)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        fixedWidth = 120f,
                        richText = true,
                        wordWrap = true
                    };
                }

                return _HelpButton;
            }
        }

        private static GUIStyle _SectionHelpButtonStyle;
        public static GUIStyle SectionHelpButtonStyle
        {
            get
            {
                if (_SectionHelpButtonStyle == null)
                {
                    _SectionHelpButtonStyle = new GUIStyle(EditorStyles.miniButtonRight)
                    {
                        fontSize = 12,
                        fontStyle = FontStyle.Normal,
                        fixedWidth = 50,
                        fixedHeight = 21.5f
                    };
                }

                return _SectionHelpButtonStyle;
            }
        }

        private static Texture _HelpIcon;
        public static Texture HelpIcon
        {
            get
            {
                if (_HelpIcon == null)
                {
                    _HelpIcon = EditorGUIUtility.FindTexture("d_UnityEditor.InspectorWindow");
                }

                return _HelpIcon;
            }
        }

        private static GUIContent _HelpButtonContent;
        public static GUIContent HelpButtonContent
        {
            get
            {
                if (_HelpButtonContent == null)
                {
                    _HelpButtonContent = new GUIContent()
                    {
                        image = HelpIcon,
                    };
                }

                return _HelpButtonContent;
            }
        }

        private static GUIStyle _CollapseButton;
        public static GUIStyle CollapseButton
        {
            get
            {
                if (_CollapseButton == null)
                {
                    _CollapseButton = new GUIStyle(EditorStyles.miniButtonLeft)
                    {
                        fontSize = 16,
                        fontStyle = FontStyle.Normal,
                        fixedWidth = 30,
                        fixedHeight = 21.5f,
                    };
                }

                return _CollapseButton;
            }
        }

        private static GUIStyle _GroupFoldout;
        public static GUIStyle GroupFoldout
        {
            get
            {
                if (_GroupFoldout == null)
                {
                    _GroupFoldout = new GUIStyle(EditorStyles.miniButtonMid)
                    {
                        fontSize = 11,
                        alignment = TextAnchor.MiddleLeft,
                        stretchWidth = true,
                        padding = new RectOffset()
                        {
                            left = 10,
                            top = 4,
                            bottom = 5
                        }
                    };
                }

                return _GroupFoldout;
            }
        }

        private static GUIStyle _UpdateText;
        public static GUIStyle UpdateText
        {
            get
            {
                if (_UpdateText == null)
                {
                    _UpdateText = new GUIStyle("Button")
                    {
                        //fontSize = 10,
                        alignment = TextAnchor.MiddleCenter,
                        stretchWidth = true,
                    };
                }

                return _UpdateText;
            }
        }


        #endregion

    }//Class
}//Namespace
