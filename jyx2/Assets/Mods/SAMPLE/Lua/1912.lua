if UseItem(195) == true then goto label0 end;
    do return end;
::label0::
    if InTeam(11) == true then goto label1 end;
        do return end;
::label1::
        Talk(1195, "请进！");
        DarkScence();
        jyx2_FixMapObject("无际坊左侍卫让路", 1);
        jyx2_FixMapObject("无际坊右侍卫让路", 1);
        ModifyEvent(-2, -2, -2, -2, 1913, -1, -1, -2, -2, -2, -2, -2, -2);
        LightScence();
do return end;
