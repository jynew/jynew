Talk(6, "魔教妖邪，来我峨嵋山有何贵事。", "talkname6", 0);
Talk(0, "上回看你手中那把宝剑，寒芒吞吐，电闪星飞，想必就是传说中的“倚天剑”？小侠我想向你借来用用。", "talkname0", 1);
Talk(6, "光明顶上被你侥幸获胜，你现在还敢来我峨嵋撒野，莫非真视我峨嵋无人。", "talkname6", 0);
if AskBattle() == true then goto label0 end;
    Talk(0, "哪里，哪里。我只不过是来劝师太，与明教间的事能和就和。自古以来冤家宜解不宜结。", "talkname0", 1);
    Talk(6, "阁下未免管的太多了吧，难道你真以为你是“武林盟主”吗！", "talkname6", 0);
    do return end;
::label0::
    if TryBattle(20) == true then goto label1 end;
        Dead();
        do return end;
::label1::
        LightScence();
        Talk(0, "宝剑还是应该配英雄，怎样？师太，这“倚天剑”可以让给我了吧。", "talkname0", 1);
        Talk(6, "魔教妖孽，想从我灭绝手中拿走倚天剑，等下辈子吧！", "talkname6", 0);
        PlayAnimation(2, 5468, 5496);--by fanyu|播放动画。场景33-2
        jyx2_SwitchRoleAnimation("NPC/miejueshitai", "Assets/BuildSource/AnimationControllers/Suicidedying.controller");
        jyx2_PlayTimelineSimple("[Timeline]ka148_峨眉派_灭绝自戕", false, "");
        jyx2_Wait(3.5);
        ModifyEvent(-2, -2, -2, -2, 149, -1, -1, 5238, 5238, 5238, -2, -2, -2);--by fanyu|启动脚本-149，改变贴图。场景33-2
        Talk(77, "师父，师父！", "talkname77", 0);
        Talk(0, "师太，师太！何苦如此呢？若真不想给我，跟我说一声就行了。唉！", "talkname0", 1);
        Talk(6, "魔教的淫徒，你若玷污了我爱徒们的清白，我做鬼也不饶过…………你！", "talkname6", 0);
        Talk(77, "师父，师父！可恶的魔教妖邪，替我师父偿命来。", "talkname77", 0);
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
