using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GPUInstancer
{
    public class GPUInstancerSettings : ScriptableObject
    {
        public float versionNo;

        public GPUInstancerShaderBindings shaderBindings;
        public GPUInstancerBillboardAtlasBindings billboardAtlasBindings;
        public ShaderVariantCollection shaderVariantCollection;
        public bool packagesLoaded;
        public bool isHDRP;
        public bool isLWRP;
        public bool isURP;
        public bool isShaderGraphPresent;
        public int instancingBoundsSize = 10000;

        private Material _foliageHDRPTemplate;

        #region Material Templates

        public Material GetFoliageHDRPTemplate()
        {
            if (!isHDRP)
                Debug.LogWarning("HDRP Foliage shader template material requested in a non HDRP project. Is this intentional?");

            if (_foliageHDRPTemplate != null)
                return _foliageHDRPTemplate;

            _foliageHDRPTemplate = (Material)Resources.Load(GPUInstancerConstants.FOLIAGE_SHADER_HDRP_TEMPLATE_MATERIAL_PATH);

            if (_foliageHDRPTemplate != null)
                return _foliageHDRPTemplate;

            Debug.LogError("HDRP Foliage shader template material not found. Please import the HDRP Foliage Shader support package from " + GPUInstancerConstants.FOLIAGE_SHADER_HDRP_PACKAGE_PATH);
            return null;

        }

        #endregion Material Templates

        #region Editor Constants
        public float MAX_DETAIL_DISTANCE = 2500;
        public float MAX_TREE_DISTANCE = 2500;
        public float MAX_PREFAB_DISTANCE = 10000;
        public int MAX_PREFAB_EXTRA_BUFFER_SIZE = 16384;
        #endregion Editor Constants

        #region Theme
        public bool useCustomPreviewBackgroundColor = false;
        public Color previewBackgroundColor = Color.white;
        #endregion Theme

        #region Editor Behaviour
        public bool disableAutoGenerateBillboards = false;
        public bool disableShaderVariantCollection = false;
        public bool disableInstanceCountWarning = false;
        public bool disableAutoShaderConversion = false;
        public bool disableAutoVariantHandling = false;
        #endregion Editor Behaviour

        #region VR
        public bool testBothEyesForVROcclusion = true;
        public int vrRenderingMode = 0; // SinglePass = 0, MultiPass = 1
        #endregion VR

        #region Extensions
        public List<GPUInstancerSettingsExtension> extensionSettings;

        public void AddExtension(GPUInstancerSettingsExtension extension)
        {
            if (extension == null)
                return;
            if (extensionSettings == null)
                extensionSettings = new List<GPUInstancerSettingsExtension>();
            extensionSettings.RemoveAll(ex => ex == null);
            if (!extensionSettings.Contains(extension))
                extensionSettings.Add(extension);
        }
        #endregion Extensions

        #region Core Settings
        public List<GPUIRenderingSettings> renderingSettingPresets;

        public bool hasCustomRenderingSettings;
        public GPUIRenderingSettings customRenderingSettings;
        public GPUIOcclusionCullingType occlusionCullingType;
        #endregion Core Settings

        public static GPUInstancerSettings GetDefaultGPUInstancerSettings()
        {
            GPUInstancerSettings gpuiSettings = Resources.Load<GPUInstancerSettings>(GPUInstancerConstants.SETTINGS_PATH + GPUInstancerConstants.GPUI_SETTINGS_DEFAULT_NAME);

            if (gpuiSettings == null)
            {
                gpuiSettings = ScriptableObject.CreateInstance<GPUInstancerSettings>();
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    if (!System.IO.Directory.Exists(GPUInstancerConstants.GetDefaultPath() + GPUInstancerConstants.RESOURCES_PATH + GPUInstancerConstants.SETTINGS_PATH))
                    {
                        System.IO.Directory.CreateDirectory(GPUInstancerConstants.GetDefaultPath() + GPUInstancerConstants.RESOURCES_PATH + GPUInstancerConstants.SETTINGS_PATH);
                    }

                    AssetDatabase.CreateAsset(gpuiSettings, GPUInstancerConstants.GetDefaultPath() + GPUInstancerConstants.RESOURCES_PATH + GPUInstancerConstants.SETTINGS_PATH + GPUInstancerConstants.GPUI_SETTINGS_DEFAULT_NAME + ".asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
#endif
            }
            gpuiSettings.SetDefultBindings();
            return gpuiSettings;
        }

        public virtual void SetDefultBindings()
        {
            SetDefaultGPUInstancerShaderBindings();
            SetDefaultGPUInstancerBillboardAtlasBindings();
            SetDefaultShaderVariantCollection();

            renderingSettingPresets = new List<GPUIRenderingSettings>
            {
                new GPUIRenderingSettings(){ platform = GPUIPlatform.Default, matrixHandlingType = GPUIMatrixHandlingType.Default, computeThreadCount = GPUIComputeThreadCount.x512 },
                new GPUIRenderingSettings(){ platform = GPUIPlatform.OpenGLCore, matrixHandlingType = GPUIMatrixHandlingType.Default, computeThreadCount = GPUIComputeThreadCount.x256 },
                new GPUIRenderingSettings(){ platform = GPUIPlatform.Metal, matrixHandlingType = GPUIMatrixHandlingType.Default, computeThreadCount = GPUIComputeThreadCount.x256 },
                new GPUIRenderingSettings(){ platform = GPUIPlatform.GLES31, matrixHandlingType = GPUIMatrixHandlingType.CopyToTexture, computeThreadCount = GPUIComputeThreadCount.x128 },
                new GPUIRenderingSettings(){ platform = GPUIPlatform.Vulkan, matrixHandlingType = GPUIMatrixHandlingType.MatrixAppend, computeThreadCount = GPUIComputeThreadCount.x128 },
                new GPUIRenderingSettings(){ platform = GPUIPlatform.PS4, matrixHandlingType = GPUIMatrixHandlingType.Default, computeThreadCount = GPUIComputeThreadCount.x512 },
                new GPUIRenderingSettings(){ platform = GPUIPlatform.XBoxOne, matrixHandlingType = GPUIMatrixHandlingType.Default, computeThreadCount = GPUIComputeThreadCount.x512 }
            };
        }

        #region Shader Bindings
        public virtual void SetDefaultGPUInstancerShaderBindings()
        {
            if (shaderBindings == null)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    Undo.RecordObject(this, "GPUInstancerShaderBindings instance generated");
#endif
                shaderBindings = GetDefaultGPUInstancerShaderBindings();
            }
        }

        public static GPUInstancerShaderBindings GetDefaultGPUInstancerShaderBindings()
        {
            GPUInstancerShaderBindings shaderBindings = Resources.Load<GPUInstancerShaderBindings>(GPUInstancerConstants.SETTINGS_PATH + GPUInstancerConstants.SHADER_BINDINGS_DEFAULT_NAME);

            if (shaderBindings == null)
            {
                shaderBindings = ScriptableObject.CreateInstance<GPUInstancerShaderBindings>();
                shaderBindings.ResetShaderInstances();
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    if (!System.IO.Directory.Exists(GPUInstancerConstants.GetDefaultPath() + GPUInstancerConstants.RESOURCES_PATH + GPUInstancerConstants.SETTINGS_PATH))
                    {
                        System.IO.Directory.CreateDirectory(GPUInstancerConstants.GetDefaultPath() + GPUInstancerConstants.RESOURCES_PATH + GPUInstancerConstants.SETTINGS_PATH);
                    }

                    AssetDatabase.CreateAsset(shaderBindings, GPUInstancerConstants.GetDefaultPath() + GPUInstancerConstants.RESOURCES_PATH + GPUInstancerConstants.SETTINGS_PATH + GPUInstancerConstants.SHADER_BINDINGS_DEFAULT_NAME + ".asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
#endif
            }

            return shaderBindings;
        }
        #endregion Shader Bindings

        #region Billboard Atlas Bindings
        public virtual void SetDefaultGPUInstancerBillboardAtlasBindings()
        {
            if (billboardAtlasBindings == null)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    Undo.RecordObject(this, "GPUInstancerBillboardAtlasBindings instance generated");
#endif
                billboardAtlasBindings = GetDefaultGPUInstancerBillboardAtlasBindings();
            }
        }

        public static GPUInstancerBillboardAtlasBindings GetDefaultGPUInstancerBillboardAtlasBindings()
        {
            GPUInstancerBillboardAtlasBindings billboardAtlasBindings = Resources.Load<GPUInstancerBillboardAtlasBindings>(GPUInstancerConstants.SETTINGS_PATH + GPUInstancerConstants.BILLBOARD_ATLAS_BINDINGS_DEFAULT_NAME);

            if (billboardAtlasBindings == null)
            {
                billboardAtlasBindings = ScriptableObject.CreateInstance<GPUInstancerBillboardAtlasBindings>();
                billboardAtlasBindings.ResetBillboardAtlases();
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    if (!System.IO.Directory.Exists(GPUInstancerConstants.GetDefaultPath() + GPUInstancerConstants.RESOURCES_PATH + GPUInstancerConstants.SETTINGS_PATH))
                    {
                        System.IO.Directory.CreateDirectory(GPUInstancerConstants.GetDefaultPath() + GPUInstancerConstants.RESOURCES_PATH + GPUInstancerConstants.SETTINGS_PATH);
                    }

                    AssetDatabase.CreateAsset(billboardAtlasBindings, GPUInstancerConstants.GetDefaultPath() + GPUInstancerConstants.RESOURCES_PATH + GPUInstancerConstants.SETTINGS_PATH + GPUInstancerConstants.BILLBOARD_ATLAS_BINDINGS_DEFAULT_NAME + ".asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
#endif
            }

            return billboardAtlasBindings;
        }
        #endregion Billboard Atlas Bindings

        #region Shader Variant Collection
        public virtual void SetDefaultShaderVariantCollection()
        {
            if (disableShaderVariantCollection)
                return;
            if (shaderVariantCollection == null)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    Undo.RecordObject(this, "GPUI ShaderVariantCollection instance generated");
#endif
                shaderVariantCollection = GetDefaultShaderVariantCollection();
            }
            SetDefaultShaderVariants();
        }

        public static ShaderVariantCollection GetDefaultShaderVariantCollection()
        {
            ShaderVariantCollection shaderVariantCollection = Resources.Load<ShaderVariantCollection>(GPUInstancerConstants.SETTINGS_PATH + GPUInstancerConstants.SHADER_VARIANT_COLLECTION_DEFAULT_NAME);

            if (shaderVariantCollection == null)
            {
                shaderVariantCollection = new ShaderVariantCollection();
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    if (!System.IO.Directory.Exists(GPUInstancerConstants.GetDefaultPath() + GPUInstancerConstants.RESOURCES_PATH + GPUInstancerConstants.SETTINGS_PATH))
                    {
                        System.IO.Directory.CreateDirectory(GPUInstancerConstants.GetDefaultPath() + GPUInstancerConstants.RESOURCES_PATH + GPUInstancerConstants.SETTINGS_PATH);
                    }

                    AssetDatabase.CreateAsset(shaderVariantCollection, GPUInstancerConstants.GetDefaultPath() + GPUInstancerConstants.RESOURCES_PATH + GPUInstancerConstants.SETTINGS_PATH + GPUInstancerConstants.SHADER_VARIANT_COLLECTION_DEFAULT_NAME + ".shadervariants");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
#endif
            }

            return shaderVariantCollection;
        }

        public virtual void SetDefaultShaderVariants()
        {
            AddShaderVariantToCollection(GPUInstancerConstants.SHADER_GPUI_SHADOWS_ONLY);
            AddShaderVariantToCollection(GPUInstancerConstants.SHADER_GPUI_HIZ_OCCLUSION_GENERATOR);
            AddShaderVariantToCollection(GPUInstancerConstants.SHADER_GPUI_TREE_PROXY);
        }

        public virtual void AddShaderVariantToCollection(string shaderName, string extensionCode = null)
        {
            if (disableShaderVariantCollection)
                return;
#if UNITY_EDITOR
            if (!Application.isPlaying && shaderBindings != null && shaderVariantCollection != null && !string.IsNullOrEmpty(shaderName))
            {
                Shader instancedShader = shaderBindings.GetInstancedShader(shaderName, extensionCode);
                if (instancedShader != null)
                {
                    ShaderVariantCollection.ShaderVariant shaderVariant = new ShaderVariantCollection.ShaderVariant();
                    shaderVariant.shader = instancedShader;
                    //shaderVariant.passType = PassType.Normal;
                    shaderVariantCollection.Add(shaderVariant);
                    // To add only the shader without the passtype or keywords, remove the specific variant but the shader remains
                    shaderVariantCollection.Remove(shaderVariant);
                }
            }
#endif
        }

        public virtual void AddShaderVariantToCollection(Material material, string extensionCode = null)
        {
            if (disableShaderVariantCollection)
                return;
#if UNITY_EDITOR
            if (!Application.isPlaying && shaderBindings != null && shaderVariantCollection != null && !string.IsNullOrEmpty(material.shader.name) && material)
            {
                Shader instancedShader = shaderBindings.GetInstancedShader(material.shader.name, extensionCode);
                if (instancedShader != null)
                {
                    ShaderVariantCollection.ShaderVariant shaderVariant = new ShaderVariantCollection.ShaderVariant();
                    shaderVariant.shader = instancedShader;
                    shaderVariant.keywords = material.shaderKeywords;
                    shaderVariantCollection.Add(shaderVariant);
                }
            }
#endif
        }
        #endregion Shader Variant Collection

        #region Rendering Settings
        public GPUIMatrixHandlingType GetMatrixHandlingType(GPUIPlatform platform)
        {
            if (hasCustomRenderingSettings && customRenderingSettings != null)
                return customRenderingSettings.matrixHandlingType;
            for (int i = 0; i < renderingSettingPresets.Count; i++)
            {
                if (renderingSettingPresets[i].platform == platform)
                    return renderingSettingPresets[i].matrixHandlingType;
            }
            return GPUIMatrixHandlingType.Default;
        }

        public GPUIComputeThreadCount GetComputeThreadCount(GPUIPlatform platform)
        {
            if (hasCustomRenderingSettings && customRenderingSettings != null)
                return customRenderingSettings.computeThreadCount;
            for (int i = 0; i < renderingSettingPresets.Count; i++)
            {
                if (renderingSettingPresets[i].platform == platform)
                    return renderingSettingPresets[i].computeThreadCount;
            }
            return GPUIComputeThreadCount.x1024;
        }

        [Serializable]
        public class GPUIRenderingSettings
        {
            public GPUIPlatform platform;
            public GPUIMatrixHandlingType matrixHandlingType;
            public GPUIComputeThreadCount computeThreadCount;
        }
        #endregion Rendering Settings

        public bool IsStandardRenderPipeline()
        {
            return !isLWRP && !isHDRP && !isURP;
        }
    }

    public enum GPUIPlatform
    {
        Default,
        OpenGLCore,
        Metal,
        GLES31,
        Vulkan,
        PS4,
        XBoxOne
    }

    public enum GPUIMatrixHandlingType
    {
        Default = 0,
        MatrixAppend = 1,
        CopyToTexture = 2
    }

    public enum GPUIComputeThreadCount
    {
        x64 = 0,
        x128 = 1,
        x256 = 2,
        x512 = 3,
        x1024 = 4
    }

    public enum GPUIOcclusionCullingType
    {
        Default = 0, //uses Graphics.Blit
        CopyTexture = 1 //uses Compute Shader
    }
}
