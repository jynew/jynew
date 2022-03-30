if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "有几个同伴是必需加入的。石破天，段誉，胡斐。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
