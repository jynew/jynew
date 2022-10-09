Talk(70, "竟然有幸结识如此心怀正气的少侠，如果需要帮助的话，随时可以叫我。");
if AskJoin() == true then goto label0 end;
    Talk(0, "罢了。");
    do return end;
::label0::
    if TeamIsFull() == false then goto label1 end;
        Talk(70, "你的队伍已满，我无法加入。");
        do return end;
::label1::
    Talk(0, "我确实还有未完成的事情，不如就请佟掌门助我一臂之力吧。");
    DarkScence();
    jyx2_ReplaceSceneObject("", "NPC/佟雯", "");
    LightScence();
    Join(70);
    ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
