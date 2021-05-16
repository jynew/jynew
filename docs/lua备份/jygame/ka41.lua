if UseItem(158) == true then goto label0 end;
    do return end;
::label0::
    AddItem(158, -1);
    Talk(2, "你找到了？挺厲害的嘛！斷腸草的解藥在這，拿去吧．", "talkname2", 0);
    GetItem(137, 1);
    ModifyEvent(-2, -2, -2, -2, 42, -1, -2, -2, -2, -2, -2, -2, -2);
    AddEthics(1);
do return end;
