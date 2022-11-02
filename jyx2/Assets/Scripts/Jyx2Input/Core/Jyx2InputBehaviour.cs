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
    }
}
