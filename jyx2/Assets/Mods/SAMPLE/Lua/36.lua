if InTeam(11) == true then goto label0 end;
    Talk(30, "真没意思，每天练功，又不能出去打架。");
    Talk(0, "哈哈，你这个小师傅还真有意思。");
    do return end;
::label0::
    Talk(30, "有架打了吗？让我也出一份力吧！");
    if AskJoin() == true then goto label1 end;
        Talk(0, "那个，我们现在还没到要打架的时候，到时候我再来叫你啊。");
        do return end;
::label1::
      if TeamIsFull() == false then goto label2 end;
            Talk(30, "你的队伍已满，我无法加入。");
            do return end;
::label2::
            Talk(0, "这样也好，多了一个帮手。");
            DarkScence();
            jyx2_ReplaceSceneObject("", "NPC/虚寂", "");
            LightScence();
            Join(30);
            ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
