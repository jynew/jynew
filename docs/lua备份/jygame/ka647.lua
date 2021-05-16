if UseItem(187) == true then goto label0 end;
    do return end;
::label0::
    AddItem(187, -1);
    --Add3EventNum(-2, 1, 0, 1, 3)  有问题
    ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    Talk(0, "哈！這刀孔大小正適合放這把鴛刀．", "talkname0", 1);
do return end;
