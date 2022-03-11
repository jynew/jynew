// 金庸群侠传3D重制版
// https://github.com/jynew/jynew
// 
// 这是本开源项目文件头，所有代码均使用MIT协议。
// 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
// 
// 金庸老先生千古！
// 
// 本文件作者：东方怂天(EasternDay)
// 文件名: ExtendFunction.cs
// 时间: 2022-01-05-1:28 PM

using UnityEngine;

namespace i18n.Ext
{
    public static class TransformAndGameobjectExt
    {
        
        /// <summary>
        /// Transform拓展方法
        /// 物体所在Hierarchy所在路径，用于GetToken使用
        /// </summary>
        /// <param name="transform">物体transform</param>
        /// <returns>物体Hierarchy所在路径</returns>
        public static string GetPath(this Transform transform)
        {
            var path = transform.name;
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }
            return path;
        }
        /// <summary>
        /// 上一个方法的重载，适用于GameObject
        /// </summary>
        /// <param name="gameObject">物体</param>
        /// <returns>物体Hierarchy所在路径</returns>
        public static string GetPath(this GameObject gameObject)
        {
            return GetPath(gameObject.transform);
        }
    }
}