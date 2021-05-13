Talk(51, "閣下考慮的怎麼樣，要不要我加入你，讓你能輕易獲得”天龍八部”？", "talkname51", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "慕容公子的”好”意，在下心領了．在下對自己的武功還有一點自信，我寧願光明正大的與喬大俠打鬥，也不願用卑鄙的方法去得到那本”天龍八部”．", "talkname0", 1);
    Talk(51, "你再考慮清楚．", "talkname51", 0);
    do return end;
::label0::
    Talk(0, "好，我就和你上少林，揭發喬峰的秘密．", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(51, "你的隊伍已滿，我無法加入．", "talkname51", 0);
        do return end;
::label1::
        DarkScence();
        ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        LightScence();
        Join(51);
        AddEthics(-4);
        ChangeMMapMusic(3);
do return end;
