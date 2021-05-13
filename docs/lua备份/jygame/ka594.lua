Talk(109, "公子近來可好？", "talkname109", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "一切還好，謝謝王姑娘的關心．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "說來慚愧，在下此次來是想麻煩王姑娘出馬幫忙的．", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(109, "你的隊伍已滿，我無法加入．", "talkname109", 0);
        do return end;
::label1::
        Talk(109, "小女子願盡綿薄之力．", "talkname109", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        LightScence();
        Join(76);
        AddEthics(2);
do return end;
