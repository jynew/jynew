if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(74, "要進藥王莊很簡單，只要在身上配朵”情花”即可克制四周的紅樹迷毒．", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
