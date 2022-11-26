using UnityEngine;

namespace Jyx2.InputCore.UI
{
    public class ReleaseNoteInputContext : Jyx2Input_UIContext
    {
        public override void OnUpdate()
        {
            if(Jyx2_Input.GetButtonDown(Jyx2PlayerAction.UIClose))
            {
                gameObject.SetActive(false);
            }
        }
    }
}
