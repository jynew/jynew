
namespace Jyx2.InputCore.UI
{
    public class BattleActionInputContext : Jyx2Input_UIContext
    {

        private BattleActionUIPanel m_BattleActionUI;

        private void Awake()
        {
            m_BattleActionUI = GetComponent<BattleActionUIPanel>();
        }

        public override void OnUpdate()
        {
            if (m_BattleActionUI == null)
                return;

            if (Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UIClose))
            {
                m_BattleActionUI.OnCancelClick();
            }

            if (Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UI_Yes))
            {
                SwitchSkillFocus();
            }

            if (Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UISwitchLeft))
            {
                m_BattleActionUI.OnSurrenderClick();
            }

            if (Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UISwitchRight))
            {
                //不能直接调用BattleActionUIPanel.OnAutoClicked
                var mainBattleUI = Jyx2_UIManager.Instance.GetUI<BattleMainUIPanel>();
                if (mainBattleUI != null)
                    mainBattleUI.SwitchAutoBattle();
            }

        }


        private void SwitchSkillFocus()
        {
            if (m_BattleActionUI.IsFocusOnSkillsItems)
            {

                m_BattleActionUI.TryFocusOnRightAction();
            }
            else
            {
                m_BattleActionUI.TryFocusOnCurrentSkill();
            }
        }
    }
}
