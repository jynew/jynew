Talk(54, "小兄弟，最近还好吗？", "talkname54", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "托袁兄的福，一切还好。", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "不瞒袁兄，最近诸事不顺……", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(54, "你的队伍已满，我无法加入。", "talkname54", 0);
        do return end;
::label1::
        Talk(54, "别说了，我们走吧。", "talkname54", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
		jyx2_ReplaceSceneObject("","NPC/袁承志","");
        LightScence();
        Join(54);
do return end;
