Talk(105, "客倌想住宿吗？本店有上好客房供应。一间２００两。", "talkname105", 0);
Talk(0, "奇怪，招牌上不是写２０两吗？怎么涨价了。", "talkname0", 1);
Talk(105, "你没看这几天有这么多人来西域么？你不要，别人还抢着要呢！", "talkname105", 0);
if AskRest() == true then goto  label0 end;
    do return end;
::label0::
    if JudgeMoney(200) == true then goto label1 end;
        Talk(105, "走，走，走，没钱就不要妨碍我做生意！", "talkname105", 0);
        do return end;
::label1::
        Talk(0, "荒野之地多凶险，龙门地界只怕兵祸临头不远。", "talkname0", 1);
        DarkScence();
        Rest();
        AddItemWithoutHint(174, -200);
        SetScencePosition2(14, 14);
		jyx2_MovePlayer("休息后","Level/Dynamic");
        SetRoleFace(3);
        LightScence();
do return end;
