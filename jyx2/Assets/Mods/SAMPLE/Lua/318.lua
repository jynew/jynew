if UseItem(36) == true then goto label0 end;
    do return end;
::label0::
    ModifyEvent(-2, -2, -2, -2, 319, -1, -1, -2, -2, -2, -2, -2, -2);
    jyx2_ReplaceSceneObject("", "NPC/虚寂 (2)", "1");
    Talk(30, "师兄，经书找到了，原来在蒲团下面。");
    Talk(132, "师弟没有垂涎经书上的武功，确是心地清净。");
    Talk(132, "这本经书也是和你有缘，且听我徐徐讲来这经书武功的奥秘。");
    Talk(30, "多谢师兄。");
    Talk(132, "<color=orange>虎拳猛气不可杀，一日之间百战加。我欲从君问奇字，恐惊龙尾堕苍葭……</color>");
    jyx2_ReplaceSceneObject("", "NPC/虚寂 (2)", "");
    LearnMagic2(30, 33, 1);
do return end;
