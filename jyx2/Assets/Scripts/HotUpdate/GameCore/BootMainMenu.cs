using Cysharp.Threading.Tasks;
using i18n.TranslatorDef;
using Jyx2;
using UnityEngine;

public class BootMainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject m_RootLoadingPanel;
    
    // Start is called before the first frame update
    async void Start()
    {
        m_RootLoadingPanel.gameObject.SetActive(true);
        await GameStart();
        m_RootLoadingPanel.gameObject.SetActive(false);
    }
    
    
    public async UniTask GameStart()
    {
        // await UniTask.WaitForEndOfFrame();
        await RuntimeEnvSetup.Setup();
        
        
        //TODO: 20220723 CG: 待调整Loading出现的逻辑，因为ResLoader的初始化很慢。但这里目前有前后依赖关系，必须在ResLoader初始化之后
        await Jyx2_UIManager.Instance.ShowUIAsync(nameof(GameMainMenu));

        string info = string.Format("<b>版本：{0} 模组：{1}</b>".GetContent(nameof(Jyx2_UIManager)),
            Application.version,
            RuntimeEnvSetup.CurrentModConfig.ModName);
        
        await Jyx2_UIManager.Instance.(nameof(GameInfoPanel), info);
        
        GraphicSetting.GlobalSetting.Execute();
    }
}
