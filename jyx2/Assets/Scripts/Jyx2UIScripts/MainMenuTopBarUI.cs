using System;
using Jyx2.Middleware;
using UnityEngine;
using UnityEngine.UI;

namespace Jyx2
{
    public class MainMenuTopBarUI : MonoBehaviour
    {
        [SerializeField] private Button githubBtn;
        [SerializeField] private Button contributorsBtn;
        [SerializeField] private Button issuesBtn;
        [SerializeField] private Button shameListBtn;
        [SerializeField] private Button bilibiliBtn;
        [SerializeField] private Button releaseNoteBtn;
        [SerializeField] private Button donateBtn;

        private void OnEnable()
        {
            ReleaseNotePanel.ShowReleaseNoteIfPossible();
        }

        public void OnReleaseNoteBtnClick()
        {
            ReleaseNotePanel.ShowReleaseNoteAnyway();
        }
        
        public void OnOpenURL(string url)
        {
            Tools.openURL(url);
        }


        private void Start()
        {
            //知大虾 20221206，版号合规性问题，避免纠纷，在移动端关闭捐助按钮。
            if (Application.isMobilePlatform)
            {
                donateBtn.gameObject.SetActive(false);
            }
        }
    }
}