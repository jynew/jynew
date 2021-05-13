Talk(105, "客倌，看你一身打扮，不像是本地人，大老遠趕來，想必旅途一定勞累了．要不要在這住上一宿，讓你的體力，元氣恢復恢復．", "talkname105", 0);
if AskRest() == true then goto  label0 end;
    do return end;
::label0::
    if JudgeMoney(40) == true then goto label1 end;
        Talk(105, "走，走，走，沒錢就不要妨礙我做生意！", "talkname105", 0);
        do return end;
::label1::
        Talk(0, "在此窮北寒苦絕地，居然能有間客棧真是惠我良多．", "talkname0", 1);
        DarkScence();
        Rest();
        AddItem(174, -40);
        SetScencePosition2(38, 18);
        SetRoleFace(3);
        LightScence();
do return end;
