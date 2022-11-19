if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(74, "我知道有个山洞，里面藏有很多硝石。地点在（４２４，４２４）。", "talkname74", 0);--两个坐标都是y坐标
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
