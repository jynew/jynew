using System;
using System.Collections.Generic;
using UnityEngine;

namespace GPUInstancer
{
    public class GPUInstancerShaderBindings : ScriptableObject
    {
        public List<ShaderInstance> shaderInstances;

        private static readonly List<string> _standardUnityShaders = new List<string> {
            GPUInstancerConstants.SHADER_UNITY_STANDARD, GPUInstancerConstants.SHADER_UNITY_STANDARD_SPECULAR,
            GPUInstancerConstants.SHADER_UNITY_STANDARD_ROUGHNESS, GPUInstancerConstants.SHADER_UNITY_VERTEXLIT,
            GPUInstancerConstants.SHADER_UNITY_SPEED_TREE, GPUInstancerConstants.SHADER_UNITY_SPEED_TREE_8,
            GPUInstancerConstants.SHADER_UNITY_TREE_CREATOR_BARK, GPUInstancerConstants.SHADER_UNITY_TREE_CREATOR_BARK_OPTIMIZED,
            GPUInstancerConstants.SHADER_UNITY_TREE_CREATOR_LEAVES, GPUInstancerConstants.SHADER_UNITY_TREE_CREATOR_LEAVES_OPTIMIZED,
            GPUInstancerConstants.SHADER_UNITY_TREE_CREATOR_LEAVES_FAST, GPUInstancerConstants.SHADER_UNITY_TREE_CREATOR_LEAVES_FAST_OPTIMIZED,
            GPUInstancerConstants.SHADER_UNITY_TREE_SOFT_OCCLUSION_BARK, GPUInstancerConstants.SHADER_UNITY_TREE_SOFT_OCCLUSION_LEAVES
        };
        private static readonly List<string> _standardUnityShadersGPUI = new List<string> {
            GPUInstancerConstants.SHADER_GPUI_STANDARD, GPUInstancerConstants.SHADER_GPUI_STANDARD_SPECULAR,
            GPUInstancerConstants.SHADER_GPUI_STANDARD_ROUGHNESS, GPUInstancerConstants.SHADER_GPUI_VERTEXLIT,
            GPUInstancerConstants.SHADER_GPUI_SPEED_TREE, GPUInstancerConstants.SHADER_GPUI_SPEED_TREE_8,
            GPUInstancerConstants.SHADER_GPUI_TREE_CREATOR_BARK, GPUInstancerConstants.SHADER_GPUI_TREE_CREATOR_BARK_OPTIMIZED,
            GPUInstancerConstants.SHADER_GPUI_TREE_CREATOR_LEAVES, GPUInstancerConstants.SHADER_GPUI_TREE_CREATOR_LEAVES_OPTIMIZED,
            GPUInstancerConstants.SHADER_GPUI_TREE_CREATOR_LEAVES_FAST, GPUInstancerConstants.SHADER_GPUI_TREE_CREATOR_LEAVES_FAST_OPTIMIZED,
            GPUInstancerConstants.SHADER_GPUI_TREE_SOFT_OCCLUSION_BARK, GPUInstancerConstants.SHADER_GPUI_TREE_SOFT_OCCLUSION_LEAVES
        };
        private static readonly List<string> _extraGPUIShaders = new List<string> {
            GPUInstancerConstants.SHADER_GPUI_FOLIAGE,
            GPUInstancerConstants.SHADER_GPUI_FOLIAGE_LWRP,
            GPUInstancerConstants.SHADER_GPUI_FOLIAGE_URP,
            GPUInstancerConstants.SHADER_GPUI_FOLIAGE_HDRP,
            GPUInstancerConstants.SHADER_GPUI_SHADOWS_ONLY,
            GPUInstancerConstants.SHADER_GPUI_BILLBOARD_2D_RENDERER_TREE,
            GPUInstancerConstants.SHADER_GPUI_BILLBOARD_2D_RENDERER_TREECREATOR,
            GPUInstancerConstants.SHADER_GPUI_BILLBOARD_2D_RENDERER_SOFTOCCLUSION,
            GPUInstancerConstants.SHADER_GPUI_BILLBOARD_2D_RENDERER_STANDARD,
            GPUInstancerConstants.SHADER_GPUI_HIZ_OCCLUSION_GENERATOR,
            GPUInstancerConstants.SHADER_GPUI_TREE_PROXY
        };

        #region Extensions
        public List<GPUInstancerShaderBindingsExtension> shaderBindingsExtensions;

        public virtual bool HasExtension(string extensionCode)
        {
            if (shaderBindingsExtensions == null)
                return false;
            return shaderBindingsExtensions.Exists(ex => ex.GetExtensionCode().Equals(extensionCode));
        }

        public virtual void AddExtension(GPUInstancerShaderBindingsExtension extension)
        {
            if (shaderBindingsExtensions == null)
                shaderBindingsExtensions = new List<GPUInstancerShaderBindingsExtension>();
            if (!shaderBindingsExtensions.Exists(ex => ex.GetExtensionCode().Equals(extension.GetExtensionCode())))
                shaderBindingsExtensions.Add(extension);
        }

        public virtual GPUInstancerShaderBindingsExtension GetExtension(string extensionCode)
        {
            if (shaderBindingsExtensions != null && shaderBindingsExtensions.Count > 0)
            {
                foreach (GPUInstancerShaderBindingsExtension extension in shaderBindingsExtensions)
                {
                    if (extension.GetExtensionCode().Equals(extensionCode))
                    {
                        return extension;
                    }
                }
            }
            Debug.LogError("GPU Instancer Shader Bindings Extension can not be found. ExtensionCode: " + extensionCode);
            return null;
        }
        #endregion Extensions

        public virtual Shader GetInstancedShader(string shaderName, string extensionCode = null)
        {
            if (!string.IsNullOrEmpty(extensionCode))
            {
                GPUInstancerShaderBindingsExtension extension = GetExtension(extensionCode);
                if (extension != null)
                    return extension.GetInstancedShader(shaderInstances, shaderName);
                return null;
            }

            if (string.IsNullOrEmpty(shaderName))
                return null;

            if (shaderInstances == null)
                shaderInstances = new List<ShaderInstance>();

            foreach (ShaderInstance si in shaderInstances)
            {
                if (si.name.Equals(shaderName) && string.IsNullOrEmpty(si.extensionCode))
                    return si.instancedShader;
            }

            if (_standardUnityShaders.Contains(shaderName))
                return Shader.Find(_standardUnityShadersGPUI[_standardUnityShaders.IndexOf(shaderName)]);

            if (_standardUnityShadersGPUI.Contains(shaderName))
                return Shader.Find(shaderName);

            if (_extraGPUIShaders.Contains(shaderName))
                return Shader.Find(shaderName);

            if (!shaderName.Equals(GPUInstancerConstants.SHADER_UNITY_STANDARD))
            {
                if (Application.isPlaying)
                    Debug.LogWarning("Can not find instanced shader for : " + shaderName + ". Using Standard shader instead.", Shader.Find(shaderName));
                return GetInstancedShader(GPUInstancerConstants.SHADER_UNITY_STANDARD);
            }
            Debug.LogWarning("Can not find instanced shader for : " + shaderName);
            return null;
        }

        public virtual Material GetInstancedMaterial(Material originalMaterial, string extensionCode = null)
        {
            if (!string.IsNullOrEmpty(extensionCode))
            {
                GPUInstancerShaderBindingsExtension extension = GetExtension(extensionCode);
                if (extension != null)
                    return extension.GetInstancedMaterial(shaderInstances, originalMaterial);
                return null;
            }
            if (originalMaterial == null || originalMaterial.shader == null)
            {
                Debug.LogWarning("One of the GPU Instancer prototypes is missing material reference! Check the Material references in MeshRenderer.");
                return new Material(GetInstancedShader(GPUInstancerConstants.SHADER_UNITY_STANDARD));
            }
            Material instancedMaterial = new Material(originalMaterial);
            instancedMaterial.shader = GetInstancedShader(originalMaterial.shader.name);
            instancedMaterial.name = originalMaterial.name + "_GPUI";

            return instancedMaterial;
        }

        public virtual void ResetShaderInstances()
        {
            if (shaderInstances == null)
                shaderInstances = new List<ShaderInstance>();
            else
                shaderInstances.Clear();

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        public virtual void ClearEmptyShaderInstances()
        {
            if (shaderInstances != null)
            {
#if UNITY_EDITOR
                bool modified = false;
                if (shaderBindingsExtensions != null && shaderBindingsExtensions.Count > 0)
                {
                    foreach (GPUInstancerShaderBindingsExtension extension in shaderBindingsExtensions)
                    {
                        modified |= extension.ClearEmptyShaderInstances(shaderInstances);
                    }
                }

                modified |= shaderInstances.RemoveAll(si => si == null || si.instancedShader == null || string.IsNullOrEmpty(si.name)) > 0;

                if (GPUInstancerConstants.gpuiSettings != null && !GPUInstancerConstants.gpuiSettings.disableAutoShaderConversion)
                {
                    for (int i = 0; i < shaderInstances.Count; i++)
                    {
                        if (shaderInstances[i].isOriginalInstanced || !string.IsNullOrEmpty(shaderInstances[i].extensionCode))
                            continue;

                        Shader originalShader = Shader.Find(shaderInstances[i].name);
                        string originalAssetPath = UnityEditor.AssetDatabase.GetAssetPath(originalShader);
                        DateTime lastWriteTime = System.IO.File.GetLastWriteTime(originalAssetPath);
                        if (lastWriteTime >= DateTime.Now)
                            continue;

                        DateTime instancedTime = DateTime.MinValue;
                        bool isValidDate = false;
                        if (!string.IsNullOrEmpty(shaderInstances[i].modifiedDate))
                            isValidDate = DateTime.TryParseExact(shaderInstances[i].modifiedDate, "MM/dd/yyyy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture,
                                System.Globalization.DateTimeStyles.None, out instancedTime);
                        if (!isValidDate || lastWriteTime > Convert.ToDateTime(shaderInstances[i].modifiedDate, System.Globalization.CultureInfo.InvariantCulture))
                        {
                            modified = true;
                            if (!GPUInstancerUtility.IsShaderInstanced(originalShader))
                            {
                                shaderInstances[i].instancedShader = GPUInstancerUtility.CreateInstancedShader(originalShader);
                                shaderInstances[i].modifiedDate = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff",
                                    System.Globalization.CultureInfo.InvariantCulture);
                            }
                            else
                                shaderInstances[i].isOriginalInstanced = true;
                        }
                    }
                }

                // remove non unique instances
                List<string> shaderNames = new List<string>();
                foreach (ShaderInstance si in shaderInstances.ToArray())
                {
                    if (shaderNames.Contains(si.name + si.extensionCode))
                    {
                        shaderInstances.Remove(si);
                        modified = true;
                    }
                    else
                        shaderNames.Add(si.name + si.extensionCode);
                }

                if (modified)
                    UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
        }

        public virtual void AddShaderInstance(string name, Shader instancedShader, bool isOriginalInstanced = false, string extensionCode = null)
        {
            shaderInstances.Add(new ShaderInstance(name, instancedShader, isOriginalInstanced, extensionCode));
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        public virtual bool IsShadersInstancedVersionExists(string shaderName, string extensionCode = null)
        {
            if (!string.IsNullOrEmpty(extensionCode))
            {
                GPUInstancerShaderBindingsExtension extension = GetExtension(extensionCode);
                if (extension != null)
                    return extension.IsShadersInstancedVersionExists(shaderInstances, shaderName);
                return false;
            }
            if (_standardUnityShaders.Contains(shaderName) || _standardUnityShadersGPUI.Contains(shaderName) || _extraGPUIShaders.Contains(shaderName))
                return true;

            foreach (ShaderInstance si in shaderInstances)
            {
                if (si.name.Equals(shaderName) && string.IsNullOrEmpty(si.extensionCode))
                    return true;
            }
            return false;
        }
    }

    [Serializable]
    public class ShaderInstance
    {
        public string name;
        public Shader instancedShader;
        public string modifiedDate;
        public bool isOriginalInstanced;
        public string extensionCode;

        public ShaderInstance(string name, Shader instancedShader, bool isOriginalInstanced, string extensionCode = null)
        {
            this.name = name;
            this.instancedShader = instancedShader;
            this.modifiedDate = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff",
                                System.Globalization.CultureInfo.InvariantCulture);
            this.isOriginalInstanced = isOriginalInstanced;
            this.extensionCode = extensionCode;
        }

        public virtual void Regenerate()
        {
            if (isOriginalInstanced)
            {
                instancedShader = GPUInstancerUtility.CreateInstancedShader(instancedShader, true);
                return;
            }

            Shader originalShader = Shader.Find(name);
            if (originalShader != null)
            {
                instancedShader = GPUInstancerUtility.CreateInstancedShader(originalShader);
                modifiedDate = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff",
                    System.Globalization.CultureInfo.InvariantCulture);
            }
        }
    }

}