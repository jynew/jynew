Talk(80, "六一，还需要我的帮助吗？");
if AskJoin() == true then goto label0 end;
    Talk(0, "暂时还不用。");
    do return end;
::label0::
    if TeamIsFull() == false then goto label1 end;
        Talk(80, "你的队伍已满，我无法加入。");
        do return end;
::label1::
    Talk(0, "还是来帮帮我吧。");
    DarkScence();
    jyx2_ReplaceSceneObject("", "NPC/童四二", "");
    LightScence();
    Join(80);
    ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
