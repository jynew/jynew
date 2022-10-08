if InTeam(90) == true then goto label0 end;
    Talk(0, "这箱子似乎被锁上了。");
    do return end;
::label0::
    Talk(90, "这箱子我来打开。");
    AddItem(92, 1);
    ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
