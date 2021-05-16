if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(74, "我知道有個山洞，裡面藏有很多硝石．地點在（１７２，１７２）", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
