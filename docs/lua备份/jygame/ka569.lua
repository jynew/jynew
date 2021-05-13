Talk(0, "徒兒，師父來看你了．", "talkname0", 1);
Talk(44, "．．．．", "talkname44", 0);
Talk(0, "叫師父啊．", "talkname0", 1);
Talk(44, "．．．師．．師父．", "talkname44", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "記得要乖哦！", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "徒兒，你就跟為師的走吧．", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(44, "你的隊伍已滿，我無法加入．", "talkname44", 0);
        do return end;
::label1::
        Talk(44, "是．", "talkname44", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        LightScence();
        Join(44);
do return end;
