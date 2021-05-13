Talk(28, "有什麼事嗎？", "talkname28", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "沒事沒事．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "這次來是要您再隨我一走，隊伍中需要大夫．", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(28, "你的隊伍已滿，我無法加入．", "talkname28", 0);
        do return end;
::label1::
        Talk(28, "走吧．", "talkname28", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
        LightScence();
        Join(28);
do return end;
