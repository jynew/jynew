if JudgeEventNum(4, 0) == true then goto label0 end;
    Talk(100, "这门打不开，应该是徐谦养鸽子的地方");
    do return end;
::label0::
    if JudgeEventNum(5, 0) == true then goto label1 end;
        Talk(100, "这门打不开，应该是徐谦养鸽子的地方");
        do return end;
::label1::
        if JudgeEventNum(6, 0) == true then goto label2 end;
            Talk(100, "这门打不开，应该是徐谦养鸽子的地方");
            do return end;
::label2::
            Talk(100, "今天这个门怎么开了？");
            DarkScence();
            jyx2_FixMapObject("鸽子楼开门", 1);
            jyx2_ReplaceSceneObject("", "Dynamic/Entrance", "1");
            ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
            LightScence();
do return end;