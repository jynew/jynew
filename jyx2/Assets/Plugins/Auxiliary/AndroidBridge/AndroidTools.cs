using UnityEngine;

namespace AndroidAuxiliary.Plugins.Auxiliary.AndroidBridge
{
    /// <summary>
    /// 这是一个用于桥接Android和Unity的类，所有主要接口均通过此处暴露给Unity
    /// </summary>
    public static class AndroidTools
    {
        /// <summary>
        /// 对应的aar包内的Tools类的实例化对象
        /// </summary>
        private static AndroidJavaObject _instance;

        /// <summary>
        /// 对应的aar包内的Tools类的实例化对象的单例
        /// </summary>
        private static AndroidJavaObject Instance =>
            _instance ?? (_instance = new AndroidJavaObject("top.easternday.Auxiliary.Tools"));

        /// <summary>
        /// 对应打包出来的应用程序的Apk的Context
        /// </summary>
        private static AndroidJavaObject _mContext;

        /// <summary>
        /// 对应打包出来的应用程序的Apk的Context的单例
        /// </summary>
        private static AndroidJavaObject Context
        {
            get
            {
                if (_mContext != null) return _mContext;
                using (AndroidJavaObject unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    _mContext = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
                }

                return _mContext;
            }
        }

        /// <summary>
        /// 对应aar包中的DirectoryPickCallback接口的代理函数
        /// </summary>
        public delegate void DirectoryPickCallback(string path);

        /// <summary>
        /// 显示一个安卓原生弹窗
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool ShowToast(string content)
        {
            // 判断是否为安卓平台并返回是否执行成功
            return Application.platform == RuntimePlatform.Android && Instance.CallStatic<bool>("unityToast", content);
        }

        /// <summary>
        /// 获取文件读取权限
        /// </summary>
        public static void GetFileAccessPermission()
        {
            // 判断是否为安卓平台
            if (Application.platform == RuntimePlatform.Android) Instance.CallStatic("GetFileAccessPermission");
        }

        /// <summary>
        /// 选择文件夹并在回调委托中进行其他操作
        /// </summary>
        /// <param name="callback">回调委托函数</param>
        public static void PickDirectory(DirectoryPickCallback callback)
        {
            // 判定是否为安卓平台
            if (Application.platform == RuntimePlatform.Android)
                // 执行文件夹选择函数
                Instance.CallStatic("PickDirectory", Context, new DirectoryReceiveCallback(callback));
            else
                // 不做任何操作
                callback?.Invoke(null);
        }
    }
}