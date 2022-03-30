if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "制造药丸非常简单，只要你队伍中有人修练医书，又有药材的话就可制造出来。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
