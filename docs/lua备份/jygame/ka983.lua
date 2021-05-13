Talk(49, "施主別來無恙？", "talkname49", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "一切還好．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "小師父，我有麻煩了，快幫幫我．", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(49, "你的隊伍已滿，我無法加入．", "talkname49", 0);
        do return end;
::label1::
        Talk(49, "阿彌陀佛！施主有難，小僧自當效力．", "talkname49", 0);
        DarkScence();
        ModifyEvent(-2, 2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
        ModifyEvent(-2, 15, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
        LightScence();
        Join(49);
do return end;
