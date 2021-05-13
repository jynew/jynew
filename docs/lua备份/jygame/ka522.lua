Talk(96, "施主若要進入寺內，還請將兵刃留下，待離寺時再歸還予你．", "talkname96", 0);
if AskBattle() == true then goto label0 end;
    Talk(0, "好，好，我下回再來．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "抱歉，恕難從命．", "talkname0", 1);
    if TryBattle(79) == true then goto label1 end;
        LightScence();
        Talk(96, "請施主下山．", "talkname96", 0);
        Talk(0, "可是我還是想進去看看，對不住了．", "talkname0", 1);
        do return end;
::label1::
        ModifyEvent(-2, 3, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|移除人物。场景28-3
        ModifyEvent(-2, 4, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|移除人物。场景28-4
        ModifyEvent(-2, 5, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|移除人物。场景28-5
        ModifyEvent(-2, 6, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|移除人物。场景28-6
        jyx2_ReplaceSceneObject("","GasWalls/Wall1","");
        LightScence();
        AddRepute(1);
do return end;
