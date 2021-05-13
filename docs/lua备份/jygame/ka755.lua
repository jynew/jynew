if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(74, "偷偷告訴你，幾個開箱子不會損害道德的法子．沒人在的場景．主人加入你隊伍後．主人請你自行拿取．", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
