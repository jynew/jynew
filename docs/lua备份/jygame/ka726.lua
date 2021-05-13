if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(73, "我有一個好朋友叫林廚子，此人作菜的手藝很好．如果你想吃好吃的菜，可以到他那兒去．他住的地方在海岸邊，就在黃河與長江出海口的中間．", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
