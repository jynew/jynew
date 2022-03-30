Talk(51, "怎么？想通了。", "talkname51", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "不行，我不能这么做。", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "好，就听你的。", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(51, "你的队伍已满，我无法加入。", "talkname51", 0);
        do return end;
::label1::
        Talk(51, "走。", "talkname51", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
		jyx2_ReplaceSceneObject("","NPC/慕容复","");
        LightScence();
        Join(51);
do return end;
