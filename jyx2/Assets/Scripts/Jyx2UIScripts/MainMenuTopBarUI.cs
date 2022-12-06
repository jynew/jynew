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
    }
}