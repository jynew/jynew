using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshCombineStudio
{
    public class LODGroupSetup : MonoBehaviour
    {

        public MeshCombiner meshCombiner;
        public LODGroup lodGroup;
        public int lodGroupParentIndex;
        public int lodCount;

        LODGroup[] lodGroups;

        public void Init(MeshCombiner meshCombiner, int lodGroupParentIndex)
        {
            this.meshCombiner = meshCombiner;
            this.lodGroupParentIndex = lodGroupParentIndex;
            lodCount = lodGroupParentIndex + 1;

            if (lodGroup == null) lodGroup = gameObject.AddComponent<LODGroup>();

            GetSetup();
        }

        void GetSetup()
        {
            LOD[] lods = new LOD[lodGroupParentIndex + 1];

            MeshCombiner.LODGroupSettings lodGroupSettings = meshCombiner.lodGroupsSettings[lodGroupParentIndex];

            lodGroup.animateCrossFading = lodGroupSettings.animateCrossFading;
            lodGroup.fadeMode = lodGroupSettings.fadeMode;

            for (int i = 0; i < lods.Length; i++)
            {
                MeshCombiner.LODSettings lodSettings = lodGroupSettings.lodSettings[i];
                lods[i] = new LOD();
                lods[i].screenRelativeTransitionHeight = lodSettings.screenRelativeTransitionHeight;
                lods[i].fadeTransitionWidth = lodSettings.fadeTransitionWidth;
            }

            lodGroup.SetLODs(lods);
        }

        public void ApplySetup()
        {
            // Debug.Log("ApplySetup");
            LOD[] lods = lodGroup.GetLODs();

            if (lodGroups == null) lodGroups = GetComponentsInChildren<LODGroup>();

            if (lods.Length != lodCount) return;

            bool lodGroupsAreRemoved = false;

            if (lodGroupParentIndex == 0)
            {
                // Debug.Log("Length " + lodGroups.Length +" " +lods[0].screenRelativeTransitionHeight);
                if (lods[0].screenRelativeTransitionHeight != 0)
                {
                    if (lodGroups == null || lodGroups.Length == 1) AddLODGroupsToChildren();
                }
                else
                {
                    if (lodGroup != null && lodGroups.Length != 1) RemoveLODGroupFromChildren();
                    lodGroupsAreRemoved = true;
                }
            }

            if (meshCombiner != null)
            {
                for (int i = 0; i < lods.Length; i++)
                {
                    meshCombiner.lodGroupsSettings[lodGroupParentIndex].lodSettings[i].screenRelativeTransitionHeight = lods[i].screenRelativeTransitionHeight;
                }
            }

            if (lodGroupsAreRemoved) return;

            for (int i = 0; i < lodGroups.Length; i++)
            {
                LOD[] childLods = lodGroups[i].GetLODs();

                lodGroups[i].animateCrossFading = lodGroup.animateCrossFading;
                lodGroups[i].fadeMode = lodGroup.fadeMode;

                for (int j = 0; j < childLods.Length; j++)
                {
                    if (j >= childLods.Length || j >= lods.Length) continue;
                    childLods[j].screenRelativeTransitionHeight = lods[j].screenRelativeTransitionHeight;
                    childLods[j].fadeTransitionWidth = lods[j].fadeTransitionWidth;
                }

                // The situation where childLods.Length != lods.Length can occur when a cell has no children
                // if the lengths are the same and SetLODs is called, the error
                //     SetLODs: Attempting to set LOD where the screen relative size is greater then or equal to a higher detail LOD level.
                // is thrown (and apparently can't be caught)
                if (childLods.Length == lods.Length)
                {
                    try
                    {
                        lodGroups[i].SetLODs(childLods);
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"ApplySetup error in {lodGroups[i].name}: {e.Message}");
                    }
                }
            }

            if (meshCombiner != null) lodGroup.size = meshCombiner.cellSize;
        }

        public void AddLODGroupsToChildren()
        {
            // Debug.Log("Add Lod Groups");
            Transform t = transform;
            List<LODGroup> lodGroupList = new List<LODGroup>();

            for (int i = 0; i < t.childCount; i++)
            {
                Transform child = t.GetChild(i);
                // Debug.Log(child.name);
                LODGroup lodGroup = child.GetComponent<LODGroup>();

                if (lodGroup == null)
                {
                    lodGroup = child.gameObject.AddComponent<LODGroup>();
                    LOD[] lods = new LOD[1];
                    lods[0] = new LOD(0, child.GetComponentsInChildren<MeshRenderer>());
                    lodGroup.SetLODs(lods);
                }

                lodGroupList.Add(lodGroup);
            }

            lodGroups = lodGroupList.ToArray();
        }

        public void RemoveLODGroupFromChildren()
        {
            // Debug.Log("Remove Lod Groups");
            Transform t = transform;

            for (int i = 0; i < t.childCount; i++)
            {
                Transform child = t.GetChild(i);
                LODGroup lodGroup = child.GetComponent<LODGroup>();
                if (lodGroup != null) DestroyImmediate(lodGroup);
            }

            lodGroups = null;
        }
    }
}
