// Stylized Water Shader by Staggart Creations http://u3d.as/A2R
// Online documentation can be found at http://staggart.xyz

using UnityEngine;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StylizedWater
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshRenderer))]
    [HelpURL("http://staggart.xyz/unity/stylized-water-shader/documentation/")]
    public class StylizedWater : MonoBehaviour
    {

        #region Shader data
        public string[] shaderNames;
        public int shaderIndex = 0;

        public Shader shader;
        private Shader DesktopShader;
        private Shader MobileAdvancedShader;
        // private Shader MobileBasicShader;
        #endregion

        #region Shader properties
        [Range(2000, 4000)]
        public int renderQueue;
        public bool enableVertexColors;

        //Color
        public bool isUnlit;
        public bool enableGradient;
        public Gradient colorGradient;
        public Color waterShallowColor;
        [Range(0f, 100f)]
        public float depth;
        public Color waterColor;
        public Color fresnelColor;
        public float fresnel;
        public Color rimColor;
        [Range(-0.5f, 0.5f)]
        public float waveTint;

        //Surface
        public bool pixelate;
        [Range(0f, 1f)]
        public float transparency;
        [Range(0.01f, 1f)]
        public float glossiness;
        [Range(0f, 3f)]
        public float edgeFade;
        public string[] tilingMethodNames;
        public float worldSpaceTiling;
        [Range(0f, 0.2f)]
        public float refractionAmount;

        //Normals
        public bool enableNormalMap = true;
        [Range(0f, 1f)]
        public float normalStrength;
        public bool enableMacroNormals;
        [Range(250f, 3000f)]
        public float macroNormalsDistance = 1000f;
        [Range(0f, 1f)]
        public float normalTiling;

        //Intersection
        public int intersectionSolver;
        public string[] intersectionSolverNames;
        [Range(0f, 30f)]
        public float rimSize;
        [Range(0.1f, 30f)]
        public float rimFalloff;
        public float rimTiling;
        [Range(0f, 1f)]
        public float rimDistortion;
        public bool enableVCIntersection;

        //Foam
        public int foamSolver;
        public string[] foamSolverNames;
        [Range(-1, 1)]
        public float foamOpacity;
        public float foamTiling;
        [Range(0f, 1f)]
        public float foamSize;
        [Range(0f, 3f)]
        public float foamDistortion;
        [Range(0f, 1f)]
        public float foamSpeed;
        [Range(0f, 1f)]
        public float waveFoam;

        //Reflection
        [Range(0f, 1f)]
        public float reflectionStrength = 1f;
        [Range(2f, 10f)]
        public float reflectionFresnel = 10f;
        public bool showReflection;
        [Range(0f, 0.2f)]
        public float reflectionRefraction;

        //Waves
        [Range(0.01f, 10f)]
        public float waveSpeed;
        [Range(0f, 1f)]
        public float waveStrength;
        [Range(-1f, 1f)]
        public Vector4 waveDirectionXZ;
        public bool enableSecondaryWaves;

        public Texture2D customIntersection;
        public Texture2D customNormal;
        public Texture2D customHeightmap;

        [Range(0.01f, 20f)]
        public float tessellation;
        #endregion

        #region Texture variables
        public string[] intersectionStyleNames;
        public int intersectionStyle = 1;

        public string[] waveStyleNames;
        public int waveStyle;

        public string[] waveHeightmapNames;
        public int waveHeightmapStyle;
        public float waveSize;

        public bool useCustomIntersection;
        public bool useCustomNormals;
        public bool useCustomHeightmap;

        //Textures
        public Texture2D normals;
        public Texture2D shadermap;
        public Texture2D colorGradientTex;

        public bool useCompression = false;
        #endregion

        #region Reflection/Refraction
        Camera reflectionCamera, refractCamera;
        public bool useReflection, useRefractionCam;
        //private float m_ReflectionStrength;
        public bool enableReflectionBlur;
        [Range(1f, 15f)]
        public float reflectionBlurLength = 4f;
        [Range(1, 4)]
        public int reflectionBlurPasses = 4;
        private StylizedWaterBlur reflectionBlurRenderer;

        public string[] refractionSolverNames;
        public int refractionSolver = 0;
        public string[] resolutionNames;
        public int reflectionRes = 1; //512
        public int refractRes = 2; //1024

        public int reflectionTextureSize = 512;
        public int refractTextureSize = 1024;

        [Range(1f, 10f)]
        public float clipPlaneOffset = 1f;
        public LayerMask reflectLayers = -1;
        public LayerMask refractLayers = -1;

        private Dictionary<Camera, Camera> m_ReflectionCameras = new Dictionary<Camera, Camera>();
        private Dictionary<Camera, StylizedWaterBlur> m_BlurRenderers = new Dictionary<Camera, StylizedWaterBlur>();
        private Dictionary<Camera, Camera> m_RefractCameras = new Dictionary<Camera, Camera>();

        private RenderTexture m_ReflectionTexture;
        private RenderTexture m_RefractTexture;

        private int m_OldReflectionTextureSize;
        private int m_OldRefractTextureSize;

        private bool s_InsideRendering;
        #endregion

        #region Mask rendering
        public bool enableMaskRendering = false;
        StylizedWaterRenderer swsRenderer;
        StylizedWaterRenderer.RendererSettings swsRenderSettings;
        public int swsRendererTextureSize = 1024;
        public LayerMask swsMaskRenderLayers;
        public static LayerMask s_swsMaskRenderLayers;

        public Camera renderCam;
        public Vector4 targetMeshInfo; //Size and center position
        #endregion

        #region Lighting
        public int lightingMethod = 2;
        public string[] lightingMethodNames;
        #endregion

        #region Shadows
        public bool enableShadows;
        StylizedWaterShadowCaster shadowRenderer;
        public Light shadowCaster;
        #endregion

        #region Script vars
        [NonSerialized]
        private MeshRenderer meshRenderer;
#if UNITY_EDITOR
        [NonSerialized]
        private MeshFilter meshFilter;
#endif
        public Material material;
        public bool isMobileAdvanced;
        public bool isMobilePlatform;
        public bool isMobileBasic = false;
        public string shaderName = null;
        public bool isWaterLayer;
        public bool hasShaderParams = false;
        public bool hasMaterial;
        public bool usingSinglePassRendering;
        #endregion

        #region Toggles
        [Header("Toggles")]
        public bool showLighting;
        public bool showColors = true;
        public bool showSurface;
        public bool showNormals;
        public bool showIntersection;
        public bool showHighlights;
        public bool showDepth;
        public bool showWaves;
        public bool showAdvanced;
#if !UNITY_5_5_OR_NEWER
        public bool hideWireframe = true;
#endif
        public bool hideMaterialInspector = true;
        public bool hideMeshRenderer = true;
        public bool realtimeEditing = false;
        #endregion

        #region Third-party

        #endregion

        /// FUNCTIONS START ///

        public void OnEnable()
        {
            if (!meshRenderer) meshRenderer = this.GetComponent<MeshRenderer>();
#if UNITY_EDITOR
            if (!meshFilter)
            {
                meshFilter = this.GetComponent<MeshFilter>();
            }
            else
            {
                //enableVertexColors = StylizedWaterUtilities.HasVertexColors(meshFilter.sharedMesh);
            }

#endif//editor

            GetProperties();
            SetProperties();

        }

        //Called through inspector OnEnable, editor-only
        public void Init()
        {
#if UNITY_EDITOR
            #region Dropdowns
            //Avoids a null ref on start up, instance will be created some frames later(?)
            if (StylizedWaterResources.Instance != null)
            {
                intersectionStyleNames = StylizedWaterUtilities.ComposeDropdown(StylizedWaterResources.Instance.intersectionStyles, "SWS_Intersection_");

                waveStyleNames = StylizedWaterUtilities.ComposeDropdown(StylizedWaterResources.Instance.waveStyles, "SWS_Waves_");

                waveHeightmapNames = StylizedWaterUtilities.ComposeDropdown(StylizedWaterResources.Instance.heightmapStyles, "SWS_Heightmap_");
            }

            tilingMethodNames = new string[] { "Mesh UV", "World-aligned (planar)" };
            intersectionSolverNames = new string[] { "Depth-based", "Vertex color (red)" };
            foamSolverNames = new string[] { "Normal map", "Intersection texture" };
            lightingMethodNames = new string[] { "Unlit", "Basic", "Advanced" };
            refractionSolverNames = new string[] { "Performance", "Appearance" };
            resolutionNames = new string[] { "Low", "Medium", "High" };
            #endregion

            //Create initial gradient
            if (colorGradient == null) colorGradient = new Gradient();
            if (colorGradient.Evaluate(0f) == Color.clear && colorGradient.Evaluate(1f) == Color.clear)
            {
                //Debug.Log("Creating initial gradient");

                colorGradient = new Gradient();
                GradientColorKey[] colorKeys = new GradientColorKey[2];
                colorKeys[0].time = 0f;
                colorKeys[0].color = waterShallowColor;
                colorKeys[1].time = 1f;
                colorKeys[0].color = waterColor;

                GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
                alphaKeys[0].time = 0f;
                alphaKeys[0].alpha = waterShallowColor.a;
                alphaKeys[1].time = 1f;
                alphaKeys[1].alpha = waterColor.a;

                colorGradient.SetKeys(colorKeys, alphaKeys);
            }
#endif
        }

        #region Get/Set properties
        //Called through: OnEnable, OnInspectorGUI
        public void GetProperties()
        {
            //Debug.Log("StylizedWater.cs: getProperties()");

#if UNITY_EDITOR

#if UNITY_5_5_OR_NEWER
            usingSinglePassRendering = (PlayerSettings.stereoRenderingPath == StereoRenderingPath.SinglePass && PlayerSettings.virtualRealitySupported) ? true : false;
#else
            usingSinglePassRendering = (PlayerSettings.singlePassStereoRendering && PlayerSettings.virtualRealitySupported) ? true : false;
#endif

            //Determine platform, so limitations can be applied
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android ||
             EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
            {
                isMobilePlatform = true;
                shaderNames = new string[] { "Mobile" };
            }
            else
            {
                isMobilePlatform = false;
                shaderNames = new string[] { "Desktop", "Mobile" };
            }

            if (meshRenderer) material = meshRenderer.sharedMaterial;

            if (material && meshRenderer)
            {
                meshRenderer.sharedMaterial.hideFlags = (hideMaterialInspector) ? HideFlags.HideInInspector : HideFlags.None;
                meshRenderer.hideFlags = (hideMeshRenderer) ? HideFlags.HideInInspector : HideFlags.None;
                meshFilter.hideFlags = (hideMeshRenderer) ? HideFlags.HideInInspector : HideFlags.None;
            }
            else
            {
                //Unhide inspectors so errors can be corrected
                hideMaterialInspector = false;
                hideMeshRenderer = false;
            }

            if (!material)
            {
                return;
            }


#if !UNITY_5_5_OR_NEWER && UNITY_EDITOR
            EditorUtility.SetSelectedWireframeHidden(meshRenderer, hideWireframe);
#endif


            //If maps are missing, bake new ones
            if (material.GetTexture("_Shadermap") == null)
            {
                GetShaderMap(useCompression, useCustomIntersection, useCustomHeightmap);
            }
            if (material.GetTexture("_Normals") == null)
            {
                GetNormalMap(useCompression, useCustomNormals);
            }

            GetShaderProperties();

            //Remove remaining reflection cams when switching to mobile
            if (isMobileAdvanced)
            {
                useReflection = false;
            }
#endif

        }

        //Grab material values
        private void GetShaderProperties()
        {
            if (!material)
            {
                hasMaterial = false;
                return;
            }
            else hasMaterial = true;

            renderQueue = material.renderQueue;

            GetShaderType();

            #region Common
            //Lighting
            //Unit
            if (material.IsKeywordEnabled("_LIGHTING_ON") == false && (material.GetFloat("_Unlit") == 1)) lightingMethod = 0;
            //Basic
            else if (material.IsKeywordEnabled("_LIGHTING_ON") == false && (material.GetFloat("_Unlit") == 0)) lightingMethod = 1;
            //Advanced
            else lightingMethod = 2;

            //Masking
            enableVertexColors = (material.GetFloat("_ENABLE_VC") == 1) ? true : false;

            //Maps
            shadermap = material.GetTexture("_Shadermap") as Texture2D;
            normals = material.GetTexture("_Normals") as Texture2D;

            //Color
            waterColor = material.GetColor("_WaterColor");
            waterShallowColor = material.GetColor("_WaterShallowColor");
            depth = material.GetFloat("_Depth");
            waveTint = material.GetFloat("_Wavetint");
            rimColor = material.GetColor("_RimColor");

            //Surface
            worldSpaceTiling = material.GetFloat("_Worldspacetiling");
            transparency = material.GetFloat("_Transparency");
            edgeFade = material.GetFloat("_EdgeFade");
            glossiness = material.GetFloat("_Glossiness");

            //Normals
            normalStrength = material.GetFloat("_NormalStrength");
            normalTiling = material.GetFloat("_NormalTiling");

            //Foam
            foamOpacity = material.GetFloat("_FoamOpacity");
            foamSolver = (int)material.GetFloat("_UseIntersectionFoam");
            foamTiling = material.GetFloat("_FoamTiling");
            foamSize = material.GetFloat("_FoamSize");
            foamSpeed = material.GetFloat("_FoamSpeed");
            waveFoam = material.GetFloat("_WaveFoam");

            //Intersection
            intersectionSolver = (int)material.GetFloat("_USE_VC_INTERSECTION");
            rimSize = material.GetFloat("_RimSize");
            rimFalloff = material.GetFloat("_Rimfalloff");
            rimTiling = material.GetFloat("_Rimtiling");

            //Waves
            waveSpeed = material.GetFloat("_Wavesspeed");
            waveDirectionXZ = material.GetVector("_WaveDirection");
            waveStrength = material.GetFloat("_WaveHeight");
            waveSize = material.GetFloat("_WaveSize");
            #endregion

            #region Desktop specific
            if (!isMobileBasic && !isMobileAdvanced)
            {

                //Lighting
                enableShadows = (material.GetFloat("_ENABLE_SHADOWS") == 1) ? true : false;
                enableNormalMap = true;

                //Color
                enableGradient = (material.GetFloat("_ENABLE_GRADIENT") == 1) ? true : false;
                fresnelColor = material.GetColor("_FresnelColor");
                fresnel = material.GetFloat("_Fresnelexponent");

                //Surface
                refractionAmount = material.GetFloat("_RefractionAmount");

                //Foam
                foamDistortion = material.GetFloat("_FoamDistortion");

                //Reflections
                if (useReflection) reflectionStrength = material.GetFloat("_ReflectionStrength");
                reflectionFresnel = material.GetFloat("_ReflectionFresnel");
                reflectionRefraction = material.GetFloat("_ReflectionRefraction");

                //Intersection
                rimDistortion = material.GetFloat("_RimDistortion");


                //Waves
                enableMacroNormals = material.IsKeywordEnabled("_MACRO_WAVES_ON");
                macroNormalsDistance = material.GetFloat("_MacroBlendDistance");
                enableSecondaryWaves = material.IsKeywordEnabled("_SECONDARY_WAVES_ON");

                //Tess
                //tessellation = material.GetFloat("_Tessellation");
            }
            #endregion

            #region Mobile specific
            if (isMobileAdvanced)
            {
                enableNormalMap = material.IsKeywordEnabled("_NORMAL_MAP_ON");

                //Intersection
                intersectionSolver = (int)material.GetFloat("_USE_VC_INTERSECTION");

                //Foam
                foamSolver = (material.IsKeywordEnabled("_USEINTERSECTIONFOAM_ON")) ? 1 : 0;

            }
            #endregion

            #endregion

            //Mobile basic doesn't have any unique parameters, only excluded ones

            hasShaderParams = true;
        }

        //Determine if Desktop or Mobile shader
        private void GetShaderType()
        {
#if UNITY_EDITOR //Only check for shader type in editor
            {
                //Find shaders
                DesktopShader = Shader.Find("StylizedWater/Desktop");
                MobileAdvancedShader = Shader.Find("StylizedWater/Mobile");
                //MobileBasicShader = Shader.Find("StylizedWater/Mobile Basic");

                //When shader is removed
                if (material.shader == Shader.Find("Hidden/InternalErrorShader") || material.shader == null)
                {
                    if (isMobilePlatform) material.shader = MobileAdvancedShader;
                    else material.shader = DesktopShader;
                }

                //Get current shader
                shader = material.shader;
                shaderName = shader.name;

                //Not an SWS shader
                if (!shaderName.Contains("StylizedWater")) return;

                #region Recognize shader
                if (isMobilePlatform)
                {
                    //Shader level specific properties
                    if (shader == MobileAdvancedShader)
                    {
                        isMobileAdvanced = true;
                        isMobileBasic = false;

                        shaderIndex = 0;
                    }
                    /*
                    if (shader == MobileBasicShader)
                    {
                        isMobileAdvanced = false;
                        isMobileBasic = true;

                        shaderIndex = 1;
                    }
                    */
                }
                else
                {
                    if (shader == DesktopShader)
                    {
                        isMobileBasic = false;
                        isMobileAdvanced = false;

                        shaderIndex = 0;
                    }
                    if (shader == MobileAdvancedShader)
                    {
                        isMobileBasic = false;
                        isMobileAdvanced = true;

                        shaderIndex = 1;
                    }
                    /*
                    if (shader == MobileBasicShader)
                    {
                        isMobileBasic = true;
                        isMobileAdvanced = false;

                        shaderIndex = 2;
                    }
                    */
                }
                #endregion

                if (hasShaderParams == true) return;
            }
#endif
        }

        private void SetShaderType()
        {
#if UNITY_EDITOR
            if (isMobilePlatform)
            {
                switch (shaderIndex)
                {
                    case 0:
                        shader = MobileAdvancedShader;
                        break;
                    case 1:
                        //shader = MobileBasicShader;
                        break;
                }
            }
            else
            {
                switch (shaderIndex)
                {
                    case 0:
                        shader = DesktopShader;
                        break;
                    case 1:
                        shader = MobileAdvancedShader;
                        break;
                    case 2:
                        //shader = MobileBasicShader;
                        break;
                }

            }

            material.shader = shader;
#endif
        }

        //Apply values
        public void SetProperties()
        {
            //Masking
            if (enableMaskRendering)
            {
                //Already enabled
                if (renderCam)
                {
                    renderCam.cullingMask = swsMaskRenderLayers.value;
                    s_swsMaskRenderLayers = swsMaskRenderLayers;
                }
                else
                {
                    EnableMaskRendering();
                }
            }
            else
            {
                DisableMaskRendering();
            }

            //Shadows
            if (enableShadows && shadowCaster)
            {
                EnableShadowRendering();
            }
            else
            {
                DisableShadowRendering();
            }

            SetShaderProperties();
        }

        private void SetShaderProperties()
        {
            if (!material) return;

            //material.renderQueue = renderQueue;

            //Set shader from dropdown list
            SetShaderType();

            #region Common properties
            if (shadermap) material.SetTexture("_Shadermap", shadermap);
            if (normals) material.SetTexture("_Normals", normals);

            //Lighting
            switch (lightingMethod)
            {
                case 0:
                    material.DisableKeyword("_LIGHTING_ON");
                    material.SetFloat("_Unlit", 1);
                    break;
                case 1:
                    material.DisableKeyword("_LIGHTING_ON");
                    material.SetFloat("_Unlit", 0);
                    break;
                case 2:
                    material.EnableKeyword("_LIGHTING_ON");
                    material.SetFloat("_Unlit", 0);
                    break;
            }

            //Color
            material.SetColor("_WaterColor", waterColor);
            material.SetColor("_WaterShallowColor", waterShallowColor);
            material.SetFloat("_Depth", depth);
            material.SetColor("_RimColor", rimColor);
            material.SetFloat("_Wavetint", waveTint);

            //Surface
            material.SetFloat("_Transparency", transparency);
            material.SetFloat("_Glossiness", glossiness);
            material.SetFloat("_Worldspacetiling", worldSpaceTiling);
            material.SetFloat("_EdgeFade", edgeFade);

            //Normals
            material.SetFloat("_NormalStrength", normalStrength);
            material.SetFloat("_NormalTiling", normalTiling);

            //Intersection
            material.SetFloat("_USE_VC_INTERSECTION", (int)intersectionSolver);
            material.SetFloat("_RimSize", rimSize);
            material.SetFloat("_Rimfalloff", rimFalloff);
            material.SetFloat("_Rimtiling", rimTiling);

            //Foam
            material.SetFloat("_UseIntersectionFoam", foamSolver);
            material.SetFloat("_FoamOpacity", foamOpacity);
            material.SetFloat("_FoamSize", foamSize);
            material.SetFloat("_FoamTiling", foamTiling);
            material.SetFloat("_FoamSpeed", foamSpeed);
            material.SetFloat("_WaveFoam", waveFoam);

            //Waves
            material.SetFloat("_Wavesspeed", waveSpeed);
            material.SetVector("_WaveDirection", waveDirectionXZ);
            material.SetFloat("_WaveHeight", waveStrength);
            material.SetFloat("_WaveSize", waveSize);

            //Advanced
            material.SetFloat("_ENABLE_VC", (enableVertexColors) ? 1 : 0);
            #endregion

            #region Desktop specific
            if (!isMobileBasic && !isMobileAdvanced)
            {
                //Lighting
                material.SetFloat("_ENABLE_SHADOWS", (enableShadows) ? 1 : 0);

                //Color
                material.SetFloat("_ENABLE_GRADIENT", (enableGradient) ? 1 : 0);
                material.SetTexture("_GradientTex", colorGradientTex);
                material.SetColor("_FresnelColor", fresnelColor);
                material.SetFloat("_Fresnelexponent", fresnel);

                //Refraction
                useRefractionCam = (refractionSolver == 1) ? true : false;
                if (!useRefractionCam) DisableRefractionCam();
                material.SetFloat("_RT_REFRACTION", refractionSolver);
                material.SetFloat("_RefractionAmount", refractionAmount);

                //Intersection
                material.SetFloat("_RimDistortion", rimDistortion);

                //Reflection
                if (usingSinglePassRendering)
                {
                    useReflection = false;
                    material.SetFloat("_ReflectionStrength", 0f);
                }
                else
                {
                    material.SetFloat("_ReflectionStrength", reflectionStrength);
                    material.SetFloat("_ReflectionFresnel", reflectionFresnel);
                    material.SetFloat("_ReflectionRefraction", reflectionRefraction);
                }
                if (reflectionBlurRenderer)
                {
                    reflectionBlurRenderer.length = reflectionBlurLength;
                    reflectionBlurRenderer.passes = reflectionBlurPasses;
                }

                //Foam
                material.SetFloat("_UseIntersectionFoam", foamSolver);
                material.SetFloat("_FoamDistortion", foamDistortion);

                //Waves
                if (enableMacroNormals) { material.EnableKeyword("_MACRO_WAVES_ON"); } else { material.DisableKeyword("_MACRO_WAVES_ON"); }
                material.SetFloat("_MacroBlendDistance", macroNormalsDistance);
                if (enableSecondaryWaves) { material.EnableKeyword("_SECONDARY_WAVES_ON"); } else { material.DisableKeyword("_SECONDARY_WAVES_ON"); }

                //Tessa
                //material.SetFloat("_Tessellation", tessellation);

            }
            #endregion

            #region Mobile specific
            if (isMobileAdvanced)
            {
                if (enableNormalMap) { material.EnableKeyword("_NORMAL_MAP_ON"); } else { material.DisableKeyword("_NORMAL_MAP_ON"); }

                //Foam
                if (foamSolver == 1) { material.EnableKeyword("_USEINTERSECTIONFOAM_ON"); } else { material.DisableKeyword("_USEINTERSECTIONFOAM_ON"); }
            }
            #endregion
        }

        #region Enable/Disable features
        public void DisableReflectionCam()
        {
            //Clear texture
            if (m_ReflectionTexture)
            {
                DestroyImmediate(m_ReflectionTexture);
                m_ReflectionTexture = null;
            }

            //Clear cameras
            foreach (var kvp in m_ReflectionCameras)
            {
                DestroyImmediate((kvp.Value).gameObject);
            }
            m_ReflectionCameras.Clear();

            //if(reflectionStrength > 0) m_ReflectionStrength = reflectionStrength;
            if (!useReflection) material.SetFloat("_ReflectionStrength", 0);
        }

        public void DisableRefractionCam()
        {
            //Clear texture
            if (m_RefractTexture)
            {
                DestroyImmediate(m_RefractTexture);
                m_RefractTexture = null;
            }

            //Clear cameras
            foreach (var kvp in m_RefractCameras)
            {
                DestroyImmediate((kvp.Value).gameObject);
            }
            m_RefractCameras.Clear();
        }

        private void EnableMaskRendering()
        {
            if (enableMaskRendering)
            {
                swsRenderer = new StylizedWaterRenderer();

                swsRenderSettings = new StylizedWaterRenderer.RendererSettings
                {
                    renderTexureSize = this.swsRendererTextureSize,
                    renderLayer = this.swsMaskRenderLayers
                };

                if (renderCam) return;

                swsRenderer.SetupMaskRenderer(swsRenderSettings, meshRenderer, out renderCam, out targetMeshInfo);
            }
        }

        private void DisableMaskRendering()
        {
            if (!enableMaskRendering)
            {
                //Debug.Log("DisableDynamicIntersection");

                //Global texture have no default color :(
                Shader.SetGlobalTexture("_SWS_RENDERTEX", Texture2D.blackTexture);

                if (renderCam) DestroyImmediate(renderCam.gameObject);
            }
        }

        private void EnableShadowRendering()
        {
            if (shadowCaster.GetComponent<StylizedWaterShadowCaster>() == null)
            {
                shadowRenderer = shadowCaster.gameObject.AddComponent<StylizedWaterShadowCaster>();
            }
        }

        private void DisableShadowRendering()
        {
            if (shadowRenderer) DestroyImmediate(shadowRenderer);
            else
            {
                try
                {
                    //Try to look for an old unreferenced component and delete it
                    if (shadowCaster) shadowRenderer = shadowCaster.GetComponent<StylizedWaterShadowCaster>();
                    if (shadowRenderer) DestroyImmediate(shadowRenderer);
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public void SetVegetationStudioWaterLevel()
        {
#if VEGETATION_STUDIO
            //Find lowest terrain
            Terrain[] terrains = GameObject.FindObjectsOfType<Terrain>();
            float depth = 0;
            foreach (Terrain t in terrains) { if (t.transform.position.y < 0) depth = t.transform.position.y; }

            float wsWaterheight = transform.position.y;

            AwesomeTechnologies.VegetationStudio.VegetationStudioManager.SetWaterLevel(this.transform.position.y - depth);
#endif
        }
        #endregion

        #region Texture rendering
#if UNITY_EDITOR
        public void GetGradientTex()
        {
            colorGradientTex = TexturePacker.GenerateGradientMap(material, colorGradient);

            //Must be invoked twice(?)
            material.SetTexture("_ColorGradient", colorGradientTex);
            material.SetTexture("_ColorGradient", colorGradientTex);
        }

        public void GetShaderMap(bool useCompression, bool useCustomIntersection, bool useCustomHeightmap)
        {
            //Save settings
            this.useCompression = useCompression;
            this.useCustomIntersection = useCustomIntersection;
            this.useCustomHeightmap = useCustomHeightmap;

            //Before baking, make sure all settings are applied
            SetProperties();

            //Combine chosen textures into packed texture
            shadermap = TexturePacker.RenderShaderMap(material, intersectionStyle, waveStyle, waveHeightmapStyle, useCompression, (useCustomIntersection) ? customIntersection : null, (useCustomHeightmap) ? customHeightmap : null);

            //Set baked textures to shader
            material.SetTexture("_Shadermap", shadermap);
        }

        public void GetNormalMap(bool useCompression, bool useCustomNormals)
        {
            this.useCompression = useCompression;
            this.useCustomNormals = useCustomNormals;

            SetProperties();

            normals = TexturePacker.RenderNormalMap(material, waveStyle, useCompression, (useCustomNormals) ? customNormal : null);

            material.SetTexture("_Normals", normals);
        }
#endif
        #endregion

        #region Reflection/Refraction render functions
        public void OnWillRenderObject()
        {
            if (!enabled || !material)
            {
                return;
            }

            Camera cam = Camera.current;
            if (!cam)
            {
                return;
            }

            // Safeguard from recursive reflections.        
            if (s_InsideRendering)
                return;
            s_InsideRendering = true;

            Camera reflectionCamera, refractCamera;
            CreateWaterObjects(cam, out reflectionCamera, out refractCamera);

            // find out the reflection plane: position and normal in world space
            Vector3 pos = transform.position;
            Vector3 normal = transform.up;

            // Disable pixel lights for reflection
            int oldPixelLightCount = QualitySettings.pixelLightCount;
            QualitySettings.pixelLightCount = 0;

            #region Reflection
            // Render reflection
            if (useReflection)
            {
                StylizedWaterUtilities.CameraUtils.CopyCameraSettings(cam, reflectionCamera);

                // Reflect camera around reflection plane
                float d = -Vector3.Dot(normal, pos) - clipPlaneOffset;
                Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

                Matrix4x4 reflection = Matrix4x4.zero;
                StylizedWaterUtilities.CameraUtils.CalculateReflectionMatrix(ref reflection, reflectionPlane);
                Vector3 oldpos = cam.transform.position;
                Vector3 newpos = reflection.MultiplyPoint(oldpos);
                reflectionCamera.worldToCameraMatrix = cam.worldToCameraMatrix * reflection;

                // Setup oblique projection matrix so that near plane is our reflection
                // plane. This way we clip everything below/above it for free.
                Vector4 clipPlane = StylizedWaterUtilities.CameraUtils.CameraSpacePlane(reflectionCamera, pos, normal, 1.0f, clipPlaneOffset);
                reflectionCamera.projectionMatrix = cam.CalculateObliqueMatrix(clipPlane);

                reflectionCamera.cullingMask = ~(1 << 4) & reflectLayers.value; // never render water layer
                reflectionCamera.targetTexture = m_ReflectionTexture;
                bool oldCulling = GL.invertCulling;
                GL.invertCulling = !oldCulling;
                reflectionCamera.transform.position = newpos;
                Vector3 euler = cam.transform.eulerAngles;
                reflectionCamera.transform.eulerAngles = new Vector3(-euler.x, euler.y, euler.z);

                //Manually invoke render method
                if (enableReflectionBlur && reflectionBlurRenderer) reflectionBlurRenderer.Render();

                reflectionCamera.Render();

                reflectionCamera.transform.position = oldpos;
                GL.invertCulling = oldCulling;

                //Set texture, otherwise blur renderer will do it
                if (!enableReflectionBlur) Shader.SetGlobalTexture("_ReflectionTex", m_ReflectionTexture);
            }
            else
            {
                DisableReflectionCam();
            }
            #endregion

            #region Refraction
            if (useRefractionCam)
            {
                StylizedWaterUtilities.CameraUtils.CopyCameraSettings(cam, refractCamera);

                refractCamera.worldToCameraMatrix = cam.worldToCameraMatrix;

                Vector4 clipPlane = StylizedWaterUtilities.CameraUtils.CameraSpacePlane(refractCamera, pos, normal, -1.0f, clipPlaneOffset);
                refractCamera.projectionMatrix = cam.CalculateObliqueMatrix(clipPlane);

                refractCamera.cullingMask = ~(1 << 4) & refractLayers.value; // never render water layer
                refractCamera.targetTexture = m_RefractTexture;
                refractCamera.transform.position = cam.transform.position;
                refractCamera.transform.rotation = cam.transform.rotation;
                refractCamera.Render();
                material.SetTexture("_RefractionTex", m_RefractTexture);
            }

            #endregion

            // Restore pixel light count
            QualitySettings.pixelLightCount = oldPixelLightCount;

            s_InsideRendering = false;
        }

        public void CreateReflectionTexture()
        {
            /*
            //Clear texture
            if (m_ReflectionTexture)
            {
                DestroyImmediate(m_ReflectionTexture);
                m_ReflectionTexture = null;
            }
            */

            // sws.reflectLayers = reflectLayers;
            switch (reflectionRes)
            {
                case 0:
                    reflectionTextureSize = 256;
                    break;
                case 1:
                    reflectionTextureSize = 512;
                    break;
                case 2:
                    reflectionTextureSize = 1024;
                    break;
            }

            // Reflection render texture
            if (!m_ReflectionTexture || m_OldReflectionTextureSize != reflectionTextureSize)
            {
                m_ReflectionTexture = new RenderTexture(reflectionTextureSize, reflectionTextureSize, 16)
                {
                    name = "__WaterReflection",
                    isPowerOfTwo = true,
                    hideFlags = HideFlags.None
                };

                //Debug.Log("New reflection texture created");

                m_OldReflectionTextureSize = reflectionTextureSize;
            }
        }

        public void CreateRefractionTexture()
        {
            //Clear texture
            if (m_RefractTexture)
            {
                DestroyImmediate(m_RefractTexture);
                m_RefractTexture = null;
            }

            // Reflection render texture
            if (!m_RefractTexture || m_OldRefractTextureSize != refractTextureSize)
            {

                switch (refractRes)
                {
                    case 0:
                        refractTextureSize = 256;
                        break;
                    case 1:
                        refractTextureSize = 512;
                        break;
                    case 2:
                        refractTextureSize = 1024;
                        break;
                }

                m_RefractTexture = new RenderTexture(refractTextureSize, refractTextureSize, 16);
                m_RefractTexture.name = "__WaterRefraction";
                m_RefractTexture.isPowerOfTwo = true;
                m_RefractTexture.hideFlags = HideFlags.DontSave;
                m_OldRefractTextureSize = refractTextureSize;
            }
        }

        // On-demand create any objects we need for water
        void CreateWaterObjects(Camera currentCamera, out Camera reflectionCamera, out Camera refractCamera)
        {

            reflectionCamera = null;
            refractCamera = null;
            reflectionBlurRenderer = null;

            #region Reflection cam
            if (useReflection)
            {
                CreateReflectionTexture();

                // Camera for reflection
                m_ReflectionCameras.TryGetValue(currentCamera, out reflectionCamera);
                if (!reflectionCamera) // catch both not-in-dictionary and in-dictionary-but-deleted-GO
                {
                    GameObject go = new GameObject("", typeof(Camera));
                    go.name = "Reflection Camera " + go.GetInstanceID() + " for " + currentCamera.name;
                    go.hideFlags = HideFlags.HideAndDontSave;

                    reflectionCamera = go.GetComponent<Camera>();
                    //Disable component as Render() is to be called manually
                    reflectionCamera.enabled = false;

                    reflectionCamera.useOcclusionCulling = false;

                    reflectionCamera.transform.position = transform.position;
                    reflectionCamera.transform.rotation = transform.rotation;

                    reflectionCamera.gameObject.AddComponent<FlareLayer>();
                    m_ReflectionCameras[currentCamera] = reflectionCamera;

                }

                if (reflectionCamera)
                {
                    if (enableReflectionBlur)
                    {
                        m_BlurRenderers.TryGetValue(currentCamera, out reflectionBlurRenderer);
                        if (!reflectionBlurRenderer)
                        {
                            reflectionBlurRenderer = reflectionCamera.gameObject.AddComponent<StylizedWaterBlur>();
                        }

                        m_BlurRenderers[currentCamera] = reflectionBlurRenderer;
                    }
                }


            }
            #endregion

            #region Refraction cam
            if (useRefractionCam)
            {

                CreateRefractionTexture();

                // Camera for refraction
                m_RefractCameras.TryGetValue(currentCamera, out refractCamera);
                if (!refractCamera) // catch both not-in-dictionary and in-dictionary-but-deleted-GO
                {
                    GameObject go = new GameObject("", typeof(Camera));
                    go.name = "Refraction Camera " + go.GetInstanceID() + " for " + currentCamera.name;
                    go.hideFlags = HideFlags.HideAndDontSave;

                    refractCamera = go.GetComponent<Camera>();
                    refractCamera.enabled = false;

                    refractCamera.useOcclusionCulling = false;

                    refractCamera.transform.position = transform.position;
                    refractCamera.transform.rotation = transform.rotation;

                    //Add box blur renderer

                    refractCamera.gameObject.AddComponent<FlareLayer>();

                    m_RefractCameras[currentCamera] = refractCamera;
                }
            }
            #endregion
        }
        #endregion

        #region Mono
        // Cleanup all the objects we possibly have created
        void OnDisable()
        {
            DisableReflectionCam();
            DisableRefractionCam();
            DisableMaskRendering();

            if (shadowRenderer) DestroyImmediate(shadowRenderer);
        }

        //When component is removed
        void OnDestroy()
        {
#if UNITY_EDITOR
            if (!meshRenderer || !meshRenderer.sharedMaterial) return;

#if !UNITY_5_5_OR_NEWER
            EditorUtility.SetSelectedWireframeHidden(meshRenderer, false);
#endif
            meshRenderer.sharedMaterial.hideFlags = HideFlags.None;

            meshRenderer.hideFlags = HideFlags.None;
            meshFilter.hideFlags = HideFlags.None;

#endif
            DisableMaskRendering();
            DisableShadowRendering();
        }
        #endregion
    }//SWS class end

}//Namespace


//Easter egg, good job :)