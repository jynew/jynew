Talk(0, "段兄，你在这过的还好吧？", "talkname0", 1);
if InTeam(76) == true then goto label0 end;
    Talk(53, "能天天在这陪着神仙姊姊，要我做牛做马我都愿意。", "talkname53", 0);
    do return end;
::label0::
    Talk(53, "兄弟，让我加入你吧，我想跟王姑娘在一起。", "talkname53", 0);
    if AskJoin () == true then goto label1 end;
        Talk(0, "很抱歉，段兄。王姑娘的风采也挺令小弟着迷的。", "talkname0", 1);
        do return end;
::label1::
        Talk(0, "段兄你真是个痴情种子，我们当然是一起走喽！", "talkname0", 1);
        if TeamIsFull() == false then goto label2 end;
            Talk(53, "你的队伍已满，我无法加入。", "talkname53", 0);
            do return end;
::label2::
            DarkScence();
            ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            jyx2_ReplaceSceneObject("", "NPC/段誉", "");--段誉
            LightScence();
            Join(53);
do return end;
