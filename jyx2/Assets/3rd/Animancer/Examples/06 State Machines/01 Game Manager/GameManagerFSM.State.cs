// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

namespace Animancer.Examples.StateMachines.GameManager
{
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.StateMachines.GameManager/GameManagerFSM
    /// 
    partial class GameManagerFSM
    {
        /// https://kybernetik.com.au/animancer/api/Animancer.Examples.StateMachines.GameManager/State
        /// 
        public abstract class State : FSM.State
        {
            /************************************************************************************************************************/

            public virtual string DisplayText => null;

            /************************************************************************************************************************/

            public override void OnEnterState()
            {
                base.OnEnterState();

                var displayText = DisplayText;
                if (displayText != null)
                {
                    Instance._Text.gameObject.SetActive(true);
                    Instance._Text.text = displayText;
                }
                else
                {
                    Instance._Text.gameObject.SetActive(false);
                }
            }

            /************************************************************************************************************************/

            public virtual void Update() { }

            /************************************************************************************************************************/
        }
    }
}
