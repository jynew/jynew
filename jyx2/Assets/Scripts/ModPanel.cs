using System.Collections;
using System.Collections.Generic;
using Jyx2.Middleware;
using Jyx2.MOD;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ModPanel : MonoBehaviour
{
    Button StartButton;
    RectTransform ModParent_RectTransform;

    void InitTrans()
    {
        ModParent_RectTransform = transform.Find("ModScroll/Viewport/ModParent").GetComponent<RectTransform>();
        StartButton = transform.Find("StartButton").GetComponent<Button>();
        
        StartButton.onClick.AddListener(onStart);
    }

    async void onStart()
    {
        await MODLoader.Init();
        SceneManager.LoadScene("0_MainMenu");
    }

    void Start()
    {
        InitTrans();
        RefreshScroll();
    }

    async void RefreshScroll()
    {
        HSUnityTools.DestroyChildren(ModParent_RectTransform);
        foreach (var modEntry in MODManager.ModEntries)
        {
            var item = ModItem.Create();
            Transform transform1;
            (transform1 = item.transform).SetParent(ModParent_RectTransform);
            transform1.localScale = Vector3.one;
            
            await item.ShowMod(modEntry);
        }
    }

    void Update()
    {
        
    }
}
