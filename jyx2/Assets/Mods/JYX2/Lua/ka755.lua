if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(74, "偷偷告诉你，几个开箱子不会损害道德的法子：没人在的场景，主人加入你队伍后，主人请你自行拿取。", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
