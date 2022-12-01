using Jyx2;
using UIWidgets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MOD.UI
{
    public class ModPanelNew : Jyx2_UIBase
    {
        
        [SerializeField] private ListViewString m_ModListView;
        
        [SerializeField] private Button m_CloseButton;
        [SerializeField] private Button m_AddButton;
        [SerializeField] private Button m_RemoveButton;
        [SerializeField] private Button m_RefreshButton;
        [SerializeField] private Button m_LaunchButton;
        [SerializeField] private AddModPanel m_AddModPanel;
        
        void Awake()
        {
            /*ModChangedSuggestLabel.gameObject.SetActive(false);
        m_Dropdown.ClearOptions();
        var _options = MODProviderBase.Items.Values.Select(x => x.Name).ToList();
        m_ModIds.AddRange(MODProviderBase.Items.Values.Select(m => m.ModId));
        m_Dropdown.AddOptions(_options);
        var idx = m_ModIds.IndexOf(RuntimeEnvSetup.CurrentModId);
        if (idx == -1) idx = 0;
        m_Dropdown.SetValueWithoutNotify(idx);
        m_Dropdown.onValueChanged.AddListener(OnSelectModChange);
        m_UploadBtn.onClick.AddListener(OnUploadMod);
        m_SteamWorkShopBtn.onClick.AddListener(OnOpenSteamWorkshop);
        m_ModWikiBtn.onClick.AddListener(OnOpenModWiki);
        m_CloseBtn.onClick.AddListener(OnCloseBtnClick);*/
        }

        void OnDestroy()
        {
            /*m_Dropdown.onValueChanged.RemoveListener(OnSelectModChange);
        m_UploadBtn.onClick.RemoveListener(OnUploadMod);
        m_SteamWorkShopBtn.onClick.RemoveListener(OnOpenSteamWorkshop);
        m_ModWikiBtn.onClick.RemoveListener(OnOpenModWiki);
        m_CloseBtn.onClick.RemoveListener(OnCloseBtnClick);*/
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


        void RebootGame()
        {
            RuntimeEnvSetup.ForceClear();
            Jyx2_UIManager.Instance.CloseAllUI();
            SceneManager.LoadScene("0_MainMenu");
        }

        protected override void OnCreate()
        {
        
        }
    
        public void OnOpenSteamWorkshop()
        {
            Jyx2.Middleware.Tools.openURL("https://steamcommunity.com/app/2098790/workshop/");
        }

    }
}
