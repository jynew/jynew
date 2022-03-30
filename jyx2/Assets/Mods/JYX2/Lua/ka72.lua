Talk(9, "你能带我去找义父吗？", "talkname9", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "这恐怕不行，我还有许多要事在身，无法带你去找他。", "talkname0", 1);
    Talk(9, "…………", "talkname9", 0);
    do return end;
::label0::
    Talk(0, "好啊，我就带你去找他。", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(9, "你的队伍已满，我无法加入。", "talkname9", 0);
        do return end;
::label1::
        Talk(9, "谢谢这位大哥。", "talkname9", 0);
        DarkScence();
        jyx2_ReplaceSceneObject("", "NPC/张无忌", ""); 
        ModifyEvent(-2, -2, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2);
        LightScence();
        Join(9);
        AddEthics(2);
do return end;
