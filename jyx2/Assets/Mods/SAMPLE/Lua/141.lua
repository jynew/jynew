jyx2_ReplaceSceneObject("", "NPC/Wolf_prefab (5)", "1");
jyx2_ReplaceSceneObject("", "NPC/Wolf_prefab (6)", "1");
jyx2_ReplaceSceneObject("", "NPC/Wolf_prefab (7)", "1");
jyx2_ReplaceSceneObject("", "NPC/Wolf_prefab (8)", "1");
jyx2_ReplaceSceneObject("", "NPC/Wolf_prefab (9)", "1");
Talk(0, "糟糕！还有野狼！");
if TryBattle(141) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    jyx2_ReplaceSceneObject("", "NPC/Wolf_prefab (5)", "");
    jyx2_ReplaceSceneObject("", "NPC/Wolf_prefab (6)", "");
    jyx2_ReplaceSceneObject("", "NPC/Wolf_prefab (7)", "");
    jyx2_ReplaceSceneObject("", "NPC/Wolf_prefab (8)", "");
    jyx2_ReplaceSceneObject("", "NPC/Wolf_prefab (9)", "");
    Talk(0, "不太对劲啊！想往回走也来不及了！硬着头皮向前走！");
    ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
