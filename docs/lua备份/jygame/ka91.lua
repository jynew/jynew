Talk(0, "好啊！成崑，原來你躲在這裡，怎麼，幾個壞蛋聚在這裡，是不是又在一起商量什麼壞勾當？", "talkname0", 1);
Talk(18, "哼！上次的事全被你壞了，我這次饒不了你．", "talkname18", 0);
Talk(0, "手下敗將還說大話，這次得小心一點，可別再讓你跑了．", "talkname0", 1);
if TryBattle(13) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    ModifyEvent(-2, 0, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 1, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 3, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 4, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    LightScence();
    Talk(0, "今天真是大快人心，替武林除了一個大害．", "talkname0", 1);
    GetItem(191, 1);
    AddRepute(5);
do return end;
