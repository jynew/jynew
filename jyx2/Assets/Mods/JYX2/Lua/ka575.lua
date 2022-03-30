Talk(105, "客倌想住宿吗？本店有上好客房供应。一间２０两。", "talkname105", 0);
if AskRest() == true then goto  label0 end;
    do return end;
::label0::
    if JudgeMoney(20) == true then goto label1 end;
        Talk(105, "走，走，走，没钱就不要妨碍我做生意！", "talkname105", 0);
        do return end;
::label1::
        Talk(0, "荒野之地多凶险，龙门地界只怕兵祸临头不远。", "talkname0", 1);
        DarkScence();
        Rest();
        AddItemWithoutHint(174, -20);
        SetScencePosition2(14, 14);
		jyx2_MovePlayer("休息后","Level/Dynamic");
        SetRoleFace(3);
        LightScence();
do return end;
