using Cysharp.Threading.Tasks;

namespace Jyx2Configs
{
    public class Jyx2ConfigShop : Jyx2ConfigBase
    {
        //韦小宝触发器名
        public int Trigger;

        //商品列表
        // public List<Jyx2ConfigShopItem> ShopItems;
        public string ShopItems;

        public override async UniTask WarmUp()
        {
            
        }
    }
    
    public class Jyx2ConfigShopItem
    {
        //ID
        public int Id;

        //数量
        public int Count;

        //价格
        public int Price;
    }
}