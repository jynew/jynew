
namespace Jyx2.InputCore.UI
{
    public class BattleMainInputContext : Jyx2Input_UIContext
    {

        private BattleMainUIPanel m_BattleMainUI;

        private void Awake()
        {
            m_BattleMainUI = GetComponent<BattleMainUIPanel>();
        }

        public override void OnUpdate()
        {
            if (m_BattleMainUI == null)
                return;

            if (Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UISwitchRight))
            {
                m_BattleMainUI.SwitchAutoBattle();
            }
        }
    }
}
