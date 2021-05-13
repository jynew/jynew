Talk(63, "公子再次拜訪，不知有何貴事？", "talkname63", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "在下途經貴府，順道進來看看程姑娘是否安好．", "talkname0", 1);
    Talk(63, "謝謝公子的關心．", "talkname63", 0);
    do return end;
::label0::
    Talk(0, "那程姑娘可否與在下一遊，幫忙在下尋找那十四天書？", "talkname0", 1);
    if JudgeEthics(0, 65, 100) == true then goto label1 end;
        Talk(63, "我看公子臉上泛有戾氣，公子還是多做些善事才是．", "talkname63", 0);
        do return end;
::label1::
        if TeamIsFull() == false then goto label2 end;
            Talk(63, "你的隊伍已滿，我無法加入．", "talkname63", 0);
            do return end;
::label2::
            Talk(63, "嗯！好吧．反正我一人在此也是無聊，就隨公子一遊吧．", "talkname63", 0);
            DarkScence();
            jyx2_ReplaceSceneObject("", "NPC/chengying", "");--程英加入队伍
            ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            LightScence();
            Join(63);
            AddEthics(2);
do return end;
