using System.IO;
using Cysharp.Threading.Tasks;
using Jyx2.MOD;
using UnityEngine;
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

    private Downloader _loader;
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

        _loader = new Downloader();
        _loader.ResourceLoadingCompleted += OnDownloadCompleted;
    }

    public async UniTask ShowMod(ModEntry modEntry)
    {
        if (!File.Exists(modEntry.Path))
        {
            m_Status.text = "<color=grey>缺失</color>";
            m_Toggle.gameObject.SetActive(false);
            m_Download.gameObject.SetActive(true);
            m_Delete.gameObject.SetActive(false);
            m_Progress.gameObject.SetActive(false);
        }
        else
        {
            m_Status.text = "<color=green>正常</color>";
            m_Toggle.gameObject.SetActive(true);
            m_Download.gameObject.SetActive(false);
            m_Delete.gameObject.SetActive(true);
            m_Progress.gameObject.SetActive(false);
        }
        
        m_Name.text = modEntry.ModMeta.name + "V" + modEntry.ModMeta.version;
        m_Desc.text = modEntry.ModMeta.description;
        m_Toggle.isOn = modEntry.Active;
        m_Toggle.onValueChanged.AddListener((isOn) =>
        {
            modEntry.Active = isOn;
        });
        m_Download.onClick.AddListener(() =>
        {
            DoDownload(modEntry);
        });
        m_Delete.onClick.AddListener(() =>
        {
            DoDelete(modEntry);
        });
        m_Image.sprite = await new Downloader().DownloadSprite(modEntry.ModMeta.poster);
    }

    async void DoDownload(ModEntry modEntry)
    {
        await _loader.DownloadFile(modEntry.ModMeta.uri, modEntry.Path);
    }

    void DoDelete(ModEntry modEntry)
    {
        if (modEntry.Active)
        {
            modEntry.Active = false;
        }
        if (File.Exists(modEntry.Path))
        {
            File.Delete(modEntry.Path);
            Debug.Log("File successfully deleted");
            m_Status.text = "<color=grey>缺失</color>";
            m_Toggle.gameObject.SetActive(false);
            m_Download.gameObject.SetActive(true);
            m_Delete.gameObject.SetActive(false);
            m_Progress.gameObject.SetActive(false);
        }
    }
    
    void Update()
    {
        // 如果DownloadProgress为-1则说明下载已经完成了，不要再刷新下载进度。
        if (_loader.DownloadProgress < 0) return;
        // 在下载过程中可以访问loader，得到下载进度。
        m_Progress.gameObject.SetActive(true);
        Debug.Log(_loader.DownloadProgress);
        m_Progress.text = (int)(_loader.DownloadProgress * 100) + "%";
    }

    void OnDownloadCompleted(object sender, ResourceLoadCompletedEventArgs args)
    {
        m_Status.text = "<color=green>正常</color>";
        m_Toggle.gameObject.SetActive(true);
        m_Download.gameObject.SetActive(false);
        m_Delete.gameObject.SetActive(true);
        m_Progress.text = 100 + "%";
    }
}
