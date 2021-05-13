Talk(38, "我要去找媽媽跟小黃．", "talkname38", 0);
ModifyEvent(-2, 1, -2, -2, -2, -2, 338, -2, -2, -2, -2, -2, -2);
if AskJoin () == true then goto label0 end;
    do return end;
::label0::
    Talk(0, "你要找你媽媽？我正好在四處旅行，不妨我們結伴一起走，好嗎？", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(38, "你的隊伍已滿，我無法加入．", "talkname38", 0);
        do return end;
::label1::
        Talk(38, "好啊！", "talkname38", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
        LightScence();
        Join(38);
        AddEthics(1);
        ModifyEvent(-2, 1, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(40, 7, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(40, 8, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
do return end;
