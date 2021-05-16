Talk(6, "魔教妖邪，來我峨嵋山有何貴事．", "talkname6", 0);
Talk(0, "上回看妳手中那把寶劍，寒芒吞吐，電閃星飛，想必就是傳說中的”倚天劍”？小俠我想向你借來用用．", "talkname0", 1);
Talk(6, "光明頂上被你僥倖獲勝，你現在還敢來我峨嵋撒野，莫非真視我峨嵋無人．", "talkname6", 0);
if AskBattle() == true then goto label0 end;
    Talk(0, "那裡，那裡．我只不過是來勸師太，與明教間的事能和就和．自古以來冤家宜解不宜結．", "talkname0", 1);
    Talk(6, "閣下未免管的太多了吧，難道你真以為你是”武林盟主”嗎！", "talkname6", 0);
    do return end;
::label0::
    if TryBattle(20) == true then goto label1 end;
        Dead();
        do return end;
::label1::
        LightScence();
        Talk(0, "寶劍還是應該配英雄，怎樣？師太，這”倚天劍”可以讓給我了吧．", "talkname0", 1);
        Talk(6, "魔教妖孽，想從我滅絕手中拿走倚天劍，等下輩子吧！", "talkname6", 0);
        PlayAnimation(2, 5468, 5496);--by fanyu|播放动画。场景33-2
        ModifyEvent(-2, -2, -2, -2, 149, -1, -1, 5238, 5238, 5238, -2, -2, -2);--by fanyu|启动脚本-149，改变贴图。场景33-2
        Talk(77, "師父，師父！", "talkname77", 0);
        Talk(0, "師太，師太！何苦如此呢？若真不想給我，跟我說一聲就行了．唉！", "talkname0", 1);
        Talk(6, "魔教的淫徒，妳若玷污了我愛徒們的清白，我做鬼也不饒過．．．．．．．．你！", "talkname6", 0);
        Talk(77, "師父，師父！可惡的魔教妖邪，替我師父嚐命來．", "talkname77", 0);
        if TryBattle(21) == true then goto label2 end;
            Dead();
            do return end;
::label2::
            LightScence();
            ModifyEvent(-2, 3, -2, -2, 151, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本-151。场景33-3
            ModifyEvent(-2, 4, -2, -2, 151, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本-151。场景33-4
            ModifyEvent(-2, 5, -2, -2, 151, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本-151。场景33-5
            ModifyEvent(-2, 6, -2, -2, 151, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本-151。场景33-6
            ModifyEvent(-2, 7, -2, -2, 151, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本-151。场景33-7
            ModifyEvent(-2, 8, -2, -2, 151, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本-151。场景33-8
            ModifyEvent(-2, 9, -2, -2, 151, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本-151。场景33-9
            ModifyEvent(-2, 10, -2, -2, 151, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu|启动脚本-151。场景33-10
            AddEthics(-5);
            AddRepute(8);
do return end;
