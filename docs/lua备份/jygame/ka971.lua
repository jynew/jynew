Talk(37, "公子別來無恙？", "talkname37", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "一切還好．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "我這次來是找狄兄幫忙的．", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(37, "你的隊伍已滿，我無法加入．", "talkname37", 0);
        do return end;
::label1::
        Talk(37, "狄某欠公子一個人情，公子需要幫忙，狄某自當義不容辭．", "talkname37", 0);
        DarkScence();
        ModifyEvent(-2, 7, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
        ModifyEvent(-2, 8, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
        ModifyEvent(-2, 10, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
        LightScence();
        Join(37);
do return end;
