Talk(22, "小兄弟，我看你武功不错，你我二人一起称霸这江湖，如何？", "talkname22", 0);
Talk(0, "你武功那么差，我看你还是安份一点。", "talkname0", 1);
Talk(22, "上回是老朽太轻敌了，你还想试试看吗？", "talkname22", 0);
if AskBattle() == true then goto label0 end;
    do return end;
::label0::
    if TryBattle(38) == true then goto label1 end;
        Dead();
        do return end;
::label1::
        LightScence();
do return end;
