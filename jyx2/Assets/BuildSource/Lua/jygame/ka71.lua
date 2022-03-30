if UseItem(181) == true then goto label0 end;
    do return end;
::label0::
    ModifyEvent(-2, -2, -2, -2, 72, -2, -2, -2, -2, -2, -2, -2, -2);
    AddItemWithoutHint(181, -1);
    Talk(9, "这……这一撮金毛是……", "talkname9", 0);
    Talk(0, "你义父是不是长的一头金发？", "talkname0", 1);
    Talk(9, "是啊。义父人称金毛狮王，当然是满头的金发。", "talkname9", 0);
    Talk(0, "那没错了，我曾在北方的一个荒岛上见过他。", "talkname0", 1);
    Talk(9, "真的？你知道荒岛的位置吗，快带我去找他。", "talkname9", 0);
    if AskJoin () == true then goto label1 end;
        Talk(0, "这恐怕不行，我还有许多要事在身，无法带你去找他。", "talkname0", 1);
        Talk(9, "…………", "talkname9", 0);
        do return end;
::label1::
        Talk(0, "好啊，我就带你去找他。", "talkname0", 1);
        if TeamIsFull() == false then goto label2 end;
            Talk(9, "你的队伍已满，我无法加入。", "talkname9", 0);
            do return end;
::label2::
            Talk(9, "谢谢这位大哥。", "talkname9", 0);
            DarkScence();
            ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);  --by fanyu|加入队伍人物消失。场景04-编号01
            jyx2_ReplaceSceneObject("", "NPC/张无忌", ""); 
            LightScence();
            Join(9);
            AddEthics(2);
do return end;
