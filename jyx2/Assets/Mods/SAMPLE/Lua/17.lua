Talk(112, "非本帮派人士不得入内！");
if InTeam(2) == true then goto label0 end;
    do return end;
::label0::
    Talk(2, "这位兄台，我是莫桥山庄的朱云天，我们想进去找刘掌门打听一些事情。");
    Talk(112, "原来是朱少侠，请进！");
    DarkScence();
    jyx2_FixMapObject("万烛山庄守门弟子让路", 1);
    ModifyEvent(-2, -2, -2, -2, 18, -1, -1, -2, -2, -2, -2, -2, -2);
    LightScence();
do return end;
