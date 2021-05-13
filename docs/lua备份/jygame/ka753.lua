if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(74, "告訴你一個極大的秘密：武林中有三大寶藏！第一個寶藏是在”悅來客棧”中．", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
