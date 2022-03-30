if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(74, "藏有很多硝石的山洞中，还藏有两个神秘宝箱，但都需要钥匙去开。其中一个是铁钥匙。", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
