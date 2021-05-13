Talk(105, "客倌，看你一身打扮，不像是本地人，大老遠趕來，想必旅途一定勞累了．要不要在這住上一宿，讓你的體力，元氣恢復恢復．", "talkname105", 0);
if AskRest() == true then goto  label0 end;
    do return end;
::label0::
    if JudgeMoney(100) == true then goto label1 end;
        Talk(105, "走，走，走，沒錢就不要妨礙我做生意！", "talkname105", 0);
        do return end;
::label1::
        Talk(0, "好，我就試試看你們悅來客棧的服務好不好．", "talkname0", 1);
        DarkScence();
        Rest();
        AddItem(174, -100);
        SetScencePosition2(14, 14);
        SetRoleFace(3);
        LightScence();
do return end;
