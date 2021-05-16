if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(74, "我那個鄰居胡小子缺個女人陪他，找個女人陪他的話，他會感激不盡的．", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
