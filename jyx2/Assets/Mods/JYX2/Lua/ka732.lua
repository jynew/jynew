if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "日月神教在前任教主任我行的带领下，好不兴旺。近来换上了东方不败担任教主，虽然没有什么大作为，但传说他武功之高，已到匪夷所思的境界。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
