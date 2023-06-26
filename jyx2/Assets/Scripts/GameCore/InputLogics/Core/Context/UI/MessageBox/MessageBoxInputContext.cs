using UnityEngine;

namespace Jyx2.InputCore.UI
{
    public class MessageBoxInputContext : Jyx2Input_UIContext
    {
        public override void OnUpdate()
        {
            if(NoValidSelect)
                TrySelectMyUIObject();
        }
    }
}
