/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */

using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AudioManager
{
    //JYX2
    public static void PlayMusic(int id)
    {
        PlayMusicAtPath("Assets/BuildSource/Musics/" + id + ".mp3").Forget();
    }

    public static async UniTask PlayMusicAtPath(string path)
    {
        if(_currentPlayMusic == path)
        {
            return;
        }

        var audioClip = await Addressables.LoadAssetAsync<AudioClip>(path).Task;
        if (audioClip != null)
        {
            var audioSource = GetAudioSource();
            audioSource.clip = audioClip;
            audioSource.Play();
            _currentPlayMusic = path;
        }
    }

    static string _currentPlayMusic;

    static AudioSource s_AudioSource = null;

    private static AudioSource GetAudioSource()
    {
        if (s_AudioSource != null)
            return s_AudioSource;

        GameObject obj = new GameObject("[AudioManager]");
        GameObject.DontDestroyOnLoad(obj);
        s_AudioSource = obj.AddComponent<AudioSource>();
        s_AudioSource.loop = true;
        return s_AudioSource;
    }
}
