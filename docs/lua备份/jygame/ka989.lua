Talk(54, "小兄弟，最近還好嗎？", "talkname54", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "托袁兄的福，一切還好．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "不瞞袁兄，最近諸事不順．．．．．．．", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(54, "你的隊伍已滿，我無法加入．", "talkname54", 0);
        do return end;
::label1::
        Talk(54, "別說了，我們走吧．", "talkname54", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
        LightScence();
        Join(54);
do return end;
