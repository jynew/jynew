jyx2_ReplaceSceneObject("", "NPC/Brown_bear_prefab (2)", "1");
Talk(0, "哇！有黑熊！");
if TryBattle(161) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    jyx2_ReplaceSceneObject("", "NPC/Brown_bear_prefab (2)", "");
    Talk(0, "这家伙好凶！");
    AddItem(161, 5);
    ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
