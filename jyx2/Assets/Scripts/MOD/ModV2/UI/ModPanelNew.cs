using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Jyx2;
using Jyx2.Middleware;
using Jyx2.MOD.ModV2;
using UIWidgets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MOD.UI
{
    public class ModPanelNew : MonoBehaviour
    {
        
        [SerializeField] private ListViewString m_ModListView;
        
        [SerializeField] private Button m_CloseButton;
        [SerializeField] private Button m_AddButton;
        [SerializeField] private Button m_RemoveButton;
        [SerializeField] private Button m_RefreshButton;
        [SerializeField] private Button m_LaunchButton;
        [SerializeField] private Button m_SteamWorkshopButton;
        

        [SerializeField] private Image m_ModPic;
        [SerializeField] private Text m_ModTitle;
        [SerializeField] private Text m_ModContent;

        [SerializeField] private AddModPanelTemp m_AddModPanel;

        [SerializeField] private InputField m_ResetInputField;

        private readonly List<GameModBase> _allMods = new List<GameModBase>();
        
        void Awake()
        {
            m_LaunchButton.onClick.AddListener(OnLanuch);
            m_RefreshButton.onClick.AddListener(OnClickedRefresh);
            m_ModListView.OnSelect.AddListener(OnItemSelect);
            m_RemoveButton.onClick.AddListener(OnRemove);
            m_AddButton.onClick.AddListener(OnAdd);
            m_CloseButton.onClick.AddListener(OnQuit);
            
            #if UNITY_STANDALONE
            m_SteamWorkshopButton.gameObject.SetActive(true);
            #else
            m_SteamWorkshopButton.gameObject.SetActive(false);
            #endif
        }

        void OnQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
		    Application.Quit();
#endif

        }

        private void OnItemSelect(int index, ListViewItem arg1)
        {
            var mod = GetCurrentSelectMod();
            if(mod != null)
                ShowModDetail(mod);
        }

        private GameModBase GetCurrentSelectMod()
        {
            int index = m_ModListView.SelectedIndex;
            if (index >= _allMods.Count) return null;
            return _allMods[index];
        }

        void OnAdd()
        {
            m_AddModPanel.Show();
        }

        void ShowModDetail(GameModBase mod)
        {
            UniTask.Run(async () =>
            {
                await UniTask.SwitchToMainThread();
                m_ModPic.gameObject.SetActive(false);
                m_ModPic.sprite = await mod.LoadPic();
                m_ModPic.gameObject.SetActive(true);
            });

            m_ModTitle.text = mod.Title;
            m_ModContent.text = mod.GetContent();
        }

        void OnRemove()
        {
            var mod = GetCurrentSelectMod();
            if (mod == null) return;

            if (mod is GameModManualInstalled)
            {
                Debug.LogError("暂不支持，请手动前往删除。");
                Tools.openURL(Application.persistentDataPath);    
            }
            //TODO :steam平台可以取消订阅
            else
            {
                Debug.LogError("现在不支持删除本类型MOD");
            }
        }
        
        void OnLanuch()
        {
            RuntimeEnvSetup.ForceClear();
            var mod = GetCurrentSelectMod();
            if (mod != null)
            {
                RuntimeEnvSetup.SetCurrentMod(mod);
                SceneManager.LoadScene("0_MainMenu");
            }
        }


        void OnClickedRefresh()
        {
            DoRefresh().Forget();
        }
        

        void Start()
        {
            DoRefresh().Forget();
        }

        private List<GameModLoader> _modLoaders = new List<GameModLoader>()
        {
            new GameModNativeLoader(),
#if UNITY_EDITOR
            new GameModEditorLoader(),
#endif
            new GameModManualInstalledLoader(),
#if UNITY_STANDALONE
            new GameModSteamWorkshopLoader(),
#endif
        };
        

        async UniTask DoRefresh()
        {
            m_RefreshButton.enabled = false;
            m_ModListView.Clear();
            _allMods.Clear();
            foreach (var modLoader in _modLoaders)
            {
                foreach (var mod in await modLoader.LoadMods())
                {
                    _allMods.Add(mod);
                    m_ModListView.Add(mod.GetDesc());
                }
            }

            if (m_ModListView.GetItemsCount() > 0)
            {
                m_ModListView.SelectedIndex = 0;    
            }
            
            m_ModListView.UpdateItems();
            m_RefreshButton.enabled = true;
        }
        

        public void OnCloseBtnClick()
        {
            /*var selectMod = m_ModIds[m_Dropdown.value];
        if(selectMod != RuntimeEnvSetup.CurrentModId)
        {
            Jyx2_PlayerPrefs.SetString("CURRENT_MOD_ID", selectMod);
            Jyx2_PlayerPrefs.Save();
            RebootGame();
        }
        else
            Hide();*/
        }
        
        
        public void OnOpenSteamWorkshop()
        {
            Jyx2.Middleware.Tools.openURL("https://steamcommunity.com/app/2098790/workshop/");
        }

        public void DoReset()
        {
            if (m_ResetInputField.text != "我要一键复原") return;
            Directory.Delete(Application.persistentDataPath, true);
            PlayerPrefs.DeleteAll();
            
        }
    }
}
