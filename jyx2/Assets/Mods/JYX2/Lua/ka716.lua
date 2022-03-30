if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "在南海上有个侠客岛，岛上每十年就派出赏善罚恶两使者到中土上，邀请各派掌门前去岛上喝一碗腊八粥。今年正好是第十年，有机会你可以上侠客岛上看一看，或许有《侠客行》一书的消息。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
