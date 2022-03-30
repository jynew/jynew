Talk(109, "公子有什么事吗？", "talkname109", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "没事，姑娘真是美丽。", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "听说姑娘武学渊博，不知是否能于在下旅程中，给予一些指导。", "talkname0", 1);
    if InTeam(51) == true then goto label1 end;
        Talk(109, "我要留下来陪我表哥。", "talkname109", 0);
        do return end;
::label1::
        if TeamIsFull() == false then goto label2 end;
            Talk(109, "你的队伍已满，我无法加入。", "talkname109", 0);
            do return end;
::label2::
            Talk(109, "既然我表哥都加入了，我当然要伴着他。", "talkname109", 0);
            DarkScence();
            ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            jyx2_ReplaceSceneObject("", "NPC/王语嫣", "");--王语嫣
            LightScence();
            Join(76);
            AddEthics(1);
do return end;
