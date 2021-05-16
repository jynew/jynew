Talk(0, "莫大先生，近來可好？", "talkname0", 1);
Talk(20, "小兄弟，我看你本質不差，給你一點忠告：對岳掌門要小心點．", "talkname20", 0);
if AskBattle() == true then goto label0 end;
    do return end;
::label0::
    Talk(0, "你說什麼？是不是不服氣岳先生當上五嶽派的掌門．看來得再給你點苦頭吃吃．", "talkname0", 1);
    if TryBattle(41) == true then goto label1 end;
        Dead();
        do return end;
::label1::
        LightScence();
        AddEthics(-1);
do return end;
