if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(73, "”連城訣”一書是藏在某個地方，但它的埋藏地點卻是記錄在一本書裡．一本很普通的書．", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
