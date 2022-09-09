Talk(0, "听说这里人迹罕至，时常有黑熊出没。");
if InTeam(30) == true then goto label0 end;
    do return end;
::label0::
    jyx2_ReplaceSceneObject("", "NPC/Brown_bear_prefab", "1");
    ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 6, -2, -2, -1, -1, 162, -2, -2, -2, -2, -2, -2);
do return end;
