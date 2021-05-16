if UseItem(195) == true then goto label0 end;
    do return end;
::label0::
    Talk(0, "動手挖墓吧！希望別有殭屍跑出來嚇人．", "talkname0", 1);
    PlayAnimation(-1, 6704, 6714);
    PlayAnimation(-1, 6704, 6714);
    PlayAnimation(-1, 6716, 6742);
    PlayAnimation(-1, 6716, 6742);
    PlayAnimation(-1, 6716, 6742);
    PlayAnimation(-1, 6716, 6742);
    DarkScence();
    ModifyEvent(-2, -2, -2, -2, 565, -1, -1, 6698, 6698, 6698, -2, -2, -2);
    PlayAnimation(-1, 6702, 6702);
    LightScence();
    Talk(0, "啊！真累，盜墓的工作真不輕鬆，好在有點收穫．這是什麼東西來著，全書盡是怪異的文字，封皮寫著．．．”廣陵散”．．．", "talkname0", 1);
    GetItem(177, 1);
    ModifyEvent(-2, -2, -2, -2, 565, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
