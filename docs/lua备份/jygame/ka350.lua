if UseItem(132) == true then goto label0 end;
    do return end;
::label0::
    AddItem(132, -1);
    jyx2_ReplaceSceneObject("", "Gaswall/Wall1", "");--移除空气墙
    Talk(41, "公子請往裡面走，島主已恭候多時了．", "talkname41", 0);
    DarkScence();
    ModifyEvent(-2, -2, -2, -2, 351, -1, -1, 5146, 5146, 5146, -2, 30, 50);
    LightScence();
do return end;
