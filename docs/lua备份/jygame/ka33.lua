if InTeam(1) == false then goto label0 end;
    Talk(3, "胡斐，你準備好了嗎？", "talkname3", 0);
    if AskBattle() == true then goto label1 end;
        do return end;
::label1::
        if TryBattle(4) == false then goto label2 end;
            LightScence();
            Talk(3, "不錯，你有這樣的武藝，你爹也可放心了．來，把我殺了，替你爹報仇．", "talkname3", 0);
            Talk(1, "兄弟，我們走吧．仇我已經報了．", "talkname1", 1);
            Talk(0, "對嘛！這才是我的好大哥．", "talkname0", 1);
            Talk(3, "走之前，拿了這把冷月寶刀，這是一把適合你的寶刀．還有，這本書拿去吧，希望能幫小兄弟解決困難．", "talkname3", 0);
            GetItem(116, 1);
            GetItem(144, 1);
            Talk(0, "呀呼！找到”飛狐外傳”了", "talkname0", 1);
            ModifyEvent(-2, -2, -2, -2, 34, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本34 场景24-编号8
            AddEthics(2);
            do return end;
::label2::
            LightScence();
            Talk(3, "再去好好琢磨琢磨．", "talkname3", 0);
            do return end;
::label0::
            Talk(3, "麻煩你轉告胡斐，等他準備好了，可隨時來找我．", "talkname3", 0);
do return end;
