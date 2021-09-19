using UnityEngine;

namespace GPUInstancer
{

    public static class GPUInstancerConstants
    {
        private static GPUInstancerSettings _gpuiSettings;
        public static GPUInstancerSettings gpuiSettings
        {
            get
            {
                if (_gpuiSettings == null)
                    _gpuiSettings = GPUInstancerSettings.GetDefaultGPUInstancerSettings();
                return _gpuiSettings;
            }
            set
            {
                _gpuiSettings = value;
            }
        }

        public static readonly Matrix4x4 zeroMatrix = Matrix4x4.zero;

        #region Stride Sizes
        // Compute buffer stride sizes
        public static readonly int STRIDE_SIZE_MATRIX4X4 = 64;
        public static readonly int STRIDE_SIZE_BOOL = 4;
        public static readonly int STRIDE_SIZE_INT = 4;
        public static readonly int STRIDE_SIZE_FLOAT = 4;
        public static readonly int STRIDE_SIZE_FLOAT4 = 16;
        #endregion Stride Sizes

        #region Platform Dependent

        public static float COMPUTE_SHADER_THREAD_COUNT = 512;
        public static float COMPUTE_SHADER_THREAD_COUNT_2D = 16;

        public static int COMPUTE_MAX_LOD_BUFFER = 4;
        public static int TEXTURE_MAX_SIZE = 16384;
        public static bool DETAIL_STORE_INSTANCE_DATA = false;

        public static readonly string GUID_COMPUTE_PLATFORM_DEFINES = "74a30c752a8958c45a96bc127e05f114";
        public static readonly string GUID_CGINC_PLATFORM_DEPENDENT = "79e50e99a1888054cb229e1c710f1795";

        #endregion Platform Dependent

        #region CS Visibility
        // Shader constants

        public static readonly string[] CAMERA_COMPUTE_KERNELS = new string[] {
            "CSInstancedCameraCalculationKernel", "CSInstancedCameraCalculationKernelCrossFade"
        };
        public static readonly string[] VISIBILITY_COMPUTE_KERNELS = new string[] {
            "CSInstancedRenderingVisibilityKernelLOD0", "CSInstancedRenderingVisibilityKernelLOD1",
            "CSInstancedRenderingVisibilityKernelLOD2", "CSInstancedRenderingVisibilityKernelLOD3"
        };

        public static readonly string CAMERA_COMPUTE_RESOURCE_PATH = "Compute/CSInstancedCameraCalculationKernel";
        public static readonly string CAMERA_VR_COMPUTE_RESOURCE_PATH = "Compute/CSInstancedCameraCalculationKernelVR";

        public static readonly string VISIBILITY_COMPUTE_RESOURCE_PATH = "Compute/CSInstancedRenderingVisibilityKernel";
        public static readonly string VISIBILITY_COMPUTE_RESOURCE_PATH_VULKAN = "Compute/CSInstancedRenderingVisibilityKernelVulkan";
        public static readonly string BUFFER_TO_TEXTURE_COMPUTE_RESOURCE_PATH = "Compute/CSInstancedBufferToTexture";
        public static readonly string BUFFER_TO_TEXTURE_KERNEL = "CSInstancedBufferToTextureKernel";
        public static class BufferToTextureKernelPoperties
        {
            public static readonly int TRANSFORMATION_MATRIX_TEXTURE = Shader.PropertyToID("gpuiTransformationMatrixTexture");
        }

        public static class VisibilityKernelPoperties
        {
            public static readonly int TRANSFORMATION_MATRIX_BUFFER = Shader.PropertyToID("gpuiTransformationMatrix");
            public static readonly int INSTANCE_LOD_BUFFER = Shader.PropertyToID("gpuiInstanceLODData");
            public static readonly int[] TRANSFORMATION_MATRIX_APPEND_BUFFERS = new int[] {
                Shader.PropertyToID("gpuiTransformationMatrix_LOD0"), Shader.PropertyToID("gpuiTransformationMatrix_LOD1"),  Shader.PropertyToID("gpuiTransformationMatrix_LOD2"),
                Shader.PropertyToID("gpuiTransformationMatrix_LOD3") };
            public static readonly int INSTANCE_DATA_BUFFER = Shader.PropertyToID("gpuiInstanceData");
            public static readonly int RENDERER_TRANSFORM_OFFSET = Shader.PropertyToID("gpuiTransformOffset");
            public static readonly int BUFFER_PARAMETER_MVP_MATRIX = Shader.PropertyToID("mvpMatrix");
            public static readonly int BUFFER_PARAMETER_MVP_MATRIX2 = Shader.PropertyToID("mvpMatrix2");
            public static readonly int BUFFER_PARAMETER_BOUNDS_CENTER = Shader.PropertyToID("boundsCenter");
            public static readonly int BUFFER_PARAMETER_BOUNDS_EXTENTS = Shader.PropertyToID("boundsExtents");
            public static readonly int BUFFER_PARAMETER_FRUSTUM_CULL_SWITCH = Shader.PropertyToID("isFrustumCulling");
            public static readonly int BUFFER_PARAMETER_HIERARCHICAL_Z_TEXTURE_SIZE = Shader.PropertyToID("hiZTxtrSize");
            public static readonly int BUFFER_PARAMETER_FRUSTUM_OFFSET = Shader.PropertyToID("frustumOffset");
            public static readonly int BUFFER_PARAMETER_MIN_VIEW_DISTANCE = Shader.PropertyToID("minDistance");
            public static readonly int BUFFER_PARAMETER_MAX_VIEW_DISTANCE = Shader.PropertyToID("maxDistance");
            public static readonly int BUFFER_PARAMETER_CAMERA_POSITION = Shader.PropertyToID("camPos");
            public static readonly int BUFFER_PARAMETER_BUFFER_SIZE = Shader.PropertyToID("bufferSize");

            public static readonly int BUFFER_PARAMETER_OCCLUSION_OFFSET = Shader.PropertyToID("occlusionOffset");
            public static readonly int BUFFER_PARAMETER_OCCLUSION_ACCURACY = Shader.PropertyToID("occlusionAccuracy");

            public static readonly int BUFFER_PARAMETER_SHADOW_DISTANCE = Shader.PropertyToID("shadowDistance");

            public static readonly int BUFFER_PARAMETER_LOD_SIZES = Shader.PropertyToID("lodSizes");
            public static readonly int BUFFER_PARAMETER_LOD_SHIFT = Shader.PropertyToID("lodShift");
            public static readonly int BUFFER_PARAMETER_LOD_APPEND_INDEX = Shader.PropertyToID("lodAppendIndex");
            public static readonly int BUFFER_PARAMETER_LOD_COUNT = Shader.PropertyToID("lodCount");
            public static readonly int BUFFER_PARAMETER_LOD_LEVEL = Shader.PropertyToID("LODLevel");
            public static readonly int BUFFER_PARAMETER_HALF_ANGLE = Shader.PropertyToID("halfAngle");
            public static readonly int BUFFER_PARAMETER_ANIMATE_CROSS_FADE = Shader.PropertyToID("animateCrossFade");
            public static readonly int BUFFER_PARAMETER_DELTA_TIME = Shader.PropertyToID("deltaTime");
            public static readonly int BUFFER_PARAMETER_FADE_LEVEL_MULTIPLIER = Shader.PropertyToID("fadeLevelMultiplier");

            public static readonly int BUFFER_PARAMETER_OCCLUSION_CULL_SWITCH = Shader.PropertyToID("isOcclusionCulling");
            public static readonly int BUFFER_PARAMETER_HIERARCHICAL_Z_TEXTURE_MAP = Shader.PropertyToID("hiZMap");

            public static readonly int BUFFER_PARAMETER_MIN_CULLING_DISTANCE = Shader.PropertyToID("minCullingDistance");
            public static readonly int BUFFER_PARAMETER_SHADOW_LOD_MAP = Shader.PropertyToID("shadowLODMap");
            public static readonly int BUFFER_PARAMETER_CULL_SHADOW = Shader.PropertyToID("cullShadows");
        }
        public static readonly int BUFFER_COROUTINE_STEP_NUMBER = 16384;
        public static int DETAIL_BUFFER_MERGE_FRAME_LIMIT = 1024;
        #endregion CS Visibility

        #region CS Set Data Partial
        // Compute Buffer Set Data Partial compute shader constants
        public static readonly string COMPUTE_SET_DATA_PARTIAL_RESOURCE_PATH = "Compute/CSInstancedComputeBufferSetDataPartialKernel";
        public static readonly string COMPUTE_SET_DATA_PARTIAL_KERNEL = "CSInstancedComputeBufferSetDataPartialKernel";
        public static readonly string COMPUTE_SET_DATA_SINGLE_KERNEL = "CSInstancedComputeBufferSetDataSingleKernel";

        public static class SetDataKernelProperties
        {
            public static readonly int BUFFER_PARAMETER_MANAGED_BUFFER_DATA = Shader.PropertyToID("gpuiManagedData");
            public static readonly int BUFFER_PARAMETER_COMPUTE_BUFFER_START_INDEX = Shader.PropertyToID("computeBufferStartIndex");
            public static readonly int BUFFER_PARAMETER_COUNT = Shader.PropertyToID("count");
            public static readonly int BUFFER_PARAMETER_DATA_TO_SET = Shader.PropertyToID("dataToSet");
        }

        public static ComputeShader computeBufferSetDataPartial;
        public static int computeBufferSetDataPartialKernelId;
        public static int computeBufferSetDataSingleKernelId;

        public static void SetupComputeSetDataPartial()
        {
            if (computeBufferSetDataPartial == null)
            {
                computeBufferSetDataPartial = Resources.Load<ComputeShader>(COMPUTE_SET_DATA_PARTIAL_RESOURCE_PATH);
                if (computeBufferSetDataPartial != null)
                {
                    computeBufferSetDataPartialKernelId = computeBufferSetDataPartial.FindKernel(COMPUTE_SET_DATA_PARTIAL_KERNEL);
                    computeBufferSetDataSingleKernelId = computeBufferSetDataPartial.FindKernel(COMPUTE_SET_DATA_SINGLE_KERNEL);
                }
            }
        }
        #endregion CS Set Data Partial

        #region CS Billboard
        public static readonly string COMPUTE_BILLBOARD_RESOURCE_PATH = "Compute/CSBillboard";
        public static readonly string COMPUTE_BILLBOARD_DILATION_KERNEL = "CSBillboardDilate";
        #endregion CS Billboard

        #region CS Texture Utils

        public static readonly string COMPUTE_TEXTURE_UTILS_PATH = "Compute/CSTextureUtils";
        public static readonly string COMPUTE_COPY_TEXTURE_KERNEL = "CSCopyTexture";

        public static class CopyTextureKernelProperties
        {
            public static readonly int SOURCE_TEXTURE = Shader.PropertyToID("source");
            public static readonly int DESTINATION_TEXTURE = Shader.PropertyToID("destination");
            public static readonly int OFFSET_X = Shader.PropertyToID("offsetX");
            public static readonly int SOURCE_SIZE_X = Shader.PropertyToID("sourceSizeX");
            public static readonly int SOURCE_SIZE_Y = Shader.PropertyToID("sourceSizeY");
            public static readonly int DESTINATION_SIZE_X = Shader.PropertyToID("destinationSizeX");
            public static readonly int DESTINATION_SIZE_Y = Shader.PropertyToID("destinationSizeY");
            public static readonly int REVERSE_Z = Shader.PropertyToID("reverseZ");
        }

        public static ComputeShader computeTextureUtils;
        public static int computeTextureUtilsCopyTextureId;

        public static void SetupComputeTextureUtils()
        {
            if (computeTextureUtils == null)
            {
                computeTextureUtils = Resources.Load<ComputeShader>(COMPUTE_TEXTURE_UTILS_PATH);

                if (computeTextureUtils != null)
                {
                    computeTextureUtilsCopyTextureId = computeTextureUtils.FindKernel(COMPUTE_COPY_TEXTURE_KERNEL);
                }
            }
        }

        #endregion CS Texture Utils

        #region CS Grass Instantiation
        // Grass compute shader constants
        public static readonly string GRASS_INSTANTIATION_KERNEL = "CSInstancedRenderingGrassInstantiationKernel";
        public static readonly string GRASS_INSTANTIATION_RESOURCE_PATH = "Compute/CSInstancedRenderingGrassInstantiationKernel";
        public static class GrassKernelProperties
        {
            public static readonly int DETAIL_MAP_DATA_BUFFER = Shader.PropertyToID("detailMapData");
            public static readonly int HEIGHT_MAP_DATA_BUFFER = Shader.PropertyToID("heightMapData");
            public static readonly int COUNTER_BUFFER = Shader.PropertyToID("counterBuffer");
            public static readonly int TERRAIN_DETAIL_RESOLUTION_DATA = Shader.PropertyToID("detailResolution");
            public static readonly int TERRAIN_HEIGHT_RESOLUTION_DATA = Shader.PropertyToID("heightResolution");
            public static readonly int GRASS_START_POSITION_DATA = Shader.PropertyToID("startPosition");
            public static readonly int TERRAIN_SIZE_DATA = Shader.PropertyToID("terrainSize");
            public static readonly int DETAIL_SCALE_DATA = Shader.PropertyToID("detailScale");
            public static readonly int DETAIL_AND_HEIGHT_MAP_SIZE_DATA = Shader.PropertyToID("detailAndHeightMapSize");
            public static readonly int HEALTHY_DRY_NOISE_TEXTURE = Shader.PropertyToID("healthyDryNoiseTexture");
            public static readonly int NOISE_SPREAD = Shader.PropertyToID("noiseSpread");
            public static readonly int DETAIL_UNIQUE_VALUE = Shader.PropertyToID("detailUniqueValue");
            public static readonly int DETAIL_DENSITY = Shader.PropertyToID("detailDensity");
        }
        #endregion CS Grass Instantiation

        #region CS Tree Instantiation

        public static readonly string TREE_INSTANTIATION_KERNEL = "CSTreeInstantiationKernel";
        public static readonly string TREE_INSTANTIATION_RESOURCE_PATH = "Compute/CSTreeInstantiationKernel";

        public static class TreeKernelProperties
        {
            public static readonly int TREE_DATA = Shader.PropertyToID("treeData");
            public static readonly int TREE_SCALES = Shader.PropertyToID("treeScales");
            public static readonly int TERRAIN_POSITION = Shader.PropertyToID("terrainPosition");
            public static readonly int IS_APPLY_ROTATION = Shader.PropertyToID("isApplyRotation");
            public static readonly int IS_APPLY_TERRAIN_HEIGHT = Shader.PropertyToID("isApplyTerrainHeight");
            public static readonly int PROTOTYPE_INDEX = Shader.PropertyToID("prototypeIndex");
        }

        #endregion CS Tree Instantiation

        #region CS Runtime Modification
        public static readonly string COMPUTE_RUNTIME_MODIFICATION_RESOURCE_PATH = "Compute/CSRuntimeModification";
        public static readonly string COMPUTE_TRANSFORM_OFFSET_KERNEL = "CSInstancedTransformOffsetKernel";
        public static readonly string COMPUTE_REMOVE_INSIDE_BOUNDS_KERNEL = "CSRemoveInsideBounds";
        public static readonly string COMPUTE_REMOVE_INSIDE_BOX_KERNEL = "CSRemoveInsideBox";
        public static readonly string COMPUTE_REMOVE_INSIDE_SPHERE_KERNEL = "CSRemoveInsideSphere";
        public static readonly string COMPUTE_REMOVE_INSIDE_CAPSULE_KERNEL = "CSRemoveInsideCapsule";

        public static class RuntimeModificationKernelProperties
        {
            public static readonly int BUFFER_PARAMETER_POSITION_OFFSET = Shader.PropertyToID("positionOffset");
            public static readonly int BUFFER_PARAMETER_MODIFIER_TRANSFORM = Shader.PropertyToID("modifierTransform");
            public static readonly int BUFFER_PARAMETER_MODIFIER_RADIUS = Shader.PropertyToID("modifierRadius");
            public static readonly int BUFFER_PARAMETER_MODIFIER_HEIGHT = Shader.PropertyToID("modifierHeight");
            public static readonly int BUFFER_PARAMETER_MODIFIER_AXIS = Shader.PropertyToID("modifierAxis");
        }

        public static ComputeShader computeRuntimeModification;
        public static int computeBufferTransformOffsetId;
        public static int computeRemoveInsideBoundsId;
        public static int computeRemoveInsideBoxId;
        public static int computeRemoveInsideSphereId;
        public static int computeRemoveInsideCapsuleId;

        public static void SetupComputeRuntimeModification()
        {
            if (computeRuntimeModification == null)
            {
                computeRuntimeModification = Resources.Load<ComputeShader>(COMPUTE_RUNTIME_MODIFICATION_RESOURCE_PATH);

                if (computeRuntimeModification != null)
                {
                    computeBufferTransformOffsetId = computeRuntimeModification.FindKernel(COMPUTE_TRANSFORM_OFFSET_KERNEL);
                    computeRemoveInsideBoundsId = computeRuntimeModification.FindKernel(COMPUTE_REMOVE_INSIDE_BOUNDS_KERNEL);
                    computeRemoveInsideBoxId = computeRuntimeModification.FindKernel(COMPUTE_REMOVE_INSIDE_BOX_KERNEL);
                    computeRemoveInsideSphereId = computeRuntimeModification.FindKernel(COMPUTE_REMOVE_INSIDE_SPHERE_KERNEL);
                    computeRemoveInsideCapsuleId = computeRuntimeModification.FindKernel(COMPUTE_REMOVE_INSIDE_CAPSULE_KERNEL);
                }
            }
        }

        #endregion CS Runtime Modification

        #region Shaders
        // Unity Shader Names
        public static readonly string SHADER_UNITY_STANDARD = "Standard";
        public static readonly string SHADER_UNITY_STANDARD_SPECULAR = "Standard (Specular setup)";
        public static readonly string SHADER_UNITY_STANDARD_ROUGHNESS = "Standard (Roughness setup)";
        public static readonly string SHADER_UNITY_VERTEXLIT = "VertexLit";

        public static readonly string SHADER_UNITY_SPEED_TREE = "Nature/SpeedTree";
        public static readonly string SHADER_UNITY_SPEED_TREE_URP = "Universal Render Pipeline/Nature/SpeedTree7";
        public static readonly string SHADER_UNITY_SPEED_TREE_8 = "Nature/SpeedTree8";
        public static readonly string SHADER_UNITY_SPEED_TREE_8_URP = "Universal Render Pipeline/Nature/SpeedTree8";
        public static readonly string SHADER_UNITY_TREE_CREATOR_BARK = "Nature/Tree Creator Bark";
        public static readonly string SHADER_UNITY_TREE_CREATOR_BARK_OPTIMIZED = "Hidden/Nature/Tree Creator Bark Optimized";
        public static readonly string SHADER_UNITY_TREE_CREATOR_LEAVES = "Nature/Tree Creator Leaves";
        public static readonly string SHADER_UNITY_TREE_CREATOR_LEAVES_OPTIMIZED = "Hidden/Nature/Tree Creator Leaves Optimized";
        public static readonly string SHADER_UNITY_TREE_CREATOR_LEAVES_FAST = "Nature/Tree Creator Leaves Fast";
        public static readonly string SHADER_UNITY_TREE_CREATOR_LEAVES_FAST_OPTIMIZED = "Hidden/Nature/Tree Creator Leaves Fast Optimized";
        public static readonly string SHADER_UNITY_TREE_SOFT_OCCLUSION_BARK = "Nature/Tree Soft Occlusion Bark";
        public static readonly string SHADER_UNITY_TREE_SOFT_OCCLUSION_LEAVES = "Nature/Tree Soft Occlusion Leaves";

        public static readonly string SHADER_UNITY_INTERNAL_ERROR = "Hidden/InternalErrorShader";

        // Default GPU Instanced Shader Names
        public static readonly string SHADER_GPUI_STANDARD = "GPUInstancer/Standard";
        public static readonly string SHADER_GPUI_STANDARD_SPECULAR = "GPUInstancer/Standard (Specular setup)";
        public static readonly string SHADER_GPUI_STANDARD_ROUGHNESS = "GPUInstancer/Standard (Roughness setup)";
        public static readonly string SHADER_GPUI_VERTEXLIT = "GPUInstancer/VertexLit";
        public static readonly string SHADER_GPUI_FOLIAGE = "GPUInstancer/Foliage";
        public static readonly string SHADER_GPUI_FOLIAGE_LWRP = "GPUInstancer/FoliageLWRP";

#if UNITY_2020_2_OR_NEWER
        public static readonly string SHADER_GPUI_FOLIAGE_HDRP = "GPUInstancer/FoliageHDRP_GPUI_SG";
        public static readonly string SHADER_GPUI_FOLIAGE_URP = "GPUInstancer/FoliageURP_GPUI_SG";
#else
        public static readonly string SHADER_GPUI_FOLIAGE_HDRP = "GPUInstancer/FoliageHDRP";
        public static readonly string SHADER_GPUI_FOLIAGE_URP = "GPUInstancer/FoliageURP";
#endif
        public static readonly string SHADER_GPUI_SHADOWS_ONLY = "Hidden/GPUInstancer/ShadowsOnly";

        public static readonly string SHADER_GPUI_HIZ_OCCLUSION_GENERATOR = "Hidden/GPUInstancer/HiZOcclusionGenerator";
        public static readonly string SHADER_GPUI_HIZ_OCCLUSION_DEBUGGER = "Hidden/GPUInstancer/HiZOcclusionDebugger";

        public static readonly string SHADER_GPUI_SPEED_TREE = "GPUInstancer/Nature/SPDTree";
        public static readonly string SHADER_GPUI_SPEED_TREE_8 = "GPUInstancer/Nature/SPDTree8";
        public static readonly string SHADER_GPUI_TREE_PROXY = "Hidden/GPUInstancer/Nature/TreeProxy";
        public static readonly string SHADER_GPUI_TREE_CREATOR_BARK = "GPUInstancer/Nature/Tree Creator Bark";
        public static readonly string SHADER_GPUI_TREE_CREATOR_BARK_OPTIMIZED = "GPUInstancer/Nature/Tree Creator Bark Optimized";
        public static readonly string SHADER_GPUI_TREE_CREATOR_LEAVES = "GPUInstancer/Nature/Tree Creator Leaves";
        public static readonly string SHADER_GPUI_TREE_CREATOR_LEAVES_OPTIMIZED = "GPUInstancer/Nature/Tree Creator Leaves Optimized";
        public static readonly string SHADER_GPUI_TREE_CREATOR_LEAVES_FAST = "GPUInstancer/Nature/Tree Creator Leaves Fast";
        public static readonly string SHADER_GPUI_TREE_CREATOR_LEAVES_FAST_OPTIMIZED = "GPUInstancer/Nature/Tree Creator Leaves Fast Optimized";
        public static readonly string SHADER_GPUI_TREE_SOFT_OCCLUSION_BARK = "GPUInstancer/Nature/Tree Soft Occlusion Bark";
        public static readonly string SHADER_GPUI_TREE_SOFT_OCCLUSION_LEAVES = "GPUInstancer/Nature/Tree Soft Occlusion Leaves";
        public static readonly string SHADER_GPUI_BILLBOARD_2D_RENDERER_TREE = "GPUInstancer/Billboard/2DRendererTree";
        public static readonly string SHADER_GPUI_BILLBOARD_2D_RENDERER_TREECREATOR = "GPUInstancer/Billboard/2DRendererTreeCreator";
        public static readonly string SHADER_GPUI_BILLBOARD_2D_RENDERER_SOFTOCCLUSION = "GPUInstancer/Billboard/2DRendererSoftOcclusion";
        public static readonly string SHADER_GPUI_BILLBOARD_2D_RENDERER_STANDARD = "GPUInstancer/Billboard/2DRendererStandard";
        public static readonly string SHADER_GPUI_BILLBOARD_ALBEDO_BAKER = "Hidden/GPUInstancer/Billboard/AlbedoBake";
        public static readonly string SHADER_GPUI_BILLBOARD_NORMAL_BAKER = "Hidden/GPUInstancer/Billboard/NormalBake";
#endregion Shaders

#region Paths
        // GPUInstancer Default Paths
        public static readonly string DEFAULT_PATH_GUID = "954b4ec3db4c00f46a67fcb9b4f72411";
        public static readonly string RESOURCES_PATH = "Resources/";
        public static readonly string SETTINGS_PATH = "Settings/";
        public static readonly string SHADERS_PATH = "Shaders/";
        public static readonly string EDITOR_TEXTURES_PATH = "Textures/Editor/";
        public static readonly string NOISE_TEXTURES_PATH = "Textures/Noise/";
        public static readonly string GPUI_SETTINGS_DEFAULT_NAME = "GPUInstancerSettings";
        public static readonly string SHADER_BINDINGS_DEFAULT_NAME = "GPUInstancerShaderBindings";
        public static readonly string BILLBOARD_ATLAS_BINDINGS_DEFAULT_NAME = "GPUInstancerBillboardAtlasBindings";
        public static readonly string SHADER_VARIANT_COLLECTION_DEFAULT_NAME = "GPUIShaderVariantCollection";
        public static readonly string PROTOTYPES_TERRAIN_PATH = "PrototypeData/Terrain/";
        public static readonly string PROTOTYPES_PREFAB_PATH = "PrototypeData/Prefab/";
        public static readonly string PROTOTYPES_BILLBOARD_TEXTURES_PATH = "PrototypeData/Billboard/Textures/";
        public static readonly string PROTOTYPES_SHADERS_PATH = "PrototypeData/Shaders/";
        public static readonly string PROTOTYPES_SERIALIZED_PATH = "PrototypeData/SerializedTransforms/";
        public static readonly string FOLIAGE_SHADER_LWRP_PACKAGE_PATH = "Extras/GPUI_Foliage_LWRP_Support.unitypackage";
#if UNITY_2020_2_OR_NEWER
        public static readonly string FOLIAGE_SHADER_URP_PACKAGE_PATH = "Extras/GPUI_Foliage_URP_Support_(10_&_Later).unitypackage";
        public static readonly string FOLIAGE_SHADER_HDRP_PACKAGE_PATH = "Extras/GPUI_Foliage_HDRP_Support_(10_&_Later).unitypackage";
#else
        public static readonly string FOLIAGE_SHADER_URP_PACKAGE_PATH = "Extras/GPUI_Foliage_URP_Support_(8_&_Before).unitypackage";
        public static readonly string FOLIAGE_SHADER_HDRP_PACKAGE_PATH = "Extras/GPUI_Foliage_HDRP_Support_(8_&_Before).unitypackage";
#endif
        public static readonly string FOLIAGE_SHADER_HDRP_TEMPLATE_MATERIAL_PATH = "Materials/FoliageHDRP_Template";

        private static string _defaultPath;
        public static string GetDefaultPath()
        {
            if (string.IsNullOrEmpty(_defaultPath))
            {
#if UNITY_EDITOR
                _defaultPath = UnityEditor.AssetDatabase.GUIDToAssetPath(DEFAULT_PATH_GUID);
                if (!string.IsNullOrEmpty(_defaultPath))
                    _defaultPath = _defaultPath.Replace("Resources/Editor/GPUInstancerPathLocator.asset", "");
#endif
                if (string.IsNullOrEmpty(_defaultPath))
                    _defaultPath = "Assets/GPUInstancer/";
            }
            return _defaultPath;
        }
#endregion Paths

#region Textures
        // Textures
        public static readonly string DEFAULT_HEALTHY_DRY_NOISE = "Fractal_Simplex_grayscale";
        public static readonly string DEFAULT_WIND_WAVE_NOISE = "Fractal_Simplex_normal";
#endregion Textures

#region Texts
        // Editor Texts
        public static readonly string TEXT_PREFAB_TYPE_WARNING_TITLE = "Prefab Type Warning";
        public static readonly string TEXT_PREFAB_TYPE_WARNING = "GPU Instancer Prefab Manager only accepts user created prefabs. Cannot add selected object.";
        public static readonly string TEXT_TREE_PREFAB_TYPE_WARNING = "GPU Instancer Tree Manager only accepts user created prefabs or model prefabs. Cannot add selected object.";
        public static readonly string TEXT_PREFAB_TYPE_WARNING_3D = "GPU Instancer Prefab Manager only accepts user created prefabs. Please create a prefab from this imported 3D model asset.";
        public static readonly string TEXT_OK = "OK";

        public static readonly string TEXT_deleteConfirmation = "Delete Confirmation";
        public static readonly string TEXT_deleteBillboard = "Do you want to remove the billboard textures of the prototype?";
        public static readonly string TEXT_keepTextures = "Keep Textures";
        public static readonly string TEXT_delete = "Delete";

        public static readonly string ERRORTEXT_cameraNotFound = "Main Camera cannot be found. GPU Instancer needs either an existing camera with the \"Main Camera\" tag on the scene to autoselect it, or a manually specified camera. If you add your camera at runtime, please use the \"GPUInstancerAPI.SetCamera\" API function.";

        // Debug
        public static readonly int DEBUG_INFO_SIZE = 105;

        public static readonly string ERRORTEXT_shaderGraph = "{0} is created with ShaderGraph, and GPUInstacer Setup is not present. Please add GPUInstacer Setup from the ShaderGraph window. Alternatively you can copy the shader code and paste it to a new shader file. When you set this new shader to your material, GPUI can run auto-conversion on this shader.";

#endregion Texts
    }
}