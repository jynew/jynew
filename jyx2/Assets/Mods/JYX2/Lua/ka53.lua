if UseItem(135) == true then goto label0 end;
    do return end;
::label0::
    ModifyEvent(-2, -2, -1, -1, -1, -1, -1, -2, -2, -2, -2, -2, -2);
    SetScenceMap(-2, 1, 20, 20, 3694); --by fanyu|门打开。场景05-编号08
    SetScenceMap(-2, 1, 20, 21, 4064);--by fanyu|门打开。场景05-编号08
    SetScenceMap(-2, 1, 21, 21, 0);--by fanyu|门打开。场景05-编号08
    jyx2_FixMapObject("闯王山洞开门",1);
    
do return end;
