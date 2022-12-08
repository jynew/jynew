using System.IO;
using System.Text;

using Jyx2.MOD.ModV2;
using UnityEngine;

#if UNITY_ANDROID
using AndroidAuxiliary.Plugins.Auxiliary.AndroidBridge;
using UnityEngine.Android;
#endif
using UnityEngine.UI;

namespace MOD.UI
{
    public class AddModPanelTemp : MonoBehaviour
    {
        [SerializeField] private Button m_OpenModDirBtn;
        [SerializeField] private Button m_OkBtn;
        [SerializeField] private Text m_ModDirText;
        [SerializeField] private Button m_CopyBtn;

        void Start()
        {
            var loader = new GameModManualInstalledLoader();
            
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < loader.ManualInstalledDir.Length; i++)
            {
                sb.AppendLine(string.Format("目录{0}: {1}", i+1, loader.ManualInstalledDir[i].Replace("\\","/")));
            }
            //m_ModDirText.text = string.Join("\n", loader.ManualInstalledDir);
            m_ModDirText.text = sb.ToString();
            
            m_OkBtn.onClick.AddListener(() =>
            {
                this.gameObject.SetActive(false);
            });
            
            m_OpenModDirBtn.onClick.AddListener(() =>
            {
                string path = loader.ManualInstalledDir[0];
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                Application.OpenURL(path);
            });
            
            m_CopyBtn.onClick.AddListener(() =>
            {
                string path = loader.ManualInstalledDir[0];
                GUIUtility.systemCopyBuffer = path;
            });
            
            gameObject.SetActive(false);
        }


        public void Show()
        {
            gameObject.SetActive(true);
            RequestUserPermission();
        }

        public void RequestUserPermission()
        {
#if UNITY_ANDROID
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            Permission.RequestUserPermission("android.permission.MANAGE_EXTERNAL_STORAGE");
            // 获取文件权限
            AndroidTools.GetFileAccessPermission();
#endif
        }
    }
}
