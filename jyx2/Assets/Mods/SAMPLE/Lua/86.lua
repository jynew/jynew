if JudgeEventNum(7, 0) == true then goto label0 end;
    Talk(0, "万万没想到王远才是凶手。");
    Talk(180, "这只是推测，你可不要冲动。");
    do return end;
::label0::
    ModifyEvent(-2, 1, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
    jyx2_ReplaceSceneObject("", "NPC/岳不群", "");--鸽子楼，徐谦，J
do return end;
