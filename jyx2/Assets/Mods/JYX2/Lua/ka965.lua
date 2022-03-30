Talk(29, "兄弟，一路上还爽吧？又搞了几个女人呀？", "talkname29", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "田兄真爱开玩笑。", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "没有你同行，小弟一人怎么玩得起来。", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(29, "你的队伍已满，我无法加入。", "talkname29", 0);
        do return end;
::label1::
        Talk(29, "那就走吧。我一个人玩也没什么意思，团体的比较好玩。", "talkname29", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
		jyx2_ReplaceSceneObject("","NPC/田伯光","");
        LightScence();
        Join(29);
do return end;
