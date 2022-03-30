Talk(0, "今天嵩山顶上似乎很热闹？", "talkname0", 1);
Talk(84, "今日是我五岳剑派并派的大日子。闲杂人等，还请离去。", "talkname84", 0);
Talk(0, "这样大的盛会，怎能少得了大爷我。快让让。", "talkname0", 1);
Talk(84, "阁下再不离去，休怪我们不客气了。", "talkname84", 0);
Talk(0, "我正有此意。", "talkname0", 1);
if TryBattle(29) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    ModifyEvent(-2, 1, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
	jyx2_ReplaceSceneObject("","NPC/嵩山弟子1","");
    ModifyEvent(-2, 2, -2, -2, 205, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 3, -2, -2, 205, -1, -1, -2, -2, -2, -2, -2, -2);
    LightScence();
    AddRepute(2);
do return end;
