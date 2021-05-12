/* TILE WORLD CREATOR
 * Copyright (c) 2015 doorfortyfour OG
 * 
 * Create awesome tile worlds in seconds.
 *
 *
 * Documentation: http://tileworldcreator.doofortyfour.com
 * Like us on Facebook: http://www.facebook.com/doorfortyfour2013
 * Web: http://www.doorfortyfour.com
 * Contact: mail@doorfortyfour.com Contact us for help, bugs or 
 * share your awesome work you've made with TileWorldCreator
*/

using UnityEngine;
using System.Collections;
using TileWorld;

namespace TileWorld
{
	
	public class TileWorldCombiner : MonoBehaviour {



        public static void CombineMesh(GameObject _g, bool _collider)
        {
            Component[] filters = _g.GetComponentsInChildren(typeof(MeshFilter));
            Matrix4x4 myTransform = _g.transform.worldToLocalMatrix;
            Hashtable materialToMesh = new Hashtable();
            //bool generateTriangleStrips = true;
            
            for (int i = 0; i < filters.Length; i++)
            {
                MeshFilter filter = (MeshFilter)filters[i];
                Renderer curRenderer = filters[i].GetComponent<Renderer>();
                MeshCombineUtility.MeshInstance instance = new MeshCombineUtility.MeshInstance();
                instance.mesh = filter.sharedMesh;

                if (curRenderer != null && curRenderer.enabled && instance.mesh != null)
                {
                    instance.transform = myTransform * filter.transform.localToWorldMatrix;

                    Material[] materials = curRenderer.sharedMaterials;

                    for (int m = 0; m < materials.Length; m++)
                    {
                        instance.subMeshIndex = System.Math.Min(m, instance.mesh.subMeshCount - 1);

                        ArrayList objects = (ArrayList)materialToMesh[materials[m]];

                        if (objects != null)
                        {
                            objects.Add(instance);
                        }
                        else
                        {
                            objects = new ArrayList();
                            objects.Add(instance);
                            materialToMesh.Add(materials[m], objects);
                        }
                    }

                    if (Application.isPlaying)
                    {
                        Destroy(curRenderer.gameObject);
                    }
                    else
                    {
                        DestroyImmediate(curRenderer.gameObject);
                    }
                }
            }




            foreach (DictionaryEntry de in materialToMesh)
            {
                ArrayList elements = (ArrayList)de.Value;
                MeshCombineUtility.MeshInstance[] instances = (MeshCombineUtility.MeshInstance[])elements.ToArray(typeof(MeshCombineUtility.MeshInstance));

                // We have a maximum of one material, so just attach the mesh to our own game object
                if (materialToMesh.Count == 1)
                {
                    // Make sure we have a mesh filter & renderer
                    if (_g.GetComponent(typeof(MeshFilter)) == null)
                        _g.gameObject.AddComponent(typeof(MeshFilter));
                    if (!_g.GetComponent("MeshRenderer"))
                        _g.gameObject.AddComponent<MeshRenderer>(); //("MeshRenderer");

                    MeshFilter filter = (MeshFilter)_g.GetComponent(typeof(MeshFilter));
                    if (Application.isPlaying) filter.mesh = MeshCombineUtility.Combine(instances); //, generateTriangleStrips);
                    else filter.sharedMesh = MeshCombineUtility.Combine(instances); //, generateTriangleStrips);
                    _g.GetComponent<Renderer>().material = (Material)de.Key;
                    _g.GetComponent<Renderer>().enabled = true;

                    if (_collider)
                    {
                        MeshCollider _mc = _g.AddComponent<MeshCollider>();
                        _mc.sharedMesh = filter.sharedMesh;
                    }
                }

                // We have multiple materials to take care of, build one mesh / gameobject for each material
                // and parent it to this object
                else
                {
                    GameObject go = new GameObject("combined_mesh");

                    go.transform.parent = _g.transform;
                    go.transform.localScale = Vector3.one;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localPosition = Vector3.zero;
                    go.AddComponent(typeof(MeshFilter));
                    go.AddComponent<MeshRenderer>(); //("MeshRenderer");
                    go.GetComponent<Renderer>().material = (Material)de.Key;
                    MeshFilter filter = (MeshFilter)go.GetComponent(typeof(MeshFilter));



                    //if(Application.isPlaying)filter.mesh = MeshCombineUtility.Combine(instances, generateTriangleStrips);
                    //else 
                    filter.sharedMesh = MeshCombineUtility.Combine(instances); //, generateTriangleStrips);


                    if (_collider)
                    {
                        MeshCollider _mc = go.AddComponent<MeshCollider>();
                        _mc.sharedMesh = filter.sharedMesh;
                    }

                }
            }
        }


        public static void CombineMulitMaterialMesh(GameObject _g, bool _collider)
        {
            

            MeshFilter[] _meshFilters = _g.GetComponentsInChildren<MeshFilter>();
            CombineInstance[] _combine = new CombineInstance[_meshFilters.Length];
            Material[] _materials = new Material[_combine.Length];

            int i = 0;
            while (i < _meshFilters.Length)
            {
                _combine[i].mesh = _meshFilters[i].sharedMesh;
                _combine[i].transform = _meshFilters[i].transform.localToWorldMatrix;

                _materials[i] = new Material(_meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial);

                i++;
            }

            //Destroy childrens
            for (int f = 0; f < _combine.Length; f++)
            {
                if (_meshFilters.Length > 1 && _meshFilters[f] != null)
                {
                    DestroyImmediate(_meshFilters[f].gameObject);
                }
            }

            MeshRenderer _gMeshRenderer;
            MeshFilter _gMeshFilter;
           
            if (_g.transform.GetComponent<MeshFilter>() == null)
            {
                _gMeshFilter = _g.AddComponent<MeshFilter>();
                _gMeshRenderer = _g.AddComponent<MeshRenderer>();
            }
            else
            {
                _gMeshFilter = _g.transform.GetComponent<MeshFilter>();
                _gMeshRenderer = _g.transform.GetComponent<MeshRenderer>();
            }

            //combine meshes to submeshes
            _gMeshFilter.mesh = new Mesh();
            _gMeshFilter.sharedMesh.CombineMeshes(_combine, false);

            //assign materials
            _gMeshRenderer.sharedMaterials = _materials;

            //add mesh collider
            if (_collider)
            {
                if (_g.transform.GetComponent<MeshCollider>() == null)
                {
                    MeshCollider _mc = _g.AddComponent<MeshCollider>();
                    _mc.sharedMesh = _gMeshFilter.sharedMesh;
                }
            }

        }

    }
}