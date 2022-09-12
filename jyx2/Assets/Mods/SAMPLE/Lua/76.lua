if InTeam(70) == true then goto label0 end;
    Talk(0, "这门打不开啊……");
    if InTeam(80) == true then goto label1 end;
        do return end;
::label1::
        Talk(80, "房间里的味道闻上去好舒服，好想进去看看啊。");
        do return end;
::label0::
    Talk(70, "我师父在里面，带你去引荐引荐。");
    DarkScence();
    jyx2_FixMapObject("罂粟谷开门", 1);
    LightScence();
    ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
