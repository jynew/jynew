if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(74, "梅庄三庄主秃笔翁钟情于书法，如果你能找到名家的书法帖子，或许……", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
