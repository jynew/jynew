using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshCombineStudio 
{
	public class SelectOriginal : MonoBehaviour 
	{
		public MeshCombiner meshCombiner;
		public Camera mainCamera;
		public Material matSelect;

		Material oldMat;
		Vector3 oldPos;
		MeshRenderer oldMr;

		// This Example script will deactive the combined mesh renderers of the cell and activate the originals.
		// When deselecting it will switch back to enabled combined mesh renderers and disabled originals.

		void Update() 
		{
			RaycastHit hit;
			Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

			if (Input.GetKeyDown(KeyCode.Tab)) Deselect(); // Deselect because Tab switches MCS ON/OFF

			if (Physics.Raycast(ray, out hit))
			{
				Transform hitT = hit.transform;
				var mr = hitT.GetComponent<MeshRenderer>();

				if (mr != null)
				{
					if (mr == oldMr) return;
					Deselect();

					oldMr = mr;
					if (meshCombiner.searchOptions.objectCenter == MeshCombiner.ObjectCenter.BoundsCenter) oldPos = oldMr.bounds.center; else oldPos = hitT.position;
					oldMat = oldMr.sharedMaterial;
					SelectOrDeselect(oldPos, oldMr, matSelect, true);
				}
			}
			else 
			{
				Deselect();
            }
		}

		void Deselect()
        {
			if (oldMr != null) SelectOrDeselect(oldPos, oldMr, oldMat, false);
		}

		void SelectOrDeselect(Vector3 position, MeshRenderer mr, Material mat, bool select)
		{
			var octree = meshCombiner.octree;

			if (octree == null) return;

			ObjectOctree.MaxCell cell = octree.GetCell(position);

			if (cell == null) return;

			mr.sharedMaterial = mat;

			if (meshCombiner.activeOriginal) return;

			ObjectOctree.LODParent[] lodParents = cell.lodParents;

			for (int i = 0; i < lodParents.Length; i++)
            {
				ObjectOctree.LODParent lodParent = lodParents[i];
				if (lodParent == null) continue;

				ObjectOctree.LODLevel[] lodLevels = lodParent.lodLevels;

				for (int j = 0; j < lodLevels.Length; j++)
                {
					ObjectOctree.LODLevel lodLevel = lodLevels[j];

					if (lodLevel == null) continue;

					Methods.SetMeshRenderersActive(lodLevel.newMeshRenderers, !select);
					Methods.SetCachedGOSActive(lodLevel.cachedGOs, select);
                }
            }
		}
	}
}
