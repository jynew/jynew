if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(74, "上次說錯了，第一個寶藏是在”龍門客棧”中．", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
