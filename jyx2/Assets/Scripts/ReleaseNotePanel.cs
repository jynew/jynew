using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using Cysharp.Threading.Tasks;
using i18n.TranslatorDef;
using Jyx2.MOD;
using Jyx2.ResourceManagement;
using UnityEngine;
using UnityEngine.UI;

public class ReleaseNotePanel : MonoBehaviour
{
    //public AssetReferenceT<TextAsset> refReleaseNote;
    public Text text;

    public bool IsPlatform = false;
    
    public async UniTask Show()
    {
        //---------------------------------------------------------------------------
        //text.text = "载入中..";
        //---------------------------------------------------------------------------
        //特定位置的翻译【载入中文本显示】
        //---------------------------------------------------------------------------
        text.text = "载入中…… ".GetContent(nameof(ReleaseNotePanel));
        //---------------------------------------------------------------------------
        //---------------------------------------------------------------------------

        if (IsPlatform)
        {
            var t = Resources.Load<TextAsset>("RELEASE_NOTE_PLATFORM");
            text.text = t.text;
        }
        else
        {
            var t = await ResLoader.LoadAsset<TextAsset>("Assets/BuildSource/RELEASE_NOTE.txt");
            text.text = t.text;    
        }
        
    }

    private void OnEnable()
    {
        Show().Forget();
    }
}
