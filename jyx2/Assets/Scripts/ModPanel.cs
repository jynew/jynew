using System.Collections;
using System.Collections.Generic;
using Jyx2.Middleware;
using Jyx2.MOD;
using UnityEngine;

public class ModPanel : MonoBehaviour
{
    private RectTransform ModParent_RectTransform;

    public void InitTrans()
    {
        ModParent_RectTransform = transform.Find("ModScroll/Viewport/ModParent").GetComponent<RectTransform>();

    }

    void Start()
    {
        InitTrans();
        RefreshScroll();
    }

    void RefreshScroll()
    {
        HSUnityTools.DestroyChildren(ModParent_RectTransform);
        foreach (var modEntry in MODManager.ModEntries)
        {
            var item = ModItem.Create();
            item.transform.SetParent(ModParent_RectTransform);
            item.transform.localScale = Vector3.one;
            
            item.ShowMod(modEntry);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
