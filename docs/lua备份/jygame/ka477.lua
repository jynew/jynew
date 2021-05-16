Talk(53, "閣下在大理玩的還開心吧？", "talkname53", 0);
Talk(0, "大理境內風光明媚，果然是個好地方．", "talkname0", 1);
if AskJoin () == true then goto label0 end;
    Talk(0, "好了，不打擾兄台了．他日有緣，再一同遊山玩水吧．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "不知兄台是否願與我同行，前往那無量山一遊？", "talkname0", 1);
    if JudgeEthics(0, 40, 100) == true then goto label1 end;
        Talk(53, "嗯．．．我還有些事要辦，恐怕無法與閣下同行．", "talkname53", 0);
        do return end;
::label1::
        if TeamIsFull() == false then goto label2 end;
            Talk(53, "你的隊伍已滿，我無法加入．", "talkname53", 0);
            do return end;
::label2::
            Talk(53, "好啊，有個人做伴，路上也有個照應．", "talkname53", 0);
            DarkScence();
            jyx2_ReplaceSceneObject("", "NPC/duanyu", "");--段誉加入队伍
            ModifyEvent(-2, 0, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            ModifyEvent(-2, 8, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            LightScence();
            Join(53);
do return end;
