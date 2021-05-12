// Stylized Water Shader by Staggart Creations http://u3d.as/A2R
// Online documentation can be found at http://staggart.xyz

using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

namespace StylizedWater
{
    public class StylizedWaterRenderer : Object
    {
        //DEV
        private static bool debug = false;

        //Setting set in inspector, passed on through struct
        public class RendererSettings
        {
            public LayerMask renderLayer;
            public int renderTexureSize = 1024;
        }
        private RendererSettings settings;

        //Object
        private GameObject targetObject;
        private Vector3 targetSize;
        private Vector3 targetCenterPosition;

        private RenderTexture _rt;
        private RenderTexture rt
        {
            get
            {
                if (_rt == null)
                {
                    _rt = new RenderTexture(settings.renderTexureSize, settings.renderTexureSize, 0);
                    _rt.hideFlags = HideFlags.DontSave;
                }
                    return _rt;
            }
        }

        private const int CLIP_PLANE_PADDING = 20;

        private static Material _intersectionMaskMat;
        public static Material intersectionMaskMat
        {
            get
            {
                if(_intersectionMaskMat == null)
                {
                    _intersectionMaskMat = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
                    _intersectionMaskMat.color = Color.red;
                    _intersectionMaskMat.name = "SWS_IntersectionMask";
                }

                return _intersectionMaskMat;
            }
        }

        private static Material _opacityMaskMat;
        public static Material opacityMaskMat
        {
            get
            {
                if (_opacityMaskMat == null)
                {
                    _opacityMaskMat = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
                    _opacityMaskMat.color = Color.green;
                    _opacityMaskMat.name = "SWS_OpacityMask";
                }

                return _opacityMaskMat;
            }
        }

        #region Mask rendering
        public void SetupMaskRenderer(RendererSettings settings, MeshRenderer r, out Camera renderCam, out Vector4 targetMeshInfo)
        {
            this.settings = settings;

            targetObject = r.gameObject;

            targetMeshInfo = GetMeshInfo(r);
            renderCam = SetupRenderCam();
            RenderCamToTexture(renderCam);
        }

        private Vector4 GetMeshInfo(MeshRenderer r)
        {
            Bounds meshBounds = r.bounds;

            //Mesh size has to be uniform
            if (StylizedWaterUtilities.IsApproximatelyEqual(r.bounds.extents.x, r.bounds.extents.z))
            {
                if (debug) Debug.LogErrorFormat("[Stylized Water] Size of \"{0}\" is not uniform at {1}! Width and Length must be equal!", r.name, r.bounds.extents.x + "x" + r.bounds.extents.z);
            }

            targetSize = meshBounds.extents * 2;
            targetCenterPosition = meshBounds.center;

            if (debug) Debug.Log(r.name + " : Size = " + targetSize + " Center = " + targetCenterPosition);

            Vector4 meshScaleOffset = new Vector4(targetSize.x, targetSize.z, targetCenterPosition.x - (targetSize.x / 2), targetCenterPosition.z - (targetSize.z / 2));
            Shader.SetGlobalVector("_SWS_RENDERTEX_POS", meshScaleOffset);

            return meshScaleOffset;
        }

        private Camera SetupRenderCam()
        {
            Camera renderCam = new GameObject().AddComponent<Camera>();
            renderCam.name = "RenderCamera";
            renderCam.transform.parent = targetObject.transform;
            renderCam.transform.hideFlags = HideFlags.NotEditable;

            //Set up a square camera rect
            float rectWidth = (float)settings.renderTexureSize;
            rectWidth /= Screen.width;
            renderCam.rect = new Rect(0, 0, rectWidth, 1);

            //Camera set up
            renderCam.orthographic = true;
            renderCam.orthographicSize = (targetSize.x / 2);
            //Add 10 unit margin, in case terrain is completly flat
            renderCam.farClipPlane = targetSize.y + (CLIP_PLANE_PADDING * 2);
            renderCam.useOcclusionCulling = false;
            renderCam.clearFlags = CameraClearFlags.SolidColor;
            renderCam.backgroundColor = Color.black;
            renderCam.depth = -1;
            renderCam.cullingMask = ~(1 << 4) & settings.renderLayer.value;

            renderCam.renderingPath = RenderingPath.Forward;

            //Camera position
            float camPosX, camPosY, camPosZ = 0;

            //Mesh center
            camPosX = targetCenterPosition.x;
            camPosY = targetCenterPosition.y;
            camPosZ = targetCenterPosition.z;

            //Position cam in given center of terrain
            renderCam.transform.position = new Vector3(camPosX, camPosY + CLIP_PLANE_PADDING, camPosZ);

            //Rotate down
            renderCam.transform.localEulerAngles = new Vector3(90, 0f, 0f);

            return renderCam;
        }

        private RenderTexture RenderCamToTexture(Camera renderCam)
        {
            //Set up render texture
            renderCam.targetTexture = rt;
            RenderTexture.active = rt;

            //Pass it to all shaders utilizing the global texture parameter
            Shader.SetGlobalTexture("_SWS_RENDERTEX", rt);

            return rt;
        }
        #endregion

    }
}
