using UnityEngine;

namespace Jyx2.InputCore
{
    //MonoSingleton 有销毁生命周期的问题，这里还是用个proxy来初始化Instance
    internal class Jyx2InputBehaviour:MonoBehaviour
    {
        private void Awake()
        {
            InputContextManager.Instance.Init();
        }

        private void OnDestroy()
        {
            InputContextManager.Instance.DeInit();
        }

        private void Update()
        {
            InputContextManager.Instance.Update();
        }

        [ContextMenu("打印所有手柄信息")]
        public void DumpJoySticks()
        {
            string result = Jyx2_Input.GetAllJoyStickJsonData();
            GUIUtility.systemCopyBuffer = result;
            Debug.Log(result);
        }

        [ContextMenu("打印键盘信息")]
        public void DumpKeyboard()
        {
            string result = Jyx2_Input.GetKeyBoardElementJsonData();
            GUIUtility.systemCopyBuffer = result;
            Debug.Log(result);
        }
    }
}
