using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jyx2Configs
{
    abstract public class Jyx2ConfigBase : ScriptableObject
    {
        protected const string DEFAULT_GROUP_NAME = "基本配置";
        
        [BoxGroup(DEFAULT_GROUP_NAME)][LabelText("ID")] 
        public int Id;
        
        [BoxGroup(DEFAULT_GROUP_NAME)][LabelText("名称")] 
        public string Name;

        /// <summary>
        /// 资源预热
        /// </summary>
        /// <returns></returns>
        public abstract UniTask WarmUp();
    }
}