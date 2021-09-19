using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace GPUInstancer
{
    public class GPUInstancerPreviewCache
    {
        private readonly Dictionary<GPUInstancerPrototype, Texture2D> _previewCache = new Dictionary<GPUInstancerPrototype, Texture2D>();

        public void AddPreview(GPUInstancerPrototype prototype, Texture2D preview)
        {
            if (_previewCache.ContainsKey(prototype))
                _previewCache[prototype] = preview;
            else
                _previewCache.Add(prototype, preview);
        }

        public void RemovePreview(GPUInstancerPrototype prototype)
        {
            if (_previewCache.ContainsKey(prototype))
                _previewCache.Remove(prototype);
        }

        public Texture2D GetPreview(GPUInstancerPrototype prototype)
        {
            if (_previewCache.ContainsKey(prototype))
                return _previewCache[prototype];
            return null;
        }

        public void ClearEmptyPreviews()
        {
            GPUInstancerPrototype[] prototypes = new GPUInstancerPrototype[_previewCache.Count];
            _previewCache.Keys.CopyTo(prototypes, 0);
            foreach (GPUInstancerPrototype prototype in prototypes)
            {
                if (!_previewCache[prototype])
                    _previewCache.Remove(prototype);
            }
        }

        public void ClearPreviews()
        {
            foreach (GPUInstancerPrototype prototype in _previewCache.Keys)
            {
                if (_previewCache[prototype])
                    Object.DestroyImmediate(_previewCache[prototype]);
            }
            _previewCache.Clear();
        }
    }

    public class GPUInstancerPreviewDrawer
    {
        private Texture2D _defaultBackgroundTexture;
        private Texture2D _additionalTexture;
#if UNITY_PRO_LICENSE
        private Color _defaultColor = new Color(0.45f, 0.45f, 0.45f, 1.0f);
#else
        private Color _defaultColor = new Color(0.7f, 0.7f, 0.7f, 1.0f);
#endif
        private readonly Color _colorMultiplier = new Color(0.5f, 0.5f, 0.5f, 1.0f);

        private Camera _camera;
        private readonly List<GameObject> _gameObjects = new List<GameObject>();
        private Light[] lights = new Light[2];
        private readonly int _sampleLayer = 31;
        public bool isVertexBased = true;

        public GPUInstancerPreviewDrawer(Texture2D defaultBackgroundTexture)
        {
            _defaultBackgroundTexture = defaultBackgroundTexture;
            Initialize();
        }

        public void Initialize()
        {
            var camGO = EditorUtility.CreateGameObjectWithHideFlags("Preview Scene Camera", HideFlags.HideAndDontSave, typeof(Camera));
            AddGameObject(camGO);
            camGO.transform.rotation = Quaternion.Euler(30, -135, 0);
            _camera = camGO.GetComponent<Camera>();
            _camera.cameraType = CameraType.Preview;
            _camera.enabled = false;
            _camera.clearFlags = CameraClearFlags.Depth;
            _camera.fieldOfView = 15;
            _camera.farClipPlane = 100;
            _camera.nearClipPlane = -100;
            _camera.renderingPath = RenderingPath.Forward;
            _camera.useOcclusionCulling = false;
            _camera.orthographic = true;
            _camera.backgroundColor = Color.clear;
            _camera.cullingMask = 1 << _sampleLayer;
            _camera.allowHDR = false;

            var l0 = CreateLight();
            AddGameObject(l0);
            lights[0] = l0.GetComponent<Light>();

            var l1 = CreateLight();
            AddGameObject(l1);
            lights[1] = l1.GetComponent<Light>();

            foreach (Light l in lights)
            {
                l.cullingMask = 1 << _sampleLayer;
            }

            lights[0].color = new Color(1f, 0.9568627f, 0.8392157f, 0f);
            lights[0].transform.rotation = Quaternion.Euler(30, -135, 0);
            lights[0].intensity = 1f;
            lights[1].color = new Color(.4f, .4f, .45f, 0f) * .7f;
            lights[1].transform.rotation = Quaternion.identity;
            lights[1].intensity = 0.5f;

            if (QualitySettings.activeColorSpace == ColorSpace.Linear)
            {
                _defaultColor = _defaultColor.linear;
                lights[0].color = lights[0].color.linear;
                lights[1].color = lights[1].color.linear;
            }
        }

        public void AddGameObject(GameObject go)
        {
            go.layer = _sampleLayer;
            if (_gameObjects.Contains(go))
                return;

            _gameObjects.Add(go);
        }

        public void SetAdditionalTexture(Texture2D additionalTexture)
        {
            _additionalTexture = additionalTexture;
        }

        public void Cleanup()
        {
            foreach (var go in _gameObjects)
                Object.DestroyImmediate(go);

            _gameObjects.Clear();
            lights = null;
        }

        public Texture2D GetPreviewForGameObject(GameObject gameObject, Rect prevRect, Color backgroundColor)
        {
            if (!_camera || lights == null)
            {
                Cleanup();
                Initialize();
            }

            #region Initialize
            float scaleFac = GetScaleFactor(prevRect.width, prevRect.height);

            int rtWidth = (int)(prevRect.width * scaleFac);
            int rtHeight = (int)(prevRect.height * scaleFac);

            RenderTexture _renderTexture = RenderTexture.GetTemporary(rtWidth, rtHeight, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
            _renderTexture.Create();
            _camera.targetTexture = _renderTexture;

            RenderTexture previousRenderTexture = RenderTexture.active;
            RenderTexture.active = _renderTexture;
            GL.Clear(true, true, Color.clear);

            foreach (var light in lights)
                light.enabled = true;


            if (QualitySettings.activeColorSpace == ColorSpace.Linear)
            {
                if (backgroundColor != Color.clear)
                    backgroundColor = backgroundColor.linear;
            }

            if (GPUInstancerConstants.gpuiSettings.isHDRP && gameObject != null)
            {
                _camera.backgroundColor = backgroundColor == Color.clear ? _defaultColor.gamma : backgroundColor.gamma;
            }
            else if (_defaultBackgroundTexture != null)
            {
                GL.PushMatrix();
                GL.LoadOrtho();
                GL.LoadPixelMatrix(0, _renderTexture.width, _renderTexture.height, 0);
                Graphics.DrawTexture(
                    new Rect(0, 0, _renderTexture.width, _renderTexture.height),
                    _defaultBackgroundTexture,
                    new Rect(0, 0, 1, 1),
                    1, 1, 1, 1, backgroundColor == Color.clear ? _defaultColor * _colorMultiplier : backgroundColor * _colorMultiplier);
                GL.PopMatrix();
            }

            if (_additionalTexture != null)
            {
                GL.PushMatrix();
                GL.LoadOrtho();
                GL.LoadPixelMatrix(0, _renderTexture.width - 2, _renderTexture.height - 2, 0);
                Graphics.DrawTexture(
                    new Rect(1, 1, _renderTexture.width - 1, _renderTexture.height - 1),
                    _additionalTexture,
                    new Rect(0, 0, 1, 1),
                    1, 1, 1, 1);
                GL.PopMatrix();
            }
            #endregion Initialize

            if (gameObject != null)
            {
                Renderer[] renderers;
                if (gameObject.GetComponent<LODGroup>() != null)
                {
                    renderers = gameObject.GetComponent<LODGroup>().GetLODs()[0].renderers;
                }
                else
                {
                    renderers = gameObject.GetComponentsInChildren<Renderer>();
                }
                Bounds bounds = new Bounds();
                bool isBoundsInitialized = false;
                foreach (Renderer renderer in renderers)
                {
                    if (renderer == null)
                        continue;
                    Mesh mesh = null;
                    if (renderer.GetComponent<MeshFilter>())
                    {
                        mesh = renderer.GetComponent<MeshFilter>().sharedMesh;
                    }
                    else if (renderer is SkinnedMeshRenderer)
                    {
                        mesh = ((SkinnedMeshRenderer)renderer).sharedMesh;
                    }

                    if (mesh != null)
                    {
                        if (isVertexBased)
                        {
                            Vector3[] verts = mesh.vertices;
                            for (var v = 0; v < verts.Length; v++)
                            {
                                if (!isBoundsInitialized)
                                {
                                    isBoundsInitialized = true;
                                    bounds = new Bounds(renderer.transform.localToWorldMatrix.MultiplyPoint3x4(verts[v]), Vector3.zero);
                                }
                                else
                                    bounds.Encapsulate(renderer.transform.localToWorldMatrix.MultiplyPoint3x4(verts[v]));
                            }
                        }
                        else
                        {
                            Bounds rendererBounds = renderer.bounds;
                            if (!isBoundsInitialized)
                            {
                                isBoundsInitialized = true;
                                bounds = new Bounds(rendererBounds.center, rendererBounds.size);
                            }
                            else
                            {
                                bounds.Encapsulate(rendererBounds);
                            }
                        }
                    }
                }

                float maxBounds = Mathf.Max(Mathf.Max(bounds.extents.x, bounds.extents.y), bounds.extents.z);

                _camera.transform.position = bounds.center;
                _camera.orthographicSize = maxBounds * 1.3f;

                UnityEditorInternal.InternalEditorUtility.SetCustomLighting(lights, Color.gray);

                foreach (Renderer renderer in renderers)
                {
                    if (renderer == null)
                        continue;
                    Matrix4x4 matrix = renderer.transform.localToWorldMatrix;
                    Mesh mesh = null;
                    if (renderer.GetComponent<MeshFilter>())
                    {
                        mesh = renderer.GetComponent<MeshFilter>().sharedMesh;
                    }
                    else if (renderer is SkinnedMeshRenderer)
                    {
                        mesh = ((SkinnedMeshRenderer)renderer).sharedMesh;
                    }
                    if (mesh != null && renderer.sharedMaterials != null)
                    {
                        int submeshIndex = 0;
                        foreach (Material mat in renderer.sharedMaterials)
                        {
                            Graphics.DrawMesh(mesh, matrix, mat, _sampleLayer, _camera, Math.Min(submeshIndex, mesh.subMeshCount - 1),
                                null, ShadowCastingMode.Off, false, null, false);
                            submeshIndex++;
                        }
                    }
                }

                _camera.Render();
                UnityEditorInternal.InternalEditorUtility.RemoveCustomLighting();
            }

            #region Generate Texture
            Texture2D newTx = new Texture2D(_renderTexture.width, _renderTexture.height, TextureFormat.RGBA32, true);
            newTx.ReadPixels(new Rect(0, 0, _renderTexture.width, _renderTexture.height), 0, 0);
            newTx.Apply();
            #endregion Generate Texture

            #region End Preview
            RenderTexture.active = previousRenderTexture;

            foreach (var light in lights)
                light.enabled = false;

            RenderTexture.ReleaseTemporary(_renderTexture);
            #endregion End Preview

            return newTx;
        }

        public float GetScaleFactor(float width, float height)
        {
            float scaleFacX = Mathf.Max(Mathf.Min(width * 2, 1024), width) / width;
            float scaleFacY = Mathf.Max(Mathf.Min(height * 2, 1024), height) / height;
            float result = Mathf.Min(scaleFacX, scaleFacY) * EditorGUIUtility.pixelsPerPoint;
            return result;
        }

        protected static GameObject CreateLight()
        {
            GameObject lightGO = EditorUtility.CreateGameObjectWithHideFlags("PreRenderLight", HideFlags.HideAndDontSave, typeof(Light));
            var light = lightGO.GetComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.0f;
            light.enabled = false;
            return lightGO;
        }
    }
}