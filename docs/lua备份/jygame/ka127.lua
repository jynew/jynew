Talk(8, "小兄弟別在崆峒派逗留了，我六大派即將圍攻魔教總壇光明頂了．", "talkname8", 0);
if AskBattle() == true then goto label0 end;
    do return end;
::label0::
    Talk(0, "在下願領教領教崆峒派的七傷拳絕技．", "talkname0", 1);
    Talk(8, "小子，你自尋死路．", "talkname8", 0);
    if TryBattle(16) == true then goto label1 end;
        Dead();
        do return end;
::label1::
        LightScence();
        Talk(0, "七傷拳也不過如此．", "talkname0", 1);
        Talk(8, "哼！小兄弟留下個字號，他日我唐文亮再向你請教．", "talkname8", 0);
        Talk(0, "字號？好吧，記著，”軟世派河洛分舵”金庸堂堂主是也．", "talkname0", 1);
        Talk(8, "”軟世派河洛分舵”？沒聽過．", "talkname8", 0);
        ModifyEvent(-2, -2, -2, -2, 128, -1, -1, -2, -2, -2, -2, -2, -2);
        AddRepute(3);
do return end;
