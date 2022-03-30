Talk(38, "我要去找妈妈跟小黄。", "talkname38", 0);
if AskJoin () == true then goto label0 end;
    do return end;
::label0::
    Talk(0, "你要找你妈妈？我正好在四处旅行，不妨我们结伴一起走，好吗？", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(38, "你的队伍已满，我无法加入。", "talkname38", 0);
        do return end;
::label1::
        Talk(38, "好啊！", "talkname38", 0);
        DarkScence();
        ModifyEvent(-2, 7, 0, 0, -1, -1, -1, -1, -1, -1, -1, -2, -2);
        ModifyEvent(-2, 8, 0, 0, -1, -1, -1, -1, -1, -1, -1, -2, -2);
        jyx2_ReplaceSceneObject("", "NPC/石破天", "");--石破天加入队伍
        LightScence();
        Join(38);
        AddEthics(1);
do return end;
