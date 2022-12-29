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
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using i18n.TranslatorDef;
using Jyx2;
using NUnit.Framework;
using Sirenix.OdinInspector;
using UnityEngine;

public class SecretNav : MonoBehaviour
{
	[InfoBox("默认为出隐秘区域触发器")]
	[LabelText("进入隐秘区域")] 
	public bool IsEnterSecret;

	private async void Start()
	{
		await RuntimeEnvSetup.Setup();
		triggerEnabled = true;
	}

	private bool triggerEnabled = false;

        Jyx2Player player
        {
            get
            {
                if (_player == null)
                {
                    LevelMaster lm = LevelMaster.Instance;
                    _player = lm.GetPlayer();
                }
                return _player;
            }
        }

        Jyx2Player _player;

	async void OnTriggerEnter(Collider other)
	{
		if (!triggerEnabled) return;
		
                if (IsEnterSecret)
                {
                    player.GetInSecret();
                }
                else
                {
                    player.GetOutSecret();
                }

	}

	void OnTriggerExit(Collider other)
	{
            return;
	}

}
