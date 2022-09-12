Talk(70, "有幸结识如此潇洒威武的少侠，如果需要帮助，随时可以叫我。");
if AskJoin() == true then goto label0 end;
    Talk(0, "罢了。");
    do return end;
::label0::
    if TeamIsFull() == false then goto label1 end;
        Talk(70, "你的队伍已满，我无法加入。");
        do return end;
::label1::
    Talk(0, "我还有未完成的事情，那你就随我一起吧。");
    DarkScence();
    jyx2_ReplaceSceneObject("", "NPC/王语嫣", "");
    LightScence();
    Join(70);
    ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
