using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace GPUInstancer
{
    public static class GPUInstancerEditorConstants
    {
        public static readonly string GPUI_VERSION = "GPU Instancer v1.4.6";
        public static readonly float GPUI_VERSION_NO = 1.46f;

        public static readonly string PUBLISHER_NAME = "GurBu Technologies";
        public static readonly string PACKAGE_NAME = "GPU Instancer";

        // Editor Text
        public static readonly string TEXT_maxDetailDistance = "Max Detail Distance";
        public static readonly string TEXT_maxTreeDistance = "Max Tree Distance";
        public static readonly string TEXT_detailDensity = "Detail Density";
        public static readonly string TEXT_detailLayer = "Detail Texture Layer";
        public static readonly string TEXT_windVector = "Wind Vector";
        public static readonly string TEXT_healthyDryNoiseTexture = "Healthy/Dry Noise Texture";
        public static readonly string TEXT_windWaveNormalTexture = "Wind Wave Normal Texture";
        public static readonly string TEXT_autoSPCellSize = "Auto SP Cell Size";
        public static readonly string TEXT_preferedSPCellSize = "Prefered SP Cell Size";
        public static readonly string TEXT_storeDetailInstanceData = "Store Detail Instance Data";
        public static readonly string TEXT_showGPUInstancingInfo = "Show GPU Instancing Info";
        public static readonly string TEXT_showRenderedAmount = "Show Rendered Amount";
        public static readonly string TEXT_simulateAtEditor = "Simulate At Scene Camera";
        public static readonly string TEXT_simulateAtEditorPrep = "Preparing Simulation";
        public static readonly string TEXT_simulateAtEditorStop = "Stop Simulation";
        public static readonly string TEXT_generatePrototypes = "Generate Prototypes";
        public static readonly string TEXT_prototypes = "Prototypes";
        public static readonly string TEXT_add = "Add\n<size=8>Click / Drop</size>";
        public static readonly string TEXT_addTextMode = "Add Prototype";
        public static readonly string TEXT_addMulti = "<size=8>Multi. Add</size>";
        public static readonly string TEXT_addMultiTextMode = "Add Multiple";
        public static readonly string TEXT_isShadowCasting = "Is Shadow Casting";
        public static readonly string TEXT_isFrustumCulling = "Is Frustum Culling";
        public static readonly string TEXT_isOcclusionCulling = "Is Occlusion Culling";
        public static readonly string TEXT_frustumOffset = "Frustum Offset";
        public static readonly string TEXT_occlusionOffset = "Occlusion Cull Offset";
        public static readonly string TEXT_occlusionAccuracy = "Occlusion Cull Accuracy";
        public static readonly string TEXT_minCullingDistance = "Min. Culling Distance";
        public static readonly string TEXT_maxDistance = "Min-Max Distance";
        public static readonly string TEXT_boundsOffset = "Bounds Size Offset";
        public static readonly string TEXT_actions = "Actions";
        public static readonly string TEXT_advancedActions = "Advanced Actions";
        public static readonly string TEXT_delete = "Delete";
        public static readonly string TEXT_remove = "Remove";
        public static readonly string TEXT_keepPrototypeDefinition = "Keep Prototype Definition";
        public static readonly string TEXT_crossQuads = "Cross Quads";
        public static readonly string TEXT_quadCount = "Quad Count";
        public static readonly string TEXT_isBillboard = "Billboard";
        public static readonly string TEXT_billboardFaceCamPos = "Billboard Face Cam. Pos.";
        public static readonly string TEXT_CQBillboardFaceCamPos = "CQ Billboard Face Cam. Pos.";
        public static readonly string TEXT_billboardDistance = "CQ Billboard Distance";
        public static readonly string TEXT_billboardDistanceDebug = "CQ Distance Debug";
        public static readonly string TEXT_billboardDistanceDebugColor = "Debug Color";
        public static readonly string TEXT_detailScale = "Detail Scale";
        public static readonly string TEXT_useCustomMaterialForTextureDetail = "Custom Material";
        public static readonly string TEXT_useCustomHealthyDryNoiseTexture = "Custom Noise Texture";
        public static readonly string TEXT_textureDetailCustomMaterial = "Custom Material";
        public static readonly string TEXT_detailHealthyColor = "Detail Healthy Color";
        public static readonly string TEXT_detailDryColor = "Detail Dry Color";
        public static readonly string TEXT_noiseSpread = "Noise Spread";
        public static readonly string TEXT_ambientOcclusion = "Ambient Occlusion";
        public static readonly string TEXT_gradientPower = "Gradient Power";
        public static readonly string TEXT_windCounterSway = "Wind Counter Sway";
        public static readonly string TEXT_windIdleSway = "Wind Idle Sway";
        public static readonly string TEXT_windWavesOn = "Wind Waves";
        public static readonly string TEXT_windWaveTintColor = "Wind Wave Tint Color";
        public static readonly string TEXT_windWaveSize = "Wind Wave Size";
        public static readonly string TEXT_windWaveTint = "Wind Wave Tint";
        public static readonly string TEXT_windWaveSway = "Wind Wave Sway";
        public static readonly string TEXT_deleteConfirmation = "Delete Confirmation";
        public static readonly string TEXT_deleteAreYouSure = "Are you sure you want to remove the prototype from prototype list?";
        public static readonly string TEXT_deletePrototypeAreYouSure = "Do you wish to delete the prototype settings?";
        public static readonly string TEXT_close = "Close";
        public static readonly string TEXT_cancel = "Cancel";
        public static readonly string TEXT_paintOnTerrain = "Paint On Terrain";
        public static readonly string TEXT_enableRuntimeModifications = "Enable Runtime Modifications";
        public static readonly string TEXT_addRemoveInstancesAtRuntime = "Add/Remove Instances At Runtime";
        public static readonly string TEXT_extraBufferSize = "Extra Buffer Size";
        public static readonly string TEXT_addRuntimeHandlerScript = "Auto. Add/Remove Instances";
        public static readonly string TEXT_autoUpdateTransformData = "Auto. Update Transform Data";
        public static readonly string TEXT_startWithRigidBody = "Start With RigidBody";
        public static readonly string TEXT_registerPrefabsInScene = "Register Instances in Scene";
        public static readonly string TEXT_registeredPrefabs = "Registered Instances";
        public static readonly string TEXT_applyChangesToTerrain = "Apply Changes To Terrain";
        public static readonly string TEXT_generatePrototypesConfirmation = "Generate Prototypes Confirmation";
        public static readonly string TEXT_generatePrototypeAreYouSure = "Are you sure you want to generate prototypes from terrain?";
        public static readonly string TEXT_debug = "Debug";
        public static readonly string TEXT_detailGlobal = "Global Detail Values";
        public static readonly string TEXT_treeGlobal = "Global Tree Values";
        public static readonly string TEXT_prefabGlobal = "Global Prefab Values";
        public static readonly string TEXT_sceneSettings = "Scene Settings";
        public static readonly string TEXT_autoSelectCamera = "Auto Select Camera";
        public static readonly string TEXT_useCamera = "Use Camera";
        public static readonly string TEXT_renderOnlySelectedCamera = "Use Selected Camera Only";
        public static readonly string TEXT_useManagerFrustumCulling = "Use Frustum Culling";
        public static readonly string TEXT_useManagerOcclusionCulling = "Use Occlusion Culling";
        public static readonly string TEXT_useOriginalShaderForShadow = "Shadow with Original Shader";
        public static readonly string TEXT_isLODCrossFade = "LOD Cross Fade";
        public static readonly string TEXT_isLODCrossFadeAnimate = "Animate Cross-Fading";
        public static readonly string TEXT_lodFadeTransitionWidth = "LOD Fade Transition Width";
        public static readonly string TEXT_lodBiasAdjustment = "LOD Bias Adjustment";
        public static readonly string TEXT_info = "Info";
        public static readonly string TEXT_noPrefabInstanceFound = "There are no prefab instances found in the scene.";
        public static readonly string TEXT_noPrefabInstanceSelected = "Please select at least one prefab instance to import to Prefab Manager.";
        public static readonly string TEXT_continue = "Continue";
        public static readonly string TEXT_confirmBillboardGeneration = "Unsupported Billboard Warning";
        public static readonly string TEXT_regenerateBillboardsConfirmation = "Regenerate Billboards Confirmation";
        public static readonly string TEXT_regenerateBillboardsAreYouSure = "Are you sure you want to regenerate all of the billboard textures for the prototypes defined on this manager?";

        public static readonly string TEXT_terrain = "Terrain";
        public static readonly string TEXT_prefabObject = "Prefab Object";
        public static readonly string TEXT_prototypeTexture = "Prototype Texture";
        public static readonly string TEXT_prototypeSO = "Prototype SO";
        public static readonly string TEXT_terrainSettingsSO = "Terrain Settings SO";
        public static readonly string TEXT_setTerrain = "Set Terrain\n<size=8>Click / Drop</size>";
        public static readonly string TEXT_removeTerrain = "Unset Terrain";
        public static readonly string TEXT_removeTerrainConfirmation = "Unset Terrain";
        public static readonly string TEXT_removeTerrainAreYouSure = "Are you sure you want to unset Terrain Instancing Data?";
        public static readonly string TEXT_unset = "Unset";
        public static readonly string TEXT_goToGPUInstancerDetail = "Go To GPUI Detail Manager";
        public static readonly string TEXT_goToGPUInstancerTree = "Go To GPUI Tree Manager";
        public static readonly string TEXT_showHelpTooltip = "Show Help";
        public static readonly string TEXT_hideHelpTooltip = "Hide Help";
        public static readonly string TEXT_foliageShaderProperties = "Foliage Shader Properties";
        public static readonly string TEXT_windSettings = "Wind Settings";
        public static readonly string TEXT_renderSettings = "Render Settings";
        public static readonly string TEXT_detailProperties = "Detail Properties";
        public static readonly string TEXT_prefabRuntimeSettings = "Runtime Settings";
        public static readonly string TEXT_treeSettings = "Tree Settings";
        public static readonly string TEXT_billboardAlbedo = "Billboard Albedo Texture";
        public static readonly string TEXT_billboardNormal = "Billboard Normal Texture";
        public static readonly string TEXT_customBillboardMesh = "Billboard Mesh";
        public static readonly string TEXT_customBillboardMaterial = "Billboard Material";
        public static readonly string TEXT_isBillboardShadowCasting = "Is Shadow Casting";
        public static readonly string TEXT_useRandomTreeRotation = "Use Random Rotation";
        public static readonly string TEXT_usePrefabScale = "Use Prefab Scale";
        public static readonly string TEXT_useTerrainHeight = "Use Random Height";

        public static readonly string TEXT_billboardSettings = "Billboard Settings";
        public static readonly string TEXT_useGeneratedBillboard = "Generate Billboard";
        public static readonly string TEXT_billboardFrameCount = "Billboard Frame Count";
        public static readonly string TEXT_billboardQuality = "Billboard Quality";
        public static readonly string TEXT_generateBillboard = "Generate Billboard";
        public static readonly string TEXT_regenerateBillboard = "Regenerate Billboard";
        public static readonly string TEXT_showBillboard = "Show Billboard";
        public static readonly string TEXT_billboardBrightness = "Billboard Brightness";
        public static readonly string TEXT_overrideOriginalCutoff = "Override Original Cutoff";
        public static readonly string TEXT_overrideCutoffAmount = "Override Cutoff Amount";
        public static readonly string TEXT_generatedBillboardDistance = "Billboard Distance";
        public static readonly string TEXT_replaceLODCull = "Replace LOD Culled With Billboard";
        public static readonly string TEXT_regenerateBillboards = "Regenerate Billboards";
        public static readonly string TEXT_useCustomBillboard = "Use Custom Billboard";

        public static readonly string[] TEXT_BillboardQualityOptions = { "Low (1024)", "Mid (2048)", "High (4096)", "Very High (8192)" };


        public static readonly string TEXT_mapMagicSet = "Set Map Magic";
        public static readonly string TEXT_mapMagicImporter = "Map Magic Importer";
        public static readonly string TEXT_mapMagicImportDetails = "Import Grass";
        public static readonly string TEXT_mapMagicImportTrees = "Import Trees";
        public static readonly string TEXT_mapMagicImportObjects = "Import Objects";
        public static readonly string TEXT_mapMagicObjectsList = "Map Magic Objects List";
        public static readonly string TEXT_mapMagicImport = "Import";
        public static readonly string TEXT_mapMagicDetailPrototypes = "Detail Prototypes";
        public static readonly string TEXT_mapMagicTreePrototypes = "Tree Prototypes";
        public static readonly string TEXT_mapMagicPrefabPrototypes = "Prefab Prototypes";

        public static readonly string TEXT_prefabInstancingActive = "Instancing active with ID: ";
        public static readonly string TEXT_prefabInstancingDisabled = "Instancing disabled for ID: ";
        public static readonly string TEXT_prefabInstancingNone = "Instancing has not been initialized";

        public static readonly string TEXT_importSelectedPrefabs = "Import Selected Prefabs";

        public static readonly string TEXT_shadows = "Shadows";
        public static readonly string TEXT_useCustomShadowDistance = "Use Custom Shadow Distance";
        public static readonly string TEXT_shadowDistance = "Shadow Distance";
        public static readonly string TEXT_culling = "Culling";
        public static readonly string TEXT_LOD = "LOD";
        public static readonly string TEXT_cullShadows = "Use Culling For Shadows";

        public static readonly string TEXT_disableMeshRenderers = "Disable Mesh Renderers";
        public static readonly string TEXT_disableMeshRenderersSimulation = "Simulate Disabled Mesh Renderers";
        public static readonly string TEXT_disableMeshRenderersAreYouSure = "This action will disable all the Mesh Renderers / LOD Groups / Rigidbodies in this prefab.\n\nThis will speed up initialization operations on the Prefab Manager, since GPUI will not loop through prefab instances to disable these components at runtime.\n\nHowever, these objects will be visible only during play mode and while the Prefab Manager is active and running. You will be able to use this button again to enable the Mesh Renderers, but it is recommended to do this after you finished your scene design.\n\nAfter you do this, you should not manually enable/disable these components on the prefab or its instances.\n\nAre you sure you wish to Disable Mesh Renderers?";

        public static readonly string TEXT_prefabInstanceSerialization = "Prefab Instance Serialization";
        public static readonly string TEXT_prefabInstanceSerializationAreYouSure = "WARNING: Make sure to backup your scene before using this method.\n\nThis action will remove the registered prefab instances from the scene and save their transform data into a TextAsset.\n\nThis is helpfull to save memory and speed up initialization by getting rid of unnecessary GameObjects.\n\nHowever, these objects will be visible only during play mode or editor simulation and while the Prefab Manager is active and running. You will be able to use this button again to recreate the GameObjects with the same transform data, but it is recommended to do this after you finished your scene design.\n\n Are you sure you wish to Serialize Registered Instances?";
        public static readonly string TEXT_prefabInstanceSerializationYes = "Yes, I made a backup.";

        public static readonly string TEXT_settingAutoGenerateBillboards = "Auto Generate Billboards";
        public static readonly string TEXT_settingAutoShaderConversion = "Auto Shader Conversion";
        public static readonly string TEXT_settingAutoShaderVariantHandling = "Auto Shader Variant Handling";
        public static readonly string TEXT_settingGenerateShaderVariant = "Generate Shader Variant Collection";
        public static readonly string TEXT_settingDisableICWarning = "Disable Instance Count Warnings";
        public static readonly string TEXT_settingMaxDetailDist = "Max Detail Distance";
        public static readonly string TEXT_settingMaxTreeDist = "Max Tree Distance";
        public static readonly string TEXT_settingMaxPrefabDist = "Max Prefab Distance";
        public static readonly string TEXT_settingMaxPrefabBuff = "Max Prefab Extra Buffer Size";
        public static readonly string TEXT_settingCustomPrevBG = "Custom Preview BG Color";
        public static readonly string TEXT_settingPrevBGColor = "Preview BG Color";
        public static readonly string TEXT_settingInstancingBoundsSize = "Instancing Bounds Size";
        public static readonly string TEXT_settingTestBothEyesForVROcclusion = "Test Both Eyes for VR Occ. Culling";
        public static readonly string TEXT_settingVRRenderingMode = "VR Rendering Mode";
        
        public static readonly string TEXT_settingHasCustomRenderingSettings = "Use Custom Rendering Settings";
        public static readonly string TEXT_settingComputeThread = "Max. Compute Thread Count";
        public static readonly string TEXT_settingMatrixHandlingType = "Max. Compute Buffers";
        public static readonly string TEXT_settingOcclusionCullingType = "Occlusion Culling Type";

        // Editor HelpText
        public static readonly string HELPTEXT_camera = "The camera to use for GPU Instancing. When \"Auto Select Camera\" checkbox is active, GPU Instancer will use the first camera with the \"MainCamera\" tag at startup. If the checkbox is inactive, the desired camera can be set manually. GPU Instancer uses this camera for various calculations including culling operations.";
        public static readonly string HELPTEXT_renderOnlySelectedCamera = "If \"Use Selected Camera Only\" is enabled, instances will only be rendered with the selected camera. They will not be rendered in other active cameras (including the Scene View camera). This may improve performance if there are other cameras in the scene that are mainly used for camera effects, weather/water effects, reflections, etc. Using this option will make them ignore GPUI, increasing performance. Please note that the scene view during play mode will no longer show GPUI instances if this is selected.";
        public static readonly string HELPTEXT_managerFrustumCulling = "\"Use Frustum Culling\" toggles frustum culling globally for all the prototypes in this manager. By turning frustum culling off, you can view GPUI prototypes in all active cameras since they will not be culled. However, please note that turning frustum culling off may result in a lower FPS than having it on. If turned on, you can still turn frustum culling off for individual prototypes.";
        public static readonly string HELPTEXT_managerOcclusionCulling = "\"Use Occlusion Culling\" toggles occlusion culling globally for all the prototypes in this manager. By using occlusion culling, the prototypes that are not actually visible at runtime will not be rendered. This will increase rendering performance depending on the amount of occluded geometry. Please note that occlusion culling may lag if the FPS falls to extremely low levels. If this is a possibility, it could be better to turn it off.";
        public static readonly string HELPTEXT_terrain = "The \"Paint On Terrain\" button is used to navigate to the Unity terrain component that this manager is referencing. Details should be painted on the terrain using Unity's native tools as usual. Detail data on the terrain will be automatically detected by GPU Instancer.\n\nThe \"Unset Terrain\" button is used to disable GPU Instancing on the terrain. It can later be enabled again by using the \"Set Terrain\" button.";
        public static readonly string HELPTEXT_simulator = "The \"Simulate At Scene Camera\" button can be used to render the terrain details on the scene camera using the current GPU Instancer setup. This simulation is designed to provide a fast sneak peak of the GPU Instanced terrain details without having to enter play mode.";
        public static readonly string HELPTEXT_maxDetailDistance = "\"Max Detail Distance\" defines the maximum distance from the camera within which the terrain details will be rendered. Details that are farther than the specified distance will not be visible. This setting also provides the upper limit for the maximum view distance of each detail prototype.";
        public static readonly string HELPTEXT_maxTreeDistance = "\"Max Tree Distance\" defines the maximum distance from the camera within which the terrain trees will be rendered. Trees that are farther than the specified distance will not be visible. This setting also provides the upper limit for the maximum view distance of each tree prototype.";
        public static readonly string HELPTEXT_detailDensity = "\"Detail Density\" goes from 0.0 to 1.0, with 1.0 being the original density, and lower numbers resulting in less detail objects being rendered.";
        public static readonly string HELPTEXT_detailLayer = "\"Detail Texture Layer\" defines the Layer that the Texture Terrain details will be in. The terrain details that use prefabs will get the Layer value from the prefab object.";
        public static readonly string HELPTEXT_windVector = "The \"Wind Vector\" specifies the [X, Z] vector (world axis) of the wind for all the prototypes (in this terrain) that use the \"GPUInstancer/Foliage\" shader (which is also the default shader for the texture type grass details). This vector supplies both direction and magnitude information for wind.";
        public static readonly string HELPTEXT_healthyDryNoiseTexture = "The \"Healthy/Dry Noise Texture\" can be used to specify the Healthy Color / Dry Color variation for all the prototypes that use the \"GPUInstancer/Foliage\" shader in this terrain (which is also the default shader for the texture type grass details). Texture type detail prototypes are also scaled by this noise. This image must be a greyscale noise texture.";
        public static readonly string HELPTEXT_windWaveNormalTexture = "The \"Wind Wave Normal Texture\" can be used to specify the vectors of all wind animations and coloring for all the prototypes that use the \"GPUInstancer/Foliage\" shader in this terrain (which is also the default shader for the texture type grass details). This image must be a normal map noise texture.";
        public static readonly string HELPTEXT_spatialPartitioningCellSize = "Detail Manager uses spatial partitioning for loading and unloading detail instances from GPU memory according to camera position. Detail instances are grouped in cells with a calculated size. By selecting \"Auto SP Cell Size\", you can let the manager decide which cell size to use. If you deselect \"Auto SP Cell Size\", you can determine a \"Prefered SP Cell Size\".";
        public static readonly string HELPTEXT_generatePrototypesDetail = "The \"Generate Prototypes\" button can be used to synchronize the detail prototypes on the Unity terrain and GPU Instancer. It will reset the detail prototype properties with those from the Unity terrain, and use default values for properties that don't exist on the Unity terrain.";
        public static readonly string HELPTEXT_generatePrototypesTree = "The \"Generate Prototypes\" button can be used to synchronize the tree prototypes on the Unity terrain and GPU Instancer. It will reset the tree prototype properties with those from the Unity terrain, and use default values for properties that don't exist on the Unity terrain.";
        public static readonly string HELPTEXT_prototypes = "\"Prototypes\" show the list of objects that will be used in GPU Instancer. To modify a prototype, click on its icon or text. Use the \"Text Mode\" or \"Icon Mode\" button to switch between preview modes.";
        public static readonly string HELPTEXT_addprototypeprefab = "Click on \"Add\" button and select a prefab to add a prefab prototype to the manager. Note that prefab manager only accepts user created prefabs. It will not accept prefabs that are generated when importing your 3D model assets.";
        public static readonly string HELPTEXT_addprototypedetail = "Click on \"Add\" button and select a texture or prefab to add a detail prototype to the manager.";
        public static readonly string HELPTEXT_addprototypetree = "Click on \"Add\" button and a prefab to add a tree prototype to the manager.";
        public static readonly string HELPTEXT_registerPrefabsInScene = "The \"Register Prefabs In Scene\" button can be used to register the prefab instances that are currently in the scene, so that they can be used by GPU Instancer. For adding new instances at runtime check API documentation.";
        public static readonly string HELPTEXT_setTerrain = "Detail Manager requires a Unity terrain to render detail instances. You can specify a Unity terrain to use with GPU Instancer by either clicking on the \"Set Terrain\" button and choosing a Unity terrain, or dragging and dropping a Unity terrain on it.";
        public static readonly string HELPTEXT_setTerrainTree = "Tree Manager requires a Unity terrain to render tree instances. You can specify a Unity terrain to use with GPU Instancer by either clicking on the \"Set Terrain\" button and choosing a Unity terrain, or dragging and dropping a Unity terrain on it.";
        public static readonly string HELPTEXT_useOriginalShaderForShadow = "When \"Shadow with Original Shader\" is enabled, GPU Instancer will use the original shader on your prototype's materials for the shadows of the objects that fall outside the camera's view frustum. If this option is turned off, a default shadowcasting shader will be used instead. Use this option if your shader does vertex operations on the mesh or supports alpha transparency. This is not the default behavior since some shaders might have complex operations on them which might be unnecessary to run for objects outside the view frustum.";

        public static readonly string HELPTEXT_isLODCrossFade = "\"LOD Cross-Fade\" enables cross-fade style blending between the LOD levels of this prototype. This can have a minor impact on performance since during cross-fading, both LOD levels will be rendering.";
        public static readonly string HELPTEXT_isLODCrossFadeAnimate = "\"Animate Cross-Fading\" animates cross-fading instead of it being distance based. The animation starts with the first occurrence of the changed LOD. During the animation, both LOD levels will be rendered.";
        public static readonly string HELPTEXT_lodFadeTransitionWidth = "You can use the \"LOD Fade Transition Width\" value to define the cross-fading zone between LOD Levels. This zone is the distance in which the prototype LODs are cross faded. Higher numbers result in wider transition zones. In the transition zone, both LOD levels will be rendered.";
        public static readonly string HELPTEXT_lodBiasAdjustment = "The \"LOD Bias Adjustment\" value effects the LOD level distances per prototype. When it is set to a value less than 1, it favors less detail. A value of more than 1 favors greater detail.";

        public static readonly string HELPTEXT_isShadowCasting = "\"Is Shadow Casting\" specifies whether the object will cast shadows or not. Shadow casting requires extra shadow passes in the shader resulting in additional rendering operations. GPU Instancer uses various techniques that boost the performance of these operations, but turning shadow casting off completely will increase performance.";
        public static readonly string HELPTEXT_isFrustumCulling = "\"Is Frustum Culling\" specifies whether the objects that are not in the selected camera's view frustum will be rendered or not. If enabled, GPU Instancer will not render the objects that are outside the selected camera's view frustum. This will increase performance. It is recommended to turn frustum culling on unless there are multiple cameras rendering the scene at the same time.";
        public static readonly string HELPTEXT_isOcclusionCulling = "\"Is Occlusion Culling\" specifies whether the objects that are occluded by other objects will be rendered or not. If enabled, GPU Instancer will not render the objects that are behind others and would normally not be visible. This will increase performance. It is recommended to turn occlusion culling on unless there are multiple cameras rendering the scene at the same time.";
        public static readonly string HELPTEXT_frustumOffset = "\"Frustum Offset\" defines the size of the area around the camera frustum planes within which objects will be rendered while frustum culling is enabled. GPU Instancer does frustum culling on the GPU which provides a performance boost. However, if there is a performance hit (usually while rendering an extreme amount of objects in the frustum), and if the camera is moving very fast at the same time, rendering can lag behind the camera movement. This could result in some objects not being rendered around the frustum edges. This offset expands the calculated frustum area so that the renderer can keep up with the camera movement in those cases.";
        public static readonly string HELPTEXT_occlusionOffset = "\"Occlusion Cull Offset\" defines the depth value (ranged between 0 to 1) that the differences will be discarded while calculating culled objects from the depth texture. Higher offset values will result in less culling.";
        public static readonly string HELPTEXT_occlusionAccuracy = "\"Occlusion Cull Accuracy\" defines the accuracy of the occlusion culling. Higher values will result in more accurate culling with higher number of texture samples. \n1 => 5 samples (middle point and corner points)\n2 => 9 samples (adds 1/4 points)\n3 => 17 samples (adds 1/8 and 3/8 points)";
        public static readonly string HELPTEXT_minCullingDistance = "\"Min. Culling Distance\" defines the minimum distance that any kind of culling will occur. If it is a value higher than 0, the instances with a distance less than the specified value to the Camera will not be culled.";
        public static readonly string HELPTEXT_maxDistanceDetail = "\"Min-Max Distance\" defines the minimum and maximum distance from the selected camera within which this prototype will be rendered. This value is limited by the general \"Max Detail Distance\" above.";
        public static readonly string HELPTEXT_maxDistance = "\"Min-Max Distance\" defines the minimum and maximum distance from the selected camera within which this prototype will be rendered.";
        public static readonly string HELPTEXT_boundsOffset = "\"Bounds Size Offset\" can be used to increase the bounding box size of the prototype. This will effect the culling and LOD calculations. It is usefull for prototypes with shaders that modify vertex positions.";
        public static readonly string HELPTEXT_crossQuads = "If \"Cross Quads\" is enabled, a mesh with multiple quads will be generated for this detail texture (instead of a single quad or billboard). The generated quads will be equiangular to each other. Cross quadding means more polygons for a given prototype, so turning this off will increase performance if there will be a huge number of instances of this detail prototype.";
        public static readonly string HELPTEXT_quadCount = "\"Quad Count\" defines the number of generated equiangular quads for this detail texture. Using less quads will increase performance (especially where there will be a huge number of instances of this prototype).";
        public static readonly string HELPTEXT_isBillboard = "If \"Is Billboard\" is enabled, the generated mesh for this prototype will be billboarded. Note that billboarding will turn off automatically if cross quads are enabled.";
        public static readonly string HELPTEXT_billboardFaceCamPos = "If \"Billboard Face Cam. Pos.\" is enabled, the billboards for this prototype will ignore the camera view matrix rotations and will always face the camera position. This is ideal for VR platforms.";
        public static readonly string HELPTEXT_CQBillboardFaceCamPos = "If \"CQ Billboard Face Cam. Pos.\" is enabled, the billboard LODs of this cross quad will ignore the camera view matrix rotations and will always face the camera position. This is ideal for VR platforms.";
        public static readonly string HELPTEXT_billboardDistance = "When Cross Quads is enabled, \"Cross Quad Billboard Distance\" specifies the distance from the selected camera where the objects will be drawn as billboards to increase performance further. This is useful beacause at a certain distance, the difference between a multiple quad mesh and a billboard is barely visible. The value used here is similar to the screenRelativeTransitionHeight property of Unity LOD groups.";
        public static readonly string HELPTEXT_billboardDistanceDebug = "When Cross Quads is enabled, \"Cross Quad Billboard Distance Debug\" can be used to test the Cross Quad billboarding distance.";
        public static readonly string HELPTEXT_billboardDistanceDebugColor = "\"Billboard Distance Debug Color\" can be used to override the billboarded prototypes' colors while testing billboarding distance when using Cross Quads.";
        public static readonly string HELPTEXT_detailScale = "\"Detail Scale\" can be used to set a range for the instance sizes for detail prototypes. For the texture type detail prototypes, this range applies to the \"Healthy/Dry Noise Texture\" above. The values here correspond to the width and height values for the detail prototypes in the Unity terrain. \nX: Minimum Width - Y: Maximum Width\nZ: Minimum Height - W: Maximum Height";
        public static readonly string HELPTEXT_useCustomMaterialForTextureDetail = "\"Custom Material\" can be used to specify that a custom material will be used for a detail prototype instead of the default generated material that uses the \"GPUInstancer/Foliage\" shader. Note that Cross Quads are available for custom materials as well, but their ability to billboard at a distance will not be avalilable since GPU Instancer handles billboarding in the \"GPUInstancer/Foliage\" shader.";
        public static readonly string HELPTEXT_useCustomHealthyDryNoiseTexture = "\"Custom Noise Texture\" can be used to specify that a custom Healthy/Dry Noise Texture will be used for a detail prototype instead of the default.";
        public static readonly string HELPTEXT_textureDetailCustomMaterial = "\"Custom Material\" can be used to specify the custom material to use for a detail prototype instead of the default \"GPUInstancer/Foliage\" shader. The shader that this material uses will be automatically set up for use with GPU Instancer. This process creates a copy of the original shader (in the same folder) that has the necessary lines of code. Note that this process will NOT modify the material.";
        public static readonly string HELPTEXT_detailHealthyColor = "\"Healthy\" color of the the Healthy/Dry noise variation for the prototypes that use the \"GPUInstancer/Foliage\" shader. This corresponds to the \"Healhy Color\" property of the detail prototypes in the Unity terrain. The Healthy/Dry noise texture can be changed globally (terrain specific) for all detail prototypes that use the \"GPUInstancer/Foliage\" shader from the \"Healthy/Dry Noise Texture\" property above.";
        public static readonly string HELPTEXT_detailDryColor = "\"Dry\" color of the the Healthy/Dry noise variation for the prototypes that use the \"GPUInstancer/Foliage\" shader. This corresponds to the \"Dry Color\" property of the detail prototypes in the Unity terrain. The Healthy/Dry noise texture can be changed globally (terrain specific) for all detail prototypes that use the \"GPUInstancer/Foliage\" shader from the \"Healthy/Dry Noise Texture\" property above.";
        public static readonly string HELPTEXT_noiseSpread = "The \"Noise Spread\" property specifies the size of the \"Healthy\" and \"Dry\" patches for the Detail Prototypes that use the \"GPUInstancer/Foliage\" shader. This corresponds to the \"Noise Spread\" propety of the detail prototypes in the Unity terrain. A higher number results in smaller patches. The Healthy/Dry noise texture can be changed globally for all detail prototypes that use the \"GPUInstancer/Foliage\" shader from the \"Healthy/Dry Noise Texture\" property above.";
        public static readonly string HELPTEXT_ambientOcclusion = "The amount of \"Ambient Occlusion\" to apply to the objects that use the \"GPUInstancer/Foliage\" shader.";
        public static readonly string HELPTEXT_gradientPower = "\"GPUInstancer/Foliage\" shader provides an option to darken the lower regions of the mesh that uses it. This results in a more realistic look for foliage. You can set the amount of this darkening effect with the \"Gradient Power\" property, or turn it off completly by setting the slider to zero.";
        public static readonly string HELPTEXT_windIdleSway = "\"Wind Idle Sway\" specifies the amount of idle wind animation for the detail prototypes that use the \"GPUInstancer/Foliage\" shader. This is the wind animation that occurs where wind waves are not present. Turning the slider down to zero disables this idle wind animation.";
        public static readonly string HELPTEXT_windWavesOn = "\"Wind Waves On\" specifies whether there will be wind waves for the detail prototypes that use the \"GPUInstancer/Foliage\" shader. The normals texture that is used to calculate winds can be changed globally (terrain specific) for all detail prototypes that use the \"GPUInstancer/Foliage\" shader from the \"Wind Wave Normal Texture\" property above.";
        public static readonly string HELPTEXT_windWaveTintColor = "\"Wind Wave Tint Color\" is a shader property that acts similar to the \"Grass Tint\" property of the Unity terrain, except it applies on a per-prototype basis. This color applies to the \"Wind Wave Tint\" properties of all the objects that use the \"GPUInstancer/Foliage\" shader (which is also the default shader for the texture type grass details).";
        public static readonly string HELPTEXT_windWaveSize = "\"Wind Wave Size\" specifies the size of the wind waves for the detail prototypes that use the \"GPUInstancer/Foliage\" shader.";
        public static readonly string HELPTEXT_windWaveTint = "\"Wind Wave Tint\" specifies how much the \"Wind Wave Tint\" color applies to the wind wave effect for the detail prototypes that use the \"GPUInstancer/Foliage\" shader. Turning the slider down to zero disables wind wave coloring.";
        public static readonly string HELPTEXT_windWaveSway = "\"Wind Wave Sway\" specifies the amount of wind animation that is applied by the wind waves for the detail prototypes that use the \"GPUInstancer/Foliage\" shader. This is the wind animation that occurs in addition to the idle wind animation. Turning the slider down to zero disables this extra wave animation.";

        public static readonly string HELPTEXT_enableRuntimeModifications = "If \"Enable Runtime Modifications\" is enabled, transform data (position, rotation, scale) for the prefab instances can be modified at runtime with API calls.";
        public static readonly string HELPTEXT_addRemoveInstancesAtRuntime = "If \"Add/Remove Instances At Runtime\" is enabled, new prefab instances can be added or existing ones can be removed at runtime with API calls or by enabling \"Automatically Add/Remove Instances\"";
        public static readonly string HELPTEXT_extraBufferSize = "\"Extra Buffer Size\" specifies the amount of prefab instances that can be added without reinitializing compute buffers at runtime. Instances can be added at runtime with API calls or by enabling \"Automatically Add/Remove Instances\".";
        public static readonly string HELPTEXT_addRuntimeHandlerScript = "If \"Auto. Add/Remove Instances\" is enabled, new prefab instances will be added or existing ones will be removed automatically at runtime without API calls.";
        public static readonly string HELPTEXT_autoUpdateTransformData = "If \"Auto. Update Transform Data\" is enabled, transform updates on prefab instances will be made automatically at runtime without API calls.";
        public static readonly string HELPTEXT_startWithRigidBody = "If \"Start With RigidBody\" is enabled, prefab instances that have a RigidBody component will start with an active RigidBody and it will be active until they go to Sleep state (stop moving).";

        public static readonly string HELPTEXT_unsupportedBillboardWarning = "The prefab for this prototype does not use any of the default Unity tree shaders. Please note that GPU Instancer's billboard system is mainly designed for terrain trees. Using billboard for this prefab might result in undesired billboards.";
        public static readonly string HELPTEXT_speedtree8BillboardReplacementInfo = "This generated (or custom) billboard will be used instead of the original SpeedTree8 Billboard. If you wish to use the original SpeedTree8 billboard, disable this feature.";
        public static readonly string HELPTEXT_useGeneratedBillboard = "If \"Generate Billboard\" is enabled, GPU Instancer will automatically generate a camera facing billboard of this prototype to use as its final LOD level. This billboard is made of a single quad mesh, and as such it can increase the performance for distant instances greatly.";
        public static readonly string HELPTEXT_billboardQuality = "\"Billboard Quality\" defines the resolution of the billboard textures. Higher resolutions result in better looking billboards, but use more memory. Texture atlases are not square for 2D billboards; so for example \"2048\" option results in a 2048x256 texture resolution.";
        public static readonly string HELPTEXT_billboardBrightness = "You can use \"Billboard Brightness\" to adjust the brightness of the billboard textures. A zero value is fully black whereas one is lighter.";
        public static readonly string HELPTEXT_overrideOriginalCutoff = "You can use the \"Override Original Cutoff\" option to override the alpha cutoff value of the original materials for the billboard textures. This is normally necessary if you use the alpha value as some other data (e.g. metallic, etc.) in the textures of the material for the prefab of this prototype.";
        public static readonly string HELPTEXT_billboardFrameCount = "\"Billboard Frame Count\" defines the total amount of snapshots that are taken from different angles for this billboard. Higher frame counts result in more accurate view differences in different view angles, but result in lower resolution frames since they are all packed in the same atlas.";
        public static readonly string HELPTEXT_replaceLODCull = "If the \"Replace LOD Cull with Billboard\" option is selected, GPU Instancer will use the generated billboard instead of the \"Culled\" distance defined in this prototype's LOD Group. Deselect this option if you want to use a custom billboard distance.";
        public static readonly string HELPTEXT_generatedBillboardDistance = "\"Billboard Distance\" defines the distance to the camera in which the prototype will render as the billboard. Increase this if you want the prototypes to switch to billboards further away from the camera.";
        public static readonly string HELPTEXT_regenerateBillboard = "The \"Regenerate Billboard\" button regenerates the billboard atlases for this prototype and updates the current textures with the changed settings.";
        public static readonly string HELPTEXT_showBillboard = "The \"Show Billboard\" button instantiates a quad mesh at (0,0,0) position and shows the generated billboard on it. You can use this to inspect how the billboards will look like at runtime.";
        public static readonly string HELPTEXT_regenerateBillboards = "The \"Regenerate Billboards\" button regenerates the billboard atlases for all of the prototypes and updates textures with the chosen settings.";
        public static readonly string HELPTEXT_useCustomBillboard = "If \"Use Custom Billboard\" is enabled, GPU Instancer will use the given mesh and the material as final LOD level for this prototype.";

        public static readonly string HELPTEXT_useRandomTreeRotation = "If selected, the Tree Manager will use random rotation for terrain trees.";
        public static readonly string HELPTEXT_usePrefabScale = "If selected, the Tree Manager will use prefab's scale for terrain trees.";
        public static readonly string HELPTEXT_useTerrainHeight = "If selected, the Tree Manager will take heights of instances on the terrain into consideration for terrain trees.";

        public static readonly string HELPTEXT_prefabImporterIntro = "The Scene Prefab Importer is designed to easily define prefabs from the existing prefab instances in your scenes to the GPUI Prefab Manager as prototypes. Press the \"?\" button on the top right corner to see more information.";
        public static readonly string HELPTEXT_prefabImporterImportCancel = "The \"Import Selected Prefabs\" button creates a new GPUI Prefab Manager with the selected prefabs defined as prototypes on it. The \"Cancel\" button closes this window.";
        public static readonly string HELPTEXT_prefabImporterSelectAllNone = "The \"Select All\" button selects all the prefabs in the \"Prefab Instances\" section. The \"Select None\" button deselects all the prefabs.";
        public static readonly string HELPTEXT_prefabImporterSelectOverCount = "You can choose an instance count and press the \"Select Min. Instance Count\" button to select the prefabs with the instance counts over the designated number.";
        public static readonly string HELPTEXT_prefabImporterInstanceList = "The \"Prefab Instances\" section shows the list of all the prefab instances that are in the current scene. You can select the ones you wish to import into GPUI Prefab Manager.";

        public static readonly string HELPTEXT_prefabReplacerIntro = "The Prefab Replacer is designed to easily replace GameObjects in your scene hierarchy with prefab instances. Press the \"?\" button on the top right corner to see more information.";
        public static readonly string HELPTEXT_prefabReplacerReplaceCancel = "The \"Replace Selection With Prefab\" button replaces the selected GameObjects with the prefab's instances. The \"Cancel\" button closes this window.";
        public static readonly string HELPTEXT_prefabReplacerPrefab = "The \"Prefab\" will be used to create instances that will replace the selected GameObjects.";
        public static readonly string HELPTEXT_prefabReplacerSelectedObjects = "The \"Selected GameObjects\" section shows the list of selected GameObjects that will be replaced with prefab instances.";
        public static readonly string HELPTEXT_prefabReplacerReplaceNames = "If \"Replace Names\" option is enabled, new instances will use the prefab name. If disabled, instances will have the same names with the GameObjects that are being replaced.";

        public static readonly string HELPTEXT_settingsIntro = "GPU Instancer Settings can be used to personalize the GPUI Manager editors according to your needs. Press the \"?\" button on the top right corner to see more information.";

        public static readonly string HELPTEXT_applyChangesToTerrain = "The \"Apply Changes To Terrain\" button can be used to modify the Unity terrain component with the changes that are made in Detail Manager.";
        public static readonly string HELPTEXT_delete = "The \"Delete\" button deletes this prototype and removes all related data.";
        public static readonly string HELPTEXT_terrainProxyWarning = "Adding and removing detail and tree prototypes should be done only from the GPU Instancer Managers. Unity terrain component can be used to paint detail and tree prototypes on the terrain.";

        public static readonly string HELPTEXT_setMapMagic = "Click on this button after you set up your Map Magic component in your scene.";
        public static readonly string HELPTEXT_useCustomShadowDistance = "If enabled, you can set a custom shadow distance for this prototype. By default GPU Instancer does not render shadows of the objects that are farther than the shadow distance value in the Quality settings for performance reasons. You can use this setting to set a higher value to have shadows for far away objects. It can also be used to have a shorter distance than the Quality setting for performance.";
        public static readonly string HELPTEXT_cullShadows = "If enabled, culling results will also be applied to the prototype shadows. This will result in more performance, but might also lead to less consistency in shadows.";

        public static readonly string HELPTEXT_runInThreads = "If enabled, makes the initialization calculations for spatial partitioning in Threads. Significantly reduces FPS drops caused by initialization, however it might take longer for the initialization to finish.";
        public static readonly string HELPTEXT_initializeWithCoroutine = "If enabled, makes the initialization calculations for tree instances in a Coroutine. Significantly reduces FPS drops caused by initialization, however it might take longer for the initialization to finish.";

        public static readonly string HELPTEXT_disableMeshRenderers = "The \"Disable Mesh Renderers\" button  will disable all the Mesh Renderers / LOD Groups / Rigidbodies in this prefab.\n\nThis will speed up initialization operations on the Prefab Manager, since GPUI will not loop through prefab instances to disable these components at runtime.\n\nHowever, these objects will be visible only during play mode and while the Prefab Manager is active and running. You will be able to use this button again to enable the Mesh Renderers, but it is recommended to do this after you finished your scene design.\n\nAfter you do this, you should not manually enable/disable these components on the prefab or its instances.";
        public static readonly string HELPTEXT_advancedActions = "These actions require advanced knowledge of GPU Instancer operations. If you are not experienced with using GPU Instancer or do not understand how it operates, it is not recommended to use these options.";
        public static readonly string HELPTEXT_prefabInstanceSerialization = "This action will remove the registered prefab instances from the scene and save their transform data into a TextAsset.\n\nThis is helpfull to save memory and speed up initialization by getting rid of unnecessary GameObjects.\n\nHowever, these objects will be visible only during play mode or editor simulation and while the Prefab Manager is active and running. You will be able to use this button again to recreate the GameObjects with the same transform data, but it is recommended to do this after you finished your scene design.";

        public static readonly string HELPTEXT_Version100Update = "With the new GPU Instancer v1.0.0, there has been some changes in the folder structure to decrease the amount of shaders that are included in the builds. These changes will be available in fresh downloads, however when upgrading it is required to move the folders in the existing project and update the generated shaders.";

        public static readonly string HELPTEXT_customRenderingSettingsApply = "Apply changes";
        public static readonly string HELPTEXT_customRenderingSettingsRevert = "Revert to saved settings";

        public static readonly string ERRORTEXT_cameraNotFound = "Main Camera cannot be found. GPU Instancer needs either an existing camera with the \"Main Camera\" tag on the scene to autoselect it, or a manually specified camera. If you add your camera at runtime, please use the \"GPUInstancerAPI.SetCamera(camera)\" API function.";

        public static readonly string HELPTEXT_autoGenerateBillboards = "If enabled, billboard textures will be automatically generated for known tree types when they are added to the GPUI Managers.";
        public static readonly string HELPTEXT_autoShaderConversion = "If enabled, shaders of the prefabs that are added to the GPUI Managers will be automatically converted to support GPU Instancer.";
        public static readonly string HELPTEXT_autoShaderVariantHandling = "If enabled, shaders variants of the prefabs that are added to the GPUI Managers will be automatically added to a ShaderVariantCollection to be included in builds.";
        public static readonly string HELPTEXT_generateShaderVariantCollection = "If enabled, a ShaderVariantCollection with reference to shaders that are used in GPUI Managers will be generated automatically inside Resources folder. This will add the GPUI shader variants automatically to your builds. These shader variants are required for GPUI to work in your builds.";
        public static readonly string HELPTEXT_disableInstanceCountWarnings = "If enabled, Prefab Manager will not show warnings for prototypes with low instance counts.";
        public static readonly string HELPTEXT_customPreviewBG = "If enabled, custom background color can be used for preview icons of the prototypes.";
        public static readonly string HELPTEXT_settingInstancingBoundsSize = "Defines the bounds parameter's size for the instanced rendering draw call.";
        public static readonly string HELPTEXT_settingTestBothEyesForVROcclusion = "If disabled, GPUI will run occlusion test calculations for the left VR eye only. When enabled, both eyes will be tested. Testing both eyes can put more load on the GPU in some cases, but culling results will always be more accurate.";
        public static readonly string HELPTEXT_settingVRRenderingMode = "You can set which VR rendering mode you are using in your project here. Setting this will allow GPUI to use the correct rendering mode for VR operations such as occlusion culling. Please note that in Unity versions 2018.3 and above, this is automatically detected by GPUI.";

        public static readonly string HELPTEXT_settingHasCustomRenderingSettings = "Use Custom Rendering Settings";
        public static readonly string HELPTEXT_settingComputeThread = "Max. Compute Thread Count";
        public static readonly string HELPTEXT_settingMatrixHandlingType = "Max. Compute Buffers";
        public static readonly string HELPTEXT_settingOcclusionCullingType = "Occlusion Culling Type";

        public static readonly string HELPTEXT_floatingOriginTerrain = "If enabled, positions of the instances will automatically change when terrain position is modified.";
        public static readonly string HELPTEXT_floatingOriginPrefab = "If enabled, positions of the instances will automatically change when the position of the given transform is modified.";

        public static readonly string HELPTEXT_disableLightProbes = "While using Indirect GPU Instancing by default all the instances share the same probe value. You can disable light probe usage by enabling this option.";

        public static readonly string HELPTEXT_layerMask = "Can be used to discard specific Mesh Renderers of the prototypes. When a Layer is not rendered by GPUI, it will be rendered by the Unity Mesh Renderer component. GPUI will also not enable/disable the Mesh Renderers of the discarded Layer when using Prefab Manager. This allows you to disable GPUI for a specific child Mesh Renderer of a prefeb.";
        public static readonly string HELPTEXT_enableMROnManagerDisable = "If enabled, Prefab Manager will automatically re-enable the Mesh Renderers of the instances when the manager is disabled.";

        public static readonly string WARNINGTEXT_instanceCounts = "When using the Prefab Manager in your scene, the best practice is to add the distinctively repeating prefabs in the scene to the manager as prototypes. Using GPUI for prototypes with very low instance counts is not recommended.";

        public static readonly string HELP_ICON = "help_gpui";
        public static readonly string HELP_ICON_ACTIVE = "help_gpui_active";
        public static readonly string DROP_ICON = "drop";
        public static readonly string DROP_ICON_PRO = "dropPro";
        public static readonly string PREVIEW_BOX_ICON = "previewBox";

        public static class Contents
        {
            public static GUIContent removeTerrain = new GUIContent(TEXT_removeTerrain);
            public static GUIContent setTerrain = new GUIContent(TEXT_setTerrain);
            public static GUIContent generatePrototypes = new GUIContent(TEXT_generatePrototypes);
            public static GUIContent paintOnTerrain = new GUIContent(TEXT_paintOnTerrain);
            public static GUIContent applyChangesToTerrain = new GUIContent(TEXT_applyChangesToTerrain);
            public static GUIContent generateBillboard = new GUIContent(TEXT_generateBillboard);
            public static GUIContent regenerateBillboard = new GUIContent(TEXT_regenerateBillboard);
            public static GUIContent showBillboard = new GUIContent(TEXT_showBillboard);
            public static GUIContent delete = new GUIContent(TEXT_delete);
            public static GUIContent registerPrefabsInScene = new GUIContent(TEXT_registerPrefabsInScene);
            public static GUIContent goToGPUInstancerDetail = new GUIContent(TEXT_goToGPUInstancerDetail);
            public static GUIContent goToGPUInstancerTree = new GUIContent(TEXT_goToGPUInstancerTree);
            public static GUIContent add = new GUIContent(TEXT_add);
            public static GUIContent addTextMode = new GUIContent(TEXT_addTextMode);
            public static GUIContent addMulti = new GUIContent(TEXT_addMulti);
            public static GUIContent addMultiTextMode = new GUIContent(TEXT_addMultiTextMode);
            public static GUIContent useCamera = new GUIContent(TEXT_useCamera);
            public static GUIContent renderOnlySelectedCamera = new GUIContent(TEXT_renderOnlySelectedCamera, TEXT_renderOnlySelectedCamera);
            public static GUIContent useManagerFrustumCulling = new GUIContent(TEXT_useManagerFrustumCulling);
            public static GUIContent useManagerOcclusionCulling = new GUIContent(TEXT_useManagerOcclusionCulling);
            public static GUIContent minManagerCullingDistance = new GUIContent(TEXT_minCullingDistance);
            public static GUIContent simulateAtEditor = new GUIContent(TEXT_simulateAtEditor);
            public static GUIContent simulateAtEditorStop = new GUIContent(TEXT_simulateAtEditorStop);
            public static GUIContent simulateAtEditorPrep = new GUIContent(TEXT_simulateAtEditorPrep);

            public static GUIContent mapMagicSet = new GUIContent(TEXT_mapMagicSet);
            public static GUIContent mapMagicImport = new GUIContent(TEXT_mapMagicImport);

            public static GUIContent importSelectedPrefabs = new GUIContent(TEXT_importSelectedPrefabs);
            public static GUIContent cancel = new GUIContent(TEXT_cancel);

            public static GUIContent regenerateBillboards = new GUIContent(TEXT_regenerateBillboards);

            public static List<GUIContent> shadowLODs = new List<GUIContent> {
                new GUIContent("LOD 0 Shadow"), new GUIContent("LOD 1 Shadow"), new GUIContent("LOD 2 Shadow"), new GUIContent("LOD 3 Shadow"),
                new GUIContent("LOD 4 Shadow"), new GUIContent("LOD 5 Shadow"), new GUIContent("LOD 6 Shadow"), new GUIContent("LOD 7 Shadow")
            };

            public static List<GUIContent> LODs = new List<GUIContent> {
                new GUIContent("LOD 0"), new GUIContent("LOD 1"), new GUIContent("LOD 2"), new GUIContent("LOD 3"),
                new GUIContent("LOD 4"), new GUIContent("LOD 5"), new GUIContent("LOD 6"), new GUIContent("LOD 7"), new GUIContent("None")
            };

            public static GUIContent runInThreads = new GUIContent("Initialize In Threads");
            public static GUIContent initializeWithCoroutine = new GUIContent("Initialize With Coroutine");
            public static GUIContent useSinglePrefabManager = new GUIContent("Use Single Prefab Manager");
            public static GUIContent disableMeshRenderers = new GUIContent("Disable Mesh Renderers");
            public static GUIContent enableMeshRenderers = new GUIContent("Enable Mesh Renderers");
            public static GUIContent serializeRegisteredInstances = new GUIContent("Serialize Registered Instances");
            public static GUIContent deserializeRegisteredInstances = new GUIContent("Deserialize Registered Instances");

            public static GUIContent prototypeSO = new GUIContent(TEXT_prototypeSO);

            public static GUIContent apply = new GUIContent("Apply");
            public static GUIContent revert = new GUIContent("Revert");
            public static GUIContent editPrefab = new GUIContent("Edit <size=8>(Click / Drop)</size>");

            public static GUIContent settingAutoGenerateBillboards = new GUIContent(TEXT_settingAutoGenerateBillboards, HELPTEXT_autoGenerateBillboards);
            public static GUIContent settingAutoShaderConversion = new GUIContent(TEXT_settingAutoShaderConversion, HELPTEXT_autoShaderConversion);
            public static GUIContent settingAutoShaderVariantHandling = new GUIContent(TEXT_settingAutoShaderVariantHandling, HELPTEXT_autoShaderVariantHandling);
            public static GUIContent settingGenerateShaderVariant = new GUIContent(TEXT_settingGenerateShaderVariant, HELPTEXT_generateShaderVariantCollection);
            public static GUIContent settingDisableICWarning = new GUIContent(TEXT_settingDisableICWarning, HELPTEXT_disableInstanceCountWarnings);
            public static GUIContent settingMaxDetailDist = new GUIContent(TEXT_settingMaxDetailDist, HELPTEXT_maxDetailDistance);
            public static GUIContent settingMaxTreeDist = new GUIContent(TEXT_settingMaxTreeDist, HELPTEXT_maxTreeDistance);
            public static GUIContent settingMaxPrefabDist = new GUIContent(TEXT_settingMaxPrefabDist, HELPTEXT_maxDistance);
            public static GUIContent settingMaxPrefabBuff = new GUIContent(TEXT_settingMaxPrefabBuff, HELPTEXT_extraBufferSize);
            public static GUIContent settingCustomPrevBG = new GUIContent(TEXT_settingCustomPrevBG, HELPTEXT_customPreviewBG);
            public static GUIContent settingPrevBGColor = new GUIContent(TEXT_settingPrevBGColor, HELPTEXT_customPreviewBG);
            public static GUIContent settingInstancingBoundsSize = new GUIContent(TEXT_settingInstancingBoundsSize, HELPTEXT_settingInstancingBoundsSize);
            public static GUIContent settingTestBothEyesForVROcclusion = new GUIContent(TEXT_settingTestBothEyesForVROcclusion, HELPTEXT_settingTestBothEyesForVROcclusion);
            public static GUIContent settingVRRenderingMode = new GUIContent(TEXT_settingVRRenderingMode, HELPTEXT_settingVRRenderingMode);
            public static GUIContent[] settingVRRenderingModeOptions = new GUIContent[] { new GUIContent("Single Pass"), new GUIContent("Multi Pass") };
            public static GUIContent settingHasCustomRenderingSettings = new GUIContent(TEXT_settingHasCustomRenderingSettings, HELPTEXT_settingHasCustomRenderingSettings);
            public static GUIContent settingComputeThread = new GUIContent(TEXT_settingComputeThread, HELPTEXT_settingComputeThread);
            public static GUIContent[] settingComputeThreadOptions = new GUIContent[] { new GUIContent("64"), new GUIContent("128"), new GUIContent("256"), new GUIContent("512"), new GUIContent("1024") };
            public static GUIContent settingMatrixHandlingType = new GUIContent(TEXT_settingMatrixHandlingType, HELPTEXT_settingMatrixHandlingType);
            public static GUIContent[] settingMatrixHandlingTypeOptions = new GUIContent[] { new GUIContent("Unlimited"), new GUIContent("1 Compute Buffer"), new GUIContent("None") };
            public static GUIContent settingOcclusionCullingType = new GUIContent(TEXT_settingOcclusionCullingType, HELPTEXT_settingOcclusionCullingType);
            public static GUIContent[] settingOcclusionCullingTypeOptions = new GUIContent[] { new GUIContent("Graphics.Blit"), new GUIContent("Compute Shader") };

            public static GUIContent useFloatingOriginHandler = new GUIContent("Use Floating Origin");
            public static GUIContent floatingOriginTransform = new GUIContent("Transform of Floating Origin");

            public static GUIContent disableLightProbes = new GUIContent("Disable Light Probes");

            public static GUIContent layerMask = new GUIContent("Layer Mask");
            public static GUIContent enableMROnManagerDisable = new GUIContent("Enable MR on Disable", "Enable Mesh Renderers when Manager is Disabled");
        }

        public static class Styles
        {
            public static GUIStyle label = new GUIStyle("Label");
            public static GUIStyle boldLabel = new GUIStyle("BoldLabel");
            public static GUIStyle button = new GUIStyle("Button");
            public static GUIStyle foldout = new GUIStyle("Foldout");
            public static GUIStyle box = new GUIStyle("Box");
            public static GUIStyle richLabel = new GUIStyle("Label");
            public static GUIStyle helpButton = new GUIStyle("Button")
            {
                padding = new RectOffset(2, 2, 2, 2)
            };
            public static GUIStyle helpButtonSelected = new GUIStyle("Button")
            {
                padding = new RectOffset(2, 2, 2, 2),
                normal = helpButton.active
            };
        }

        public static class Colors
        {
            public static Color lightBlue = new Color(0.5f, 0.6f, 0.8f, 1);
            public static Color darkBlue = new Color(0.07f, 0.27f, 0.35f, 1);
            public static Color lightGreen = new Color(0.2f, 1f, 0.2f, 1);
            public static Color green = new Color(0, 0.4f, 0, 1);
            public static Color lightred = new Color(0.4f, 0, 0, 1);
            public static Color darkred = new Color(0.8f, 0.2f, 0.2f, 1);
            public static Color gainsboro = new Color(220 / 255f, 220 / 255f, 220 / 255f);
            public static Color dimgray = new Color(105 / 255f, 105 / 255f, 105 / 255f);
        }

        public static void DrawCustomLabel(string text, GUIStyle style, bool center = true)
        {
            if (center)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
            }

            GUILayout.Label(text, style);

            if (center)
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
        }

        public static void DrawColoredButton(GUIContent guiContent, Color backgroundColor, Color textColor, FontStyle fontStyle, Rect buttonRect, UnityAction clickAction,
            bool isRichText = false, bool dragDropEnabled = false, UnityAction<Object> dropAction = null, GUIStyle style = null)
        {
            Color oldBGColor = GUI.backgroundColor;
            GUI.backgroundColor = backgroundColor;
            if (style == null)
                style = new GUIStyle("button");
            style.normal.textColor = textColor;
            style.active.textColor = textColor;
            style.hover.textColor = textColor;
            style.focused.textColor = textColor;
            style.fontStyle = fontStyle;
            style.richText = isRichText;

            if (buttonRect == Rect.zero)
            {
                if (GUILayout.Button(guiContent, style))
                {
                    if (clickAction != null)
                        clickAction.Invoke();
                }
            }
            else
            {
                if (GUI.Button(buttonRect, guiContent, style))
                {
                    if (clickAction != null)
                        clickAction.Invoke();
                }
            }

            GUI.backgroundColor = oldBGColor;

            if (dragDropEnabled && dropAction != null)
            {
                Event evt = Event.current;
                switch (evt.type)
                {
                    case EventType.DragUpdated:
                    case EventType.DragPerform:
                        if (!buttonRect.Contains(evt.mousePosition))
                            return;

                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                        if (evt.type == EventType.DragPerform)
                        {
                            DragAndDrop.AcceptDrag();

                            foreach (Object dragged_object in DragAndDrop.objectReferences)
                            {
                                dropAction(dragged_object);
                            }
                        }
                        break;
                }
            }
        }

        // Toolbar buttons
        [MenuItem("Tools/GPU Instancer/Add Prefab Manager", validate = false, priority = 1)]
        public static void ToolbarAddPrefabManager()
        {
            GameObject go = new GameObject("GPUI Prefab Manager");
            go.AddComponent<GPUInstancerPrefabManager>();

            Selection.activeGameObject = go;
            Undo.RegisterCreatedObjectUndo(go, "Add GPUI Prefab Manager");
        }

        [MenuItem("Tools/GPU Instancer/Add Detail Manager For Terrains", validate = false, priority = 2)]
        public static void ToolbarAddDetailManager()
        {
            Terrain[] terrains = Terrain.activeTerrains;
            GameObject go = null;
            if (terrains != null && terrains.Length > 0)
            {
                foreach (Terrain terrain in terrains)
                {
                    if (terrain.GetComponent<GPUInstancerTerrainProxy>() == null || terrain.GetComponent<GPUInstancerTerrainProxy>().detailManager == null)
                    {
                        go = new GameObject("GPUI Detail Manager (" + terrain.terrainData.name + ")");
                        GPUInstancerDetailManager detailManager = go.AddComponent<GPUInstancerDetailManager>();
                        detailManager.SetupManagerWithTerrain(terrain);
                    }
                    else
                    {
                        go = terrain.GetComponent<GPUInstancerTerrainProxy>().detailManager.gameObject;
                    }
                }

                Selection.activeGameObject = go;
                Undo.RegisterCreatedObjectUndo(go, "Add GPUI Detail Manager");
            }
            else
            {
                EditorUtility.DisplayDialog("GPU Instancer", "Detail Manager requires a terrain added to the scene.", "OK");
            }
        }

        [MenuItem("Tools/GPU Instancer/Add Detail Manager For Terrains", validate = true)]
        public static bool ValidateToolbarAddDetailManager()
        {
            Terrain[] terrains = Terrain.activeTerrains;
            return terrains != null && terrains.Length > 0;
        }

        [MenuItem("Tools/GPU Instancer/Add Tree Manager For Terrains", validate = false, priority = 2)]
        public static void ToolbarAddTreeManager()
        {
            Terrain[] terrains = Terrain.activeTerrains;
            GameObject go = null;
            if (terrains != null && terrains.Length > 0)
            {
                List<GPUInstancerTreeManager> treeManagers = new List<GPUInstancerTreeManager>();
                foreach (Terrain terrain in terrains)
                {
                    if (terrain.GetComponent<GPUInstancerTerrainProxy>() == null || terrain.GetComponent<GPUInstancerTerrainProxy>().treeManager == null)
                    {
                        bool addedToManager = false;
                        foreach (GPUInstancerTreeManager tm in treeManagers)
                        {
                            TreePrototype[] managerTreePrototypes = tm.terrain.terrainData.treePrototypes;
                            TreePrototype[] terrainTreePrototypes = terrain.terrainData.treePrototypes;
                            bool hasSamePrototypes = true;
                            if (managerTreePrototypes.Length == terrainTreePrototypes.Length)
                            {
                                for (int i = 0; i < managerTreePrototypes.Length; i++)
                                {
                                    if (managerTreePrototypes[i].prefab != terrainTreePrototypes[i].prefab)
                                    {
                                        hasSamePrototypes = false;
                                        break;
                                    }
                                }
                            }
                            else
                                hasSamePrototypes = false;
                            if (hasSamePrototypes)
                            {
                                if (tm.additionalTerrains == null)
                                    tm.additionalTerrains = new List<Terrain>();
                                tm.additionalTerrains.Add(terrain);
                                addedToManager = true;
                                break;
                            }
                        }
                        if (!addedToManager)
                        {
                            go = new GameObject("GPUI Tree Manager (" + terrain.terrainData.name + ")");
                            GPUInstancerTreeManager treeManager = go.AddComponent<GPUInstancerTreeManager>();
                            treeManagers.Add(treeManager);
                            treeManager.SetupManagerWithTerrain(terrain);
                        }
                    }
                    else
                    {
                        go = terrain.GetComponent<GPUInstancerTerrainProxy>().treeManager.gameObject;
                    }
                }

                Selection.activeGameObject = go;
                Undo.RegisterCreatedObjectUndo(go, "Add GPUI Tree Manager");
            }
            else
            {
                EditorUtility.DisplayDialog("GPU Instancer", "Tree Manager requires a terrain added to the scene.", "OK");
            }
        }

        [MenuItem("Tools/GPU Instancer/Add Tree Manager For Terrains", validate = true)]
        public static bool ValidateToolbarAddTreeManager()
        {
            Terrain[] terrains = Terrain.activeTerrains;
            return terrains != null && terrains.Length > 0;
        }

        [MenuItem("Tools/GPU Instancer/Show Scene Prefab Importer", validate = false, priority = 101)]
        public static void ToolbarShowPrefabImporter()
        {
            GameObject[] prefabInstances = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
            List<GameObject> prefabList = new List<GameObject>();
            foreach (GameObject go in prefabInstances)
            {
#if UNITY_2018_3_OR_NEWER
                AddPrefabObjectsToList(go, prefabList);
#else
                if (PrefabUtility.GetPrefabType(go) == PrefabType.PrefabInstance)
                {
#if UNITY_2018_2_OR_NEWER
                    GameObject prefab = (GameObject)PrefabUtility.GetCorrespondingObjectFromSource(go);
#else
                    GameObject prefab = (GameObject)PrefabUtility.GetPrefabParent(go);
#endif
                    if (prefab.transform.parent == null && !prefabList.Contains(prefab))
                        prefabList.Add(prefab);
                }
#endif
            }
            GPUInstancerPrefabImporterWindow.ShowWindow(prefabList);
        }

#if UNITY_2018_3_OR_NEWER
        private static void AddPrefabObjectsToList(GameObject go, List<GameObject> prefabList)
        {
            PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(go);
            if (prefabType == PrefabAssetType.Regular || prefabType == PrefabAssetType.Variant)
            {
                GameObject prefab = (GameObject)PrefabUtility.GetCorrespondingObjectFromSource(go);
                if (prefabType == PrefabAssetType.Variant)
                {
                    GameObject newPrefabObject = (GameObject)PrefabUtility.GetCorrespondingObjectFromSource(prefab);
                    if (newPrefabObject != null)
                    {
                        while (newPrefabObject.transform.parent != null)
                            newPrefabObject = newPrefabObject.transform.parent.gameObject;
                        prefab = newPrefabObject;
                    }
                }
                if (prefab != null && prefab.transform.parent == null && !prefabList.Contains(prefab))
                {
                    prefabList.Add(prefab);
                    GameObject prefabContents = GPUInstancerUtility.LoadPrefabContents(prefab);
                    List<Transform> childTransforms = new List<Transform>(prefabContents.GetComponentsInChildren<Transform>());
                    childTransforms.Remove(prefabContents.transform);
                    foreach (Transform childTransform in childTransforms)
                    {
                        AddPrefabObjectsToList(childTransform.gameObject, prefabList);
                    }
                    GPUInstancerUtility.UnloadPrefabContents(prefab, prefabContents, false);
                }
            }
        }
#endif

        [MenuItem("Tools/GPU Instancer/Show Prefab Replacer", validate = false, priority = 102)]
        public static void ToolbarShowPrefabReplacer()
        {
            GPUInstancerPrefabReplacerWindow.ShowWindow();
        }

        [MenuItem("Tools/GPU Instancer/Shaders/Clear Shader Bindings", validate = false, priority = 201)]
        public static void ClearShaderBindings()
        {
            GPUInstancerShaderBindings shaderBindings = GPUInstancerDefines.GetGPUInstancerShaderBindings();
            if (shaderBindings != null && shaderBindings.shaderInstances != null && shaderBindings.shaderInstances.Count > 0)
            {
                if (EditorUtility.DisplayDialog("Clear Shader bindings", "This operation will clear references for GPUI compatible shaders and they will be recreated when needed.\n\nDo you wish to continue?",
                    "Yes", "No"))
                {

                    if (EditorUtility.DisplayDialog("Delete GPUI Shaders", "Do you want to delete the auto generated GPUI shaders? They will be recreated when needed.",
                        "Yes", "No"))
                    {
                        foreach (ShaderInstance si in shaderBindings.shaderInstances)
                        {
                            if (!si.isOriginalInstanced)
                                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(si.instancedShader));
                        }
                    }
                    shaderBindings.shaderInstances.Clear();
                    EditorUtility.SetDirty(shaderBindings);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }

        [MenuItem("Tools/GPU Instancer/Shaders/Regenerate GPUI Shaders", validate = false, priority = 202)]
        public static void RegenrateGPUIShaders()
        {
            GPUInstancerShaderBindings shaderBindings = GPUInstancerDefines.GetGPUInstancerShaderBindings();
            if (shaderBindings != null && shaderBindings.shaderInstances != null && shaderBindings.shaderInstances.Count > 0)
            {
                if (EditorUtility.DisplayDialog("Regenerate GPUI Shaders", "This operation will update all shaders that is generated by GPU Instancer.\n\nDo you wish to continue?",
                    "Yes", "No"))
                {
                    foreach (ShaderInstance si in shaderBindings.shaderInstances)
                    {
                        si.Regenerate();
                    }
                    EditorUtility.SetDirty(shaderBindings);
                }
            }
        }

        [MenuItem("Tools/GPU Instancer/Shaders/Clear Shader Variants", validate = false, priority = 301)]
        public static void ClearShaderVariants()
        {
            ShaderVariantCollection shaderVariantCollection = GPUInstancerDefines.GetShaderVariantCollection();
            if (shaderVariantCollection != null && shaderVariantCollection.shaderCount > 0)
            {
                if (EditorUtility.DisplayDialog("Clear Shader Variants", "This operation will clear shader variant references for GPUI compatible shaders and they will be recreated when needed.\n\nDo you wish to continue?",
                    "Yes", "No"))
                {
                    shaderVariantCollection.Clear();
                    EditorUtility.SetDirty(shaderVariantCollection);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }

        [MenuItem("Tools/GPU Instancer/Shaders/Edit Shader Variants", validate = false, priority = 301)]
        public static void EditShaderVariants()
        {
            ShaderVariantCollection shaderVariantCollection = GPUInstancerDefines.GetShaderVariantCollection();
            if (shaderVariantCollection != null)
                Selection.activeObject = shaderVariantCollection;
        }

        static Dictionary<GPUInstancerSettingsExtension, Editor> extensionEditors;
#if UNITY_2019_1_OR_NEWER
        [SettingsProvider]
        public static SettingsProvider PreferencesGPUInstancerGUI()
        {
            var provider = new SettingsProvider("Preferences/GPU Instancer", SettingsScope.User)
            {
                label = "GPU Instancer",
                guiHandler = (searchContext) =>
                {
                    DrawGPUISettings();
                },
                keywords = new HashSet<string>(new[] { "GPU", "Instancer", "GPUI" })
            };

            return provider;
        }
#else
        [PreferenceItem("GPU Instancer")]
        public static void PreferencesGPUInstancerGUI()
        {
            float previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 210;

            DrawGPUISettings();

            EditorGUIUtility.labelWidth = previousLabelWidth;
        }
#endif

        [MenuItem("Assets/GPU Instancer/Setup Shader for GPUI", validate = false, priority = 2001)]
        public static void SetupShaderForGPUIMenuItem()
        {
            Object selected = Selection.activeObject;
            if (selected != null && selected is Shader)
            {
                Shader shader = (Shader)selected;
                GPUInstancerAPI.SetupShaderForGPUI(shader);
            }
        }

        [MenuItem("Assets/GPU Instancer/Setup Shader for GPUI", validate = true, priority = 2001)]
        public static bool SetupShaderForGPUIMenuItemValidate()
        {
            Object selected = Selection.activeObject;
            return selected != null && selected is Shader;
        }

        [MenuItem("Assets/GPU Instancer/Setup Material for GPUI", validate = false, priority = 2002)]
        public static void SetupMaterialForGPUIMenuItem()
        {
            Object selected = Selection.activeObject;
            if (selected != null && selected is Material)
            {
                Material mat = (Material)selected;
                if (!GPUInstancerConstants.gpuiSettings.shaderBindings.IsShadersInstancedVersionExists(mat.shader.name))
                {
                    GPUInstancerAPI.SetupShaderForGPUI(mat.shader);
                }
                if (GPUInstancerConstants.gpuiSettings.shaderBindings.IsShadersInstancedVersionExists(mat.shader.name))
                {
                    GPUInstancerAPI.AddShaderVariantToCollection(mat);
                    Debug.Log("Material has been successfully added to Shader Variant Collection.");
                }
            }
        }

        [MenuItem("Assets/GPU Instancer/Setup Material for GPUI", validate = true, priority = 2002)]
        public static bool SetupMaterialForGPUIMenuItemValidate()
        {
            Object selected = Selection.activeObject;
            return selected != null && selected is Material;
        }

        private static bool _loadedSettings;
        private static bool _hasCustomRenderingSettings;
        private static int _threadCountSelection;
        private static int _matrixHandlingTypeSelection;
        private static bool _customRenderingSettingsChanged;

        public static void DrawGPUISettings()
        {
            GPUInstancerSettings gPUInstancerSettings = GPUInstancerConstants.gpuiSettings;

            if (!gPUInstancerSettings)
                return;
            if (!_loadedSettings)
            {
                _hasCustomRenderingSettings = gPUInstancerSettings.hasCustomRenderingSettings;
                if (gPUInstancerSettings.customRenderingSettings != null)
                {
                    _threadCountSelection = (int)gPUInstancerSettings.customRenderingSettings.computeThreadCount;
                    _matrixHandlingTypeSelection = (int)gPUInstancerSettings.customRenderingSettings.matrixHandlingType;
                }
                _loadedSettings = true;
                _customRenderingSettingsChanged = false;
            }

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical(Styles.box);
            DrawCustomLabel("Constants", Styles.boldLabel);
            GUILayout.Space(5);

            gPUInstancerSettings.MAX_DETAIL_DISTANCE = EditorGUILayout.IntField(Contents.settingMaxDetailDist, (int) gPUInstancerSettings.MAX_DETAIL_DISTANCE);
            gPUInstancerSettings.MAX_TREE_DISTANCE = EditorGUILayout.IntField(Contents.settingMaxTreeDist, (int) gPUInstancerSettings.MAX_TREE_DISTANCE);
            gPUInstancerSettings.MAX_PREFAB_DISTANCE = EditorGUILayout.IntField(Contents.settingMaxPrefabDist, (int) gPUInstancerSettings.MAX_PREFAB_DISTANCE);
            gPUInstancerSettings.MAX_PREFAB_EXTRA_BUFFER_SIZE = EditorGUILayout.IntField(Contents.settingMaxPrefabBuff, gPUInstancerSettings.MAX_PREFAB_EXTRA_BUFFER_SIZE);
            gPUInstancerSettings.instancingBoundsSize = EditorGUILayout.IntSlider(Contents.settingInstancingBoundsSize, gPUInstancerSettings.instancingBoundsSize, 1, 10000);

            GUILayout.Space(5);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(Styles.box);
            GUILayout.Space(5);
            DrawCustomLabel("Behaviour", Styles.boldLabel);

            gPUInstancerSettings.disableAutoGenerateBillboards = !EditorGUILayout.Toggle(Contents.settingAutoGenerateBillboards, !gPUInstancerSettings.disableAutoGenerateBillboards);
            gPUInstancerSettings.disableAutoShaderConversion = !EditorGUILayout.Toggle(Contents.settingAutoShaderConversion, !gPUInstancerSettings.disableAutoShaderConversion);
            gPUInstancerSettings.disableAutoVariantHandling = !EditorGUILayout.Toggle(Contents.settingAutoShaderVariantHandling, !gPUInstancerSettings.disableAutoVariantHandling);
            gPUInstancerSettings.disableShaderVariantCollection = !EditorGUILayout.Toggle(Contents.settingGenerateShaderVariant, !gPUInstancerSettings.disableShaderVariantCollection);
            gPUInstancerSettings.disableInstanceCountWarning = EditorGUILayout.Toggle(Contents.settingDisableICWarning, gPUInstancerSettings.disableInstanceCountWarning);

            GUILayout.Space(5);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(Styles.box);
            GUILayout.Space(5);
            DrawCustomLabel("VR", Styles.boldLabel);

#if !UNITY_2018_3_OR_NEWER
            gPUInstancerSettings.vrRenderingMode = EditorGUILayout.Popup(Contents.settingVRRenderingMode, gPUInstancerSettings.vrRenderingMode, Contents.settingVRRenderingModeOptions);
#endif

            gPUInstancerSettings.testBothEyesForVROcclusion = EditorGUILayout.Toggle(Contents.settingTestBothEyesForVROcclusion, gPUInstancerSettings.testBothEyesForVROcclusion);

            GUILayout.Space(5);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(Styles.box);
            GUILayout.Space(5);
            DrawCustomLabel("Theme", Styles.boldLabel);

            EditorGUI.BeginChangeCheck();
            gPUInstancerSettings.useCustomPreviewBackgroundColor = EditorGUILayout.Toggle(Contents.settingCustomPrevBG, gPUInstancerSettings.useCustomPreviewBackgroundColor);
            EditorGUI.BeginDisabledGroup(!gPUInstancerSettings.useCustomPreviewBackgroundColor);
            gPUInstancerSettings.previewBackgroundColor = EditorGUILayout.ColorField(Contents.settingPrevBGColor, gPUInstancerSettings.previewBackgroundColor);
            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck())
            {
                if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<GPUInstancerManager>() != null)
                    Selection.activeGameObject = null;
                GPUInstancerDefines.previewCache.ClearPreviews();
            }

            GUILayout.Space(5);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(Styles.box);
            GUILayout.Space(5);
            DrawCustomLabel("Rendering Settings [ADVANCED]", Styles.boldLabel);

            EditorGUI.BeginChangeCheck();
            _hasCustomRenderingSettings = EditorGUILayout.Toggle(Contents.settingHasCustomRenderingSettings, _hasCustomRenderingSettings);
            if (_hasCustomRenderingSettings)
            {
                _threadCountSelection = EditorGUILayout.Popup(Contents.settingComputeThread, _threadCountSelection, Contents.settingComputeThreadOptions);
                _matrixHandlingTypeSelection = EditorGUILayout.Popup(Contents.settingMatrixHandlingType, _matrixHandlingTypeSelection, Contents.settingMatrixHandlingTypeOptions);
            }
            if (EditorGUI.EndChangeCheck())
            {
                _customRenderingSettingsChanged = true;
            }

            if (_customRenderingSettingsChanged)
            {
                EditorGUILayout.BeginHorizontal();
                DrawColoredButton(new GUIContent("Apply", HELPTEXT_customRenderingSettingsApply), Colors.green, Color.white, FontStyle.Bold, Rect.zero,
                () =>
                {
                    if (_hasCustomRenderingSettings)
                    {
                        gPUInstancerSettings.hasCustomRenderingSettings = true;
                        gPUInstancerSettings.customRenderingSettings = new GPUInstancerSettings.GPUIRenderingSettings();
                        gPUInstancerSettings.customRenderingSettings.platform = GPUIPlatform.Default;
                        gPUInstancerSettings.customRenderingSettings.computeThreadCount = (GPUIComputeThreadCount)_threadCountSelection;
                        gPUInstancerSettings.customRenderingSettings.matrixHandlingType = (GPUIMatrixHandlingType)_matrixHandlingTypeSelection;
                    }
                    else
                    {
                        gPUInstancerSettings.hasCustomRenderingSettings = false;
                        gPUInstancerSettings.customRenderingSettings = null;
                    }
                    _customRenderingSettingsChanged = false;

                    GPUInstancerUtility.UpdatePlatformDependentFiles();
                });

                DrawColoredButton(new GUIContent("Revert", HELPTEXT_customRenderingSettingsRevert), Colors.lightred, Color.white, FontStyle.Bold, Rect.zero,
                () =>
                {
                    _hasCustomRenderingSettings = gPUInstancerSettings.hasCustomRenderingSettings;
                    if (gPUInstancerSettings.customRenderingSettings != null)
                    {
                        _threadCountSelection = (int)gPUInstancerSettings.customRenderingSettings.computeThreadCount;
                        _matrixHandlingTypeSelection = (int)gPUInstancerSettings.customRenderingSettings.matrixHandlingType;
                    }
                    _customRenderingSettingsChanged = false;
                });
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }

            gPUInstancerSettings.occlusionCullingType = (GPUIOcclusionCullingType)EditorGUILayout.Popup(Contents.settingOcclusionCullingType, (int)gPUInstancerSettings.occlusionCullingType, Contents.settingOcclusionCullingTypeOptions);

            GUILayout.Space(5);
            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(gPUInstancerSettings);
            }

            bool updateExists = false;

            EditorGUILayout.BeginVertical(Styles.box);
            GUILayout.Space(5);
            DrawCustomLabel("Update Jobs", Styles.boldLabel);

            if (GPUInstancerDefines.IsVersionUpdateRequired(0.99f, 1.0f))
            {
                updateExists = true;

                DrawColoredButton(new GUIContent("Apply GPU Instancer v1.0.0 Auto Update", HELPTEXT_Version100Update), Colors.green, Color.white, FontStyle.Bold, Rect.zero,
                () =>
                {
                    GPUInstancerDefines.UpdateVersion(0.99f, 1.0f);
                });
            }

            if (!updateExists)
                EditorGUILayout.HelpBox("No actions required for the current version.", MessageType.Info);

            GUILayout.Space(5);
            EditorGUILayout.EndVertical();

#if UNITY_2018_1_OR_NEWER
            EditorGUILayout.BeginVertical(Styles.box);
            GUILayout.Space(5);
            DrawCustomLabel("Package Definitions", Styles.boldLabel);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Toggle("HDRP Loaded", gPUInstancerSettings.isHDRP);
            EditorGUILayout.Toggle("LWRP Loaded", gPUInstancerSettings.isLWRP);
            EditorGUILayout.Toggle("URP Loaded", gPUInstancerSettings.isURP);
            EditorGUILayout.Toggle("ShaderGraph Loaded", gPUInstancerSettings.isShaderGraphPresent);
            EditorGUI.EndDisabledGroup();

            DrawColoredButton(new GUIContent("Reload Packages"), Colors.green, Color.white, FontStyle.Bold, Rect.zero,
            () =>
            {
                gPUInstancerSettings.packagesLoaded = false;
                GPUInstancerDefines.LoadPackageDefinitions();
            });

            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
#endif

            #region Extensions
            if (gPUInstancerSettings.extensionSettings != null && gPUInstancerSettings.extensionSettings.Count > 0)
            {
                if (extensionEditors == null)
                    extensionEditors = new Dictionary<GPUInstancerSettingsExtension, Editor>();
                EditorGUILayout.BeginVertical(Styles.box);
                GUILayout.Space(5);
                DrawCustomLabel("Extension Settings", Styles.boldLabel);

                Editor extensionEditor;
                gPUInstancerSettings.extensionSettings.RemoveAll(s => s == null);
                foreach (GPUInstancerSettingsExtension extensionSettings in gPUInstancerSettings.extensionSettings)
                {
                    if (!extensionEditors.ContainsKey(extensionSettings))
                    {
                        extensionEditor = Editor.CreateEditor(extensionSettings);
                        extensionEditors.Add(extensionSettings, extensionEditor);
                    }
                    else
                        extensionEditor = extensionEditors[extensionSettings];
                    if (extensionEditor != null)
                        extensionEditor.OnInspectorGUI();
                }

                GUILayout.Space(5);
                EditorGUILayout.EndVertical();
            }
            #endregion
        }
    }
}