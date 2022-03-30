if UseItem(193) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(193, -1);
    Talk(82, "这位少侠请进。", "talkname82", 0);
    DarkScence();
    ModifyEvent(-2, 0, -2, -2, 183, -1, -1, 5192, 5192, 5192, -2, 30, 48);
    ModifyEvent(-2, 20, -2, -2, 183, -1, -1, 5186, 5186, 5186, -2, 27, 48);
	jyx2_FixMapObject("衡山守门弟子让路",1);
    ModifyEvent(-2, 1, -2, -2, 183, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 2, -2, -2, 183, -1, -1, -2, -2, -2, -2, -2, -2);
    LightScence();
do return end;
