if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(73, "有些東西你服用後就可百毒不侵，非常的好用．如白駝山的通犀地龍丸．", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
