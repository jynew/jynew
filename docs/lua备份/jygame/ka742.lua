if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(73, "老頑童喜歡養蜜蜂，所以送他一瓶玉蜂漿好招蜂來．你會有好報的．", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
