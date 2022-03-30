This folder contains specialized shaders for Mali 400/450 or other GPU that uses 32bit vertex and 16bit fragment.

In that case, whenever the uv is calculated in fragment shader, it degenerates from 32bit to 16bit. So the uv should be somehow baked into the mesh instead of being transformed in shaders; then used by sampler directly in fragment shader without any calculation.

Those shader should be used on a tilling-baked mesh created by the Mesh Tilling Baker: Window -> Mesh Terrain Editor -> Tools -> Specialized -> Mesh Tilling Backer .
See https://docs.unity3d.com/Manual/SL-DataTypesAndPrecision.html

Don't use these shaders if you don't know how to use it.

Usage:

1. Convert a normal mesh terrain to tilling-baked mesh with the Mesh Tilling Baker.
2. Set the material to a specialized shader
3. Set the control texture's tilling to (1 ÷ originalTilling), for example, set the control map tilling to (0.06666 = 1 ÷ 15) if the original tilling of every splat-textures is 15.
