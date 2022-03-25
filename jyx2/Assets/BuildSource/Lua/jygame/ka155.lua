Talk(5, "小兄弟想要与老朽切磋武学的奥妙吗？", "talkname5", 0);
if AskBattle() == true then goto label0 end;
    do return end;
::label0::
    Talk(0, "还望前辈指导。", "talkname0", 1);
    if TryBattle(22) == false then goto label1 end;
        LightScence();
        Talk(5, "少侠武功已到如此境界，老朽也没什么好教你的了。", "talkname5", 0);
        ModifyEvent(-2, -2, -2, -2, 158, -1, -1, -2, -2, -2, -2, -2, -2);
        AddRepute(20);
        do return end;
::label1::
        LightScence();
        if JudgeEthics(0, 50, 100) == true then goto label2 end;
            Talk(5, "小兄弟，看来你还需再下一番努力才是。", "talkname5", 0);
            do return end;
::label2::
            if JudgeAttack(0, 0, 100) == true then goto label3 end;
                Talk(5, "小兄弟，看来你还需再下一番努力才是。", "talkname5", 0);
                do return end;
::label3::
                Talk(5, "小兄弟资质不错，我这有一些提升轻功的心得，你拿去好好参详吧。", "talkname5", 0);
                Talk(0, "谢谢前辈。", "talkname0", 1);
                AddItem(45, 1);
                ModifyEvent(-2, -2, -2, -2, 156, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
