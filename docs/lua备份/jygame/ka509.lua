Talk(96, "施主若要進入寺內，還請將兵刃留下，待離寺時再歸還予你．", "talkname96", 0);
Talk(0, "抱歉，恕難從命．", "talkname0", 1);
if TryBattle(80) == true then goto label0 end;
    LightScence();
    Talk(96, "請施主下山．", "talkname96", 0);
    Talk(0, "可是我還是想進去看看，對不住了．", "talkname0", 1);
    do return end;
::label0::
    ModifyEvent(-2, 7, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu战斗结束，移除npc，可以通过，场景28-7
    ModifyEvent(-2, 8, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu战斗结束，移除npc，可以通过，场景28-8
    ModifyEvent(-2, 9, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu战斗结束，移除npc，可以通过，场景28-9
    jyx2_ReplaceSceneObject("","GasWalls/Wall2","");
    LightScence();
    AddRepute(2);
do return end;
