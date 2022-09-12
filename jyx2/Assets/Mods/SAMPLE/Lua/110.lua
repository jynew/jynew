Talk(0, "怎么样朱兄，找到什么线索了吗？");
Talk(11, "我还是毫无头绪，我看还是让我跟你一起调查吧。");
if AskJoin() == true then goto label0 end;
    Talk(0, "朱兄，我们还是各自去找线索，到时候再一起交流。");
    do return end;
::label0::
    if TeamIsFull() == false then goto label1 end;
        Talk(11, "你的队伍已满，我无法加入。");
        do return end;
::label1::
    Talk(100, "这家伙虽然脑袋不好用，但是好歹也多个帮手。");
    Talk(0, "好啊，求之不得，我们一起去探寻真相吧。");
    DarkScence();
    jyx2_ReplaceSceneObject("", "NPC/朱云天", "");
    LightScence();
    Join(11);
    ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
