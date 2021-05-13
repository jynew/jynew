if UseItem(165) == true then goto label0 end;
    do return end;
::label0::
    ModifyEvent(-2, -2, -2, -2, -1, -1, -1, 2608, 2608, 2608, -2, -2, -2);
    GetItem(39, 1);
    GetItem(5, 5);
    GetItem(186, 2);
    GetItem(99, 5);
    GetItem(170, 1);
    if InTeam(35) == false then goto label1 end;
        do return end;
::label1::
        AddEthics(-1);
do return end;
