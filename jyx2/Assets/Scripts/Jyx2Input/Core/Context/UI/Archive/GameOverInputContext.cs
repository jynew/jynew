using UnityEngine;

namespace Jyx2.InputCore.UI
{
    public class GameOverInputContext : Jyx2Input_UIContext
    {
        private GameOver m_GameOverPanel;

        private void Start()
        {
            m_GameOverPanel = GetComponent<GameOver>();
        }

        public override void OnUpdate()
        {
            if (Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UIClose))
            {
                m_GameOverPanel?.BackToMainMenu();
            }
        }
    }
}
