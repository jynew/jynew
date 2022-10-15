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
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Jyx2;
using Jyx2.Middleware;
using Jyx2.MOD;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class GameStart : MonoBehaviour
{
	public CanvasGroup introPanel;

	void Start()
	{
		StartAsync().Forget();
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

		//await MODManager.Init();
		SceneManager.LoadScene("0_MainMenu");
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
