using GPUInstancer.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GPUInstancer
{
    [InitializeOnLoad]
    public class GPUInstancerDefines
    {
        private static readonly string DEFINE_GPU_INSTANCER = "GPU_INSTANCER";

        // billboard extensions
        private static Type _billboardExtensionType;
        private static Assembly _billboardExtensionAssebly;
        public static List<GPUInstancerBillboardExtension> billboardExtensions;
        public static GPUInstancerPreviewCache previewCache;

#if UNITY_2018_1_OR_NEWER
        public static UnityEditor.PackageManager.Requests.ListRequest _packageListRequest;
#endif

        static GPUInstancerDefines()
        {
            if (EditorUserBuildSettings.selectedBuildTargetGroup == BuildTargetGroup.Unknown)
                return;
            List<string> defineList = new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';'));
            if (!defineList.Contains(DEFINE_GPU_INSTANCER))
            {
                defineList.Add(DEFINE_GPU_INSTANCER);
                string defines = string.Join(";", defineList.ToArray());
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
            }

            GetBillboardExtensions();

            EditorApplication.update -= GenerateSettings;
            EditorApplication.update += GenerateSettings;

            if (previewCache == null)
                previewCache = new GPUInstancerPreviewCache();
        }

        static void GenerateSettings()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
                return;

            SetVersionNo();

            EditorApplication.update -= GenerateSettings;
        }

        static void GetBillboardExtensions()
        {
            try
            {
                if (billboardExtensions == null)
                    billboardExtensions = new List<GPUInstancerBillboardExtension>();

                if (_billboardExtensionType == null)
                    _billboardExtensionType = typeof(GPUInstancerBillboardExtension);

                if (_billboardExtensionAssebly == null)
                    _billboardExtensionAssebly = Assembly.GetAssembly(_billboardExtensionType);

                IEnumerable<Type> types = _billboardExtensionAssebly.GetTypes()
                    .Where(p => _billboardExtensionType.IsAssignableFrom(p) && p != _billboardExtensionType);

                foreach (Type type in types)
                {
                    try
                    {
                        ConstructorInfo ci = type.GetConstructor(new Type[] { });
                        GPUInstancerBillboardExtension billboardExtension = (GPUInstancerBillboardExtension)ci.Invoke(new object[] { });
                        billboardExtensions.Add(billboardExtension);
                    }
                    catch (Exception) { }
                }

            }
            catch (Exception) { }
        }

        static void SetVersionNo()
        {
#if UNITY_2018_1_OR_NEWER
            bool forcePackageLoad = false;
#endif
            if (GPUInstancerConstants.gpuiSettings.versionNo != GPUInstancerEditorConstants.GPUI_VERSION_NO)
            {
                UpdateVersion(GPUInstancerConstants.gpuiSettings.versionNo, GPUInstancerEditorConstants.GPUI_VERSION_NO);
                GPUInstancerConstants.gpuiSettings.versionNo = GPUInstancerEditorConstants.GPUI_VERSION_NO;
                if (GPUInstancerConstants.gpuiSettings != null)
                    EditorUtility.SetDirty(GPUInstancerConstants.gpuiSettings);
#if UNITY_2018_1_OR_NEWER
                forcePackageLoad = true;
#endif
            }

#if UNITY_2018_1_OR_NEWER
            LoadPackageDefinitions(forcePackageLoad);
#endif
        }

        public static bool IsVersionUpdateRequired(float previousVersion, float newVersion)
        {
            if (previousVersion < 1 && newVersion >= 1)
            {
                Shader standardShader = Shader.Find(GPUInstancerConstants.SHADER_GPUI_STANDARD);
                if (standardShader != null)
                {
                    string standardShaderPath = AssetDatabase.GetAssetPath(standardShader);
                    if (!string.IsNullOrEmpty(standardShaderPath))
                        return standardShaderPath.Contains("GPUInstancer/Resources/Shaders");
                }
            }
            return false;
        }

        public static void UpdateVersion(float previousVersion, float newVersion)
        {
            // v1.0.0 Update
            if (previousVersion < 1 && newVersion >= 1)
            {
                Shader standardShader = Shader.Find(GPUInstancerConstants.SHADER_GPUI_STANDARD);
                if (standardShader != null)
                {
                    string standardShaderPath = AssetDatabase.GetAssetPath(standardShader);
                    if (!string.IsNullOrEmpty(standardShaderPath))
                    {
                        if (standardShaderPath.Contains("GPUInstancer/Resources/Shaders"))
                        {
                            EditorUtility.DisplayDialog(GPUInstancerEditorConstants.GPUI_VERSION + " Auto Update", GPUInstancerEditorConstants.HELPTEXT_Version100Update, "Proceed with the Update");

                            string gpuiPath = standardShaderPath.Substring(0, standardShaderPath.IndexOf("GPUInstancer")) + "GPUInstancer/";
                            string[] files = null;
                            if (Directory.Exists(gpuiPath + "Shaders/"))
                            {
                                Directory.Move(gpuiPath + "Shaders/", gpuiPath + "Shaders_moved/");
                                File.Delete(gpuiPath + "Shaders.meta");
                                files = Directory.GetFiles(gpuiPath + "Shaders_moved/");
                            }

                            // Update Shaders folder path
                            FileUtil.MoveFileOrDirectory(gpuiPath + "Resources/Shaders", gpuiPath + "Shaders");
                            FileUtil.MoveFileOrDirectory(gpuiPath + "Resources/Shaders.meta", gpuiPath + "Shaders.meta");
                            // delete ShaderVariants folder
                            FileUtil.DeleteFileOrDirectory(gpuiPath + "Resources/ShaderVariants");
                            FileUtil.DeleteFileOrDirectory(gpuiPath + "Resources/ShaderVariants.meta");

                            if (files != null)
                            {
                                foreach (string file in files)
                                {
                                    File.Move(file, file.Replace("Shaders_moved", "Shaders"));
                                }
                                Directory.Delete(gpuiPath + "Shaders_moved/");
                            }

                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

                            EditorApplication.update -= RegenerateShaders;
                            EditorApplication.update += RegenerateShaders;
                        }
                    }
                }
            }
        }

        private static void RegenerateShaders()
        {
            try
            {
                if (GPUInstancerConstants.gpuiSettings != null && GPUInstancerConstants.gpuiSettings.shaderBindings != null)
                {
                    if (GPUInstancerConstants.gpuiSettings.shaderBindings.shaderInstances != null && GPUInstancerConstants.gpuiSettings.shaderBindings.shaderInstances.Count > 0)
                    {
                        foreach (ShaderInstance si in GPUInstancerConstants.gpuiSettings.shaderBindings.shaderInstances)
                        {
                            si.Regenerate();
                        }
                        if (GPUInstancerConstants.gpuiSettings.shaderBindings != null)
                            EditorUtility.SetDirty(GPUInstancerConstants.gpuiSettings.shaderBindings);
                    }
                }
            }
            catch (Exception) { }
            EditorApplication.update -= RegenerateShaders;
        }

#if UNITY_2018_1_OR_NEWER
        public static void LoadPackageDefinitions(bool forceNew = false)
        {
            if (forceNew || !GPUInstancerConstants.gpuiSettings.packagesLoaded)
            {
                _packageListRequest = UnityEditor.PackageManager.Client.List(true);
                GPUInstancerConstants.gpuiSettings.isHDRP = false;
                GPUInstancerConstants.gpuiSettings.isLWRP = false;
                GPUInstancerConstants.gpuiSettings.isShaderGraphPresent = false;
                EditorApplication.update -= PackageListRequestHandler;
                EditorApplication.update += PackageListRequestHandler;
            }
        }

        private static void PackageListRequestHandler()
        {
            try
            {
                if (_packageListRequest != null)
                {
                    if (!_packageListRequest.IsCompleted)
                        return;
                    if (_packageListRequest.Result != null)
                    {
                        foreach (var item in _packageListRequest.Result)
                        {
                            if (item.name.Contains("com.unity.render-pipelines.high-definition"))
                            {
                                GPUInstancerConstants.gpuiSettings.isHDRP = true;
                                Debug.Log("GPUI detected HD Render Pipeline.");
                            }
                            else if (item.name.Contains("com.unity.render-pipelines.lightweight"))
                            {
                                GPUInstancerConstants.gpuiSettings.isLWRP = true;
                                Debug.Log("GPUI detected LW Render Pipeline.");
                            }
                            else if (item.name.Contains("com.unity.render-pipelines.universal"))
                            {
                                GPUInstancerConstants.gpuiSettings.isURP = true;
                                Debug.Log("GPUI detected Universal Render Pipeline.");
                            }
                            else if (item.name.Contains("com.unity.shadergraph"))
                            {
                                GPUInstancerConstants.gpuiSettings.isShaderGraphPresent = true;
                                Debug.Log("GPUI detected ShaderGraph package.");
                            }
                        }
                        if (GPUInstancerConstants.gpuiSettings.IsStandardRenderPipeline())
                            Debug.Log("GPUI detected Standard Render Pipeline.");
                        EditorUtility.SetDirty(GPUInstancerConstants.gpuiSettings);
                    }
                }
            }
            catch (Exception) { }
            _packageListRequest = null;
            GPUInstancerConstants.gpuiSettings.packagesLoaded = true;
            EditorApplication.update -= PackageListRequestHandler;
        }
#endif

        public static GPUInstancerShaderBindings GetGPUInstancerShaderBindings()
        {
            if (GPUInstancerConstants.gpuiSettings.shaderBindings == null)
                GPUInstancerConstants.gpuiSettings.shaderBindings = GPUInstancerSettings.GetDefaultGPUInstancerShaderBindings();
            return GPUInstancerConstants.gpuiSettings.shaderBindings;
        }

        public static ShaderVariantCollection GetShaderVariantCollection()
        {
            if (GPUInstancerConstants.gpuiSettings.shaderVariantCollection == null)
                GPUInstancerConstants.gpuiSettings.shaderVariantCollection = GPUInstancerSettings.GetDefaultShaderVariantCollection();
            return GPUInstancerConstants.gpuiSettings.shaderVariantCollection;
        }
    }
}
