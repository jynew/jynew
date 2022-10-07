if InTeam(80) == true then goto label0 end;
    Talk(170, "请离开吧……");
    do return end;
::label0::
    Talk(80, "娘！原来你是我娘！");
    Talk(170, "哎，看来你都知道了。");
    Talk(80, "我长这么大都没有和娘在一起生活过，六一兄，对不住了，我先不陪你了，我想在这里陪陪我娘。");
    Talk(0, "好的！兄弟保重！");
    Leave(80);
    jyx2_ReplaceSceneObject("", "NPC/童四二", "1");
    ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 3, -2, -2, 723, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 6, -2, -2, 717, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 0, -2, -2, -1, -1, 718, -2, -2, -2, -2, -2, -2);
    SetFlagInt("四二认母", 1);
do return end;
