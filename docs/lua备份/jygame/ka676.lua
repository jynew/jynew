instruct_50(138, 139, 140, 141, 142, 6, 0);
Talk(75, "想要”玉笛誰家聽落梅”，就趕緊將羊羔坐臀，小豬耳朵，小牛腰子，獐腿肉，兔肉這五種材料找來．", "talkname75", 0);
do return end;
AddItem(138, -1);
AddItem(139, -1);
AddItem(140, -1);
AddItem(141, -1);
AddItem(142, -1);
Talk(75, "好，材料通通都有了，我馬上就把”玉笛誰家聽落梅”作給你．", "talkname75", 0);
DarkScence();
ModifyEvent(-2, 0, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
ModifyEvent(-2, 1, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
ModifyEvent(-2, 2, 1, 1, -1, -1, -1, 7536, 7536, 7536, -2, -2, -2);--by fanyu 改变贴图 场景32-编号2
ModifyEvent(-2, 3, 1, 1, -1, -1, -1, 7580, 7580, 7580, -2, -2, -2);--by fanyu 改变贴图 场景32-编号3
SetRoleFace(0);
LightScence();
Play2Amination(2, 7536, 7578, 3, 7580, 7622, 44);
Play2Amination(2, 7536, 7578, 3, 7580, 7622, 44);
Play2Amination(2, 7536, 7578, 3, 7580, 7622, 14);
DarkScence();
ModifyEvent(-2, 2, 1, 1, -1, -1, -1, 2718, 2718, 2718, -2, -2, -2);--by fanyu 改变贴图 场景32-编号2
ModifyEvent(-2, 3, 1, 1, -1, -1, -1, 2720, 2720, 2720, -2, -2, -2);--by fanyu 改变贴图 场景32-编号3
ModifyEvent(-2, 5, 1, 1, 689, -1, -1, 5100, 5100, 5100, -2, -2, -2);--by fanyu 改变贴图，启动脚本689 场景32-编号5
ModifyEvent(-2, 4, -2, -2, -1, -1, 677, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本677 场景32-编号4
LightScence();
Talk(75, "好了，完成了，這一道”玉笛誰家聽落梅”你拿去吧．", "talkname75", 0);
Talk(0, "謝謝林師父．", "talkname0", 1);
GetItem(176, 1);
do return end;
