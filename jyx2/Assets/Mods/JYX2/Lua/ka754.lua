if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(74, "上次说错了，第一个宝藏是在“龙门客栈”中。", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
