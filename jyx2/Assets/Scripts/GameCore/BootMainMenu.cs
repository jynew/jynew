using UnityEngine;

public class BootMainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject m_RootLoadingPanel;
    
    // Start is called before the first frame update
    async void Start()
    {
        m_RootLoadingPanel.gameObject.SetActive(true);
        await Jyx2_UIManager.Instance.GameStart();
        m_RootLoadingPanel.gameObject.SetActive(false);
    }
}
