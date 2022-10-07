Talk(0, "我一定要尽快查出自己到底是怎么死的，话说看着自己的尸体，还真是不习惯……");
if ShowYesOrNoSelectPanel("是否埋葬尸体？") == true then goto label0 end;
    Talk(0, "我还是先出去转转，找找线索吧。");
    do return end;
::label0::
    Talk(0, "自己亲手埋自己，还真是别有一翻滋味在心头。");
    Talk(0, "奇怪，我的脖子后面有<color=orange>一个很小很小的十字形针孔</color>，这一定就是致死的原因吧！");
    jyx2_FixMapObject("主角移动", 1);
    jyx2_PlayTimelineSimple("[Timeline]1_埋葬", true);
    jyx2_Wait(5.4);
    DarkScence();
    jyx2_ReplaceSceneObject("", "NPC/莫穿林", "");
    jyx2_ReplaceSceneObject("", "Dynamic/Grave", "1");
    jyx2_ReplaceSceneObject("13", "FX/ExitLight (1)", "1");--野狼洞开洞
    jyx2_ReplaceSceneObject("13", "Triggers/Leave2", "1");
    AddItem(0, 1);
    ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 3, -2, -2, 4, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 4, -2, -2, 6, -1, -1, -2, -2, -2, -2, -2, -2);
    SetFlagInt("埋葬尸体", 1);
    LightScence();
do return end;
