using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using Jyx2.ResourceManagement;

namespace Jyx2.InputCore.Data
{
    [CreateAssetMenu(fileName = "ControllerSpriteAsset", menuName = "JYX2/创建控制器Sprite")]
    public class ControllerSpriteAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        private Dictionary<string, TMP_SpriteAsset> m_SpriteAssetDic;

        public TMP_SpriteAsset GetSpriteAsset(string identifier)
        {
            if (m_SpriteAssetDic == null)
                return null;
            m_SpriteAssetDic.TryGetValue(identifier, out TMP_SpriteAsset result);
            return result;
        }
    }
}