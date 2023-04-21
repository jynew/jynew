using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshCombineStudio
{
    [ExecuteInEditMode]
    public class FindLodGroups : MonoBehaviour
    {

        public bool find;

        void Start()
        {
            FindLods();
        }

        void Update()
        {
            if (find)
            {
                find = false;
                FindLods();
            }
        }

        void FindLods()
        {
            LODGroup[] lodGroups = GetComponentsInChildren<LODGroup>(true);

            for (int i = 0; i < lodGroups.Length; i++)
            {
                Debug.Log(lodGroups[i].name);
            }
            Debug.Log("---------------------------------------------");
            Debug.Log("LODGroups found " + lodGroups.Length);
            Debug.Log("---------------------------------------------");


        }
    }
}