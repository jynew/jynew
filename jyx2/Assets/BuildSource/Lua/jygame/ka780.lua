if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(74, "神算子瑛姑住的黑龙潭，经过她巧妙的布置后，常人难以进入。给你个提示：带某个女人去就解决了。", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
