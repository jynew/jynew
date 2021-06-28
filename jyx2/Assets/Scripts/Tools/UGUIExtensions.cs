/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Jyx2.Middleware
{
    /// <summary>
    /// UI的扩展方法
    /// </summary>
    public static class ToolsExtensions
    {
        /// <summary>
        /// 设置image的alpha值
        /// </summary>
        /// <param name="image"></param>
        /// <param name="alpha"></param>
        public static void SetAlpha(this Image image, float alpha)
        {
            var c = image.color;
            image.color = new Color(c.r, c.g, c.b, alpha);
        }

        /// <summary>
        /// 设置text的alpha值
        /// </summary>
        /// <param name="image"></param>
        /// <param name="alpha"></param>
        public static void SetAlpha(this Text text, float alpha)
        {
            var c = text.color;
            text.color = new Color(c.r, c.g, c.b, alpha);
        }
    }
}
