Talk(29, "怎麼，還想殺我嗎？還是想跟我學幾招對付女人呀？", "talkname29", 0);
if AskBattle() == false then goto label0 end;
    Talk(0, "你這採花淫賊，死到臨頭還不覺悟．你受死吧！", "talkname0", 1);
    if TryBattle(53) == true then goto label1 end;
        Dead();
        do return end;
::label1::
        ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(30, 0, -2, -2, 303, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本303 场景30-0
        SetScenceMap(-2, 1, 17, 15, 2674);--田伯光死掉
        LightScence();
        AddRepute(4);
        do return end;
::label0::
        if AskJoin () == false then goto label2 end;
            Talk(0, "這可是你說的，我們就一起走吧，到時可得傳授小弟幾招．", "talkname0", 1);
            if TeamIsFull() == false then goto label3 end;
                Talk(29, "你的隊伍已滿，我無法加入．", "talkname29", 0);
                do return end;
::label3::
                DarkScence();
                ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 田伯光加入队伍 场景59-0
                SetScenceMap(-2, 1, 17, 15, 2674);
                LightScence();
                Join(29);
                AddEthics(-6);
                do return end;
::label2::
                Talk(0, "你們倆的事，我不想管．", "talkname0", 1);
do return end;
