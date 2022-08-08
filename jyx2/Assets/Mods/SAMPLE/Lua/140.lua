jyx2_ReplaceSceneObject("", "NPC/Wolf_prefab", "1");
jyx2_ReplaceSceneObject("", "NPC/Wolf_prefab (1)", "1");
jyx2_ReplaceSceneObject("", "NPC/Wolf_prefab (2)", "1");
jyx2_ReplaceSceneObject("", "NPC/Wolf_prefab (3)", "1");
jyx2_ReplaceSceneObject("", "NPC/Wolf_prefab (4)", "1");
Talk(0, "糟糕！有野狼！");
if TryBattle(140) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    LightScence();
    jyx2_ReplaceSceneObject("", "NPC/Wolf_prefab", "");
    jyx2_ReplaceSceneObject("", "NPC/Wolf_prefab (1)", "");
    jyx2_ReplaceSceneObject("", "NPC/Wolf_prefab (2)", "");
    jyx2_ReplaceSceneObject("", "NPC/Wolf_prefab (3)", "");
    jyx2_ReplaceSceneObject("", "NPC/Wolf_prefab (4)", "");
    Talk(0, "洞里有股血腥味！继续进去看看！");
    ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
