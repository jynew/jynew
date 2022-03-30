Talk(28, "有什么事吗？", "talkname28", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "没事没事。", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "这次来是要您再随我一走，队伍中需要大夫。", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(28, "你的队伍已满，我无法加入。", "talkname28", 0);
        do return end;
::label1::
        Talk(28, "走吧。", "talkname28", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
		jyx2_ReplaceSceneObject("","NPC/平一指","");
        LightScence();
        Join(28);
do return end;
