if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(74, "我那个邻居胡小子刀谱缺了两页，练功练的很郁闷。如果你能帮他找回失物，他肯定会感激不尽的。", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
