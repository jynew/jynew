if UseItem(162) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(162, -1);
    SetScenceMap(-2, 1, 21, 30, 3698);--by fanyu|门打开。场景08-编号0607
    SetScenceMap(-2, 1, 21, 31, 0);--by fanyu|门打开。场景08-编号0607
    SetScenceMap(-2, 1, 20, 30, 3696);--by fanyu|门打开。场景08-编号0607
    jyx2_FixMapObject("大伦寺开门",1);
    ModifyEvent(-2, 6, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|门打开。场景08-编号0607
    ModifyEvent(-2, 7, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|门打开。场景08-编号0607
    AddEthics(2);
do return end;
