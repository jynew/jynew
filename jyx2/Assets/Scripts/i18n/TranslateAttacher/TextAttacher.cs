/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 *
 * 本文件作者：东方怂天(EasternDay)
 */

using System;
using i18n.Ext;
using i18n.TranslatorDef;
using UnityEngine;
using UnityEngine.UI;

namespace i18n.TranslateAttacher
{
    /// <summary>
    /// TextAttacher是关联到Text组件的翻译中介组件
    /// Translator对于Text组件的翻译工作只能通过预先在Text组件上添加本组件才能用更小的资源浪费来获得翻译体验
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class TextAttacher : BaseAttacher
    {
        /// <summary>
        /// 与本脚本关联的Text组件
        /// 添加此字段的原因在于方便得到组件，用空间换时间
        /// </summary>
        /// <returns>当前脚本绑定的Text组件</returns>
        private Text TextScript => this.gameObject.GetComponent<Text>();

        /// <summary>
        /// 组件从未激活状态到激活状态则触发
        /// </summary>
        private void OnEnable()
        {
            Refresh();
        }

        public override void Refresh()
        {
            //刷新翻译内容
            TextScript.text = TextScript.text.GetContent(GetToken());
        }

        public override string GetToken()
        {
            //返回注释信息
            return $"来自于{transform.GetPath()}的{nameof(TextAttacher)}组件";
        }
    }
}