if JudgeEventNum(0, 0) == true then goto label0 end;
    Talk(0, "四二，你先回鸽子楼，帮我照顾好师父，我过些日子就回来。");
    Leave(80);
    ModifyEvent(8, 0, -2, -2, 814, -1, -1, -2, -2, -2, -2, -2, -2);
    jyx2_ReplaceSceneObject("8", "NPC/童四二", "1");
    do return end;
::label0::
    Talk(0, "四二，你先回罂粟谷，陪陪你娘亲，我过些日子再去找你。");
    ModifyEvent(7, 6, -2, -2, 716, -1, -1, -2, -2, -2, -2, -2, -2);
    jyx2_ReplaceSceneObject("7", "NPC/童四二", "1");
do return end;
