using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using i18n.TranslatorDef;
using Jyx2.MOD;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class ReleaseNotePanel : MonoBehaviour
{
    public AssetReferenceT<TextAsset> refReleaseNote;
    public Text text;
    
    public async UniTask Show()
    {
        //---------------------------------------------------------------------------
        //text.text = "载入中..";
        //---------------------------------------------------------------------------
        //特定位置的翻译【载入中文本显示】
        //---------------------------------------------------------------------------
        text.text = "载入中.. ".GetContent(nameof(ReleaseNotePanel));
        //---------------------------------------------------------------------------
        //---------------------------------------------------------------------------

        var t = await MODLoader.LoadAsset<TextAsset>(Jyx2ResourceHelper.GetAssetRefAddress(refReleaseNote, typeof(TextAsset)));
        text.text = t.text;
    }

    private void OnEnable()
    {
        Show().Forget();
    }

	private void Update()
	{
		if (GamepadHelper.IsCancel()
            || GamepadHelper.IsConfirm())
		{
            Jyx2_UIManager.Instance.HideUI(nameof(ReleaseNotePanel));
        }
    }
}
