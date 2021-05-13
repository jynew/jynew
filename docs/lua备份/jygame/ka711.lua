if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(73, "製造藥丸非常簡單，只要你隊伍中有人修練醫書，又有藥材的話就可製造出來．", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
