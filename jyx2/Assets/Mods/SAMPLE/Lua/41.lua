Talk(0, "这位就是渡城武痴萨擎苍萨掌门吧。");
Talk(40, "怎么，想来练练吗？");
if AskBattle() == true then goto label0 end;
    Talk(0, "我哪是您的对手。");
    do return end;
::label0::
    Talk(0, "那就讨教讨教！");
    if TryBattle(40) == false then goto label1 end;
        LightScence();
        Talk(40, "少侠武功了得啊！");
        do return end;
::label1::
        LightScence();
        Talk(40, "哈哈，你还得多练练，随时可以来找我切磋。");
do return end;
