using Cysharp.Threading.Tasks;
using i18n.TranslatorDef;
using Jyx2;
using Jyx2.ResourceManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Jyx2
{
    public enum ReleaseNoteType
    {
        Platform,
        Mod,
    }


    public class ReleaseNotePanel : Jyx2_UIBase
    {
        public Text text;

        public ReleaseNoteType NoteType { get; private set; }

        public async UniTask LoadReleaseNote()
        {
            text.text = "载入中…… ".GetContent(nameof(ReleaseNotePanel));

            if (NoteType == ReleaseNoteType.Platform)
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

        private static bool AlreadyDisplayModNote()
        {
            string key = "RELEASENOTE_MOD_" + RuntimeEnvSetup.CurrentModId + "_" + Application.version;
            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetInt(key, 1);
                PlayerPrefs.Save();
                return false;
            }
            return true;
        }

        private static bool AlreadyDisplayPlatformNote()
        {
            string key = "RELEASENOTE_PLATFORM_" + Application.version;
            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetInt(key, 1);
                PlayerPrefs.Save();
                return false;
            }
            return true;
        }

        public static void ShowReleaseNoteAnyway(ReleaseNoteType noteType = ReleaseNoteType.Platform)
        {
            ShowReleaseNote(noteType);
        }

        public static void ShowReleaseNoteIfPossible(ReleaseNoteType noteType = ReleaseNoteType.Platform)
        {
            bool isPlatform = noteType == ReleaseNoteType.Platform;
            if (isPlatform && AlreadyDisplayPlatformNote())
                return;
            if (!isPlatform && AlreadyDisplayModNote())
                return;
            ShowReleaseNote(noteType);
        }

        private static async void ShowReleaseNote(ReleaseNoteType noteType = ReleaseNoteType.Platform)
        {
            var notePanel = await Jyx2_UIManager.Instance.ShowUIAsync<ReleaseNotePanel>();
            if (notePanel != null)
            {
                notePanel.NoteType = noteType;
                notePanel.LoadReleaseNote().Forget();
            }
        }
    }
}