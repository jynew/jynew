Talk(17, "少俠如果有需要的話，儘管說出來．", "talkname17", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "改日如果有需要時，我一定會來找王前輩．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "好吧！那就麻煩王前輩與我一起奔波江湖了．", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(17, "你的隊伍已滿，我無法加入．", "talkname17", 0);
        do return end;
::label1::
        DarkScence();
        ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        LightScence();
        Join(17);
        AddEthics(1);
do return end;
