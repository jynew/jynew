Talk(59, "公子近来无恙？", "talkname59", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "托龙姑娘的福，一切还好。", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "还好，不过是否能再请龙姑娘出马帮忙呢？", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(59, "你的队伍已满，我无法加入。", "talkname59", 0);
        do return end;
::label1::
        Talk(59, "好的。", "talkname59", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
		jyx2_ReplaceSceneObject("","NPC/小龙女","");
        LightScence();
        Join(59);
do return end;
