if UseItem(194) == true then goto label0 end;
    do return end;
::label0::
    AddItem(194, -1);
    PlayAnimation(3, 5722, 5748);
    ModifyEvent(-2, -2, -2, -2, 237, 241, -1, 5722, 5748, 5722, -2, -2, -2);
    ModifyEvent(-2, 2, -2, -2, 239, -1, -1, -2, -2, -2, -2, -2, -2);
    Talk(35, "这烧刀子真是辛辣有劲，可惜美味不足．", "talkname35", 0);
do return end;
