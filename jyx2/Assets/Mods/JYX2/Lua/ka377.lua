Talk(0, "又有一群大蜘蛛，我得小心点，免得当了蜘蛛的点心。", "talkname0", 1);
if TryBattle(62) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    ModifyEvent(-2, 10, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|战斗结束，移除蜘蛛。场景10-编号10-16
    ModifyEvent(-2, 11, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 12, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 13, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 14, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 15, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 16, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
	jyx2_ReplaceSceneObject("","NPC/zhizhu10","");
	jyx2_ReplaceSceneObject("","NPC/zhizhu11","");
	jyx2_ReplaceSceneObject("","NPC/zhizhu12","");
	jyx2_ReplaceSceneObject("","NPC/zhizhu13","");
    LightScence();
    AddRepute(1);
do return end;
