Talk(0, "咦！把自己的坟墓挖了能找到什么线索吗，想想都心痒。");
if ShowYesOrNoSelectPanel("是否挖掘坟墓？") == true then goto label0 end;
    Talk(0, "我又不是傻子，怎么会有这么奇怪的想法。");
    AddAptitude(0, 20);
    ModifyEvent(-2, -2, -2, -2, 7, -1, -1, -2, -2, -2, -2, -2, -2);
    do return end;
::label0::
    jyx2_FixMapObject("挖坟移动", 1);
    jyx2_PlayTimelineSimple("[Timeline]1_挖坟", true);
    jyx2_Wait(5.4);
    DarkScence();
    jyx2_ReplaceSceneObject("", "NPC/莫穿林 (2)", "1");
    jyx2_ReplaceSceneObject("", "Dynamic/Grave/Tomb", "");
    LightScence();
    Talk(0, "并没有什么线索，明明是自己埋的还挖开，我想我脑子坏掉了。");
    AddAptitude(0, -20);
    ModifyEvent(-2, -2, -2, -2, 9, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 5, -2, -2, 8, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
