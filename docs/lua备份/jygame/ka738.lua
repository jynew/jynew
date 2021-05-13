if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(73, "等你聲望達到２００以上，並找齊十四本天書後，可回你住的地方看看．或許你會受邀參加今年在華山頂上舉辦之武林大會．", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
