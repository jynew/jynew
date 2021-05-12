using HSFrameWork.Common;
using UnityEngine;

namespace Jyx2.Setup
{
    public interface IXLsReloader
    {
        void Do();
    }
    
    /// <summary>
    /// 依赖注入代码绑定的类
    /// </summary>
    public class CrossplatformSetupHelper
    {
        /// <summary>
        /// 除了注入、注册、或者绑定外不会有任何代码运行。
        /// </summary>
        public static void ColdBind()
        {
            _OTRH.TryRunOnce(() =>
            {
                Debug.Log("↓↓↓↓↓↓↓↓↓ Game.ColdBind ↓↓↓↓↓↓↓↓↓↓↓");
                HSBootApp.IceBind();
                IceBind();
                HSBootApp.ColdBind(ConStr.GLOBAL_DESKEY,
#if UNITY_EDITOR
                    null,
#else
                    HSConfigTableInitHelperPhone.Create(),
#endif
                    ConStr.PrefabPoolConfig);
                Debug.Log("↑↑↑↑↑↑↑↑↑ Game.ColdBind ↑↑↑↑↑↑↑↑↑↑↑");
                HSLogManager.GetLogger("Booter").Info("ColdBind完成。");


            });
        }

        private static void IceBind()
        {
#if false
            Container.Register<Dictionary<int, string>>("ActionNameDict", x => ConStr.ActionNameDict, ReuseScope.Container);

            //IOC启动注入
            Container.Register<IActionClientContext>(x => ScutContext.Instance, ReuseScope.Container);
            Container.Register<IActionClientDefaultErrorHandler>(x => ReconnectUIDefaultErrorHandler.Instance, ReuseScope.Container);
            Container.Register<ILuaManager>(x => new LuaManagerImpl(), ReuseScope.Container);
            Container.Register<IAudioManager>(x => AudioManagerImpl.Instance, ReuseScope.Container);
            Container.Register<IConfigManager>(x => new ConfigManagerImpl(), ReuseScope.Container);
            Container.Register<ITriggerManager>(x => new GameClientTriggerManager(), ReuseScope.Container);
            Container.Register<IUIManager>(x => new DefaultUIManager(), ReuseScope.Container);
            Container.Register<IStoryManager>(x => new StoryManager(), ReuseScope.Container);
            Container.Register<IExcelFuncManage>(x => new ExcelFuncManage(), ReuseScope.Container);

            //BY CG：这里是一个拆分静态逻辑类的一个典型例子
            Container.Register<IBossRushModel>(x => new BossRushModel(), ReuseScope.None);
            Container.Register<ITeamBuildModel>(x => new TeamBuildModel(), ReuseScope.Container);
            Container.Register<ITransactionModel>(x => new TransactionModel(), ReuseScope.Container);

            //BY AN
            Container.Register<IBanghuiLogic>(x => new BanghuiLogic(), ReuseScope.None);
            Container.Register<IBanghuiBossLogic>(x => new BanghuiBossLogic(), ReuseScope.None);
            Container.Register<IBattleLearnLogic>(x => new BattleLearnLogic(), ReuseScope.None);
            Container.Register<ICanzhangLogic>(x => new CanzhangLogic(), ReuseScope.None);
            Container.Register<IDailyTaskLogic>(x => new DailyTaskLogic(), ReuseScope.Container);
            Container.Register<IEquipmentSetLogic>(x => new EquipmentSetLogic(), ReuseScope.Container);
            Container.Register<IGuajiLogic>(x => new GuajiLogic(), ReuseScope.Container);
            Container.Register<IKOFRushLogic>(x => new KOFRushLogic(), ReuseScope.None);
            Container.Register<IMenpaiLogic>(x => new MenpaiLogic(), ReuseScope.None);
            Container.Register<IMeridianLogic>(x => new MeridianLogic(), ReuseScope.Container);
            Container.Register<INotifyLogic>(x => new NotifyLogic(), ReuseScope.Container);
            Container.Register<ISaodangLogic>(x => new SaodangLogic(), ReuseScope.Container);
            Container.Register<ISPXinfaLogic>(x => new SPXinfaLogic(), ReuseScope.Container);
            Container.Register<ITeamTowerLogic>(x => new TeamTowerLogic(), ReuseScope.Container);
            Container.Register<IWuxueChongxiuLogic>(x => new WuxueChongxiuLogic(), ReuseScope.Container);
            Container.Register<IXinghunLogic>(x => new XinghunLogic(), ReuseScope.Container);
            Container.Register<IHintPanelLogic>(x => new HintPanelLogic(), ReuseScope.Container);
            Container.Register<INetShopLogic>(x => new NetShopLogic(), ReuseScope.Container);
            Container.Register<IStoryLogic>(x => new StoryLogic(), ReuseScope.Container);
            Container.Register<IUserItemLogic>(x => new UserItemLogic(), ReuseScope.Container);
            Container.Register<IBufaLogic>(x => new BufaLogic(), ReuseScope.Container);
            Container.Register<IExploreLogic>(x => new ExploreLogic(), ReuseScope.Container);
            Container.Register<IChallengeTicketLogic>(x => new ChallengeTicketLogic(), ReuseScope.Container);
            Container.Register<IItemLogic>(x => new ItemLogic(), ReuseScope.Container);
            Container.Register<IHechengLogic>(x => new HechengLogic(), ReuseScope.Container);
            Container.Register<IExploreBossLogic>(x => new ExploreBossLogic(), ReuseScope.Container);

            //BY WENZI
            Container.Register<IIslandLogic>(x => new IslandLogic(), ReuseScope.Container);     

            //by tools
            Container.Register<IUITools>(x => new UIToolsImpl(), ReuseScope.Container);
            Container.Register<IChatCore>(x => new ChatCore(), ReuseScope.Container);
            Container.Register<IFriendCore>(x => new FriendCore(), ReuseScope.Container);
            Container.Register<ITianJiangLogic>(x => new TianJiangLogic(), ReuseScope.Container);
            Container.Register<IBattleLoaderV3>(x => new BattleLoaderV3(), ReuseScope.Container);

            Container.Register<IBattleUI>(x => BattleUI2D.Instance, ReuseScope.Container);
            Container.Register<IBattleFieldEffectLogic>(x => new BattleFieldEffectLogic(), ReuseScope.None);
            Container.Register<IBattleStoryManage>(x => new BattleStoryManage(), ReuseScope.Container);
#endif
        }

        private static OneTimeRunHelper _OTRH = new OneTimeRunHelper();
    }
}
