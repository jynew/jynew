Talk(150, "客官需要休息吗？");
if AskRest() == true then goto label0 end;
    do return end;
::label0::
    if JudgeMoney(100) == true then goto label1 end;
        Talk(150, "走，走，走，没钱就不要妨碍我做生意！");
        do return end;
::label1::
        Talk(0, "好，我就试试看你们怡麟楼的服务好不好。");
        DarkScence();
        Rest();
        AddItemWithoutHint(174, -100);
	    jyx2_MovePlayer("休息后", "Level/Dynamic");
        SetRoleFace(3);
        LightScence();
        Talk(0, "神清气爽，开始新的旅途。");
do return end;
