**************************************
*             BEAUTIFY               *
* Created by Ramiro Oliva (Kronnect) * 
*            README FILE             *
**************************************


How to use this asset
---------------------
We recommend importing the asset in an empty project and run the Demo Scenes provided to get an idea of the overall functionality.
Read the documentation and experiment with the tool.

Later, you can import the asset into your project and remove the demo folder.

Hint: to use the asset, select your camera and add "Beautify" script to it.



Documentation/API reference
---------------------------
The PDF is located in the Documentation folder. It contains additional instructions and description about the asset, as well as some recommendations.



Support
-------
Please read the documentation PDF and browse/play with the demo scene and sample source code included before contacting us for support :-)

* Support: contact@kronnect.me
* Website-Forum: http://kronnect.me
* Twitter: @KronnectGames



Future updates
--------------

All our assets follow an incremental development process by which a few beta releases are published on our support forum (kronnect.com).
We encourage you to signup and engage our forum. The forum is the primary support and feature discussions medium.

Of course, all updates of Beautify will be eventually available on the Asset Store.



Version history
---------------

Version 6.2
- Depth of Field: added High Quality option to Foreground blur
- Added Better Fast LUT in Shader Options
- Editor & standalone optimizations

Version 6.1.1
- [Fix] Fixed profile not preserving changes between sessions when hitting "Apply" from Beautify's inspector

Version 6.1
- Added Vignetting Aspect Ratio
- Added Vignetting Blink effect
- Depth of field: added min/max autofocus distance
- Clicking "Apply" to save profile changes will refresh automatically other cameras using same profile
- [Fix] Fixed Metal issue with Sun flares on Unity 2018.1

Version 6.0
- Added LUT support
- Added Pixelate effect with downsampling option
- Added Vignetting Fade Out effect

Version 5.6
- Renamed and expanded Build Options to include new shader options
- Custom bloom & anamorphic flares blur handling for Mali-T720 GPU
- Performance improvements
- Change: Outline Sobel setting is now a shader advanced option

Version 5.5.2
- Added new shader option to enable Sun flares occlusion with objects that don't have colliders (see manual)
- Added new tonemap operator which produces less saturation on bright colors (see manual)
- [Fix] Fixed issue with Sun flares not showing up when bloom and anamorphic flares are not used
- [Fix] Fixed bloom blur aspect ratio bug in best performace mode
- [Fix] Automatic fix for Mali-T720 bug with floating point render textures

Version 5.5.1
- [Fix] Fixed vignetting mask not respecting fully transparent areas
- [Fix] Fixed a compilation issue in BeautifyMobile.cginc for Unity 2017.2+ version

Version 5.5
- VR: added support for VR Single Pass Instanced mode
- Sun Flares: added occlusion mask option
- Eye Adaptation: improved algorithm in Best Quality setting
- Anamorphic Flares: added option to remove blur passes with Best Performance mode
- [Fix] Added shader workaround for Mali G71 GPU hardware issue with normals
- [Fix] Fixed bloom/flares temporarily disappearing when saving the scene

Version 5.4.1
- Improved blur transition

Version 5.4
- New Blur effect: adds a screen blur effect with customizable intensity
- Added Depth Of Field Filter Mode option to control potential artifacts when using exclusion layer mask and/or downsampling
- [Fix] Selecting "Basic" quality setting no longer change extra effects settings

Version 5.3
- Profiles. Create, reuse and share persistent settings profiles
- Changed "Blurred" bloom preset to make it more blurry
- Dithering moved to the end of effect stack to affect bloom and other effects
- [Fix] Under-the-hood inspector improvemeents (undo now works, standard slider behaviour, ...)
- [Fix] Removed some artifacts with DoF Exclusion layer option when MSAA is enabled

Version 5.2.2
- Depth of Field: added max brightness clamp to prevent black artifacts with out of range HDR pixels
- [Fix] Now changing inspector properties will mark the scene as modified
- [Fix] VR: compare mode now works in VR (also SPSR)

Version 5.2.1
- New demo scene 5: particle + bloom layer mask example
- Added Mask Z Bias option to bloom layer mask (enables blooming behind opaque objects)
- [Fix] Fixed bloom layer mask clipping issue with transparent objects
- [Fix] Fixed banding issue with Unity 2017.1 on Android platform

Version 5.2
- VR: Support for Single Pass Stereo Rendering changes in Unity 2017.1
- Added downsampling options to Bloom Culling Mask, Depth of Field Exclusion Mask and Depth of Field Transparent Mask features.
- Added "Boost" options to bloom layers
- Added Max Bright parameter to prevent out of range HDR issues with Bloom and to clamp high luminosity pixels

Version 5.1
- Basic mode no longer activates Depth buffer generation which may save performance in forward rendering path
- Added inspector warning when setting bloom layer mask to everything.
- [Fix] Fixed bloom layer mask issue with Lens Dirt

Version 5.0
- Support for orthographic camera
- Added Rotation DeadZone to Sun Flares
- Added DoF exclusion layer bias parameter
- [Fix] Fixed bloom layer mask issue with VR
- [Fix] Fixed pink issue with DoF exclusion layer and water/reflections shaders

Version 4.5
- Added Sun Flares Tint and Solar Wind Speed options
- Added Bloom layer mask

Version 4.4.1
- [Fix] Fixed black out issue on certain scenes with some pixels out of HDR range
- [Fix] Fixed Depth Of Field grain effect on skybox

Version 4.4
- New feature: Sun Flares! Beautiful, fast and procedural lens flares.
- Added bloom depth atten (reduces bloom effect with distance, useful to cancel bloom on skybox)
- Added bloom & anamorphic flares ultra mode which improves quality especially for VR
- Added full support for Unity Animator window
- Added Basic mode with a super-reduced/fast feature set
- Added Depth of Field exclusion mask
- Changed Outline order so it renders before bloom
- [Fix] Optimization: prevented execution of some script code associated with disabled effects

Version 4.3.2
- Added bloom blur option for Best Performance mode
- Reduced shader keyword usage in compare mode pass
- Texture fetch optimizations for the Best Performance mode
- Vignette Mask and Frame Mask come now disabled by default (to use them, re-enable in Build Options)
- [Fix] Removed unharmful console warnings on Unity 5.6

Version 4.3.1
- [Fix] Fixed shader error when enabling night or thermal vision plus outline

Version 4.3
- Added Outline Sobel method
- Reduced shader keywords by one

Version 4.2.1
- Fixed Eye Adaptation error with Single Pass Stereo Rendering
- Added reminder in inspector to use build options

Version 4.2
- New mask texture support for vignetting and frame effects
- Fixed depth of field transparency issue with antialias on DX11
- Minor internal improvements

Version 4.1.3
- Fixed issue with Unity 5.4 and Beautify attached to prefab

Version 4.1.2
- Fixed Single Pass Stereo Rendering on some configurations

Version 4.1.1
- Added support for Unity 5.5
- Fixed lens dirt effect not working correctly when bloom and anamorphic flares is enabled

Version 4.1
- Added transparency support to Depth of Field effect
- Improved bokeh effect with option to enable/disable it

Version 4.0
- Added ACES tonemap operator
- Added eye adaptation effect
- Added purkinje effect (achromatic vision in the dark + spectrum shift)
- New build options to optimize compilation time and build size
- Better bloom & anamorphic flares when using best performance setting
- Added layer mask field to depth of field autofocus option
- Fixed anamorphic flares vertical spread using incorrect aspect ratio

Version 3.2.1
- Fixed depth of field goint to full blur strength when looking aside an assigned target focus
- Fixed depth of field shader unroll issue

Version 3.2
- Added autofocus option to depth of field
- Fixed depth of field affecting scene camera

Version 3.1
- Added depth of field
- Fixed daltonize filter issue with pure black pixels

Version 3.0.0
- Added anamorphic flares!
- Added sepia intensity slider
- Improved bloom performance

Version 2.4.1
- Demo folder resources renamed to DemoSources to prevent accidental inclusion in builds
- Fixed issue when changing Beautify properties using scripting from Awake event

Version 2.4
- Improved support for 2D / orthographic camera

Version 2.3
- Added vignetting circular shape option
- Improved bloom effect in gamma color space and mobile
- Added 3 new lens dirt textures

Version 2.2.2
- Fixed compare mode with DX/Antialias enabled
- Throttled sharpen presets in linear color space

Version 2.2.1
- Effect in Scene View now updates correctly in Unity 5.4

Version 2.2
- VR: Experimental Single Pass Stereo Rendering support for VR (Unity 5.4)
- Effect now shows in scene view in Unity 5.4
- New Compare Mode options

Version 2.1
- Bloom antiflicker filter

Version 2.0
- Redesigned inspector
- New extra effects

Version 1.0
- Initial Release






