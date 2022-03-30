Talk(45, "公子别来无恙？", "talkname45", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "一切还好。", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "少了薛先生的奇妙医术，一路上难免病痛烦身，是否可以再请薛先生帮忙呢？", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(45, "你的队伍已满，我无法加入。", "talkname45", 0);
        do return end;
::label1::
        Talk(45, "公子有需，薛某自当效力。", "talkname45", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
		jyx2_ReplaceSceneObject("","NPC/xuemuhua","");
        LightScence();
        Join(45);
do return end;
