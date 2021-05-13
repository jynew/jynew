Talk(105, "客倌想住宿嗎？本店有上好客房供應．一間２０兩．", "talkname105", 0);
if AskRest() == true then goto  label0 end;
    do return end;
::label0::
    if JudgeMoney(20) == true then goto label1 end;
        Talk(105, "走，走，走，沒錢就不要妨礙我做生意！", "talkname105", 0);
        do return end;
::label1::
        Talk(0, "荒野之地多兇險，龍門地界只怕兵禍臨頭不遠．", "talkname0", 1);
        DarkScence();
        Rest();
        AddItem(174, -20);
        SetScencePosition2(14, 14);
        SetRoleFace(3);
        LightScence();
do return end;
