Talk(0, "四二，我最近遇到了一些困难。");
Talk(80, "需不需要我来帮忙？");
if AskJoin() == true then goto label0 end;
    Talk(0, "你还是先陪陪娘吧。");
    Talk(80, "也好，兄弟有需要再来找我。");
    do return end;
::label0::
    if TeamIsFull() == false then goto label1 end;
        Talk(80, "你的队伍已满，我无法加入。");
        do return end;
::label1::
    Talk(0, "我正有此意，兄弟还是来帮我一把吧。");
    Talk(80, "好的！");
    DarkScence();
    jyx2_ReplaceSceneObject("", "NPC/童四二", "");
    LightScence();
    Join(80);
    ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
