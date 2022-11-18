if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(74, "昆仑山脉中有个山洞，里面有只千年冰蚕，可让练毒者毒力大增。地点在（１２，９４４）。", "talkname74", 0);--数字反着说
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
