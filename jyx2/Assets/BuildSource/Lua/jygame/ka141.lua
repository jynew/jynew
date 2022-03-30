Talk(7, "不知少侠光临我三圣坳有何贵事？", "talkname7", 0);
if AskBattle() == true then goto label0 end;
    Talk(0, "我是来叮咛你的，以后要跟明教和平相处哦！不要再互相残杀了。", "talkname0", 1);
    Talk(7, "哼！", "talkname7", 0);
    do return end;
::label0::
    Talk(0, "我想找你练练功，赚些经验点数。", "talkname0", 1);
    Talk(7, "哼！那就来吧。", "talkname7", 0);
    if TryBattle(19) == true then goto label1 end;
        Dead();
        do return end;
::label1::
        LightScence();
        Talk(0, "嗯，这经验点数还真好赚。", "talkname0", 1);
        AddEthics(-1);
do return end;
