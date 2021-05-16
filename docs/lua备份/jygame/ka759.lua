if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(74, "我那個鄰居胡小子刀譜缺了兩頁，練功練的很鬱悶．如果你能幫他找回失物，他肯定會感激不盡的．", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
