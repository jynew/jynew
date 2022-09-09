Talk(90, "六一哥哥，想我了吧，快带我去玩吧！");
if AskJoin() == true then goto label0 end;
    Talk(0, "我现在忙得狠，下次吧。");
    Talk(90, "哼！");
    do return end;
::label0::
    if TeamIsFull() == false then goto label1 end;
        Talk(90, "你的队伍已满，我无法加入。");
        do return end;
::label1::
    Talk(0, "真是怕了你。");
    Talk(90, "走走走，带我闯荡江湖去。");
    DarkScence();
    jyx2_ReplaceSceneObject("", "NPC/牛妞妞", "");
    LightScence();
    Join(90);
    ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;