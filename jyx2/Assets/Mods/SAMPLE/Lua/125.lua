Talk(123, "上次多亏了少侠相救，才保得性命。");
Talk(0, "哈哈，能救隋仙人的命是我的荣幸，这等于是救了渡城百姓的命啊。");
Talk(123, "这朵千彩花送给少侠。");
AddItem(122, 1);
Talk(0, "这真是太感谢了。");
Talk(123, "少侠如果需要我帮忙，请随时召唤。");
if AskJoin() == true then goto label0 end;
    Talk(0, "好的！");
    ModifyEvent(-2, -2, -2, -2, 126, -1, -1, -2, -2, -2, -2, -2, -2);
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
    ModifyEvent(-2, 1, -2, -2, 129, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
