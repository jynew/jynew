if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(74, "我那个邻居胡小子缺个女人陪他，找个女人陪他的话，他会感激不尽的。", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
