using UnityEditor;
using UnityEngine;

namespace MTE
{
    internal class Preview
    {
        public bool IsReady = false;
        private bool IsArray = false;
        private Shader shader;
        private RenderPipeline renderPipeline = RenderPipeline.NotDetermined;
        private GameObject previewObj;
        private Texture2D UpDirectionNormalTexture;

        public Preview(bool isArray)
        {
            this.IsArray = isArray;
        }

        /// <summary>
        /// Load preview from target GameObjects
        /// </summary>
        public void LoadPreview(Texture texture, float brushSizeInU3D, int brushIndex)
        {
            LoadShader();

            if (IsArray)
            {
                Material material = null;
                foreach (var target in MTEContext.Targets)
                {
                    material = FindMaterialInObject(target, texture, out _);
                    if(material)
                    {
                        break;
                    }
                }
                if (material == null)
                {
                    throw new MTEEditException(
                        "Failed to load texture in to preview: " +
                        "selected texture isn't a source texture slice of any texture array used in any targets' material. " +
                        "Please refresh the filter to reload the texture list.\n\n" +
                        $"Note: MTE finds source texture slice via the {nameof(TextureArraySettings)} asset next to the texture array being used.");
                }

                UnLoadPreview();

                CreatePreviewObject();

                previewObj.hideFlags = HideFlags.HideAndDontSave;

                var textureScale = GetPreviewSplatTextureScale(material, brushIndex);
                //TODO consider normal roughness and AO
                SetPreviewTexture(textureScale, texture, null);
                SetPreviewSize(brushSizeInU3D / 2);
                SetPreviewMaskTexture(brushIndex);
            }
            else
            {
                int splatIndex = -1;
                Material material = null;
                foreach (var target in MTEContext.Targets)
                {
                    material = FindMaterialInObject(target, texture, out _);
                    if (material)
                    {
                        break;
                    }
                }
                if (material == null)
                {
                    throw new MTEEditException(
                        "Failed to load texture in to preview: " +
                        "selected texture isn't used in any GameObject's material. " +
                        "Please refresh the filter to reload the texture list.");
                }

                UnLoadPreview();

                CreatePreviewObject();

                previewObj.hideFlags = HideFlags.HideAndDontSave;

                var textureScale = GetPreviewSplatTextureScale(material, splatIndex);
                Texture normalTexture = null;
                if (material.HasProperty("_Normal" + splatIndex))
                {
                    normalTexture = material.GetTexture("_Normal" + splatIndex);
                }

                SetPreviewTexture(textureScale, texture, normalTexture);
                SetPreviewSize(brushSizeInU3D / 2);
                SetPreviewMaskTexture(brushIndex);
            }

            IsReady = true;
        }

        /// <summary>
        /// Load preview from a GameObject
        /// </summary>
        public void LoadPreviewFromObject(Texture texture, float brushSizeInU3D,
            int brushIndex, GameObject obj)
        {
            if (obj == null)
            {
                MTEDebug.LogError("Cannot load preview from an invalid GameObject.");
                return;
            }

            LoadShader();
            
            if (IsArray)
            {
                Material material = null;
                var target = obj;
                material = FindMaterialInObject(target, texture, out _);
                if (material == null)
                {
                    throw new MTEEditException(
                        "Failed to load texture in to preview: " +
                        "selected texture isn't a source texture slice of any texture array used in any targets' material. " +
                        "Please refresh the filter to reload the texture list.\n\n" +
                        $"Note: MTE finds source texture slice via the {nameof(TextureArraySettings)} asset next to the texture array being used.");
                }

                UnLoadPreview();

                CreatePreviewObject();

                previewObj.hideFlags = HideFlags.HideAndDontSave;

                var textureScale = GetPreviewSplatTextureScale(material, brushIndex);
                //TODO consider normal roughness and AO
                SetPreviewTexture(textureScale, texture, null);
                SetPreviewSize(brushSizeInU3D / 2);
                SetPreviewMaskTexture(brushIndex);
            }
            else
            {
                int splatIndex = -1;
                Material material = null;
                var target = obj;
                material = FindMaterialInObject(target, texture, out splatIndex);
                if (material == null)
                {
                    throw new MTEEditException(
                        "Failed to load texture in to preview: " +
                        "selected texture isn't used in any GameObject's material. " +
                        "Please refresh the filter to reload the texture list.");
                }

                UnLoadPreview();

                CreatePreviewObject();

                previewObj.hideFlags = HideFlags.HideAndDontSave;

                var textureScale = GetPreviewSplatTextureScale(material, splatIndex);
                Texture normalTexture = null;
                if (material.HasProperty("_Normal" + splatIndex))
                {
                    normalTexture = material.GetTexture("_Normal" + splatIndex);
                }

                SetPreviewTexture(textureScale, texture, normalTexture);
                SetPreviewSize(brushSizeInU3D / 2);
                SetPreviewMaskTexture(brushIndex);
            }
            IsReady = true;
        }

        /// <summary>
        /// Destory the preview.
        /// </summary>
        public void UnLoadPreview()
        {
            if (previewObj != null)
            {
                UnityEngine.Object.DestroyImmediate(previewObj);
                previewObj = null;
            }
            IsReady = false;
        }

        public void SetPreviewTexture(Vector2 textureScale,
            Texture texture, Texture normalTexture)
        {
            if(!previewObj)
            {
                return; 
            }

            if(renderPipeline != RenderPipeline.Builtin)
            {
                var renderer = previewObj.GetComponent<MeshRenderer>();
                renderer.sharedMaterial.SetTexture("_MainTex", texture);
                renderer.sharedMaterial.SetTextureScale("_MainTex", textureScale);

                if (!normalTexture)
                {//Default "bump" direction is (0, 0, 1), encoded as(0.5, 0.5, 1.0);
                 //  not expected up-direction (0, 1, 0), encoded as(0.5, 1.0, 0.5).
                 //So a default up-direction normal texture is created here lazily to replaced the "bump".
                    if (!UpDirectionNormalTexture)
                    {
                        UpDirectionNormalTexture = new Texture2D(1, 1, TextureFormat.RGB24, false);
                        UpDirectionNormalTexture.SetPixel(0, 0, new Color(0.5f, 1.0f, 0.5f));
                        UpDirectionNormalTexture.Apply();
                    }
                    normalTexture = UpDirectionNormalTexture;
                }
                renderer.sharedMaterial.SetTexture("_NormalTex", normalTexture);
            }
            else
            {
                var projector = previewObj.GetComponent<Projector>();
                projector.material.SetTexture("_MainTex", texture);
                projector.material.SetTextureScale("_MainTex", textureScale);
                projector.material.SetTexture("_NormalTex", normalTexture);
            }

            SceneView.RepaintAll();
        }

        public void SetPreviewMaskTexture(int maskIndex)
        {
            if(!previewObj)
            {
                return; 
            }

            if (renderPipeline == RenderPipeline.Builtin)
            {
                var projector = previewObj.GetComponent<Projector>();
                projector.material.SetTexture("_MaskTex", MTEStyles.brushTextures[maskIndex]);
                projector.material.SetTextureScale("_MaskTex", Vector2.one);
            }
            else
            {
                var renderer = previewObj.GetComponent<MeshRenderer>();
                renderer.sharedMaterial.SetTexture("_MaskTex", MTEStyles.brushTextures[maskIndex]);
                renderer.sharedMaterial.SetTextureScale("_MaskTex", Vector2.one);
            }
            SceneView.RepaintAll();
        }

        public void SetPreviewSize(float value)
        {
            if(!previewObj)
            {
                return; 
            }

            if (renderPipeline == RenderPipeline.Builtin)
            {
                var projector = previewObj.GetComponent<Projector>();
                projector.orthographicSize = value;
            }
            else
            {
                var halfBrushSizeInUnityUnit = value;
                previewObj.transform.localScale = new Vector3(
                    halfBrushSizeInUnityUnit*2,
                    halfBrushSizeInUnityUnit*2, 10000);
            }
            SceneView.RepaintAll();
        }

        public void MoveTo(Vector3 worldPosition)
        {
            if(!previewObj)
            {
                return; 
            }
            previewObj.transform.position = worldPosition;
        }

        public void SetNormalizedBrushCenter(Vector2 normalizedBrushCenter)
        {
            if (renderPipeline != RenderPipeline.Builtin)
            {
                var renderer = previewObj.GetComponent<MeshRenderer>();
                renderer.sharedMaterial.SetVector("_BrushCenter", normalizedBrushCenter);
            }
            else
            {
                //nothing
            }
        }

        public void SetNormalizedBrushSize(float normalizeBrushSize)
        {
            if (renderPipeline != RenderPipeline.Builtin)
            {
                var renderer = previewObj.GetComponent<MeshRenderer>();
                renderer.sharedMaterial.SetFloat("_NormalizedBrushSize", normalizeBrushSize);
            }
            else
            {
                //nothing
            }
        }

        private void LoadShader()
        {
            if (shader != null && RenderPipelineUtil.Current == renderPipeline)
            {
                return;
            }

            renderPipeline = RenderPipelineUtil.Current;

            var urpShaderRelativePath = Utility.GetUnityPath(Res.ShaderDir + "PaintTexturePreview_URP.shader");

            switch (RenderPipelineUtil.Current)
            {
                case RenderPipeline.Builtin:
                    shader = Shader.Find("Hidden/MTE/PaintTexturePreview");
                    break;
                case RenderPipeline.URP:
                    this.shader = AssetDatabase.LoadAssetAtPath<Shader>(urpShaderRelativePath);
                    if (shader == null)
                    {
                        MTEDebug.LogError("MTE Preview shader for URP is not found.");
                    }
                    else
                    {
                        MTEDebug.Log("Loaded Preview shader for URP.");
                    }
                    break;
                //fallback to URP
                case RenderPipeline.HDRP://HDRP is not supported yet.
                default:
                    this.shader = AssetDatabase.LoadAssetAtPath<Shader>(urpShaderRelativePath);
                    if (shader == null)
                    {
                        MTEDebug.LogError("MTE Preview shader for URP (fallback) is not found.");
                    }
                    else
                    {
                        MTEDebug.Log("Loaded Preview shader for URP (fallback).");
                    }
                    break;
            }
        }

        private void CreatePreviewObject()
        {
            if (renderPipeline != RenderPipeline.Builtin)
            {
                previewObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                var boxCollider = previewObj.GetComponent<BoxCollider>();
                Object.DestroyImmediate(boxCollider);
                previewObj.name = "MTEPreview";

                var meshRenderer = previewObj.GetComponent<MeshRenderer>();
                var material = new Material(shader);
                meshRenderer.sharedMaterial = material;

                previewObj.transform.eulerAngles = new Vector3(90, 0, 0);
            }
            else
            {
                previewObj = new GameObject("MTEPreview");
                var projector = previewObj.AddComponent<Projector>();
                projector.material = new Material(shader);
                projector.orthographic = true;
                projector.nearClipPlane = -1000;
                projector.farClipPlane = 1000;
                projector.transform.Rotate(90, 0, 0);
            }
        }

        private Material FindMaterialInObject(GameObject obj, Texture texture, out int layerIndexBuiltinOnly)
        {
            layerIndexBuiltinOnly = -1;
            if (IsArray)
            {
                var meshRenderer = obj.GetComponent<MeshRenderer>();
                if (meshRenderer == null)
                {
                    return null;
                }
                var m = meshRenderer.sharedMaterial;
                if (m == null)
                {
                    return null;
                }
                var texturePropertyValue = m.GetTexture(
                    TextureArrayShaderPropertyNames.AlbedoArrayPropertyName);
                if (!texturePropertyValue)
                {
                    return null;
                }

                var textureArray = texturePropertyValue as Texture2DArray;
                if (textureArray == null)
                {
                    return null;
                }

                if (!TextureArrayManager.Instance.IsCached(textureArray))
                {
                    return null;
                }

                if (TextureArrayManager.Instance.GetTextureSliceIndex(textureArray, texture) < 0)
                {
                    return null;
                }

                return m;
            }
            else
            {
                var meshRenderer = obj.GetComponent<MeshRenderer>();
                if (meshRenderer == null)
                {
                    return null;
                }
                var m = meshRenderer.sharedMaterial;
                if (m == null)
                {
                    return null;
                }

                layerIndexBuiltinOnly = m.FindSplatTexture(texture);
                if (layerIndexBuiltinOnly < 0)
                {
                    return null;
                }

                return m;
            }
        }

        private Vector2 GetPreviewSplatTextureScale(Material material, int splatIndex)
        {
            if(IsArray)
            {
                //We use unique uv scale offset for all layers in array shaders.
                //So splatIndex is only valid for non-array shaders.
                var UVScaleOffset = TextureArrayShaderPropertyNames.UVScaleOffsetPropertyName;
                if (material.HasProperty(UVScaleOffset))
                {
                    var scaleOffset = material.GetVector(UVScaleOffset);
                    return new Vector2(scaleOffset.x, scaleOffset.y);
                }

                MTEDebug.LogWarning($"No {UVScaleOffset} property found in texture array shader " +
                    material.shader.name);
                return new Vector2(15, 15);
            }
            else
            {
                var splatXName = "_Splat" + splatIndex;
                if (material.HasProperty(splatXName))
                {
                    return material.GetTextureScale(splatXName);
                }

                if (0 <= splatIndex && splatIndex <= 3)
                {
                    var packedSplatName = "_PackedSplat0";
                    if (material.HasProperty(packedSplatName))
                    {
                        return material.GetTextureScale(packedSplatName);
                    }
                }
                else if (4 <= splatIndex && splatIndex <= 7)
                {
                    var packedSplatName = "_PackedSplat3";
                    if (material.HasProperty(packedSplatName))
                    {
                        return material.GetTextureScale(packedSplatName);
                    }
                }

                return new Vector2(10, 10);
            }
        }
    }
}
