Talk(53, "阁下在大理玩的还开心吧？", "talkname53", 0);
Talk(0, "大理境内风光明媚，果然是个好地方。", "talkname0", 1);
if AskJoin () == true then goto label0 end;
    Talk(0, "好了，不打扰兄台了。他日有缘，再一同游山玩水吧。", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "不知兄台是否愿与我同行，前往那无量山一游？", "talkname0", 1);
    if JudgeEthics(0, 40, 100) == true then goto label1 end;
        Talk(53, "嗯……我还有些事要办，恐怕无法与阁下同行。", "talkname53", 0);
        do return end;
::label1::
        if TeamIsFull() == false then goto label2 end;
            Talk(53, "你的队伍已满，我无法加入。", "talkname53", 0);
            do return end;
::label2::
            Talk(53, "好啊，有个人做伴，路上也有个照应。", "talkname53", 0);
            DarkScence();
            ModifyEvent(-2, 0, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            ModifyEvent(-2, 8, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            jyx2_ReplaceSceneObject("", "NPC/段誉", "");--段誉加入队伍
            LightScence();
            Join(53);
do return end;
