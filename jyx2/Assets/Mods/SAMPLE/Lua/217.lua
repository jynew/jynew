if InTeam(90) == true then goto label0 end;
    Talk(0, "这坛子里泡的什么酒，闻起来味道好奇怪。");
    do return end;
::label0::
    if GetFlagInt("妞妞复仇") == 1 then goto label1 end;
        Talk(0, "这坛子里泡的什么酒，闻起来味道好奇怪。");
        do return end;
::label1::
        Talk(0, "童岿然这家伙，实在是罪孽深重。");
        Talk(90, "我想把母亲的骨骸带回去。");
        Talk(0, "先把里面的酒倒掉吧。");
        Talk(90, "娘亲，女儿不孝，这么晚才找到您。");
        jyx2_ReplaceSceneObject("", "Dynamic/Tank_2", "");
        AddItem(24, 1);
        ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
        ModifyEvent(9, 1, -2, -2, 99, 910, -1, -2, -2, -2, -2, -2, -2);
do return end;

