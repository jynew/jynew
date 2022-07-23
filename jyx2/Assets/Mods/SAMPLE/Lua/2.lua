Talk(0, "我一定要尽快查出自己到底是怎么死的，话说看着自己的尸体，还真是不习惯…");
if ShowYesOrNoSelectPanel("是否埋葬尸体？") == true then goto label0 end;
    Talk(0, "我还是先出去转转，找找线索吧。");
    do return end;
::label0::
    Talk(0, "自己亲手埋自己，还真是别有一翻滋味在心头。");
    Talk(0, "奇怪，我的脖子后面有一个很小很小的十字形状针孔，这一定就是致死的原因吧！");
    jyx2_ReplaceSceneObject("", "Player/丐帮弟子", "");
    jyx2_PlayTimelineSimple("[Timeline]1_埋葬", false);
    jyx2_Wait(5.4);
    AddItem(168, 1);
    jyx2_ReplaceSceneObject("", "NPC/杨逍", "");
    jyx2_ReplaceSceneObject("", "Dynamic/Grave", "1");
    jyx2_ReplaceSceneObject("", "Player/丐帮弟子", "1");
    ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
    do return end;
