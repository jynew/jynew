using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPUInstancer
{
    /// <summary>
    /// Can be added to a scene to displays LOD levels with different colors for debugging purposes. Only works if the prototype's shader has a _Color property in it.
    /// </summary>
    public class GPUInstancerLODColorDebugger : MonoBehaviour
    {
        public GPUInstancerManager gPUIManager;
        public List<Color> lODColors = new List<Color> { Color.red, Color.blue, Color.yellow  };

        private Dictionary<Material, Color> _previousColors;

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
            ChangeLODColors();
        }

        public void ChangeLODColors()
        {
            _previousColors = new Dictionary<Material, Color>();
            foreach (GPUInstancerRuntimeData runtimeData in gPUIManager.runtimeDataList)
            {
                for (int l = 1; l < runtimeData.instanceLODs.Count && l <= lODColors.Count; l++)
                {
                    for (int r = 0; r < runtimeData.instanceLODs[l].renderers.Count; r++)
                    {
                        for (int m = 0; m < runtimeData.instanceLODs[l].renderers[r].materials.Count; m++)
                        {
                            if (runtimeData.instanceLODs[l].renderers[r].materials[m].HasProperty("_Color"))
                            {
                                if (!_previousColors.ContainsKey(runtimeData.instanceLODs[l].renderers[r].materials[m]))
                                    _previousColors.Add(runtimeData.instanceLODs[l].renderers[r].materials[m], runtimeData.instanceLODs[l].renderers[r].materials[m].color);
                                runtimeData.instanceLODs[l].renderers[r].materials[m].color = lODColors[l - 1];
                            }
                            if (runtimeData.instanceLODs[l].renderers[r].materials[m].HasProperty("_BaseColor"))
                            {
                                if (!_previousColors.ContainsKey(runtimeData.instanceLODs[l].renderers[r].materials[m]))
                                    _previousColors.Add(runtimeData.instanceLODs[l].renderers[r].materials[m], runtimeData.instanceLODs[l].renderers[r].materials[m].GetColor("_BaseColor"));
                                runtimeData.instanceLODs[l].renderers[r].materials[m].SetColor("_BaseColor", lODColors[l - 1]);
                            }
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
                for (int l = 1; l < runtimeData.instanceLODs.Count && l <= lODColors.Count; l++)
                {
                    for (int r = 0; r < runtimeData.instanceLODs[l].renderers.Count; r++)
                    {
                        for (int m = 0; m < runtimeData.instanceLODs[l].renderers[r].materials.Count; m++)
                        {
                            if (runtimeData.instanceLODs[l].renderers[r].materials[m].HasProperty("_Color") && _previousColors.ContainsKey(runtimeData.instanceLODs[l].renderers[r].materials[m]))
                                runtimeData.instanceLODs[l].renderers[r].materials[m].color = _previousColors[runtimeData.instanceLODs[l].renderers[r].materials[m]];
                            if (runtimeData.instanceLODs[l].renderers[r].materials[m].HasProperty("_BaseColor") && _previousColors.ContainsKey(runtimeData.instanceLODs[l].renderers[r].materials[m]))
                                runtimeData.instanceLODs[l].renderers[r].materials[m].SetColor("_BaseColor", _previousColors[runtimeData.instanceLODs[l].renderers[r].materials[m]]);
                        }
                    }
                }
            }
        }
    }
}