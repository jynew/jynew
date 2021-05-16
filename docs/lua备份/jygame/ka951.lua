Talk(1, "兄弟別來無恙？", "talkname1", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "一切還好．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "少了大哥胡家刀法助威，小弟辦起事來總覺得不順，．．．", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(1, "你的隊伍已滿，我無法加入．", "talkname1", 0);
        do return end;
::label1::
        Talk(1, "別說了，我就再助你一臂之力．", "talkname1", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
        LightScence();
        Join(1);
do return end;
