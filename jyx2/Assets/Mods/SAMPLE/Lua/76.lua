if InTeam(70) == true then goto label0 end;
    Talk(0, "这门打不开啊。。。");
    do return end;
::label0::
    Talk(70, "我师傅在里面，带你去引荐引荐。");
    DarkScence();
    jyx2_FixMapObject("罂粟谷开门", 1);
    LightScence();
do return end;
