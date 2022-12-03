using System;
using UnityEngine;

namespace Jyx2
{
    public class PlatformReleaseNoteJudge : MonoBehaviour
    {
        [SerializeField] private GameObject ReleaseNote_Panel;
        
        private void Start()
        {
            JudgeShowReleaseNotePanel();
        }
        
        void JudgeShowReleaseNotePanel()
        {
            //每个更新显示一次 这里就不用Jyx2_PlayerPrefs了
            string key = "RELEASENOTE_" + Application.version;
            if (!PlayerPrefs.HasKey(key))
            {
                ReleaseNote_Panel.gameObject.SetActive(true);
                PlayerPrefs.SetInt(key, 1);
                PlayerPrefs.Save();
            }
        }
    }
}