using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Jyx2Configs
{
    [CreateAssetMenu(menuName = "金庸重制版/配置文件/商店", fileName = "商店配置")]
    public class Jyx2ConfigShop : Jyx2ConfigBase
    {
        [LabelText("韦小宝触发器名")] 
        public int Trigger;

        [LabelText("商品列表")][TableList] 
        public List<Jyx2ConfigShopItem> ShopItems;

        public override async UniTask WarmUp()
        {
            
        }
    }
    
    [Serializable]
    public class Jyx2ConfigShopItem
    {
        [LabelText("道具")] [SerializeReference] [InlineEditor]
        public Jyx2ConfigItem Item;

        [LabelText("数量")] public int Count;

        [LabelText("价格")] public int Price;
    }
}