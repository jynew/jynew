using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Jyx2;
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
        [SerializeField] private AddModPanel m_AddModPanel;


        private readonly List<GameModBase> _allMods = new List<GameModBase>();
        
        void Awake()
        {
            m_LaunchButton.onClick.AddListener(OnLanuch);
        }

        void OnLanuch()
        {
            RuntimeEnvSetup.ForceClear();
            int index = m_ModListView.SelectedIndex;
            if (index < _allMods.Count)
            {
                var selectMod = _allMods[index];
                RuntimeEnvSetup.SetCurrentMod(selectMod);
                SceneManager.LoadScene("0_MainMenu");
            }
        }
        

        void Start()
        {
            DoRefresh().Forget();
        }

        private List<GameModLoader> _modLoaders = new List<GameModLoader>()
        {
            new GameModNativeLoader(),
            new GameModEditorLoader()
        };
        

        async UniTask DoRefresh()
        {
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
            m_ModListView.UpdateItems();
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
        
    }
}
