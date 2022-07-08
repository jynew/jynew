using ProtoBuf;

namespace Jyx2Configs
{
    [ProtoContract]
    public class Jyx2ConfigShop : Jyx2ConfigBase
    {
        //韦小宝触发器名
        [ProtoMember(1)]
        public int Trigger;

        //商品列表
        [ProtoMember(2)]
        public string ShopItems;
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