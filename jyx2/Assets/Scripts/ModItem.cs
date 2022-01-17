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
        var prefab = Resources.Load<GameObject>("ModItem");
        var obj = Instantiate(prefab);
        var modItem = obj.GetComponent<ModItem>();
        modItem.InitTrans();
        return modItem;
    }
    
    Toggle m_Toggle;
    Image m_Image;
    Text m_Name;
    Text m_Desc;
    Text m_Status;
    Button m_Download;
    Button m_Delete;
    Text m_Progress;
    
    void InitTrans() 
    {
        m_Toggle = transform.Find("Toggle").GetComponent<Toggle>();
        m_Image = transform.Find("Image").GetComponent<Image>();
        m_Name = transform.Find("Name").GetComponent<Text>();
        m_Desc = transform.Find("Desc").GetComponent<Text>();
        m_Status = transform.Find("Status").GetComponent<Text>();
        m_Download = transform.Find("Download").GetComponent<Button>();
        m_Delete = transform.Find("Delete").GetComponent<Button>();
        m_Progress = transform.Find("Progress").GetComponent<Text>();
    }

    public async UniTask ShowMod(ModEntry modEntry)
    {

        m_Name.text = modEntry.ModMeta.name + " Version." + modEntry.ModMeta.version;
        m_Desc.text = modEntry.ModMeta.description;
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
        await new DownloadManager().DownloadFile(modEntry.ModMeta.uri, modEntry.Path, (progress) =>
        {
            m_Progress.gameObject.SetActive(true);
            m_Progress.text = (int)(progress * 100) + "%";
        });
        m_Download.gameObject.SetActive(false);
        m_Delete.gameObject.SetActive(true);
    }

    void DoDelete(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("File successfully deleted");
            m_Download.gameObject.SetActive(true);
            m_Delete.gameObject.SetActive(false);
            m_Progress.gameObject.SetActive(false);
        }
    }
}
