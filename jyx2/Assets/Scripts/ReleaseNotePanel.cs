using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
        text.text = "载入中..";
        var t = await MODLoader.LoadAsset<TextAsset>(Jyx2ResourceHelper.GetAssetRefAddress(refReleaseNote, typeof(TextAsset)));
        text.text = t.text;
    }

    private void OnEnable()
    {
        Show().Forget();
    }
}
