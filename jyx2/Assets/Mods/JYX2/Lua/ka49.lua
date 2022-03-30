Talk(0, "哇塞！这山洞有怪物住着，难道就是传说中的雪怪。糟了，被发现了。", "talkname0", 1);
if TryBattle(5) == false then goto label0 end;
    ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);--by fanyu|杀死雪怪。场景05-编号00
    ModifyEvent(-2, 1, 0, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|杀死雪怪。场景05-编号01
    ModifyEvent(-2, 2, 0, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|杀死雪怪。场景05-编号02
	jyx2_ReplaceSceneObject("","NPC/雪怪1","0");
	jyx2_ReplaceSceneObject("","NPC/雪怪2","0");
    LightScence();
    AddRepute(2);
    do return end;
::label0::
    Dead();
do return end;
