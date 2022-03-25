Talk(16, "少侠别来无恙？", "talkname16", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "一切还好。", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "少了胡先生的奇妙医术，一路上难免病痛烦身，是否可以再请胡先生帮忙呢？", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(16, "你的队伍已满，我无法加入。", "talkname16", 0);
        do return end;
::label1::
        Talk(16, "少侠有求，胡某自当效力。", "talkname16", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
		jyx2_ReplaceSceneObject("","NPC/胡青牛","");
        LightScence();
        Join(16);
do return end;
