using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPUInstancer
{
    /// <summary>
    /// Can be added to a scene to displays Draw Calls with different colors for debugging purposes. Only works if the prototype's shader has a _Color property in it.
    /// </summary>
    public class GPUInstancerDrawCallColorDebugger : MonoBehaviour
    {
        public GPUInstancerManager gPUIManager;
        public List<Color> drawCallColors = new List<Color> { Color.red, Color.blue, Color.yellow, Color.green, Color.magenta };
        public bool removeMainTextureWhenColored = false;

        private Dictionary<Material, Color> _previousColors;
        private Dictionary<Material, Texture> _previousTextures;

        void OnEnable()
        {
            if (gPUIManager != null)
                StartCoroutine(ColorLODs());
        }

        void OnDisable()
        {
            if (gPUIManager != null)
                ResetColors();
        }

        private void Reset()
        {
            if (GetComponent<GPUInstancerManager>() != null)
                gPUIManager = GetComponent<GPUInstancerManager>();
        }

        IEnumerator ColorLODs()
        {
            while (!gPUIManager.isInitialized)
                yield return null;
            ChangeDrawCallColors();
        }

        public void ChangeDrawCallColors()
        {
            _previousColors = new Dictionary<Material, Color>();
            if (removeMainTextureWhenColored)
                _previousTextures = new Dictionary<Material, Texture>();
            else
                _previousTextures = null;
            int drawCallCount = 0;
            foreach (GPUInstancerRuntimeData runtimeData in gPUIManager.runtimeDataList)
            {
                for (int l = 0; l < runtimeData.instanceLODs.Count; l++)
                {
                    for (int r = 0; r < runtimeData.instanceLODs[l].renderers.Count; r++)
                    {
                        for (int m = 0; m < runtimeData.instanceLODs[l].renderers[r].materials.Count; m++)
                        {
                            if (runtimeData.instanceLODs[l].renderers[r].materials[m].HasProperty("_Color"))
                            {
                                if (!_previousColors.ContainsKey(runtimeData.instanceLODs[l].renderers[r].materials[m]))
                                    _previousColors.Add(runtimeData.instanceLODs[l].renderers[r].materials[m], runtimeData.instanceLODs[l].renderers[r].materials[m].color);
                                runtimeData.instanceLODs[l].renderers[r].materials[m].color = drawCallCount < drawCallColors.Count ? drawCallColors[drawCallCount] : Random.ColorHSV();
                            }
                            if (runtimeData.instanceLODs[l].renderers[r].materials[m].HasProperty("_BaseColor"))
                            {
                                if (!_previousColors.ContainsKey(runtimeData.instanceLODs[l].renderers[r].materials[m]))
                                    _previousColors.Add(runtimeData.instanceLODs[l].renderers[r].materials[m], runtimeData.instanceLODs[l].renderers[r].materials[m].GetColor("_BaseColor"));
                                runtimeData.instanceLODs[l].renderers[r].materials[m].SetColor("_BaseColor", drawCallCount < drawCallColors.Count ? drawCallColors[drawCallCount] : Random.ColorHSV());
                            }
                            if (_previousTextures != null && runtimeData.instanceLODs[l].renderers[r].materials[m].HasProperty("_MainTex"))
                            {
                                if (!_previousTextures.ContainsKey(runtimeData.instanceLODs[l].renderers[r].materials[m]))
                                    _previousTextures.Add(runtimeData.instanceLODs[l].renderers[r].materials[m], runtimeData.instanceLODs[l].renderers[r].materials[m].mainTexture);
                                runtimeData.instanceLODs[l].renderers[r].materials[m].mainTexture = null;
                            }
                            if (_previousTextures != null && runtimeData.instanceLODs[l].renderers[r].materials[m].HasProperty("_BaseMap"))
                            {
                                if (!_previousTextures.ContainsKey(runtimeData.instanceLODs[l].renderers[r].materials[m]))
                                    _previousTextures.Add(runtimeData.instanceLODs[l].renderers[r].materials[m], runtimeData.instanceLODs[l].renderers[r].materials[m].GetTexture("_BaseMap"));
                                runtimeData.instanceLODs[l].renderers[r].materials[m].SetTexture("_BaseMap", null);
                            }
                            if (_previousTextures != null && runtimeData.instanceLODs[l].renderers[r].materials[m].HasProperty("_BaseColorMap"))
                            {
                                if (!_previousTextures.ContainsKey(runtimeData.instanceLODs[l].renderers[r].materials[m]))
                                    _previousTextures.Add(runtimeData.instanceLODs[l].renderers[r].materials[m], runtimeData.instanceLODs[l].renderers[r].materials[m].GetTexture("_BaseColorMap"));
                                runtimeData.instanceLODs[l].renderers[r].materials[m].SetTexture("_BaseColorMap", null);
                            }
                            drawCallCount++;
                        }
                    }
                }
            }
        }

        void ResetColors()
        {
            if (_previousColors == null)
                return;

            foreach (GPUInstancerRuntimeData runtimeData in gPUIManager.runtimeDataList)
            {
                for (int l = 0; l < runtimeData.instanceLODs.Count; l++)
                {
                    for (int r = 0; r < runtimeData.instanceLODs[l].renderers.Count; r++)
                    {
                        for (int m = 0; m < runtimeData.instanceLODs[l].renderers[r].materials.Count; m++)
                        {
                            if (runtimeData.instanceLODs[l].renderers[r].materials[m].HasProperty("_Color") &&
                                _previousColors.ContainsKey(runtimeData.instanceLODs[l].renderers[r].materials[m]))
                            {
                                runtimeData.instanceLODs[l].renderers[r].materials[m].color = _previousColors[runtimeData.instanceLODs[l].renderers[r].materials[m]];
                            }
                            if (runtimeData.instanceLODs[l].renderers[r].materials[m].HasProperty("_BaseColor") &&
                                _previousColors.ContainsKey(runtimeData.instanceLODs[l].renderers[r].materials[m]))
                            {
                                runtimeData.instanceLODs[l].renderers[r].materials[m].SetColor("_BaseColor", _previousColors[runtimeData.instanceLODs[l].renderers[r].materials[m]]);
                            }
                            if (_previousTextures != null && runtimeData.instanceLODs[l].renderers[r].materials[m].HasProperty("_MainTex") &&
                                _previousTextures.ContainsKey(runtimeData.instanceLODs[l].renderers[r].materials[m]))
                            {
                                runtimeData.instanceLODs[l].renderers[r].materials[m].mainTexture = _previousTextures[runtimeData.instanceLODs[l].renderers[r].materials[m]];
                            }
                            if (_previousTextures != null && runtimeData.instanceLODs[l].renderers[r].materials[m].HasProperty("_BaseMap") &&
                                _previousTextures.ContainsKey(runtimeData.instanceLODs[l].renderers[r].materials[m]))
                            {
                                runtimeData.instanceLODs[l].renderers[r].materials[m].SetTexture("_BaseMap", _previousTextures[runtimeData.instanceLODs[l].renderers[r].materials[m]]);
                            }
                            if (_previousTextures != null && runtimeData.instanceLODs[l].renderers[r].materials[m].HasProperty("_BaseColorMap") &&
                                _previousTextures.ContainsKey(runtimeData.instanceLODs[l].renderers[r].materials[m]))
                            {
                                runtimeData.instanceLODs[l].renderers[r].materials[m].SetTexture("_BaseColorMap", _previousTextures[runtimeData.instanceLODs[l].renderers[r].materials[m]]);
                            }
                        }
                    }
                }
            }
        }
    }
}