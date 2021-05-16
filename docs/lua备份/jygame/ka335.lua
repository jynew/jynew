if UseItem(136) == true then goto label0 end;
    do return end;
::label0::
    AddEthics(4);
    ModifyEvent(-2, -2, -2, -2, 337, -2, -2, -2, -2, -2, -2, -2, -2);
    AddItem(136, -1);
    Talk(0, "小兄弟，快將這酒喝下去！", "talkname0", 1);
    Talk(38, "我不喜歡喝酒．", "talkname38", 0);
    Talk(0, "快喝了他吧！喝下去之後，你就不會忽冷忽熱了．", "talkname0", 1);
    Talk(38, "真的．那我要喝．", "talkname38", 0);
    DarkScence();
    ModifyEvent(-2, -2, -2, -2, -2, -2, -2, 5152, 5152, 5152, -2, -2, -2);
    jyx2_ReplaceSceneObject("", "NPC/shipotian", "");--躺姿结束
    jyx2_ReplaceSceneObject("", "NPC/shipotian1", "1");--战姿开始
    ModifyEvent(-2, 1, -2, -2, -2, -2, 338, -2, -2, -2, -2, -2, -2);
    LightScence();
    Talk(38, "哇！這酒怎麼這麼辛辣！不過好像真的挺有效的，謝謝你！既然老伯伯不在了，我要去找媽媽跟小黃了．", "talkname38", 0);
    if AskJoin () == true then goto label1 end;
        do return end;
::label1::
        Talk(0, "你要找你媽媽？我正好在四處旅行，不妨我們結伴一起走，好嗎？", "talkname0", 1);
        if TeamIsFull() == false then goto label2 end;
            Talk(38, "你的隊伍已滿，我無法加入．", "talkname38", 0);
            do return end;
::label2::
            Talk(38, "好啊！", "talkname38", 0);
            DarkScence();
            jyx2_ReplaceSceneObject("", "NPC/shipotian1", "1");--石破天加入队伍
            ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
            LightScence();
            Join(38);
            ModifyEvent(-2, 1, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            ModifyEvent(40, 7, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            ModifyEvent(40, 8, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
do return end;
