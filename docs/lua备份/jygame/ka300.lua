if InTeam(29) == false then goto label0 end;
    Talk(28, "田伯光！你這惡賊，我跟你拼了！", "talkname28", 0);
    if TryBattle(52) == true then goto label1 end;
        Dead();
        do return end;
::label1::
        ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 战斗结束，移除平一指 场景30-0
        LightScence();
        AddEthics(-5);
        do return end;
::label0::
        Talk(28, "你還在這做什麼？", "talkname28", 0);
        if AskJoin () == true then goto label2 end;
            Talk(0, "沒事逛逛．", "talkname0", 1);
            do return end;
::label2::
            Talk(0, "雖然我目前沒什麼病痛，但難保以後路程上會出什麼差錯，你就跟著我一起走吧", "talkname0", 1);
            Talk(28, "你知道我的外號？", "talkname28", 0);
            Talk(0, "不是”殺人名醫”嗎？", "talkname0", 1);
            Talk(28, "知道就好．你如果要我加入你，你得先幫我去殺一個人．", "talkname28", 0);
            Talk(0, "誰？", "talkname0", 1);
            Talk(28, "萬里獨行田伯光那個淫蟲．", "talkname28", 0);
            ModifyEvent(-2, -2, -2, -2, 302, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本302 场景30-0
            ModifyEvent(59, 0, -2, -2, 307, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本307 场景59-0
do return end;
