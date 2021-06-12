using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JYX2Outline : MonoBehaviour
{
    class OutlinePara
    {
        public Color color;
        public float width;
    }
    Dictionary<GameObject, OutlinePara[]> originalMats;
    bool originalMatsSaved = false;
    private void Awake()
    {
    }

    bool IsShaderValid(Material mat)
    {
        return mat.shader.name == "SkillEffect/Character" || mat.shader.name == "MK/Toon/Free";
    }

    Color GetMaterialColor(Material mat)
    {
        if (mat.shader.name == "SkillEffect/Character")
            return mat.GetColor("_OutColor");
        else if (mat.shader.name == "MK/Toon/Free")
            return mat.GetColor("_OutlineColor");
        return Color.white;
    }

    void SetMaterialColor(Material mat,Color color)
    {
        if(mat.shader.name == "SkillEffect/Character")
            mat.SetColor("_OutColor", color);
        else if (mat.shader.name == "MK/Toon/Free")
            mat.SetColor("_OutlineColor", color);
    }

    float GetMaterialOutlineWidth(Material mat)
    {
        if (mat.shader.name == "SkillEffect/Character")
            return mat.GetFloat("_Outline");
        else if (mat.shader.name == "MK/Toon/Free")
            return mat.GetFloat("_OutlineSize");
        return 0f;
    }

    void SetMaterialOutlineWidth(Material mat, float width)
    {
        if (mat.shader.name == "SkillEffect/Character")
            mat.SetFloat("_Outline", width);
        else if (mat.shader.name == "MK/Toon/Free")
            mat.SetFloat("_OutlineSize", width);
    }

    public void SetOutlineProperty(Color color,float width)
    {
        var renderers = GetComponentsInChildren<Renderer>();
        if(!originalMatsSaved)
        {
            originalMats = new Dictionary<GameObject, OutlinePara[]>();
        }
        
        foreach (var renderer in renderers)
        {
            var mats = renderer.materials;
            if (mats == null || mats.Length == 0) continue;

            if (!originalMatsSaved)
            {
                originalMats[renderer.gameObject] = new OutlinePara[mats.Length];
            }
            
            for(int i=0;i<mats.Length;++i)
            {
                var mat = mats[i];
                if (!IsShaderValid(mat)) continue;


                if (!originalMatsSaved)
                {
                    OutlinePara p = new OutlinePara();
                    originalMats[renderer.gameObject][i] = p;
                    p.color = GetMaterialColor(mat);
                    p.width = GetMaterialOutlineWidth(mat);
                }

                SetMaterialColor(mat, color);
                //SetMaterialOutlineWidth(mat, width);

                mats[i] = mat;
            }
            renderer.materials = mats;
        }
        originalMatsSaved = true;
    }

    private void OnDestroy()
    {
        //还原所有的shader
        if (originalMats == null) return;

        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            if(originalMats.ContainsKey(renderer.gameObject))
            {
                Material[] mats = renderer.sharedMaterials;
                for (int i = 0; i < mats.Length; ++i)
                {
                    var p = originalMats[renderer.gameObject][i];
                    if (p == null) continue;
                    SetMaterialColor(mats[i], p.color);
                    SetMaterialOutlineWidth(mats[i], p.width);
                }
                //renderer.materials = mats;
            }
        }
    }
}
