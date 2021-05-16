Talk(50, "怎麼，你準備好了嗎？", "talkname50", 0);
if AskBattle() == true then goto label0 end;
    Talk(0, "嗯，還沒．", "talkname0", 1);
    Talk(50, "喬某隨時在此等候少俠．", "talkname50", 0);
    do return end;
::label0::
    Talk(0, "在下特來領教喬大俠的”降龍十八掌”．", "talkname0", 1);
    if TryBattle(83) == true then goto label1 end;
        LightScence();
        Talk(50, "你還不行，再回去苦練吧．", "talkname50", 0);
        do return end;
::label1::
        LightScence();
        Talk(50, "少俠經這多日來的修練，武功果然不凡，喬某拜服．”天龍八部”你就拿去吧．", "talkname50", 0);
        Talk(0, "那裡，喬幫主承讓了．", "talkname0", 1);
        GetItem(147, 1);
        ModifyEvent(-2, -2, -2, -2, 529, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本529 场景51-14
        ModifyEvent(-2, 20, -2, -2, -1, -1, 530, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本530 场景51-20
        ModifyEvent(-2, 21, -2, -2, -1, -1, 530, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本530 场景51-21
        if HaveItem(183) == true then goto label2 end;
            do return end;
::label2::
            ModifyEvent(28, 12, -2, -2, 518, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本518 场景28-12
            AddEthics(12);
            AddRepute(15);
do return end;
