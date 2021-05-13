if HaveItem(173) == false then goto label0 end;
    ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
    Talk(0, "咦！頭比較不暈了，反而有股淡淡的微香，這朵藍花真是神奇．", "talkname0", 1);
    jyx2_ReplaceSceneObject("", "Bake/Static/Wall/Wall_22_423", "");--kaimen
    do return end;
::label0::
    Talk(0, "啊！又聞到這香味了，看來我又得昏倒了．", "talkname0", 1);
    PlayAnimation(-1, 5974, 5992);
    DarkScence();
    SetScencePosition2(30, 49);
    LightScence();
    PlayAnimation(-1, 6014, 6024);
    Talk(0, "又昏倒了，看來我得想想辦法才是．", "talkname0", 1);
do return end;
