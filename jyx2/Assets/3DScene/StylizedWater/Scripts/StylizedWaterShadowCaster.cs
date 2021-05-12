// Stylized Water Shader by Staggart Creations http://u3d.as/A2R
// Online documentation can be found at http://staggart.xyz

using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StylizedWater
{
    [RequireComponent(typeof(Light))]
    [ExecuteInEditMode]
    public class StylizedWaterShadowCaster : MonoBehaviour
    {
        CommandBuffer cmd = null;
        public Light dirLight;

        void OnEnable()
        {
            if (!dirLight) dirLight = this.GetComponent<Light>();

            if (dirLight)
            {
                if (dirLight.GetCommandBuffers(LightEvent.AfterScreenspaceMask).Length < 1)
                {
                    cmd = new CommandBuffer();
                    cmd.name = "Water Shadow Mask";
                    cmd.SetGlobalTexture("_ShadowMask", new RenderTargetIdentifier(BuiltinRenderTextureType.CurrentActive));
                    dirLight.AddCommandBuffer(LightEvent.AfterScreenspaceMask, cmd);
                }
            }
        }

        void OnDisable()
        {
            if (dirLight)
            {
                dirLight.RemoveAllCommandBuffers();
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(StylizedWaterShadowCaster))]
    public class StylizedWaterShadowCasterInspector : Editor
    {
        override public void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("\nThis script is automatically added when shadows are enabled on a Stylized Water script component\n", MessageType.Info);
        }
    }
#endif
}
