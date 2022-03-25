Talk(37, "谢谢兄台救了我。", "talkname37", 0);
if AskJoin () == true then goto label0 end;
    do return end;
::label0::
    Talk(0, "这样好了，你要不要和我一起走，路上也有个照应。", "talkname0", 1);
    if JudgeEthics(0, 60, 100) == true then goto label1 end;
        Talk(37, "不了！我这个不幸之人还是别害人的好。", "talkname37", 1);
        do return end;
::label1::
        if TeamIsFull() == false then goto label2 end;
            Talk(37, "你的队伍已满，我无法加入。", "talkname37", 0);
            do return end;
::label2::
            Talk(37, "好吧！如果你不怕被我这个不幸之人连累的话。", "talkname37", 0);
            DarkScence();
            ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2); --by fanyu|狄云加入队伍。场景08-编号08
            jyx2_ReplaceSceneObject("", "NPC/狄云", ""); 
            ModifyEvent(-2, 9, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            ModifyEvent(-2, 10, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            LightScence();
            Join(37);
            AddEthics(3);
do return end;
