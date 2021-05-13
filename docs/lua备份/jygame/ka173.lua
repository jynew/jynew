Talk(21, "閣下又來我恆山派做什麼？我定閒可是不承認這五嶽派的．", "talkname21", 0);
if AskBattle() == true then goto label0 end;
    do return end;
::label0::
    Talk(0, "好啊，你居然不承認我五嶽派，看來得再教訓教訓你．", "talkname0", 1);
    if TryBattle(39) == true then goto label1 end;
        Dead();
        do return end;
::label1::
        LightScence();
        AddEthics(-1);
do return end;
