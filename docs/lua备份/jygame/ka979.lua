Talk(47, "愣小子，要幹嘛？", "talkname47", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "沒事．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "阿紫姑娘，再加入我好嗎？", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(47, "你的隊伍已滿，我無法加入．", "talkname47", 0);
        do return end;
::label1::
        Talk(47, "你不怕我的話就走吧．", "talkname47", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
        LightScence();
        Join(47);
do return end;
