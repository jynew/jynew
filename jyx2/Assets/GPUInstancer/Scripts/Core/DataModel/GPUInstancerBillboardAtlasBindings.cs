using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace GPUInstancer
{
    public class GPUInstancerBillboardAtlasBindings : ScriptableObject
    {
        public List<BillboardAtlasBinding> billboardAtlasBindings;

        public BillboardAtlasBinding GetBillboardAtlasBinding(GameObject prefab, int atlasResolution, int frameCount)
        {
            foreach (BillboardAtlasBinding bab in billboardAtlasBindings)
            {
                if (bab.prefab == prefab && bab.atlasResolution == atlasResolution && bab.frameCount == frameCount)
                    return bab;
            }
            return null;
        }

        public void ResetBillboardAtlases()
        {
            if (billboardAtlasBindings == null)
                billboardAtlasBindings = new List<BillboardAtlasBinding>();
            else
                billboardAtlasBindings.Clear();

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        public void ClearEmptyBillboardAtlases()
        {
            if (billboardAtlasBindings != null)
            {
#if UNITY_EDITOR             
                bool modified = billboardAtlasBindings.RemoveAll(bab => bab == null || bab.prefab == null || bab.albedoAtlasTexture == null || bab.normalAtlasTexture == null) > 0;
                if (modified)
                    EditorUtility.SetDirty(this);
#endif
            }
        }

        public void AddBillboardAtlas(GameObject prefab, int atlasResolution, int frameCount, Texture2D albedoAtlasTexture, Texture2D normalAtlasTexture, float quadSize, float yPivotOffset)
        {
            billboardAtlasBindings.Add(new BillboardAtlasBinding(prefab, atlasResolution, frameCount, albedoAtlasTexture, normalAtlasTexture, quadSize, yPivotOffset));
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        public void RemoveBillboardAtlas(BillboardAtlasBinding billboardAtlasBinding)
        {
            billboardAtlasBindings.Remove(billboardAtlasBinding);
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        public bool IsBillboardAtlasExists(GameObject prefab)
        {
            foreach (BillboardAtlasBinding bab in billboardAtlasBindings)
            {
                if (bab.prefab == prefab)
                    return true;
            }
            return false;
        }
        
        public bool DeleteBillboardTextures(GPUInstancerPrototype selectedPrototype)
        {
            bool billboardsDeleted = false;
#if UNITY_EDITOR
            if (selectedPrototype.billboard != null && selectedPrototype.billboard.albedoAtlasTexture != null)
            {
                BillboardAtlasBinding billboardAtlasBinding = GetBillboardAtlasBinding(selectedPrototype.prefabObject, selectedPrototype.billboard.atlasResolution,
                        selectedPrototype.billboard.frameCount);

                if (billboardAtlasBinding != null)
                {
                    if (selectedPrototype.isBillboardDisabled
                        || (selectedPrototype is GPUInstancerDetailPrototype && !((GPUInstancerDetailPrototype)selectedPrototype).usePrototypeMesh)
                        || EditorUtility.DisplayDialog(
                        GPUInstancerConstants.TEXT_deleteConfirmation, GPUInstancerConstants.TEXT_deleteBillboard + "\n\"" + selectedPrototype.ToString() + "\"",
                        GPUInstancerConstants.TEXT_delete, GPUInstancerConstants.TEXT_keepTextures))
                    {
                        RemoveBillboardAtlas(billboardAtlasBinding);
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(billboardAtlasBinding.albedoAtlasTexture));
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(billboardAtlasBinding.normalAtlasTexture));
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        billboardsDeleted = true;
                    }
                }
            }
#endif
            return billboardsDeleted;
        }
    }

    [Serializable]
    public class BillboardAtlasBinding
    {
        public GameObject prefab;
        public int atlasResolution;
        public int frameCount;
        public Texture2D albedoAtlasTexture;
        public Texture2D normalAtlasTexture;
        public float quadSize;
        public float yPivotOffset;
        public string modifiedDate;

        public BillboardAtlasBinding(GameObject prefab, int atlasResolution, int frameCount,  Texture2D albedoAtlasTexture, Texture2D normalAtlasTexture, float quadSize, float yPivotOffset)
        {
            this.prefab = prefab;
            this.atlasResolution = atlasResolution;
            this.frameCount = frameCount;
            this.albedoAtlasTexture = albedoAtlasTexture;
            this.normalAtlasTexture = normalAtlasTexture;
            this.quadSize = quadSize;
            this.yPivotOffset = yPivotOffset;
            this.modifiedDate = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff",
                                System.Globalization.CultureInfo.InvariantCulture);
        }
    }

}