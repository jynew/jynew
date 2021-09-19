GPU Instancer v1.4.6
Copyright ©2018 GurBu Technologies
---------------------------------
Thank you for supporting GPU Instancer!

---------------------------------
DOCUMENTATION
---------------------------------
Please read our online documentation for more in-depth explanations and customization options at:
http://wiki.gurbu.com/

---------------------------------
SETUP
---------------------------------
1. Add the desired Managers to your scene

1.1. Add Prefab Manager
Tools -> GPU Instancer -> Add Prefab Manager

1.2. Add Detail Managers to your scene (Requires at least one Unity terrain present in the scene)
Tools -> GPU Instancer -> Add Detail Manager For Terrains

1.3. Add Tree Managers to your scene (Requires at least one Unity terrain present in the scene)
Tools -> GPU Instancer -> Add Tree Manager For Terrains

2. In the Inspector window, press the "?" button at the top-right corner to get detailed information about setting up the manager.

Additionally, you can check the Getting Started Documentation from the following website:
https://wiki.gurbu.com/index.php?title=GPU_Instancer:GettingStarted


Best Practices
---------------------------------
It is highly recommended to read the best practices wiki page to get the most out of GPU Instancer:
https://wiki.gurbu.com/index.php?title=GPU_Instancer:BestPractices

Tools
---------------------------------
Tools -> GPU Instancer -> Scene Prefab Importer
The Scene Prefab Importer is designed to easily define prefabs from the existing prefab instances in your scenes to the GPUI Prefab Manager as prototypes.

Tools -> GPU Instancer -> Prefab Replacer
Replaces the selected GameObjects in the scene hierarchy with the selected prefab as instances of that prefab.

Edit -> Preferences -> GPU Instancer
GPU Instancer Settings can be used to personalize the GPUI Manager editors according to your needs.

Additional Components
---------------------------------
GPU Instancer includes additional MonoBehaviour components that you can use to manage custom operations. For detailed information on these components, you can 
check the GPU Instancer Getting Started Documentation page at:
https://wiki.gurbu.com/index.php?title=GPU_Instancer:GettingStarted#Components

---------------------------------
SUPPORT
---------------------------------
F.A.Q.: https://wiki.gurbu.com/index.php?title=GPU_Instancer:FAQ

If you have any questions, requests or bug reports, please email us at: support@gurbu.com
Unity Forum Thread: https://forum.unity.com/threads/gpu-instancer.529746/

---------------------------------
DESCRIPTION
---------------------------------
GPU Instancer is an out of the box solution to display extreme numbers of objects on screen with high performance. 
With a few mouse clicks, you can instance your prefabs and Unity terrain details and trees.

GPU Instancer provides user friendly tools to allow everyone to use Indirect GPU Instancing without having to go through 
the deep learning curve of Compute Shaders and GPU infrastructure. Also, an API with extensive documentation is provided 
to manage runtime changes.

To provide the fastest possible performance, GPU Instancer utilizes Indirect GPU Instancing using Unity's 
DrawMeshInstancedIndirect method and Compute Shaders.

GPU Instancing results in magnitudes of performance improvement over static batching and mesh combining. Also, other available 
solutions for GPU Instancing (including Unity's material option and the DrawMeshInstanced method) fail short on limited buffer 
sizes and therefore result in more draw calls and less performance. By using the indirect method GPU Instancer aims to provide 
the ultimate solution for this, and increases performance considerably while rendering the same mesh multiple times.

GPU Instancer consists of three main Monobehavior classes named "GPU Instancer Prefab Manager", "GPU Insancer Detail Manager" and  "GPU Insancer Tree Manager".

GPU Instancer Prefab Manager
---------------------------------
By adding your prefabs to the Prefab Manager, the prefab instances you add to your scenes are automatically rendered by GPU Instancer.
It also provides additional functionality such as adding/removing instances at runtime, changing material properties per instance, 
and enabling/disabling GPU Instancing and Rigidbodies on instanced objects (at a specified area) at runtime.

Note that prefab manager only accepts user created prefabs. It will not accept prefabs that are generated when importing your 3D model assets.

GPU Instancer Detail Manager
---------------------------------
Detail Manager takes over rendering of detail prototypes added to your Unity terrain.
It comes with a grass shader which gives you the ability to customize how your grass will look on your terrain with a set of shader 
properties that can be edited from the manager.

GPU Instancer Tree Manager
---------------------------------
Tree Manager takes over rendering of tree prototypes added to your Unity terrain. Billboards are automatically generated for Trees and added as the 
final LOD level in the prototype's rendering pipeline.

---------------------------------
FEATURES
---------------------------------
- Out of the box solution for complex GPU Instancing.
- VR compatible. Works with both single pass and multipass rendering modes.
- Mobile compatible. Works with both iOS and Android.
- Easy to use interface.
- Tens of thousands of objects rendered lightning fast in a single draw call.
- GPU frustum culling.
- GPU occlusion culling (also supports VR platforms with both single pass and multipass rendering modes).
- Automatically configured custom shader support.
- Supports Standard, Universal and HD Render Pipelines.
- Complex hierarchies of prefabs instanced with a single click.
- Multiple sub-meshes support.
- LOD Groups and cross-fading support. (Cross-fading is supported on Standard Render Pipeline only)
- Automatic 2D Billboard generation system (auto-added as last LOD).
- Shadows casting and receiving support for instances (frustum culled instances still can cast shadows).
- Ability to use custom shadow distance per prototype and to choose the LOD to render shadows with.
- Support for Floating Origin handling.
- Multiple camera support.
- Unity 5.6 support.
- Well documented API for procedural scenes and runtime modifications (examples included).
- Ability to Remove instances inside bounds or colliders at runtime.
- Ability to extend with custom Compute Shaders.
- Example scenes that showcase GPU Instancer capabilities.

Prefab Instancing Features:
- Ability to automatically instance prefabs at your scene that you distribute with your favorite prefab painting tool.
- Automatically Add-Remove prefab instances without any aditional code.
- Automatic detection and updating of transform position, rotation and scale changes.
- Full or area localized rigidbody and physics support.
- Nested Prefabs support (Unity 2018.3 and later).
- Add-Remove-Update prefab instances with or without instantiating GameObjects (examples included).
- Instance based material variations through API (similar to Material Property Blocks). (Not supported on Android because of buffer limitations)
- Enabling and disabling instancing at runtime per instance basis.
- API to manage instanced prefabs at runtime.
- Includes mobile demo scene with custom controllers.

Detail Instancing Features:
- Dense grass fields and vegetation with very high frame rates.
- Included vegetation shader with wind, shadows, AO, billboarding and various other properties.
- Support for custom shaders and materials.
- Cross quadding support: automatically turns grass textures to crossed quads.
- Ability to paint prefabs with custom materials on Unity terrain (with Unity terrain tools).
- Ability to use prefabs with LOD Groups on Unity terrain.
- Further performance improvements with automatic spatial partitioning.
- API to manage instanced terrain detail prototypes at runtime (examples included).
- Editor GPU Instancing simulation. 

Tree Instancing Features:
- Dense forests with very high frame rates.
- Speed Tree support with wind animations.
- Speed Tree 8 support with wind animations.
- Tree Creator support with wind animations.
- Included billboard baker and renderers.

Extension Packs:
- Crowd Animations: Provides workflows for GPU Instancer to be used with animated crowds.
  https://assetstore.unity.com/packages/tools/animation/gpu-instancer-crowd-animations-145114

Third Party Integrations:
- Gaia integration.
- Map Magic integration.

---------------------------------
MINIMUM REQUIREMENTS
---------------------------------
- DirectX 11 or DirectX 12 and Shader Model 5.0 GPU (Windows, Windows Store)
- Metal (macOS, iOS)
- OpenGL Core 4.3 (Windows, Linux)
- Vulkan (Android, Windows, Linux)
- OpenGL ES 3.1 (Android 8.0 Oreo or later)
- Modern Consoles (PS4, Xbox One)

---------------------------------
KNOWN LIMITATIONS
---------------------------------
There are certain limitations that are introduced by the way Indirect GPU instancing works and/or how Unity handles it. 
You can find a list of the known limitations in the following wiki page:
https://wiki.gurbu.com/index.php?title=GPU_Instancer:FAQ#What_are_the_Known_Limitations.3F

---------------------------------
DEMO SCENES
---------------------------------
You can find demo scenes that showcase GPU Instancer capabilities in the "GPUInstancer/Demos" folder. 
These scenes are only for demonstration and you can safely remove this folder from your builds.
Some demos also have a "PostProcessing" folder. For best visual results, you can add a "Post-Processing Behavior" 
script to the scene cameras and add the setting object to this script from this folder.

SRP NOTE: If you are using URP or HDRP, you can extract the corresponding override packages under the "GPUInstancer/Demos/" 
folder to view the demo scenes in your project. For more information, please check the "Demo_Packages_SRP-Readme" file under the same folder.

---------------------------------
CHANGE LOG
---------------------------------

v1.4.6

Fixed: Occlusion culling depth texture format error on HDRP
Fixed: PrefabInstancing demo scene out of bounds error

v1.4.5

New: Receive Shadows option on Mesh Renderers of a prefab is taken into consideration while making the DrawMeshInstancedIndirect calls
New: Added ChangeLODShadow API method to edit LOD shadows at editor mode and/or runtime
New: Added "autoIncreaseBufferSize" optional parameter to AddPrefabInstance API method

Changed: Improved editor performance of managers by significantly reducing the amount of text read calls made by auto-shader conversion

Fixed: GPUInstancerPrefab component serializing state
Fixed: Map Magic 2 integration IMultiBiome compile error
Fixed: Unnecessary draw calls are being made for disabled LOD shadows
Fixed: SetDataPartial setting data more than once in some cases

v1.4.4

New: Added foliage support packages for HDRP 10 and URP 10
New: Added demo override packages that support HDRP 10 and URP 10

Fixed: Occlusion culling depth texture not scaling correctly when render scale is not 1 on URP
Fixed: CopyTexture format incompatibility issue for HDRP 10
Fixed: AddModifyTerrainsRuntimeDemo generated terrains now use the correct terrain shader in URP

Changed: HDRP 10 and URP 10 GPUI shaders are remade with ShaderGraph 10

v1.4.3

New: Added a sub graph and a demo scene for material variations with Shader Graph

Fixed: Instances are not rendered when editor is paused
Fixed: Map Magic 2 integration can not import objects from graphs with biomes
Fixed: Shadow distance is now correctly read from the Universal Render Pipeline Asset in URP.
Fixed: XR and VR libraries are now checked for and the VR logic defaults to non-VR if both libraries are not present.

v1.4.2

New: Multiple Terrains support for Tree Manager
New: Map Magic 2 integration
New: Added API methods to set buffer data using NativeArrays
New: Added Assembly Definition files

Changed: MapMagic integration files added to an extra unitypackage which can be extracted on demand

Fixed: When generating new prototype data Tree and Detail manager overriding existing one if the file names are the same

v1.4.1

New: ShaderGraph Node for adding GPU Instancer setup on newer Shader Graph versions

Changed: Increased Occlusion Cull Offset max value to 0.1
Changed: When GPUInstancer folder is moved under another folder (e.g. Plugins) new files will be created at the new location instead of Assets/GPUInstancer
Changed: LOD cross-fade feature is disabled for HDRP and URP (should fix Tree Manager sometimes not showing trees in HDRP)

Fixed: Settings file not being loaded when an API call is made before a GPUI Manager is initialized (Should also fix some Demo scenes getting null pointer exception when first loaded in newer Unity versions)
Fixed: Occlusin culling bounding box depth value mapped to -1,1 while it should be 0,1 (Many thanks to Dmitry Nozhnin for pointing out this issue!) - This now results in more but still accurate culling
Fixed: SRP auto-shader conversion for custom shaders updating include paths incorrectly
Fixed: Auto-converted shaders being updated after original shader is modified when auto-shader conversion is disabled
Fixed: DefineGameObjectAsPrefabPrototypeAtRuntime API null pointer exception when attachScript parameter is false
Fixed: Billboard Generator creating faulty billboard atlasses when texture resolution in project quality settings is set to a value other than "Full Res"
Fixed: LOD Color Debugger and Draw Call Debugger not showing colors on SRP

v1.4.0

New: Added HDRP foliage shader support
New: Added an override package containing the Universal Render Pipeline versions of GPUI demo scenes
New: Added an override package containing the High Defition Render Pipeline versions of GPUI demo scenes
New: Added Universal Render Pipeline support for SpeedTree 7 and 8 trees
New: Added Update Simulation On Change option to update terrain detail simulation when terrain is edited in Editor Mode

Changed: Detail Manager now logs a relevant error message if the required pipeline version of the GPUI foliage shader is missing
Changed: DefineAndAddVariationFromArray now allows to use an array smaller than the buffer size

Fixed: Infinite import loop issue when using Unity 2019.3 or higher
Fixed: Obsolete virtualRealitySupported API warning
Fixed: URP foliage shader culling back of vegetation quads

v1.3.0

New: Added support for Universal Render Pipeline
New: Foliage Shader Support for URP

Changed: Render pipeline detection will run automatically when GPUI is upgraded to a new version

Fixed: Editor layout issues in Unity 2019.3

v1.2.7

Changed: Initialize in threads default value set to false for Detail Manager (causing issues for some setups)

Fixed: Occasional removal warnings when quiting the game while Auto Add/Remove Instances feature is enabled
Fixed: Gaia integration spawner error for Pro version
Fixed: Pink terrain on AddModifyTerrainsRuntime demo on new Unity versions

v1.2.6

New: Added an option to keep simulation live for the Detail Manager in Editor mode
New: Added a version of the Foliage shader that uses Lambert Lighting

Fixed: Detail simulation not working in the HDRP 6.9.0 in Editor mode
Fixed: Shader conversion removing UnityCG.cginc reference within CGINCLUDE

v1.2.5

Fixed: MapMagic integration build error

v1.2.4

Changed: Added unassigned value and model prefab warnings to MapMagic integration

Fixed: Invalid Cast exception when using multiple variations with different types
Fixed: Out of range exception when removing a destroyed object from prefab manager
Fixed: Missing profiler endsample error when occlusion culling is disabled

v1.2.3

New: Added option to disable light probe usage

Changed: Auto. Add/Remove Instances freature now also works with enabling/disabling GameObjects at runtime
Changed: Prefab Manager API methods GC optimization

Fixed: Dither Crossfade errors on 2019.2
Fixed: compute shader errors on the new HDRP versions

v1.2.2

New: Added Occlusion Cull Offset setting to prototypes
New: Added Occlusion Cull Accuracy setting with 3 levels of accuracy to prototypes
New: Added option to increase custom shadow distance farther than the shadow distance of the Quality settings

Changed: Auto Add/Remove feature now also initializes Prefab Manager when a new instance is added before initialization
Changed: DefineGameObjectAsPrefabPrototypeAtRuntime method sets the Prefab Manager's defult minCullingDistance to the defined prototype
Changed: Improvements for occlusion culling mip calculations

Fixed: Move component error when Terrain is a prefab
Fixed: HiZOcclusionGenerator and TreeProxy shaders not being included in builds automatically 

v1.2.1

New: Added settings to edit GPU max. thread counts and max. compute buffers
New: Added settings to switch between Occlusion culling modes
New: Added Draw Call Count info on the Registered Instances section on the manager editors
New: Option to not to re-enable mesh renderers when prefab manager is disabled
New: Option to set layer mask for ignoring renderers on managers

Changed: C# and HLSL code refactoring for better multi-platform support
Changed: PrefabsWithoutGameObjects demo script to exemplify usage of GPUI API for partial buffer update
Changed: Default max compute shader thread group counts for each platform

Fixed: Incorrect occlusion culling mip map calculation
Fixed: Incorrect occlusion culling on Metal
Fixed: SetLODBias API index out of bounds exception when using more than 4 LOD levels
Fixed: Prefab editor simulation not working when mesh renderers are disabled
Fixed: UpdateVisibilityBufferWithMatrix4x4Array API can not find runtime data error when instance count is zero
Fixed: Shader code to show no LOD cross fade warnings
Fixed: Compute Shader code to show no temp register warnings and uninitialized variable warnings on DX11 and GLCore

v1.2.0

New: Occlusion Culling now works in LWRP and HDRP
New: Added Occlusion Culling debugger with overlaying option
New: Added LWRP and HDRP support for instancing simulation in the Editor
New: Added setting option to disable auto shader conversion and auto shader variation handling
New: Added DrawCallColorDebugger

Changed: Occlusion Culling performance improvements
Changed: Added error messages for materials with missing shaders
Changed: Added error messages for detail and tree prototypes with missing prefabs

Fixed: Prototype ordering error when same terrain used in multiple scenes
Fixed: GPUIStandardInclude.cginc Unity 5.6.6-7 compatibility issues
Fixed: GPUInstancerPrefabRuntimeHandler error when using multiple prefab managers in multiple scenes with the same prototypes

v1.1.4

New: Context menu item to setup materials for GPUI support from editor without using managers

Changed: Added set renderers enabled/disabled option for enable/disable instancing API methods
Changed: UpdateVisibilityBufferWithMatrix4x4Array can now optionally be used for uploading partial data to buffers
Changed: Variation buffers can now be defined after the Prefab Manager is initialized
Changed: UpdateVariationFromArray can now optionally be used for uploading partial data to buffers
Changed: Color variation shader example edited to work in all platforms

Fixed: Auto. Add/Remove does not modify variation buffer sizes

v1.1.3

New: Context menu item to setup shaders for GPUI support from editor without using managers
New: API method to change mesh at runtime
New: API method to run auto shader conversion through editor scripts
New: API methods to start/stop GPUI rendering simulation in editor mode
New: API method to add shader variants through editor scripts
New: Added Min Distance culling option
New: Added Bounds Size Offset option

Changed: TerrainData instanceID check changed to GUID to not to update SOs frequently
Changed: Duplicated prefabs are now added to the Managers as seperate prototypes

Fixed: Instance Remover wrong position calculation with box colliders
Fixed: Scene Prefab Importer window error with child transforms

v1.1.2

New: API method to change materials at runtime
New: API method to change instance counts at runtime
New: API method to get the instance data array
New: API method to get the prototype list of the GPUI manager

Changed: Optimization for Auto. Add/Remove instances feature
Changed: Optimization for runtime API methods

Fixed: Instance count being ignored on Compute Shader visibility calculations
Fixed: Unity 2019.1 obsolete PreferenceItem warning fix
Fixed: Compile error with AssetDatabase reference

v1.1.1

New: Added LWRP support for the GPUI Foliage Shader
New: Added ShaderGraph GPUI Setup node (ShaderGraph v4.x only)
New: Added a new overload for the InitializeGPUInstancer API method where you can initialize a specific prefab prototype
New: Added Prefab Instance Serialization advanced action, to render prefab instances without GameObjects by saving transform data without aditional coding (Unty 2018.1 and above only)
New: Added experimental Simulation feature to render instances in Edit Mode for disabled mesh renderers and serialized prefab instances
New: Added LOD color debugger tool to visualize prototype LOD levels

Changed: DrawMeshInstancedIndirect argsBuffer stride size is fixed to uint and the count is set according to the renderer amount
Changed: InitializeWithMatrix4x4Array API method so that it is not required to call the InitializeGPUInstancer before calling this method
Changed: Optimization for Prefab Manager Editor which will no longer re-enable renderers while exiting Play Mode
Changed: Nested Prefabs Optimization for Scene Prefab Importer tool

Fixed: DefineGameObjectAsPrefabPrototypeAtRuntime API method null reference exception when the manager is created at runtime
Fixed: HiZ Occlusion Generator controls for stereoTargetEye as "both eyes" when VR is enabled only
Fixed: Compute Buffer not set error when using 5 or more LOD levels

v1.1.0

New: Occlusion culling VR support (both single pass and multi pass)
New: API method to change LOD Bias values for prototypes at runtime
New: Setting to edit instancing bounds size
New: Integration to version control systems like Perforce which uses readonly files
New: Options to disable scaling on Tree prototypes
New: Added custom depth normals shader
New: MapMagic integration Biome support

Changed: Modified foliage shader to use the custom depth normals shader

Fixed: GLES shadow rendering error
Fixed: Occlusion culling disabling DepthNormals on the camera
Fixed: Null pointer error when Prefab Manager manager is disabled with Auto. Add/Remove Instances enabled
Fixed: Prefab Manager not creating Proxy for SpeedTree8
Fixed: Map Magic integration GameObject not being saved in scene when imported
Fixed: Detail Manager not finalizing initialization when the activeThreads list is not cleared

v1.0.2

Changed: LOD cross-fading feature is optimized to run according to shader feature and graphics API
Changed: OpenGL compute shader max thread count changed to 512

Fixed: OpenGL compute shader matrix type error
Fixed: OpenGL not rendering instances with shaders that has cross-fading enabled
Fixed: DrawMesh error when there are more materials than the number of submeshes on the renderer
Fixed: SpeedTree 8 shader import dialog showing multiple times during prototype generation

v1.0.1

Fixed: Settings showing wrong labels for prefab options

v1.0.0

New: SpeedTree8 support with top-down billboards
New: Editor multiple add/edit/remove option for Prototypes
New: Edit function to Tree and Detail prototypes to change the prefabs
New: GPU Instancer Preferences to personalize various editor behaviours
New: Auto-update system for upgrading from previous GPUI versions
New: Auto. generated Shader Variant Collection for including used shader variants in builds automatically
New: Auto. Floating Origin handling option
New: Instance count warning system for Prefab Managers
New: CameraHandler component to manage camera switching automatically
New: Text-Mode for prototype lists
New: Custom Preview texture drawing system for prototypes

Changed: Shaders moved out of Resources folder to reduce the amount of shaders included in builds
Changed: Included shaders are modified according to new folder structure
Changed: Tree Manager coroutine initialization is now optional
Changed: Manager Editor sections changed to Foldouts
Changed: Reduced asset package size considerably by optimizing demo assets

Fixed: Dark preview icons for Unity 2018.3 versions on Linear color space
Fixed: Tree Manager showing trees in wrong positions when terrain position is changed during initialization
Fixed: Show Rendered Amount GUI not showing zero instance count when draw calls are disabled
Fixed: Prefab manager adding incorrect rigidbodies to instances when disabled

v0.9.9

New: Threading option for Detail Manager initialization
New: Option to disable Mesh Renderers on prefabs to increase initialization speed
New: Prefab streaming system with PrefabListRuntimeHandler component
New: Added an option to all billboards to face camera position (ignoring camera View Matrix rotations, ideal for VR)
New: Min. Culling Distance option per prototype
New: Option to filter prototypes when removing instances with colliders or bounds
New: MapMagic Integration single Prefab Manager option
New: MapMagic Integration Threading option for Prefabs when using single Prefab Manager with disabled Mesh Renderers
New: Added GetDetailMapData/SetDetailMapData API methods for Detail Manager

Changed: SetGlobalPositionOffset null checks and warnings added
Changed: AddPrefabInstance method optimization when automatically increasing buffer size

Fixed: Tree Proxy not being created when using no-GameObject workflow
Fixed: Tree Proxy not being cleared when proxy GameObject is destroyed
Fixed: Built-in shader source code not recognized without updating material when first added to project
Fixed: Prototype preview icon lighting problem for Unity 2017 on Linear color space
Fixed: Cross-fade method error on shaders when building in Unity 5.6 versions
Fixed: Possible memory leak on repeated initialization for visibility buffer
Fixed: SetGlobalPositionOffset not updating spatial partitions on Detail Manager
Fixed: GPUInstancerInclude PSSL dot method casting error when building for PS4

v0.9.8

New: Managers can now continue rendering in Scene View camera while the editor is paused
New: All managers and tools now have "Wiki" buttons that will take you to their respective documentation pages
New: Tree Managers now have an option to disable the random rotation of trees on Unity terrains

Changed: Automatic shader conversion update for LWRP/HDRP 4.3.0 version
Changed: Menu items moved under Tools
Changed: The way prototype previews are displayed
Changed: The GPUI Soft Occlusion Shader is renewed to match the original Nature/Soft Occlusion Shaders better
Changed: Billboard Generator brightness option has been modified to match the original colors better when brightening

Fixed: GPUIStandardInclude.cginc errors on some Unity 2017.2 versions
Fixed: Standard Shader Cutout variant not showing in build
Fixed: Meshes with a single submesh but multiple materials causing issues in rendering
Fixed: LOD cross-fading results changing with time scale

v0.9.7

New: Added Render With Selected Camera option to limit rendering to one camera
New: Added Detail Texture Layer option to Detail Manager
New: Added support for the internal "Standard (Roughness setup)" shader
New: Added option to use frustum and occlusion culling for shadows
New: Added UpdateVariation API method to update variation buffer at runtime
New: Added a new demo scene that showcases the use of custom compute shaders to extend GPUI

Changed: Managers now show the Unity Archive link to download built-in shaders, if necessary
Changed: Improved the CPU times when handling transform changes

Fixed: Billboards not using the original prefab's Layer
Fixed: SpeedTrees without BillboardRenderer not showing billboard distance options
Fixed: Standard shader error for Unity 2017.3 versions
Fixed: Billboard renderer causing a minor artifact in forward rendering and single-pass VR when there is a CommandBuffer using the depth texture
Fixed: A bug that would prevent builds from Unity 2018.3 beta versions 

v0.9.6

New: Added a grass mower model and grass mowing mode for the detail instancing scene
New: Added shadow casting option for custom billboards

Fixed: Possible initialization problems when having multiple prototypes with no-gameobjects
Fixed: Manager showing as initialized before gpu buffers are generated
Fixed: MapMagic integration missing min. culling distance setting

v0.9.5:

New: Nested-Prefabs support (Unity 2018.3 and later)
New: Tree instancing demo scene
New: Optional minimum culling distance setting
New: Shadow Distance option per prototype
New: Shadow LOD options
New: Clear Shader Bindings menu item
New: Prefab Replacer tool for easily replacing GameObjects in scene with prefab instances
New: Brightness setting for baked billboard textures
New: Added a cutoff override option for the billboard generator

Changed: Shader conversion automatically disables other procedural instancing setups to enable GPUI support for various shader assets
Changed: Internal shaders are now marked hidden to remove clutter

Fixed: An error in 2018.3 Beta where an issue with the way DateTime worked caused the managers to re-convert shaders
Fixed: Billboard normal atlas dilation causing bright outlines
Fixed: TreeCreator billboard material properties and reversed normals issue
Fixed: Shader conversion error when project is updated while the Manager is running in editor
Fixed: Wrong bounds calculation when first mesh in a prefab is scaled
Fixed: Standard (Specular) shader now works with LOD cross-fading

v0.9.4:

New: Tree instantiation calculations are made in Compute Shaders for better performance

Changed: Automatically create GPUI supported version for custom billboard shader

Fixed: Map Magic integration not adding Tree Manager for pinned terrains

v0.9.3:

New: API Method to remove instances inside a given Bounds or Collider
New: Added grass removal function to Detail Instancing Demo
New: Added Instance Remover script which removes instances based on colliders of the object when initialized
New: Support for Unity 2018.3 prefab system
New: Added custom billboard option to billboard generator
New: Added billboard extensions for custom billboard generators

Changed: Added forceNew parameter to InitializeGPUInstancer API method
Changed: Map Magic integration Editor max prefab distance increased to 10000
Changed: Added SetCamera method to Map Magic integration class

Fixed: Can not find manager warning when instances were initialized before the manager with "Auto. Add/Remove Instances" option enabled
Fixed: Map Magic integration Editor not showing prototypes when there are only Trees
Fixed: Map Magic integration Editor not updating shader and billboard bindings

v0.9.2:

New: API Method to define prototypes for procedurally generated GameObjects without prefabs at runtime
New: API Method to add instances to a prototype from a list of GameObjects at runtime
New: API Method to remove prototype definition on the manager at runtime
New: Demo Scene showcasing how to add-remove procedurally generated GameObjects on the Prefab Manager
New: Tree Creator billboards now have their own shader that supports their translucency color

Changed: Enable Auto. Update Transform by default if there is rigidbody on prefab
Changed: Do not create runtimedata when there are no MeshRenderers
Changed: Prefab Manager max distance editor limit increased to 10.000
Changed: Memory optimization on Tree Manager initialization
Changed: Improved SpeedTree billboards

Fixed: UpdateVariationFromArray invalid cast exception for multiple variation with different types
Fixed: Duplicate Hi-Z Generator components when SetCamera is called for the same camera multiple times
Fixed: Tree Manager not calculating prefab scale
Fixed: Missing reference exception when disabling manager for procedurally generated GameObjects
Fixed: Prefab instances duplicating when dropped on the manager multiple times
Fixed: Forward rendering GPUIStandardInclude.cginc fog error for Unity 2018.2.1 and later versions
Fixed: Billboard mesh showing wrong atlas frames on some angles (Requires rebake of previous billboards)
Fixed: Billboard colors and lighting corrected for linear color space
Fixed: Tree Creator trees windzones now work correctly
Fixed: Tree Creator billboards crossfading now works correctly 

v0.9.1:

New: Added support for the High Definition Render Pipeline (HDRP)
New: Added support for the Lightweight Render Pipeline (LWRP)
New: Automatic shader conversion now also works for SRP shaders
New: Added option to use custom Health/Dry noise texture for each detail prototype

Changed: Further optimized Detail Manager memory usage for Android

Fixed: Automatic shader conversion now creates shaders in an internal folder if the original shader is in "Packages" (Package Manager)
Fixed: Error on shader conversion for ShaderGraphs
Fixed: GPUIStandardInclude.cginc errors on some Unity 2017 versions

v0.9.0:

New: Tree Manager for Unity terrain trees
New: Speed Tree support with wind animations
New: Automatic Billboard generation system (designed and tested for Unity terrain trees)
New: Gaia integration
New: "Auto. Add/Remove Instances" option for Prefab Prototypes
New: "Auto. Update Transform Data" option for Prefab Prototypes
New: API Methods to Add-Remove-Update prefab instances without instantiating GameObjects (Demo scene included)
New: API Method to offset transform positions of all instances
New: Shadow rendering performance improvements and LOD settings
New: LOD cross-fade support (with animation or fade transition width)
New: LOD bias adjustments
New: Prefab Scene Importer Tool
New: Custom vertex color wind animation support for Soft Occlusion Tree shaders
New: Limited support for Tree Creator trees
New: Added mobile version of the Prefab Instancing Demo scene with mobile controllers

Changed: Prefab instances in the scene can now also be added to the Prefab Manager to create prototypes
Changed: Furter optimized update functions to reduce overhead on loops
Changed: Mesh Renderer "Cast Shadow" option is taken into consideration when rendering shadows
Changed: Various editor improvements for ease of use

Fixed: Vulkan API detail rendering compute shader error and UAV count warning
Fixed: Vulkan API foliage shader blinking issue
Fixed: Vulkan API not rendering prefabs with more than 3 LOD levels
Fixed: Added Standard shader vairants (Cutout, Transparent, Fade) for instanced shader features not compiling in builds
Fixed: Missing AddRemoveInstancesDemo scene prefab references

v0.8.3:

New: Added Detail Density for Detail Manager

Fixed: SetCamera now does not add the Occlusion Culling buffer to the camera if Occlusion Culling is turned off in the manager.
Fixed: Grass initialization kernel position 0,0 random position and rotation generation problem
Fixed: Prototype list null check on initialization and registered prefabs count check on register

v0.8.2:

Fixed: Multiple fixes and improvements for OpenGL ES 3.1 and Vulkan API on Android target platforms
Fixed: Auto custom shader conversion DateTime error when updating project

v0.8.1:

New: GPU Occlusion Culling (for non-VR platforms)
New: Map Magic integration
New: Max LOD levels increased to 8
New: Detail initialization is now faster
New: Detail spatial partitioning memory and performance optimizations
New: Auto-converted versions of custom shaders are now automatically updated when the originals are modified
New: Option to use original shaders for shadow casters on prototypes that fall outside the view frustum
New: Layers are now taken into consideration with instancing
New: Warning messages in manager editor windows
New: Terrain is now set automatically if Detail Manager is added on the terrain GameObject
New: Additional API methods added to manage registered prefabs
New: Out of bounds check for prefab API methods
New: Added scripting define symbol: GPU_INSTANCER

Changed: Detail simulation improvements, works with frustum culling
Changed: Max detail viewing distance increased to 2500
Changed: LOD calculations now better reflects Unity LOD Group settings
Changed: Custom editor properties are reordered and grouped
Changed: Terrain Proxy will not be added if the manager is already added to terrain
Changed: Removed terrainData reference on TerrainSettings for terrains that are generated from scripts
Changed: PrefabInstancingDemoScene now has its own shader for the Asteroid Haze, which is also instanced as a prefab prototype

Fixed: Prefab Manager Custom Editor null reference exception when manager is disabled at runtime
Fixed: TerrainProxy DestroyImmediate multiple times error when detail manager is added under the terrain GameObject
Fixed: Built-in shader can not be instanced warning loop. Now shows error in custom editor when instanced shader was not able to be created, and throws a warning in play mode
Fixed: Multiple fixes for some iOS, tvOS and macOSX platforms using the Metal API
Fixed: Multiple fixes and improvements for Vulkan API on android target platforms