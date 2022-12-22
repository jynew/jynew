using System;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;
using Unity.SharpZipLib.Utils;
using System.IO;
using Jyx2.MOD.ModV2;
using System.Text;
using Jyx2.Middleware;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Threading;
using Jyx2;
using DG.Tweening;

namespace MOD.UI
{
    struct ModDownloadResult
    {
        public bool isSuccess { get; set; }
        public string ModDownloadPath { get; set; }
    }


    public class AddModPanel : MonoBehaviour
    {
        [SerializeField] private InputField m_UrlInputField;
        [SerializeField] private Button m_OkButton;
        [SerializeField] private Button m_CancelButton;
        [SerializeField] private Button m_ClipboardButton;
        [SerializeField] private Button m_FilePickerButton;
        [SerializeField] private Text m_LoggerText;

        private StringBuilder m_LogMsgBuilder = new StringBuilder();

        private UnityWebRequest m_CurrentRequest;

        private CancellationTokenSource m_DisableCancellation;
        

        private bool _isAddingMod = false;

        private void Awake()
        {
            m_OkButton.onClick.AddListener(OnOkButtonClick);
            m_CancelButton.onClick.AddListener(OnCancelButtonClick);
            m_ClipboardButton.onClick.AddListener(OnClipboardButtonClick);
            m_FilePickerButton.onClick.AddListener(OnFilePickerButtonClick);
        }

        private void OnDestroy()
        {
            m_OkButton.onClick.RemoveListener(OnOkButtonClick);
            m_CancelButton.onClick.RemoveListener(OnCancelButtonClick);
            m_ClipboardButton.onClick.RemoveListener(OnClipboardButtonClick);
            m_FilePickerButton.onClick.RemoveListener(OnFilePickerButtonClick);
        }

        private void OnEnable()
        {
            AllocateNewCancellation();
        }

        private void OnDisable()
        {
            TryCancelToken();
        }

        
        private void AllocateNewCancellation()
        {
            if (m_DisableCancellation != null)
            {
                m_DisableCancellation.Dispose();
            }
            m_DisableCancellation = new CancellationTokenSource();
        }

        private void TryCancelToken()
        {
            if (m_DisableCancellation != null && !m_DisableCancellation.IsCancellationRequested)
                m_DisableCancellation.Cancel();
            SetIsAddingMod(false);
        }

        private void StopModImport()
        {
            if (m_CurrentRequest != null)
            {
                m_CurrentRequest.Abort();
            }
            TryCancelToken();
        }
        
        private void SetIsAddingMod(bool isAdding)
        {
            _isAddingMod = isAdding;
            m_UrlInputField.interactable = !isAdding;
            m_OkButton.gameObject.SetActive(!isAdding);
            m_ClipboardButton.gameObject.SetActive(!isAdding);
            m_FilePickerButton.gameObject.SetActive(!isAdding);
        }
        

        private async void OnOkButtonClick()
        {
            if (_isAddingMod)
            {
                CommonTipsUIPanel.ShowPopInfo("已经正在载入一个MOD了");
                return;
            }
            if (!TryGetValidUrl(m_UrlInputField.text, out Uri uri))
                return;
            ClearLog();
            AllocateNewCancellation();
            SetIsAddingMod(true);
            if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
            {
                //远程下载
                var downloadResult = await DownloadModPackageAsync(uri);
                if (downloadResult.isSuccess)
                {
                    await UnZipModPackageAsync(downloadResult.ModDownloadPath, GetModExtractPath(), m_DisableCancellation.Token);
                }
                else
                {
                    if (File.Exists(downloadResult.ModDownloadPath))
                    {
                        //有错误缓存就删了
                        File.Delete(downloadResult.ModDownloadPath);
                    }
                    AppendErrorMsg("下载失败");
                }
            }
            else
            {
                //否则本地路径解压
                await UnZipModPackageAsync(uri.LocalPath, GetModExtractPath(), m_DisableCancellation.Token);
            }
            SetIsAddingMod(false);
        }
        
        private void OnCancelButtonClick()
        {
            if (!_isAddingMod)
            {
                gameObject.SetActive(false);
                return;
            }
            MessageBox.ConfirmOrCancel("正在下载和解压缩MOD, 确定要取消吗？", () =>
            {
                StopModImport();
                gameObject.SetActive(false);
            });
        }

        private void OnClipboardButtonClick()
        {
            m_UrlInputField.text = GUIUtility.systemCopyBuffer;
        }

        private void OnFilePickerButtonClick()
        {
            FileBrowser.SetFilters(true, new FileBrowser.Filter(".7z", ".zip"));
            FileBrowser.SetDefaultFilter(".zip");
            FileBrowser.ShowLoadDialog( OnFileSelectSuccess, 
                                        OnFileSelectCancel, 
                                        FileBrowser.PickMode.Files, false, 
                                        Application.dataPath,"",
                                        "Select a File", "Select");
        }

        private void OnFileSelectSuccess(string[] paths)
        {
            if (paths == null || paths.Length == 0)
                return;
            m_UrlInputField.text = paths[0];
        }

        private void OnFileSelectCancel()
        {
            Debug.Log("取消文件选择");
        }


        private bool TryGetValidUrl(string url, out Uri result)
        {
            var realUrl = Uri.EscapeUriString(url);
            bool ret = Uri.TryCreate(url, UriKind.Absolute, out result);
            if (!ret)
            {
                MessageBox.ShowMessage("不是合法的URL地址");
                return false;
            }
            var extensionName = Path.GetExtension(url);
            if (extensionName != ".zip")
            {
                MessageBox.ShowMessage("不是有效的Zip压缩包文件路径");
                return false;
            }
            return true;
        }

        private string GetModExtractPath()
        {
#if UNITY_EDITOR
            return Application.streamingAssetsPath;
#endif

#if UNITY_STANDALONE_WIN
            return Path.Combine(Application.dataPath, "mods");
#endif

#if UNITY_ANDROID
            return Path.Combine("/storage/emulated/0/", "jynew/mods");
#else
            return Path.Combine(Application.persistentDataPath, "mods");
#endif
        }

        private string GetDownloadSpeedText(ulong lastDownloadBytes, ulong currentDownloadBytes, float deltaTime)
        {
            //暂时不考虑溢出
            var speed = (currentDownloadBytes - lastDownloadBytes) / deltaTime;
            var sizeText = FileTools.FormatSize((long)speed);
            return sizeText + "/s";
        }


        private async UniTask<ModDownloadResult> DownloadModPackageAsync(Uri uri)
        {
            string fileName = Path.GetFileName(uri.LocalPath);
            ModDownloadResult result = new ModDownloadResult();
            try
            {
                m_CurrentRequest = new UnityWebRequest(uri.AbsoluteUri, UnityWebRequest.kHttpVerbGET);
                string savePath = Path.Combine(GetModExtractPath(), fileName);
                result.ModDownloadPath = savePath;
                m_CurrentRequest.downloadHandler = new DownloadHandlerFile(savePath);
                m_CurrentRequest.SendWebRequest();
                ulong lastDownloadBytes = 0;
                while (!m_CurrentRequest.isDone)
                {
                    await UniTask.Delay(330);
                    var downloadSpeedTxt = GetDownloadSpeedText(lastDownloadBytes, m_CurrentRequest.downloadedBytes, 0.33f);
                    lastDownloadBytes = m_CurrentRequest.downloadedBytes;
                    ClearLog();
                    AppendLogMessage(string.Format("下载中...进度{0:F2}% \n下载速度:{1}", 
                        100 * m_CurrentRequest.downloadProgress, downloadSpeedTxt));
                }
                if (m_CurrentRequest.result != UnityWebRequest.Result.Success)
                {
                    result.isSuccess = false;
                    AppendErrorMsg(m_CurrentRequest.error);
                    return result;
                }
                else
                {
                    AppendLogMessage("Mod压缩包下载成功，保存路径:" + savePath);
                    result.isSuccess = true;
                    return result;
                }
            }
            catch(Exception ex)
            {
                AppendErrorMsg(ex.ToString());
                result.isSuccess = false;
                return result;
            }
        }

        
        private async UniTask UnZipModPackageAsync(string zipFilePath, string outPath, CancellationToken token)
        {
            if(!File.Exists(zipFilePath))
            {
                AppendErrorMsg("错误, 压缩包文件不存在");
                return;
            }
            try
            {
                int bufferSize = 1024 * 1024; //给个1mb 缓冲区 不然解压太慢了
                await FileTools.UnZipAsync(zipFilePath, "", outPath, token, OnUnzipProgress, bufferSize);
                AppendLogMessage("解压MOD完毕，请刷新Mod列表");
            }
            catch(TaskCanceledException cancelEx)
            {
                AppendLogMessage("Mod解压缩已终止");
            }
            catch(Exception ex)
            {
                AppendErrorMsg(ex.ToString());
            }
        }

        private void OnUnzipProgress(long finishedBytes, long totalBytes)
        {
            AppendLogMessage(string.Format("解压中...进度{0:F1}%", finishedBytes * 100.0f / totalBytes));
        }
        

        private void AppendLogMessage(string msg)
        {
#if UNITY_EDITOR
            Debug.Log(msg);
#endif
            m_LogMsgBuilder.AppendLine(msg);
            m_LoggerText.text = m_LogMsgBuilder.ToString();
        }
        
        private void AppendErrorMsg(string msg)
        {
            Debug.LogError(msg);
            m_LogMsgBuilder.AppendFormat("<color=red>{0}</color>\n", msg);
            m_LoggerText.text = m_LogMsgBuilder.ToString();
        }

        private void ClearLog()
        {
            m_LogMsgBuilder.Clear();
            m_LoggerText.text = string.Empty;
        }
    }
}
