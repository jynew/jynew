Talk(91, "小子，竟敢擅闯我大轮寺，找死！", "talkname91", 0);
if TryBattle(92) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    ModifyEvent(-2, 2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|敌人死亡贴图变没。场景08-编号02
    ModifyEvent(-2, 3, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|敌人死亡贴图变没。场景08-编号03
    ModifyEvent(-2, 4, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|敌人死亡贴图变没。场景08-编号04
    ModifyEvent(-2, 5, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|移除贴图。场景08-编号05
    jyx2_ReplaceSceneObject("","NPC/LaMa (2)","");
    jyx2_ReplaceSceneObject("","NPC/LaMa (3)","");
    jyx2_ReplaceSceneObject("","NPC/LaMa (4)","");
    LightScence();
    AddItem(162, 1);
    AddRepute(1);
do return end;
