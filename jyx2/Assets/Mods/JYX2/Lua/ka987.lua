Talk(53, "公子别来无恙？", "talkname53", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "一切还好。", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "段兄弟愿再陪我一游吗？", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(53, "你的队伍已满，我无法加入。", "talkname53", 0);
        do return end;
::label1::
        Talk(53, "好啊。", "talkname53", 0);
        DarkScence();
        ModifyEvent(-2, 0, 0, 0, -1, -1, -1, -1, -1, -1, -1, -2, -2);
        ModifyEvent(-2, 8, 0, 0, -1, -1, -1, -1, -1, -1, -1, -2, -2);
		jyx2_ReplaceSceneObject("","NPC/段誉","");
        LightScence();
        Join(53);
do return end;
