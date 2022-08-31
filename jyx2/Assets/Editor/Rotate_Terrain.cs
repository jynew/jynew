using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
 
 
public class terraintest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
 
    // Update is called once per frame
    void Update()
    {
        
    }
 
    [MenuItem("Game Tools/Rotate 90 Deg")]
    static void rotate()
    {
        int i, j;
        var ter = Terrain.activeTerrain;
        var td = ter.terrainData;
 
        //rotate heightmap
        var hgts = td.GetHeights(0, 0, td.heightmapResolution, td.heightmapResolution);
        var newhgts = new float[hgts.GetLength(0), hgts.GetLength(1)];
        for (j = 0; j < td.heightmapResolution; j++)
        {
            for (i = 0; i < td.heightmapResolution; i++)
            {
                newhgts[td.heightmapResolution - 1 - j, i] = hgts[i, j];
            }
        }
        td.SetHeights(0, 0, newhgts);
        ter.Flush();
 
        //rotate splatmap
        var alpha = td.GetAlphamaps(0, 0, td.alphamapWidth, td.alphamapHeight);
        var newalpha = new float[alpha.GetLength(0), alpha.GetLength(1), alpha.GetLength(2)];
        for (j = 0; j < td.alphamapHeight; j++)
        {
            for (i = 0; i < td.alphamapWidth; i++)
            {
                for (int k = 0; k < td.splatPrototypes.Length; k++)
                {
                    newalpha[td.alphamapHeight - 1 - j, i, k] = alpha[i, j, k];
                }
            }
        }
        td.SetAlphamaps(0, 0, newalpha);
 
        //rotate trees
        var size = td.size;
        var trees = td.treeInstances;
        for (i = 0; i < trees.Length; i++)
        {
            trees[i].position = new Vector3(1 - trees[i].position.z, 0, trees[i].position.x);
            trees[i].position.y = td.GetInterpolatedHeight(trees[i].position.x, trees[i].position.z) / size.y;
        }
        td.treeInstances = trees;
 
        //rotate detail layers
        var num = td.detailPrototypes.Length;
        for (int k = 0; k < num; k++)
        {
            var map = td.GetDetailLayer(0, 0, td.detailWidth, td.detailHeight, k);
            var newmap = new int[map.GetLength(0), map.GetLength(1)];
            for (j = 0; j < td.detailHeight; j++)
            {
                for (i = 0; i < td.detailWidth; i++)
                {
                    newmap[td.detailHeight - 1 - j, i] = map[i, j];
                }
            }
            td.SetDetailLayer(0, 0, k, newmap);
        }
    }
 
 
}