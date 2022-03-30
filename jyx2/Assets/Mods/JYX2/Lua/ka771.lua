if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(74, "在一些世外桃源之地，常生长一些珍奇异果，食后滋补养气，可增加生命。如昆仑山脉附近就有一处世外桃源，里面长着一种仙果：大蟠桃。", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
