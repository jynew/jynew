if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(73, "俠客島上的武功秘密幾十年來無人解得出來，原因就是讀過書的人太鑽牛角尖了．試試看帶沒讀過書的石破天去碰碰運氣好了．", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
