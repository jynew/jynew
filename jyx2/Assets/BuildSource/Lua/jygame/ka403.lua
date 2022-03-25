Talk(63, "公子再次拜访，不知有何贵事？", "talkname63", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "在下途经贵府，顺道进来看看程姑娘是否安好。", "talkname0", 1);
    Talk(63, "谢谢公子的关心。", "talkname63", 0);
    do return end;
::label0::
    Talk(0, "那程姑娘可否与在下一游，帮忙在下寻找那十四天书？", "talkname0", 1);
    if JudgeEthics(0, 65, 100) == true then goto label1 end;
        Talk(63, "我看公子脸上泛有戾气，公子还是多做些善事才是。", "talkname63", 0);
        do return end;
::label1::
        if TeamIsFull() == false then goto label2 end;
            Talk(63, "你的队伍已满，我无法加入。", "talkname63", 0);
            do return end;
::label2::
            Talk(63, "嗯！好吧．反正我一人在此也是无聊，就随公子一游吧。", "talkname63", 0);
            DarkScence();
            jyx2_ReplaceSceneObject("", "NPC/程英", "");--程英加入队伍
            ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            LightScence();
            Join(63);
            AddEthics(2);
do return end;
