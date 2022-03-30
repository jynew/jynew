using System.Collections.Generic;
using UnityEngine;

namespace MTE
{
    internal class GrassEditorUtil
    {
        /// <summary>
        /// Reload grass and build the grass map.
        /// </summary>
        /// <param name="grassLoader"></param>
        internal static void ReloadGrassesFromFile(GrassLoader grassLoader)
        {
            //clear grass GameObjects
            grassLoader.RemoveOldGrasses();

            //clear grass items in the map
            GrassMap.Clear();

            var grassList = grassLoader.grassInstanceList;

            // the star: three quads
            List<GrassStar> stars = grassList.grasses;
            if (stars != null && stars.Count != 0)
            {
                for (int i = 0; i < stars.Count; i++)
                {
                    //create grass object
                    var star = stars[i];
                    GameObject grassObject;
                    MeshRenderer grassMeshRenderer;
                    Mesh grassMesh;
                    GrassUtil.GenerateGrassStarObject(
                        star.Position,
                        Quaternion.Euler(0, star.RotationY, 0),
                        star.Width, star.Height,
                        star.Material,
                        out grassObject, out grassMeshRenderer, out grassMesh);

                    grassObject.transform.SetParent(grassLoader.transform, true);
                    
                    //apply existing lightmap data to generated grass object
                    grassMeshRenderer.lightmapIndex = star.LightmapIndex;
                    grassMeshRenderer.lightmapScaleOffset = star.LightmapScaleOffset;

                    GrassMap.Insert(new GrassItem(star, grassObject));
                }
            }

            // the quad: one quad
            List<GrassQuad> quads = grassList.quads;
            if (quads != null && quads.Count != 0)
            {
                for (int i = 0; i < quads.Count; i++)
                {
                    //create grass object
                    var quad = quads[i];
                    GameObject grassObject;
                    MeshRenderer grassMeshRenderer;//not used
                    Mesh grassMesh;//not used
                    GrassUtil.GenerateGrassQuadObject(
                        quad.Position,
                        Quaternion.Euler(0, quad.RotationY, 0),
                        quad.Width, quad.Height,
                        quad.Material,
                        out grassObject, out grassMeshRenderer, out grassMesh);

                    grassObject.transform.SetParent(grassLoader.transform, true);

                    //apply exist lightmap data to generated grass object
                    grassMeshRenderer.lightmapIndex = quad.LightmapIndex;
                    grassMeshRenderer.lightmapScaleOffset = quad.LightmapScaleOffset;

                    GrassMap.Insert(new GrassItem(quad, grassObject));
                }

                // billboards shouldn't be static-batched
            }
            
            Utility.ShowNotification(StringTable.Get(C.Info_ReloadedGrassesFromFile));
        }
    }
}