if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "等你声望达到２００以上，并找齐十四本天书后，可回你住的地方看看。或许你会受邀参加今年在华山顶上举办之武林大会。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
