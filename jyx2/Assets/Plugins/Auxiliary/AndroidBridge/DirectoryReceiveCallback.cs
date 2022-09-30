using UnityEngine;

namespace AndroidAuxiliary.Plugins.Auxiliary.AndroidBridge
{
    /// <summary>
    /// 这是aar包中对应部分的回调函数的代理接口的Unity实现
    /// </summary>
    public class DirectoryReceiveCallback : AndroidJavaProxy
    {
        private readonly AndroidTools.DirectoryPickCallback _callback;

        private readonly CallbackHelper _callbackHelper;

        /// <summary>
        /// 本回调类的构造函数
        /// </summary>
        /// <param name="callback">对应AndroidTools里的委托回调</param>
        public DirectoryReceiveCallback(AndroidTools.DirectoryPickCallback callback) : base(
            "top.easternday.Auxiliary.NativeDirectoryReceiver")
        {
            _callback = callback; //将本类中的回调委托设置为AndroidTools中的回调委托
            _callbackHelper =
                new GameObject("CallbackHelper").AddComponent<CallbackHelper>(); //创建一个物体并将给其赋予一个CallbackHelper脚本
        }

        /// <summary>
        /// 当aar中对应部分接收到返回值后的操作
        /// </summary>
        /// <param name="path">接收到的返回路径的值</param>
        public void OnDirectoryReceived(string path)
        {
            // 在主线程中开启一个子线程用于处理回调委托
            _callbackHelper.CallOnMainThread(() => OnDirectoryReceiveCallback(path));
        }

        /// <summary>
        /// 处理回调委托的函数
        /// </summary>
        /// <param name="path">读取到的目录的路径</param>
        private void OnDirectoryReceiveCallback(string path)
        {
            try
            {
                //处理回调
                if (!string.IsNullOrEmpty(path))
                    _callback?.Invoke(path);
            }
            finally
            {
                // 回调完成删除脚本
                Object.Destroy(_callbackHelper.gameObject);
            }
        }
    }
}