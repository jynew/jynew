/* TILE WORLD CREATOR
 * Copyright (c) 2015 doorfortyfour OG
 * 
 * TileWorldCruncher.cs
 * 
 * attach this script to any gameobject in the scene and it 
 * will subtract or add cell to the map from its position.
 * 
*/
using UnityEngine;
using System.Collections;
using TileWorld;

public class TileWorldCruncher : MonoBehaviour {

    public TileWorldCreator creator;

    public int radius;
    public float crunchRate;
    public bool subtract = true;
    public bool heightDependent = false;

    private float nextCrunch;
    private bool isReady = false;
    private Vector3 lastPosition;
    
    void Start () {
	    
        if (creator == null)
        {
            creator = GameObject.FindObjectOfType<TileWorldCreator>();
            
            if (creator == null)
            {
                Debug.LogWarning("No TileWorldCreator prefab in the scene");
            }
        }

        Init();
    }


    void Init()
    {
        //use settings from the reference prefab
        //creator.useSettingsFromReference = true;
        //cruncher is ready
        isReady = true;
	}

	
	void Update () {

        if (Time.time > nextCrunch && isReady)
        {   
            nextCrunch = Time.time + crunchRate;
            
            if (lastPosition != this.transform.position)
            {
                lastPosition = this.transform.position;

                //set new values to the map based on our position
                Crunch(subtract);
                //rebuild the map
                BuildMap();
            }
        }

	}



    void Crunch(bool _subtract)
    {
        if (creator.configuration.worldMap.Count < 1)
            return;
        
        for (int l = 0; l < creator.configuration.global.layerCount; l++)
        {


            if (_subtract)
            {
                Vector3 _gP = this.transform.position;
                Vector3 _gPAbs = new Vector3(Mathf.Round(_gP.x) - 1, Mathf.Round(_gP.y), Mathf.Round(_gP.z) - 1);
                
                int layerHeightIndex = l;
                
                //if height dependent is true, set layer index according to the transform height of the object
                if (heightDependent)
                {
                    layerHeightIndex = (int)_gPAbs.y;
                }

                if (layerHeightIndex < creator.configuration.global.layerCount && layerHeightIndex >= 0)
                {
                    
                    for (int rY = -radius; rY < radius; rY++)
                    {
                        for (int rX = -radius; rX < radius; rX++)
                        {
     
                            if (_gPAbs.x + rX < 0)
                            {
                                _gPAbs.x = -1;
                            }
                            if (_gPAbs.z + rY < 0)
                            {
                                _gPAbs.z = -1;
                            }

                            if (_gPAbs.x + rX >= 0 && _gPAbs.z + rY >= 0 && _gPAbs.x + rX < creator.configuration.worldMap[layerHeightIndex].cellMap.GetLength(0) && _gPAbs.z + rY < creator.configuration.worldMap[layerHeightIndex].cellMap.GetLength(1))
                            {
                                creator.configuration.worldMap[layerHeightIndex].cellMap[(int)_gPAbs.x + rX, (int)_gPAbs.z + rY] = !creator.configuration.global.invert;
                            }

                            if (_gPAbs.x + 1 + rX < creator.configuration.worldMap[layerHeightIndex].cellMap.GetLength(0) && _gPAbs.z + rY < creator.configuration.worldMap[layerHeightIndex].cellMap.GetLength(1) && _gPAbs.z + rY >= 0 && _gPAbs.x + rX >= 0)
                            {
                                creator.configuration.worldMap[layerHeightIndex].cellMap[(int)_gPAbs.x + 1 + rX, (int)_gPAbs.z + rY] = !creator.configuration.global.invert;
                            }

                            if (_gPAbs.x + rX < creator.configuration.worldMap[layerHeightIndex].cellMap.GetLength(0) && _gPAbs.z + 1 + rY < creator.configuration.worldMap[layerHeightIndex].cellMap.GetLength(1) && _gPAbs.x + rX >= 0 && _gPAbs.z + rY >= 0)
                            {
                                creator.configuration.worldMap[layerHeightIndex].cellMap[(int)_gPAbs.x + rX, (int)_gPAbs.z + 1 + rY] = !creator.configuration.global.invert;
                            }

                            if (_gPAbs.x + 1 + rX < creator.configuration.worldMap[layerHeightIndex].cellMap.GetLength(0) && _gPAbs.z + 1 + rY < creator.configuration.worldMap[layerHeightIndex].cellMap.GetLength(1) && _gPAbs.x + rX >= 0 && _gPAbs.z + rY >= 0)
                            {
                                creator.configuration.worldMap[layerHeightIndex].cellMap[(int)_gPAbs.x + 1 + rX, (int)_gPAbs.z + 1 + rY] = !creator.configuration.global.invert;
                            }

                        }
                    }
                }
            }
            else
            {

                Vector3 _gP = this.transform.position;
                Vector3 _gPAbs = new Vector3(Mathf.Round(_gP.x) - 1, Mathf.Round(_gP.y), Mathf.Round(_gP.z) - 1);

                int layerHeightIndex = l;
                
                //if height dependant is true, set layer index according to the transform height of the object
                if (heightDependent)
                {
                    layerHeightIndex = (int)_gPAbs.y;
                }

                if (layerHeightIndex < creator.configuration.global.layerCount && layerHeightIndex >= 0)
                {

                    for (int rY = -radius; rY < radius; rY++)
                    {
                        for (int rX = -radius; rX < radius; rX++)
                        {
                            if (_gPAbs.x + rX < 0)
                            {
                                _gPAbs.x = -1;
                            }
                            if (_gPAbs.z + rY < 0)
                            {
                                _gPAbs.z = -1;
                            }

                            if (_gPAbs.x + rX >= 0 && _gPAbs.z + rY >= 0 && _gPAbs.x + rX < creator.configuration.worldMap[layerHeightIndex].cellMap.GetLength(0) && _gPAbs.z + rY < creator.configuration.worldMap[layerHeightIndex].cellMap.GetLength(1))
                            {
                                creator.configuration.worldMap[layerHeightIndex].cellMap[(int)_gPAbs.x + rX, (int)_gPAbs.z + rY] = creator.configuration.global.invert;
                            }

                            if (_gPAbs.x + 1 + rX < creator.configuration.worldMap[layerHeightIndex].cellMap.GetLength(0) && _gPAbs.z + rY < creator.configuration.worldMap[layerHeightIndex].cellMap.GetLength(1) && _gPAbs.z + rY >= 0 && _gPAbs.x + rX >= 0)
                            {
                                creator.configuration.worldMap[layerHeightIndex].cellMap[(int)_gPAbs.x + 1 + rX, (int)_gPAbs.z + rY] = creator.configuration.global.invert;
                            }

                            if (_gPAbs.x + rX < creator.configuration.worldMap[layerHeightIndex].cellMap.GetLength(0) && _gPAbs.z + 1 + rY < creator.configuration.worldMap[layerHeightIndex].cellMap.GetLength(1) && _gPAbs.x + rX >= 0 && _gPAbs.z + rY >= 0)
                            {
                                creator.configuration.worldMap[layerHeightIndex].cellMap[(int)_gPAbs.x + rX, (int)_gPAbs.z + 1 + rY] = creator.configuration.global.invert;
                            }

                            if (_gPAbs.x + 1 + rX < creator.configuration.worldMap[layerHeightIndex].cellMap.GetLength(0) && _gPAbs.z + 1 + rY < creator.configuration.worldMap[layerHeightIndex].cellMap.GetLength(1) && _gPAbs.x + rX >= 0 && _gPAbs.z + rY >= 0)
                            {
                                creator.configuration.worldMap[layerHeightIndex].cellMap[(int)_gPAbs.x + 1 + rX, (int)_gPAbs.z + 1 + rY] = creator.configuration.global.invert;
                            }
                        }
                    }

                }

            }
        }
    }

    //build map
    void BuildMap()
    {
        if (creator.firstTimeBuild)
        {
            creator.BuildMapComplete(false, false, false);
        }
        else
        {
            Vector3 _p = this.transform.position;
            //this is a custom optimization loop because we are using a radius 
            //instead of just a single point position             
            for (int l = 0; l < creator.configuration.global.layerCount; l++)
            {
                for (int rY = -radius; rY < radius; rY++)
                {
                    for (int rX = -radius; rX < radius; rX++)
                    {
                        creator.OptimizePassPartial(l, (int)_p.x + rX, (int)_p.z + rY);
                    }
                }
            }
            
            creator.BuildMapPartial(false, true);
        }
    }
}
