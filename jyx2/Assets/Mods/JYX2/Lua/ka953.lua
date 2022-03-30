Talk(2, "公子别来无恙？", "talkname2", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "一切还好。", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "没有灵姑娘这个大毒枭在，一路上都挺麻烦的，是否可请灵姑娘再出马呢？", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(2, "你的队伍已满，我无法加入。", "talkname2", 0);
        do return end;
::label1::
        Talk(2, "那有什么问题。", "talkname2", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
		jyx2_ReplaceSceneObject("","NPC/chenglingsu","");
        LightScence();
        Join(2);
do return end;
