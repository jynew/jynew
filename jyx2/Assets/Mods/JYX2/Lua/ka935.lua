if UseItem(189) == true then goto label0 end;
    do return end;
::label0::
    Talk(81, "武林大会即将展开，请少侠赶快进场。", "talkname81", 0);
    DarkScence();
    ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    jyx2_ReplaceSceneObject("","NPC/华山弟子24","");
    LightScence();
do return end;
