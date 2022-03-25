if InTeam(1) == false then goto label0 end;
    Talk(3, "胡斐，你准备好了吗？", "talkname3", 0);
    if AskBattle() == true then goto label1 end;
        do return end;
::label1::
        if TryBattle(4) == false then goto label2 end;
            LightScence();
            Talk(3, "不错，你有这样的武艺，你爹也可放心了。来，把我杀了，替你爹报仇。", "talkname3", 0);
            Talk(1, "兄弟，我们走吧。仇我已经报了。", "talkname1", 1);
            Talk(0, "对嘛！这才是我的好大哥。", "talkname0", 1);
            Talk(3, "走之前，拿了这把冷月宝刀，这是一把适合你的宝刀。还有，这本书拿去吧，希望能帮小兄弟解决困难。", "talkname3", 0);
            AddItem(116, 1);
            AddItem(144, 1);
            Talk(0, "呀呼！找到《飞狐外传》了！", "talkname0", 1);
            ModifyEvent(-2, -2, -2, -2, 34, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本34 场景24-编号8
            AddEthics(2);
            do return end;
::label2::
            LightScence();
            Talk(3, "再去好好琢磨琢磨。", "talkname3", 0);
            do return end;
::label0::
            Talk(3, "麻烦你转告胡斐，等他准备好了，可随时来找我。", "talkname3", 0);
do return end;
