Talk(0, "看来，我是来到蜘蛛精的巢穴了。", "talkname0", 1);
if TryBattle(61) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    ModifyEvent(-2, 3, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|战斗结束，移除蜘蛛。场景10-编号3-9
    ModifyEvent(-2, 4, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 5, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 6, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 7, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 8, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 9, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
	jyx2_ReplaceSceneObject("","NPC/zhizhu3","");
	jyx2_ReplaceSceneObject("","NPC/zhizhu4","");
	jyx2_ReplaceSceneObject("","NPC/zhizhu5","");
	jyx2_ReplaceSceneObject("","NPC/zhizhu6","");
    LightScence();
    AddRepute(1);
do return end;
