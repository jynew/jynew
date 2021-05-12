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
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    public class StylizedWaterBlur : MonoBehaviour
    {
        public Camera cam;
        public float length = 6;
        public int passes = 4;

        private static Shader m_BlurRenderShader;
        private static Shader BlurRenderShader
        {
            get
            {
                if (m_BlurRenderShader == null)
                {
                    m_BlurRenderShader = Shader.Find("Hidden/SWS/Blur");
                    return m_BlurRenderShader;
                }
                else
                {
                    return m_BlurRenderShader;
                }
            }
        }
        private static Material m_BlurRenderMat;
        private static Material BlurRenderMat
        {
            get
            {
                if (m_BlurRenderMat == null)
                {
                    m_BlurRenderMat = new Material(BlurRenderShader);
                    m_BlurRenderMat.hideFlags = HideFlags.HideAndDontSave;
                    return m_BlurRenderMat;
                }
                else
                {
                    return m_BlurRenderMat;
                }
            }
        }

        private CommandBuffer cmd;

        private void OnEnable()
        {
            if (!cam) cam = this.GetComponent<Camera>();

            Render();

            //Debug.Log("Blur added to " + cam.name);
        }
        private void OnDisable()
        {
            DestroyImmediate(BlurRenderMat);
            if (cmd != null)
            {
                cmd.Clear();
                cam.RemoveCommandBuffer(CameraEvent.AfterSkybox, cmd);
            }

            //Debug.Log("Blur removed from " + cam.name);
        }

        public void Render()
        {
            if (!cam) return;

            //Remove all occurances of the buffer
            if (cmd != null) cam.RemoveCommandBuffer(CameraEvent.AfterSkybox, cmd);

            cmd = new CommandBuffer();
            cmd.name = "Grab screen and blur";

            // get two smaller RTs
            int blurredID = Shader.PropertyToID("_BlurBuffer1");
            int blurredID2 = Shader.PropertyToID("_BlurBuffer2");
            cmd.GetTemporaryRT(blurredID, 0, 0, 0, FilterMode.Bilinear);
            cmd.GetTemporaryRT(blurredID2, 0, 0, 0, FilterMode.Bilinear);

            //Copy reflection texture into new RT to be ping-ponged
            cmd.Blit(BuiltinRenderTextureType.CurrentActive, blurredID);

            for (int i = 0; i < passes; i++)
            {
                //Safeguard for exploding GPU
                if (i > 4) return;

                cmd.SetGlobalFloat("BlurLength", length / Screen.height);

                //Ping-pong blurring
                cmd.Blit(blurredID, blurredID2, BlurRenderMat);
                cmd.Blit(blurredID2, blurredID, BlurRenderMat);
            }

            cmd.SetGlobalTexture("_ReflectionTex", blurredID);

            cam.AddCommandBuffer(CameraEvent.AfterSkybox, cmd);

            cmd.ReleaseTemporaryRT(blurredID);
            cmd.ReleaseTemporaryRT(blurredID2);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(StylizedWaterBlur))]
    public class StylizedWaterBlurInspector : Editor
    {
        override public void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("\nThis script should not be manually added to an object!\n", MessageType.Error);
        }
    }
#endif
}