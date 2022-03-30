if UseItem(158) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(158, -1);
    Talk(2, "你找到了？挺厉害的嘛！断肠草的解药在这，拿去吧。", "talkname2", 0);
    AddItem(137, 1);
    ModifyEvent(-2, -2, -2, -2, 42, -1, -2, -2, -2, -2, -2, -2, -2);
    AddEthics(1);
do return end;
