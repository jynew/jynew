using UnityEngine;

public class AudioManager
{
    //JYX2
    static public void PlayMusic(int id)
    {
        PlayMusicAtPath("Assets/BuildSource/Musics/" + id + ".mp3");
    }

    static public void PlayMusicAtPath(string path)
    {
        if(_currentPlayMusic == path)
        {
            return;
        }

        Jyx2ResourceHelper.LoadAsset<AudioClip>(path, audioClip=> {
            if (audioClip != null)
            {
                var audioSource = GetAudioSource();
                audioSource.clip = audioClip;
                audioSource.Play();
                _currentPlayMusic = path;
            }
        });
    }

    static string _currentPlayMusic;

    static AudioSource s_AudioSource = null;

    static private AudioSource GetAudioSource()
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
