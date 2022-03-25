Talk(58, "兄弟近来如何？", "talkname58", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "托杨兄的福，一切还好。", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "近日旅途有些不顺，此次前来是想请杨兄加入，助我一臂之力。", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(58, "你的队伍已满，我无法加入。", "talkname58", 0);
        do return end;
::label1::
        Talk(58, "那有什么问题，别的没有，就是有“一臂”。", "talkname58", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
		jyx2_ReplaceSceneObject("","NPC/杨过","");
        LightScence();
        Join(58);
do return end;
