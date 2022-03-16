// 金庸群侠传3D重制版
// https://github.com/jynew/jynew
// 
// 这是本开源项目文件头，所有代码均使用MIT协议。
// 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
// 
// 金庸老先生千古！
// 
// 本文件作者：东方怂天(EasternDay)
// 文件名: BaseAttacher.cs
// 时间: 2022-01-05-12:21 PM

using i18n.Ext;
using UnityEngine;

namespace i18n.TranslateAttacher
{
    public abstract class BaseAttacher : MonoBehaviour
    {
        /// <summary>
        /// 刷新文本内容函数，所有Attacher都会用到
        /// </summary>
        public abstract void Refresh();

        /// <summary>
        /// 获取Attacher来源Token的函数，用于GetContent函数注册
        /// </summary>
        public virtual string GetToken()
        {
            return transform.GetPath();
        }
    }
}