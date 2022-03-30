if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(74, "梅庄大庄主黄钟公钟情于琴曲，音律。如果你能找到珍贵的曲谱，或许……", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
