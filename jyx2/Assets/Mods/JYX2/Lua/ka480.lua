Talk(105, "客倌，看你一身打扮，不像是本地人，大老远赶来，想必旅途一定劳累了。要不要在这住上一宿，让你的体力，元气恢复恢复。", "talkname105", 0);
if AskRest() == true then goto  label0 end;
    do return end;
::label0::
    if JudgeMoney(50) == true then goto label1 end;
        Talk(105, "走，走，走，没钱就不要妨碍我做生意！", "talkname105", 0);
        do return end;
::label1::
        Talk(0, "好，我就试试看你们高升客栈的服务好不好。", "talkname0", 1);
        DarkScence();
        Rest();
        AddItemWithoutHint(174, -50);
        SetScencePosition2(38, 18);
		jyx2_MovePlayer("休息后","Level/Dynamic");
        SetRoleFace(3);
        LightScence();
do return end;
