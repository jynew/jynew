using System;
using System.Collections.Generic;
using UnityEngine;

namespace MTE
{
    //Please keep this attribute, it is used by MTE editor.
    [ExecuteInEditMode]
    public class GrassLoader : MonoBehaviour
    {
        public GrassList grassInstanceList = null;

        private void Start()
        {
            this.RemoveOldGrasses();
            this.GenerateGrasses(new GrassGenerationSettings
            {
                HideGrassObjectInEditor = true,
                UseStaticBatch = true
            });
        }

        /// <summary>
        /// Remove all grass objects under this GameObject
        /// </summary>
        /// <remarks>
        /// Please keep this method public, because MTE Editor will call this method.
        /// </remarks>
        public void RemoveOldGrasses()
        {
            bool isEditor = !Application.isPlaying;

            // remove child GameObjects
            for (var i = this.transform.childCount - 1; i >= 0; i--)
            {
                var objTransform = this.transform.GetChild(i);
                if (isEditor)
                {
                    DestroyImmediate(objTransform.gameObject);
                }
                else
                {
                    Destroy(objTransform.gameObject);
                }
            }
        }

        /// <summary>
        /// Generate grass instances in the scene
        /// </summary>
        /// <remarks>
        /// Please keep this method public, because MTE will call this method to generate grasses in editor.
        /// </remarks>
        public void GenerateGrasses(GrassGenerationSettings settings)
        {
            if (grassInstanceList == null) return;

            // the star: three quads
            var stars = grassInstanceList.grasses;
            if (stars != null && stars.Count != 0)
            {
                foreach (var star in stars)
                {
                    GameObject grassObject;
                    MeshRenderer grassMeshRenderer;
                    Mesh grassMesh;
                    GrassUtil.GenerateGrassStarObject(
                        star.Position,
                        Quaternion.Euler(0, star.RotationY, 0),
                        star.Width, star.Height,
                        star.Material,
                        out grassObject, out grassMeshRenderer, out grassMesh);

                    grassObject.transform.SetParent(this.transform, true);
                    grassObject.hideFlags = settings.HideGrassObjectInEditor ?
                        HideFlags.HideInHierarchy : HideFlags.None;

                    //apply existing lightmap data to generated grass object
                    grassMeshRenderer.lightmapIndex = star.LightmapIndex;
                    grassMeshRenderer.lightmapScaleOffset = star.LightmapScaleOffset;
                }

                if (settings.UseStaticBatch)
                {
                    StaticBatchingUtility.Combine(this.gameObject);
                }
            }

            // the quad: one quad
            var quads = grassInstanceList.quads;
            if (quads != null && quads.Count != 0)
            {
                foreach (var quad in quads)
                {
                    GameObject grassObject;
                    MeshRenderer grassMeshRenderer;
                    Mesh grassMesh;
                    GrassUtil.GenerateGrassQuadObject(
                        quad.Position,
                        Quaternion.Euler(0, quad.RotationY, 0),
                        quad.Width, quad.Height,
                        quad.Material,
                        out grassObject, out grassMeshRenderer, out grassMesh);

                    grassObject.transform.SetParent(this.transform, true);
                    grassObject.hideFlags = settings.HideGrassObjectInEditor ?
                        HideFlags.HideInHierarchy : HideFlags.None;

                    //apply exist lightmap data to generated grass object
                    grassMeshRenderer.lightmapIndex = quad.LightmapIndex;
                    grassMeshRenderer.lightmapScaleOffset = quad.LightmapScaleOffset;
                }

                // billboards shouldn't be static-batched
            }

        }

    }

}
