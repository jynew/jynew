Talk(45, "少侠有什么吩咐？", "talkname45", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "没什么事，我路过这里，进来看看你。", "talkname0", 1);
    do return end;
::label0::
    if TeamIsFull() == false then goto label1 end;
        Talk(45, "你的队伍已满，我无法加入。", "talkname45", 0);
        do return end;
::label1::
        Talk(0, "你的医术很高明，就加入我的队伍，日后也能帮我们治病疗伤。", "talkname0", 1);
        Talk(45, "是。", "talkname45", 0);
        DarkScence();
        jyx2_ReplaceSceneObject("", "NPC/xuemuhua", "");--薛慕华
        ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        LightScence();
        Join(45);
do return end;
