// 金庸群侠传3D重制版
// https://github.com/jynew/jynew
// 
// 这是本开源项目文件头，所有代码均使用MIT协议。
// 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
// 
// 金庸老先生千古！
// 
// 本文件作者：东方怂天(EasternDay)
// 文件名: DropdownAttacher.cs
// 时间: 2022-01-05-1:31 PM

using System.Collections.Generic;
using i18n.Ext;
using i18n.TranslatorDef;
using UnityEngine;
using UnityEngine.UI;

namespace i18n.TranslateAttacher
{
    [RequireComponent(typeof(Dropdown))]
    public class DropdownAttacher : BaseAttacher
    {
        /// <summary>
        /// 与脚本共享的Dropdown组件的选项内容
        /// </summary>
        private List<Dropdown.OptionData> OptionDatas => this.gameObject.GetComponent<Dropdown>().options;

        /// <summary>
        /// 组件从未激活状态到激活状态则触发
        /// </summary>
        private void OnEnable()
        {
            Refresh();
        }

        public override void Refresh()
        {
            OptionDatas.ForEach(option =>
            {
                option.text = option.text.GetContent(GetToken());
            });
        }

        public override string GetToken()
        {
            //返回注释信息
            return $"来自于{transform.GetPath()}的{nameof(DropdownAttacher)}组件";
        }
    }
}