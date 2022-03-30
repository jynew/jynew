Talk(49, "施主别来无恙？", "talkname49", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "一切还好。", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "小师父，我有麻烦了，快帮帮我。", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(49, "你的队伍已满，我无法加入。", "talkname49", 0);
        do return end;
::label1::
        Talk(49, "阿弥陀佛！施主有难，小僧自当效力。", "talkname49", 0);
        DarkScence();
        ModifyEvent(-2, 2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
        ModifyEvent(-2, 15, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
		jyx2_ReplaceSceneObject("","NPC/xuzhu","");
        LightScence();
        Join(49);
do return end;
