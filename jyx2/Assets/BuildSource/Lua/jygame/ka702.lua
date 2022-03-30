if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "东北雪地中，有一名为胡斐的年轻男子，他的外号叫做“雪山飞狐”，刚好与十四天书中的《雪山飞狐》一书及《飞狐外传》一书有着极大的相似之处。或许你可以上关外找此人问个究竟。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
