Talk(160, "闲杂人等不得入内！");
if InTeam(11) == true then goto label0 end;
    do return end;
::label0::
    jyx2_ReplaceSceneObject("", "NPC/朱云天", "1");
    Talk(11, "这位兄台，我是莫桥山庄的朱云天，我们想进去找王将军报告一些事情。");
    Talk(160, "朱少侠请进！");
    DarkScence();
    jyx2_FixMapObject("将军府守门士兵让路", 1);
    jyx2_ReplaceSceneObject("", "NPC/朱云天", "");
    ModifyEvent(-2, 1, -2, -2, 62, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 13, -2, -2, 62, -1, -1, -2, -2, -2, -2, -2, -2);
    LightScence();
do return end;
