if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(74, "我知道有个山洞，里面藏有很多硝石。地点在（１７２，４２４）。", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
