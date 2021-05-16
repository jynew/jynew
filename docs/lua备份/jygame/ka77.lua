Talk(0, "＜哇！這裡戒備森嚴，如臨大敵似的，我可得小心一點．＞小弟遠從中原來到這西域，請問這裡就是明教吧？", "talkname0", 1);
Talk(80, "從中原來的？想必是六大派的爪牙．竟敢如此大搖大擺走進來，莫非欺我明教無能人？", "talkname80", 0);
Talk(0, "我．．．．．．", "talkname0", 1);
Talk(80, "先拿下再說！", "talkname80", 0);
if TryBattle(8) == false then goto label0 end;
    ModifyEvent(-2, 1, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|战斗后移除人物。场景12-编号1
    ModifyEvent(-2, 2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|战斗后移除人物。场景12-编号2
    ModifyEvent(-2, 3, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu|战斗后移除人物。场景12-编号3
    jyx2_ReplaceSceneObject("", "NPC/NPC2", "");--喽喽死掉
    jyx2_ReplaceSceneObject("", "NPC/NPC3", "");--喽喽死掉
    jyx2_ReplaceSceneObject("", "NPC/NPC4", "");--喽喽死掉
    jyx2_ReplaceSceneObject("","GasWalls/Wall1","");
    LightScence();
    AddRepute(2);
    do return end;
::label0::
    Dead();
do return end;
