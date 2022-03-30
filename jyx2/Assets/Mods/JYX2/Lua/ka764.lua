if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(74, "梅庄四庄主丹青生钟情于丹青，图画。如果你能找到珍贵的图画，或许……", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
