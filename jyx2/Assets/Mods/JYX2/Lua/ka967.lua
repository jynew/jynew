Talk(35, "来来来，好久不见，我们喝一杯。", "talkname35", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "令狐兄还是这么潇洒。", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "提起喝酒，我就想起一路上少了令狐兄为伴，旅途中就好像少了点什么。", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(35, "你的队伍已满，我无法加入。", "talkname35", 0);
        do return end;
::label1::
        Talk(35, "那我们就再一起结伴天涯，喝尽世间的美酒！", "talkname35", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
		jyx2_ReplaceSceneObject("","NPC/令狐冲","");
        LightScence();
        Join(35);
do return end;
