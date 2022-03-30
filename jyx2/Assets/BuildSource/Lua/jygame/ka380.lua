Talk(0, "又有一群大蜘蛛，我得小心点，免得当了蜘蛛的点心。", "talkname0", 1);
if TryBattle(65) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    ModifyEvent(-2, 36, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|战斗结束，移除蜘蛛。场景10-编号36-41
    ModifyEvent(-2, 37, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 38, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 39, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 40, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 41, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
	jyx2_ReplaceSceneObject("","NPC/zhizhu36","");
	jyx2_ReplaceSceneObject("","NPC/zhizhu37","");
	jyx2_ReplaceSceneObject("","NPC/zhizhu38","");
	jyx2_ReplaceSceneObject("","NPC/zhizhu39","");
	jyx2_ReplaceSceneObject("","NPC/zhizhu40","");
    LightScence();
    AddRepute(1);
do return end;
