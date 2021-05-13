Talk(36, "公子別來無恙？", "talkname36", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "一切還好．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "不知林公子是否還有意與我為伴，一同行走江湖．", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(36, "你的隊伍已滿，我無法加入．", "talkname36", 0);
        do return end;
::label1::
        Talk(36, "好啊．", "talkname36", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
        LightScence();
        Join(36);
do return end;
