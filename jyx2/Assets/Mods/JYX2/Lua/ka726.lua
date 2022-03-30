if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "我有一个好朋友叫林厨子，此人做菜的手艺很好。如果你想吃好吃的菜，可以到他那儿去。他住的地方在海岸边，就在黄河与长江出海口的中间。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
