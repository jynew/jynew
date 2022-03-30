if JudgeScenePic(80, 1, -6068, 0, 21) then goto label0 end;--如果小龙女不在绝情谷底，杨过直接回古墓
    Talk(0, "杨兄，请你先回神雕穴中，若有需要你帮忙时，我再去找你。", "talkname0", 1);
    Leave(58);
    ModifyEvent(7, 6, 1, 1, 991, -1, -1, 6186, 6186, 6186, 0, -2, -2);
    jyx2_ReplaceSceneObject("7","NPC/杨过","1");
    do return end;
::label0::
    Talk(0, "杨兄，请你先回古墓，若有需要你帮忙时，我再去找你。", "talkname0", 1);
    Leave(58);
    ModifyEvent(18, 1, 1, 1, 991, -1, -1, 6188, 6188, 6188, 0, -2, -2);
	jyx2_ReplaceSceneObject("18","NPC/杨过","1");
    SetScenceMap(18, 1, 44, 31, 0);
    SetScenceMap(18, 1, 44, 30, 0);
    jyx2_FixMapObject("古墓开门",1);
do return end;
