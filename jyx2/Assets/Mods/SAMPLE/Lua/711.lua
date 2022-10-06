if InTeam(70) == true then goto label0 end;
    Talk(0, "这箱子似乎被锁上了。");
    do return end;
::label0::
    Talk(70, "这箱子我来打开。");
    AddItem(77, 1);
    AddItem(71, 20);
    AddItem(72, 20);
    AddItem(174, 300);
    ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
