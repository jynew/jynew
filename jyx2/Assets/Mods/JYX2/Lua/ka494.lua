Talk(51, "阁下考虑的怎么样，要不要我加入你，让你能轻易获得《天龙八部》？", "talkname51", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "慕容公子的“好”意，在下心领了。在下对自己的武功还有一点自信，我宁愿光明正大的与乔大侠打斗，也不愿用卑鄙的方法去得到那本《天龙八部》。", "talkname0", 1);
    Talk(51, "你再考虑清楚。", "talkname51", 0);
    do return end;
::label0::
    Talk(0, "好，我就和你上少林，揭发乔峰的秘密。", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(51, "你的队伍已满，我无法加入。", "talkname51", 0);
        do return end;
::label1::
        DarkScence();
        ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
		jyx2_ReplaceSceneObject("", "NPC/慕容复", "");
        LightScence();
        Join(51);
        AddEthics(-4);
        ChangeMMapMusic(3);
do return end;
