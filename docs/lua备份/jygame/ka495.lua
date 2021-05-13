Talk(109, "公子有什麼事嗎？", "talkname109", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "沒事，姑娘真是美麗．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "聽說姑娘武學淵博，不知是否能於在下旅程中，給予一些指導．", "talkname0", 1);
    if InTeam(51) == true then goto label1 end;
        Talk(109, "我要留下來陪我表哥．", "talkname109", 0);
        do return end;
::label1::
        if TeamIsFull() == false then goto label2 end;
            Talk(109, "你的隊伍已滿，我無法加入．", "talkname109", 0);
            do return end;
::label2::
            Talk(109, "既然我表哥都加入了，我當然要伴著他．", "talkname109", 0);
            DarkScence();
            jyx2_ReplaceSceneObject("", "NPC/wangyuyan", "");--王语嫣
            ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            LightScence();
            Join(76);
            AddEthics(1);
do return end;
