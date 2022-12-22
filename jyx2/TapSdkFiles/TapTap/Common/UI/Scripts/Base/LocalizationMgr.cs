using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TapSDK.UI
{
    public enum ELanguageType
    {
        cn = 1,
        en = 2,
        fr = 3,
    }

    public class LocalizationMgr: Singleton<LocalizationMgr>
    {
        
        private ELanguageType _currentLanguageType = ELanguageType.cn;

        /// <summary>
        /// 当前语言
        /// </summary>
        /// <value></value>
        public ELanguageType CurrentLanguageType
        {
            get => _currentLanguageType;
        }

        /// <summary>
        /// 设置当前语言
        /// </summary>
        /// <param name="newLanguageType"></param>
        public void SetLanguageType(ELanguageType newLanguageType)
        {
            _currentLanguageType = newLanguageType;
        }
        
    }
}
