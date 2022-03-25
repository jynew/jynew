if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(74, "我知道还有几个古墓可以挖掘。一个是在猴掌山。", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
