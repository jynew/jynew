if JudgeEthics(0, 90, 100) == false then goto label0 end;
    Talk(55, "你在江湖上的作为，我夫妇俩已有所闻，这书你就拿去吧。", "talkname55", 0);
    AddItem(148, 1);
    ModifyEvent(-2, 1, -2, -2, 467, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 2, -2, -2, 468, -1, -1, -2, -2, -2, -2, -2, -2);
    do return end;
::label0::
    Talk(55, "你目前的所作所为，还不配称为一个侠者，再努力吧。或者你想试试我的武功？", "talkname55", 0);
    if AskBattle() == true then goto label1 end;
        do return end;
::label1::
        Talk(0, "晚辈不才，斗胆向前辈请教。", "talkname0", 1);
        if TryBattle(76) == true then goto label2 end;
            LightScence();
            Talk(0, "晚辈功夫不济，下回再登门拜访二位。", "talkname0", 1);
            do return end;
::label2::
            LightScence();
            Talk(55, "你的功夫这么强，希望你不要滥用才是。这书拿去吧。", "talkname55", 0);
            Talk(56, "等会儿，我也想领教一下，看看他是不是真的有实力从我桃花岛上取走这本书。", "talkname56", 0);
            if TryBattle(77) == true then goto label3 end;
                LightScence();
                Talk(0, "晚辈功夫不济，下回再登门拜访二位。", "talkname0", 1);
                do return end;
::label3::
                LightScence();
                Talk(0, "承蒙两位前辈高抬贵手，这书我就拿走了。", "talkname0", 1);
                AddItem(148, 1);
                ModifyEvent(-2, 1, -2, -2, 470, -1, -1, -2, -2, -2, -2, -2, -2);
                ModifyEvent(-2, 2, -2, -2, 471, -1, -1, -2, -2, -2, -2, -2, -2);
                AddRepute(20);
do return end;
