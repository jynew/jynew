Talk(17, "少侠如果有需要的话，尽管说出来．", "talkname17", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "改日如果有需要时，我一定会来找王前辈．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "好吧！那就麻烦王前辈与我一起奔波江湖了．", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(17, "你的队伍已满，我无法加入．", "talkname17", 0);
        do return end;
::label1::
        DarkScence();
        ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
		jyx2_ReplaceSceneObject("", "NPC/王难姑", ""); 
        LightScence();
        Join(17);
        AddEthics(1);
do return end;
