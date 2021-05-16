Talk(51, "怎麼？想通了．", "talkname51", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "不行，我不能這麼做．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "好，就聽你的．", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(51, "你的隊伍已滿，我無法加入．", "talkname51", 0);
        do return end;
::label1::
        Talk(51, "走．", "talkname51", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
        LightScence();
        Join(51);
do return end;
