if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(73, "洞庭湖畔有座藥王莊，裡面住著一位很會用毒的奇人，叫做”毒手藥王”．不過藥王莊四周種有奇特的樹，使人難以進入．", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
