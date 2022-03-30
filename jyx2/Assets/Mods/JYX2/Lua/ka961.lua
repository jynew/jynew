Talk(25, "这么久都不来看我，可想死人家了。这次来是不是来带我走的。", "talkname25", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "没有。经过你这边，顺路进来看看你。", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "我这不是来了吗。走吧。", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(25, "你的队伍已满，我无法加入。", "talkname25", 0);
        do return end;
::label1::
        Talk(25, "真的，没骗我。我原以为你也是个负心汉，看来是误会你了。走吧。", "talkname25", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
		jyx2_ReplaceSceneObject("","NPC/lanfenghuang","");
        LightScence();
        Join(25);
do return end;
