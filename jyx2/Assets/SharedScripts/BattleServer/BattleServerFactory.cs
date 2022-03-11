namespace Jyx2.SharedScripts.BattleServer
{
    /// <summary>
    /// 战斗服务器工厂类
    /// </summary>
    public static class BattleServerFactory
    {
        public static IBattleC2S CreateServer()
        {
            return new BattleC2SImpl();
        }
    }
}