/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using Jyx2;
using Jyx2.Middleware;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Keiwando.NFSO;
using UnityEngine.SceneManagement;

public partial class SavePanel:Jyx2_UIBase
{
	
#if UNITY_STANDALONE_WIN
	private const string testDirectory = @"C:\Users";
#elif UNITY_STANDALONE_OSX
	private const string testDirectory = @"~/Desktop";
#else
	private const string testDirectory = "";
#endif

    public override UILayer Layer => UILayer.NormalUI;

    Action<int> m_selectCallback;
    protected override void OnCreate()
    {
        InitTrans();

        BindListener(BackButton_Button, OnBackClick);
		BindListener(ImButton_Button, OnImportClick);
		BindListener(ExButton_Button, OnExportClick);
    }
    
    private void OnEnable()
    {
	    GlobalHotkeyManager.Instance.RegistHotkey(this, KeyCode.Escape, OnBackClick);
    }

    private void OnDisable()
    {
	    GlobalHotkeyManager.Instance.UnRegistHotkey(this, KeyCode.Escape);
    }

    protected override void OnShowPanel(params object[] allParams)
    {
        base.OnShowPanel(allParams);

        m_selectCallback = allParams[0] as Action<int>;
		Main_Text.text=allParams[1] as string;
		
		var curScene=SceneManager.GetActiveScene().name;
		var isHouse=(curScene!="0_GameStart" && curScene!="0_BigMap");
		(ImButton_Button.gameObject).SetActive(!isHouse);
		(ExButton_Button.gameObject).SetActive(!isHouse);
        RefreshSave();
    }

    void RefreshSave() 
    {
        HSUnityTools.DestroyChildren(SaveParent_RectTransform);

        for (int i = 0; i < GameConst.SAVE_COUNT; i++)
        {
            var btn = Instantiate(SaveItem_Button);
            btn.transform.SetParent(SaveParent_RectTransform);
            btn.transform.localScale = Vector3.one;
            btn.name = i.ToString();
            Text title = btn.transform.Find("Title").GetComponent<Text>();
            title.text = "存档" + GameConst.GetUPNumber(i+1);

            var txt = btn.transform.Find("SummaryText").GetComponent<Text>();
            var summaryInfoKey = GameRuntimeData.ARCHIVE_SUMMARY_PREFIX + i;
            if (PlayerPrefs.HasKey(summaryInfoKey))
            {
                txt.text = PlayerPrefs.GetString(summaryInfoKey);
            }
            else
            {
                txt.text = "空档位";
            }

            BindListener(btn, () =>
            {
                OnSaveItemClick(btn);
            });
        }
    }

    protected override void OnHidePanel()
    {
        base.OnHidePanel();

        HSUnityTools.DestroyChildren(SaveParent_RectTransform);
    }

    void OnSaveItemClick(Button btn) 
    {
        Action<int> cb = m_selectCallback;
        Jyx2_UIManager.Instance.HideUI(nameof(SavePanel));
        cb?.Invoke(int.Parse(btn.name));
    }

    private void OnBackClick()
    {
        Jyx2_UIManager.Instance.HideUI(nameof(SavePanel));
    }

    private void OnImportClick()
    {
		#if UNITY_ANDROID
			for(int i=0;i<3;i++){
				string fileName = string.Format(GameRuntimeData.ARCHIVE_FILE_NAME, i);
				string sFolderPath = Application.persistentDataPath + "/" + GameRuntimeData.ARCHIVE_FILE_DIR;
				string sPath = sFolderPath + "/" + fileName;
				if (File.Exists(sPath))
				{
					PlayerPrefs.SetString(GameRuntimeData.ARCHIVE_SUMMARY_PREFIX +i, "import  save data: "+i);
				}
			}
			//NativeFileSOMobile.shared.OpenFile(new SupportedFileType[]{SupportedFileType.Any}, delegate (bool didSelectPath, OpenedFile file) {
			//	if (didSelectPath) {
			//		string sFolderPath = Application.persistentDataPath + "/" + GameRuntimeData.ARCHIVE_FILE_DIR+"/"+file.Name;
			//		NativeFileSOMobile.shared.SaveFile(new FileToSave(sFolderPath, file+".dat"));
			//	}
			//	PlayerPrefs.SetString(ARCHIVE_SUMMARY_PREFIX + SaveIndex, summaryInfo);
			//	RefreshSave();
			//});
		#else
			NativeFileSOMacWin.shared.SelectSavePath(GetFileToSave(), "", testDirectory, delegate (bool didSelectPath, string savePath) {
				if (didSelectPath) {
					string sFolderPath = Application.persistentDataPath + "/" + GameRuntimeData.ARCHIVE_FILE_DIR+"/";
					int SaveIndex=-1;
					if (File.Exists(savePath)&&savePath.Contains("archive_")&&savePath.Contains(".dat"))
					{
						var lines=savePath.Split('\\');
						File.Copy(savePath,sFolderPath+lines[lines.Length-1]);
						try{
							SaveIndex=int.Parse(savePath.Substring(savePath.Length-5,1));
						}catch{}
					}
					if(SaveIndex>-1){
						PlayerPrefs.SetString(GameRuntimeData.ARCHIVE_SUMMARY_PREFIX +SaveIndex, "import  save data: "+SaveIndex);
					}
				}
			});
		#endif
		RefreshSave();
    }
	
    private void OnExportClick()
    {
		#if UNITY_ANDROID
			transform.Find("FileIO/Export/Text").GetComponent<Text>().text="暂不支持";
		#else
			NativeFileSOMacWin.shared.SelectSavePath(GetFileToSave(), "", testDirectory, delegate (bool didSelectPath, string savePath) {
				if (didSelectPath) {
					ExportSaveData(savePath);
				}
			});
		#endif
    }

	private FileToSave GetFileToSave() 
	{
		var testFilePath = Application.persistentDataPath;
		return new FileToSave(testFilePath, "archive_{0}.dat", SupportedFileType.Any);
	}
	
	private void ExportSaveData(string savePath)
	{
		for(int i=0;i<3;i++){
			string fileName = string.Format(GameRuntimeData.ARCHIVE_FILE_NAME, i);
			string sFolderPath = Application.persistentDataPath + "/" + GameRuntimeData.ARCHIVE_FILE_DIR;
			string sPath = sFolderPath + "/" + fileName;
			if (File.Exists(sPath))
			{
				File.Copy(sPath,string.Format(savePath, i));
			}
		}
	}
}
