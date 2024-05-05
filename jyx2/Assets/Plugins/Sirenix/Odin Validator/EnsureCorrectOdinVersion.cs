//-----------------------------------------------------------------------
// <copyright file="EnsureCorrectOdinVersion.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR

namespace Sirenix.OdinValidator.Editor
{
    using System;
    using System.IO;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    internal static class EnsureCorrectOdinVersion
    {
        private const string validatorVersion = "3.3.0.4";

        private static bool IsHeadlessOrBatchMode { get { return SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null || UnityEditorInternal.InternalEditorUtility.inBatchMode; } }

        [InitializeOnLoadMethod]
        private static void Init()
        {
#if SIRENIX_INTERNAL
            return;
#endif

            if (EditorPrefs.HasKey("PREVENT_SIRENIX_FILE_GENERATION"))
                return;

            if (IsHeadlessOrBatchMode)
                return;

            if (TryGetOdinInspectorVersion(out var inspectorVersion))
            {
                var iVer = Version.Parse(inspectorVersion);
                var vVer = Version.Parse(validatorVersion);

                if (iVer.Major == vVer.Major && iVer.Minor == vVer.Minor && iVer.Build == vVer.Build) // Ignore Revision
                {
                    TryInstall();
                }
                else
                {
                    var latestVersion = iVer > vVer ? inspectorVersion : validatorVersion;
                    var oldestProduct = iVer > vVer ? "Odin Validator" : "Odin Inspector";
                    var misMatchText = inspectorVersion + " : " + validatorVersion;

                    if (TryGetOdinInstallPath(out var path))
                    {
                        var versionMismatchFile = path + "Odin Validator/ignoreVersionMismatch.txt";

                        if (File.Exists(versionMismatchFile))
                        {
                            var misMatch = File.ReadAllText(versionMismatchFile).Trim();
                            if (misMatch == misMatchText)
                                return;
                        }
                    }

                    if (!EditorUtility.DisplayDialog(
                        "Odin Version Mismatch",

                        "Odin Inspector and Odin Validator need to be on the same version to function correctly.\n" +
                        $"\n" +
                        $"Current Odin Inspector: {inspectorVersion}\n" +
                        $"Current Odin Validator: {validatorVersion}\n" +
                        $"\n" +
                        $"Please install {oldestProduct} {latestVersion}",

                        "OK", "Ignore until next version mismatch"))
                    {
                        var versionMismatchFile = path + "Odin Validator/ignoreVersionMismatch.txt";
                        File.WriteAllText(versionMismatchFile, misMatchText);
                    };
                }
            }
            else
            {
                EditorUtility.DisplayDialog(
                      "Odin Validator requires Odin Inspector",
                     $"Please install Odin Inspector {validatorVersion}",

                      "OK");
            }
        }

        private static void TryInstall()
        {
            if (TryGetOdinInstallPath(out var path))
            {
                var tmp_extension = "_tmp";
                var assemblyFiles = new string[]
                {
                    path + "Assemblies/Sirenix.OdinValidator.Editor.dll",
                    path + "Assemblies/Sirenix.OdinValidator.Editor.dll.meta",
                    path + "Assemblies/Sirenix.OdinValidator.Editor.xml",
                    path + "Assemblies/Sirenix.OdinValidator.Editor.xml.meta",
                    path + "Assemblies/Sirenix.OdinValidator.Editor.pdb",
                    path + "Assemblies/Sirenix.OdinValidator.Editor.pdb.meta",
                };

                var requireUpdate = File.Exists(assemblyFiles[0] + tmp_extension);

                if (requireUpdate)
                {
                    // Install / update Odin Validator.
                    AssetDatabase.StartAssetEditing();

                    foreach (var item in assemblyFiles)
                    {
                        var oldFile = item;
                        var newFile = item + tmp_extension;

                        if (File.Exists(newFile))
                        {
                            if (File.Exists(oldFile))
                                File.Delete(oldFile);

                            File.Move(newFile, oldFile);
                        }
                    }

                    AssetDatabase.StopAssetEditing();
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                    EditorPrefs.SetBool("ODIN_VALIDATOR_SHOW_GETTING_STARTED", true);
                }
#if ODIN_INSPECTOR
                else if (EditorPrefs.GetBool("ODIN_VALIDATOR_SHOW_GETTING_STARTED", false))
                {
                    EditorPrefs.SetBool("ODIN_VALIDATOR_SHOW_GETTING_STARTED", false);
                    EditorApplication.delayCall += () =>
                    {
                        var t = Sirenix.Serialization.TwoWaySerializationBinder.Default.BindToType("Sirenix.OdinInspector.Editor.GettingStarted.GettingStartedWindow");
                        if (t != null)
                        {
                            var action = Utilities.Editor.Expressions.ExpressionUtility.ParseAction<bool, bool>("ShowWindow(false, true)", true, t, out var _);
                            action.Invoke(false, true);
                        }
                    };
                }
#endif
            }
            else
            {
                Debug.LogError("Odin Validator was unable to find Sirenix.Utilities.SirenixAssetPaths.SirenixPluginPath");
            }
        }

        private static bool TryGetOdinInstallPath(out string path)
        {
            var t = Type.GetType("Sirenix.Utilities.SirenixAssetPaths, Sirenix.Utilities");

            if (t == null)
            {
                path = null;
                return false;
            }

            var v = t.GetField("SirenixPluginPath", BindingFlags.Public | BindingFlags.Static);
            if (v == null)
            {
                path = null;
                return false;
            }

            path = v.GetValue(null) as string;
            return true;
        }

        private static bool TryGetOdinInspectorVersion(out string version)
        {
            var t = Type.GetType("Sirenix.OdinInspector.Editor.OdinInspectorVersion, Sirenix.OdinInspector.Editor");

            if (t == null)
            {
                version = null;
                return false;
            }

            var v = t.GetProperty("Version", BindingFlags.Public | BindingFlags.Static);
            if (v == null)
            {
                version = null;
                return false;
            }

            version = v.GetValue(null) as string;
            return true;
        }
    }
}

#endif
