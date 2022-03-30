Talk(82, "阁下若无请帖，还请离开。", "talkname82", 0);
if AskBattle() == true then goto label0 end;
    do return end;
::label0::
    Talk(0, "说不得只好打了。", "talkname0", 1);
    if TryBattle(27) == true then goto label1 end;
        Dead();
        do return end;
::label1::
        ModifyEvent(-2, 0, -2, -2, 183, -1, -1, 5192, 5192, 5192, -2, 30, 48);
        ModifyEvent(-2, 20, -2, -2, 183, -1, -1, 5186, 5186, 5186, -2, 27, 48);
		jyx2_FixMapObject("衡山守门弟子让路",1);
        ModifyEvent(-2, 1, -2, -2, 183, -1, -1, -2, -2, -2, -2, -2, -2);
        ModifyEvent(-2, 2, -2, -2, 183, -1, -1, -2, -2, -2, -2, -2, -2);
        LightScence();
        AddEthics(-1);
        AddRepute(1);
do return end;
