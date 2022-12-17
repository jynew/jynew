/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Jyx2;
using Jyx2.Middleware;
using Jyx2.MOD;
using MOD.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class GameStart : MonoBehaviour
{
	public CanvasGroup introPanel;

	void Start()
	{
		//修复存档，下个版本删掉(202306之前)
		FixSaves();

		StartAsync().Forget();
	}


	void FixSaves()
	{

#if true
		try
		{
			if (!PlayerPrefs.HasKey("save_fixed_20221204_1"))
			{
				DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
				if (!dir.Exists)
				{
					dir.Create();
				}

				var movedDir = Path.Combine(dir.Parent.FullName, "wuxia_launch");
				if (Directory.Exists(movedDir))
				{
					CopyDirectory(movedDir, dir.FullName); //迁移存档
				}
			
				PlayerPrefs.SetInt("save_fixed_20221204_1", 1);
				PlayerPrefs.Save();
			}
		}
		catch (Exception e)
		{
			Debug.LogError(e.ToString());
		}
#endif
	}
	
	static void CopyDirectory(string srcDir, string tgtDir)
	{
		DirectoryInfo source = new DirectoryInfo(srcDir);
		DirectoryInfo target = new DirectoryInfo(tgtDir);

		if (target.FullName.StartsWith(source.FullName, StringComparison.CurrentCultureIgnoreCase))
		{
			throw new Exception("父目录不能拷贝到子目录！");
		}

		if (!source.Exists)
		{
			return;
		}

		if (!target.Exists)
		{
			target.Create();
		}

		FileInfo[] files = source.GetFiles();

		for (int i = 0; i < files.Length; i++)
		{
			File.Copy(files[i].FullName, target.FullName + @"\" + files[i].Name, true);
		}

		DirectoryInfo[] dirs = source.GetDirectories();

		for (int j = 0; j < dirs.Length; j++)
		{
			CopyDirectory(dirs[j].FullName, target.FullName + @"\" + dirs[j].Name);
		}
	}

	async UniTask StartAsync()
	{
		introPanel.gameObject.SetActive(true);

		introPanel.alpha = 0;
		await introPanel.DOFade(1, 1f).SetEase(Ease.Linear);
		await UniTask.Delay(TimeSpan.FromSeconds(1f));
		await introPanel.DOFade(0, 1f).SetEase(Ease.Linear).OnComplete(() =>
		{
			Destroy(introPanel.gameObject);
		});

		Application.logMessageReceived += OnErrorMsg;

		ModPanelNew.SwitchSceneTo();
	}

	private void OnErrorMsg(string condition, string stackTrace, LogType logType)
	{
		if (logType == LogType.Exception)
		{
			UnityEngine.Debug.LogWarningFormat("Exception版本:{0}, 触发时间:{1}", Application.version, DateTime.Now);
		}
		else if (logType == LogType.Error)
		{
			UnityEngine.Debug.LogWarningFormat("Error版本:{0}, 触发时间:{1}", Application.version, DateTime.Now);
		}
	}
}
