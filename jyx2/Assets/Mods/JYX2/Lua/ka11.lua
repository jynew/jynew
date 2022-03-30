Talk(1, "有什么要我帮忙的，尽管说出来。", "talkname1", 0);
if AskJoin () == true then goto label0 end;
    do return end;
::label0::
    Talk(0, "胡大哥肯随我闯荡江湖否？", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(1, "你的队伍已满，我无法加入。", "talkname1", 0);
        do return end;
::label1::
        Talk(1, "好！我就随你一走。", "talkname1", 0);
        Talk(0, "胡大哥肯随我闯荡江湖帮这个忙，那再好也不过了。", "talkname0", 1);
        DarkScence();
        ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
        jyx2_ReplaceSceneObject("","NPC/胡斐","");
        LightScence();
        Join(1);
        AddEthics(1);
do return end;
