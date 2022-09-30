using System;
using UnityEngine;

namespace AndroidAuxiliary.Plugins.Auxiliary.AndroidBridge
{
    /// <summary>
    /// 用于挂载在物体上接受回调内容的脚本
    /// </summary>
    public class CallbackHelper : MonoBehaviour
    {
        /// <summary>
        /// 对应开启的主线程
        /// </summary>
        private Action _mainThreadAction;

        /// <summary>
        /// 物体挂载后保持物体永久存在直到回调完成后的删除
        /// </summary>
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// 监听物体回调信号
        /// </summary>
        private void Update()
        {
            if (_mainThreadAction == null) return;
            var temp = _mainThreadAction;
            _mainThreadAction = null;
            temp();
        }

        /// <summary>
        /// 回调处理
        /// </summary>
        /// <param name="function">传入的回调委托内容</param>
        public void CallOnMainThread(Action function)
        {
            _mainThreadAction = function;
        }
    }
}