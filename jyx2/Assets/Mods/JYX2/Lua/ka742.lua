if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "老顽童喜欢养蜜蜂，所以送他一瓶玉蜂浆好招蜂来。你会有好报的。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
