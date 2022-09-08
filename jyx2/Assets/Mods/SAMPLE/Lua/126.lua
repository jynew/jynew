Talk(123, "少侠如果需要我帮忙，请随时召唤。");
if AskJoin() == true then goto label0 end;
    Talk(0, "好的！");
    do return end;
::label0::
    if TeamIsFull() == false then goto label1 end;
        Talk(123, "你的队伍已满，我无法加入。");
        do return end;
::label1::
    Talk(0, "那太好了，有神医相助，我一定能更顺利的找到答案。");
    DarkScence();
    jyx2_ReplaceSceneObject("", "NPC/苏星河", "");
    LightScence();
    Join(123);
    ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
