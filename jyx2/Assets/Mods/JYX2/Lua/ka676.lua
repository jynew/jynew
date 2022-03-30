--instruct_50(138, 139, 140, 141, 142, 6, 0);
if HaveItem(138) and  HaveItem(139) and  HaveItem(140) and  HaveItem(141) and  HaveItem(142) then goto labelS end;
	Talk(75, "想要“玉笛谁家听落梅”，就赶紧将羊羔坐臀，小猪耳朵，小牛腰子，獐腿肉，兔肉这五种材料找来。", "talkname75", 0);
	do return end;

::labelS::
	AddItemWithoutHint(138, -1);
	AddItemWithoutHint(139, -1);
	AddItemWithoutHint(140, -1);
	AddItemWithoutHint(141, -1);
	AddItemWithoutHint(142, -1);
	Talk(75, "好，材料通通都有了，我马上就把“玉笛谁家听落梅”做给你。", "talkname75", 0);
	DarkScence();
	ModifyEvent(-2, 0, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
	ModifyEvent(-2, 1, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
	ModifyEvent(-2, 2, 1, 1, -1, -1, -1, 7536, 7536, 7536, -2, -2, -2);--by fanyu 改变贴图 场景32-编号2
	ModifyEvent(-2, 3, 1, 1, -1, -1, -1, 7580, 7580, 7580, -2, -2, -2);--by fanyu 改变贴图 场景32-编号3
	jyx2_FixMapObject("林厨子做菜",1);
	SetRoleFace(0);
	LightScence();
	Play2Amination(2, 7536, 7578, 3, 7580, 7622, 44);
	Play2Amination(2, 7536, 7578, 3, 7580, 7622, 44);
	Play2Amination(2, 7536, 7578, 3, 7580, 7622, 14);
	DarkScence();
	ModifyEvent(-2, 2, 1, 1, -1, -1, -1, 2718, 2718, 2718, -2, -2, -2);--by fanyu 改变贴图 场景32-编号2
	ModifyEvent(-2, 3, 1, 1, -1, -1, -1, 2720, 2720, 2720, -2, -2, -2);--by fanyu 改变贴图 场景32-编号3
	ModifyEvent(-2, 5, 1, 1, 689, -1, -1, 5100, 5100, 5100, -2, -2, -2);--by fanyu 改变贴图，启动脚本689 场景32-编号5
	jyx2_FixMapObject("林厨子做菜",0);
	jyx2_FixMapObject("林厨子做完菜",1);
	ModifyEvent(-2, 4, -2, -2, -1, -1, 677, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本677 场景32-编号4
	LightScence();
	Talk(75, "好了，完成了，这一道“玉笛谁家听落梅”你拿去吧。", "talkname75", 0);
	Talk(0, "谢谢林师父。", "talkname0", 1);
	AddItem(176, 1);
	do return end;
