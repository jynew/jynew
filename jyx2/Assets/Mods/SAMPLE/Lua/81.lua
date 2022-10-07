if GetFlagInt("见过徐谦") == 1 then goto label0 end;
    Talk(80, "快去见师父吧，师父可担心你了。");
    do return end;
::label0::
    Talk(80, "六一你最近在忙些什么，感觉你怪怪的，好像变了一个人似的。");
    Talk(0, "我在调查莫桥山庄掌门人莫穿林是怎么死的。");
    Talk(80, "听上去好好玩啊，我也跟你一起吧。");
    if AskJoin() == true then goto label1 end;
        Talk(0, "你还是在家先帮师父干干活吧。");
        do return end;
::label1::
        if TeamIsFull() == false then goto label2 end;
            Talk(80, "你的队伍已满，我无法加入。");
            do return end;
::label2::
        Talk(0, "好啊，我们得去跟师父说一声。");
        DarkScence();
        jyx2_ReplaceSceneObject("", "NPC/童四二", "");
        LightScence();
        Join(80);
        ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
