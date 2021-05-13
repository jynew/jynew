Talk(48, "你要幹嘛？", "talkname48", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "沒事．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "鐵面，我需要你的幫忙，走吧．", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(48, "你的隊伍已滿，我無法加入．", "talkname48", 0);
        do return end;
::label1::
        Talk(48, "嗯．", "talkname48", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
        LightScence();
        Join(48);
do return end;
