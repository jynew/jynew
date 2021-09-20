The example scenes contained under the Demos folder are by default designed for the Standard Pipeline. 
If you are using URP or HDRP in your project, you can extract the respective override packages in order to view these scenes correctly.

If you are using Unity 2020.2 or later, you will need the SRP 10 and later packages:
- If you are using Universal Render Pipeline, you can extract the package "URP_Demos_Package_(10_&_Later)" under the "GPUInstancer/Demos/" folder.
- If you are using High Definition Render Pipeline, you can extract the package "HDRP_Demos_Package_(10_&_Later)" under the "GPUInstancer/Demos/" folder.

If you are using a Unity version below 2020.2, you can extract the SRP 8 and before packages:
- If you are using Universal Render Pipeline, you can extract the package "URP_Demos_Package_(8_&_Before)" under the "GPUInstancer/Demos/" folder.
- If you are using High Definition Render Pipeline, you can extract the package "HDRP_Demos_Package_(8_&_Before)" under the "GPUInstancer/Demos/" folder.

These packages contain the SRP versions of the demo scene materials, custom shaders, and in some cases scene and terrain files as well. 
When extracted, they will override the Standard Pipeline versions of these files in the Demos folder. Please also note that the "10 & Later" packages contain 
ShaderGraph 10 versions of GPUI shaders.

If, for any reason, you need to revert back to the Standard Pipeline versions of these files after extracting a pipeline override package, 
you can simply re-import the "Demos" folder from the asset store and the SRP changes will be reverted back.