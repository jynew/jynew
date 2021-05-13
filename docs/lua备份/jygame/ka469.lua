if JudgeEthics(0, 90, 100) == false then goto label0 end;
    Talk(55, "你在江湖上的作為，我夫婦倆已有所聞，這書你就拿去吧．", "talkname55", 0);
    GetItem(148, 1);
    ModifyEvent(-2, 1, -2, -2, 467, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 2, -2, -2, 468, -1, -1, -2, -2, -2, -2, -2, -2);
    do return end;
::label0::
    Talk(55, "你目前的所作所為，還不配稱為一個俠者，再努力吧．或者你想試試我的武功？", "talkname55", 0);
    if AskBattle() == true then goto label1 end;
        do return end;
::label1::
        Talk(0, "晚輩不才，斗膽向前輩請教．", "talkname0", 1);
        if TryBattle(76) == true then goto label2 end;
            LightScence();
            Talk(0, "晚輩功夫不濟，下回再登門拜訪二位．", "talkname0", 1);
            do return end;
::label2::
            LightScence();
            Talk(55, "你的功夫這麼強，希望你不要濫用才是．這書拿去吧．", "talkname55", 0);
            Talk(56, "等會兒，我也想領教一下，看看他是不是真的有實力從我桃花島上取走這本書．", "talkname56", 0);
            if TryBattle(77) == true then goto label3 end;
                LightScence();
                Talk(0, "晚輩功夫不濟，下回再登門拜訪二位．", "talkname0", 1);
                do return end;
::label3::
                LightScence();
                Talk(0, "承蒙兩位前輩高抬貴手，這書我就拿走了．", "talkname0", 1);
                GetItem(148, 1);
                ModifyEvent(-2, 1, -2, -2, 470, -1, -1, -2, -2, -2, -2, -2, -2);
                ModifyEvent(-2, 2, -2, -2, 471, -1, -1, -2, -2, -2, -2, -2, -2);
                AddRepute(20);
do return end;
