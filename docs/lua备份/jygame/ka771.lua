if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(74, "在一些世外桃源之地，常生長一些珍奇異果，食後滋補養氣，可增家生命．如崑崙山脈附近就有一處世外桃源，裡面長著一種仙果大蟠桃．", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
