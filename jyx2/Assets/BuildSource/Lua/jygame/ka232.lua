Talk(0, "好啊，你们居然在这里计划要杀人，亏你们还是同盟的帮派。", "talkname0", 1);
Talk(84, "小子，偷听了我们的计划，只好杀了你灭口。怪不得我们了。上！", "talkname84", 0);
if TryBattle(42) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    ModifyEvent(-2, 14, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 15, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 16, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
	jyx2_ReplaceSceneObject("", "NPC/嵩山弟子14", "");--嵩山弟子打败离开
	jyx2_ReplaceSceneObject("", "NPC/嵩山弟子15", "");--嵩山弟子打败离开
	jyx2_ReplaceSceneObject("", "NPC/嵩山弟子16", "");--嵩山弟子打败离开
    LightScence();
	Add3EventNum(27, 0, 0, 0, 1);--需要拜访嵩山以外4派才会触发五岳并派事件。嵩山入口0号trigger对应起始事件为198，每拜访1派事件序号+1，202号事件为并派事件。
    AddEthics(3);
    AddRepute(1);
do return end;
