using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Jyx2;
using Jyx2.MOD;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using ModEntry = Jyx2.MOD.MODManager.ModEntry;


public class ModItem : MonoBehaviour
{
    public static ModItem Create()
    {
        var obj = Jyx2ResourceHelper.CreatePrefabInstance("ModItem");
        var modItem = obj.GetComponent<ModItem>();
        modItem.InitTrans();
        return modItem;
    }
    
    Toggle m_Toggle;
    Image m_Image;
    Text m_Info;
    Text m_Status;
    Button m_Download;
    Button m_Delete;
    
    void InitTrans() 
    {
        m_Toggle = transform.Find("Toggle").GetComponent<Toggle>();
        m_Image = transform.Find("Image").GetComponent<Image>();
        m_Info = transform.Find("Info").GetComponent<Text>();
        m_Status = transform.Find("Status").GetComponent<Text>();
        m_Download = transform.Find("Download").GetComponent<Button>();
        m_Delete = transform.Find("Delete").GetComponent<Button>();
    }

    public async UniTask ShowMod(ModEntry modEntry)
    {

        m_Info.text = "信息";
        m_Status.text = "状态";
        m_Download.onClick.AddListener(() =>
        {
            DoDownload(modEntry);
        });
        m_Delete.onClick.AddListener(() =>
        {
            DoDelete(modEntry.Path);
        });
        await new DownloadManager().DownloadSprite(modEntry.ModMeta.poster, sprite =>
        {
            m_Image.sprite = sprite;
        });
    }

    async void DoDownload(ModEntry modEntry)
    {
        await new DownloadManager().DownloadFile(modEntry.ModMeta.uri, modEntry.Path);
    }

    void DoDelete(string filePath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);
    }
}
