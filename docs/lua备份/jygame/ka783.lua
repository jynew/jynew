if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(74, "你知道嗎？有很多古人死去時會將生平喜愛的東西拿去陪葬．所以如果你想找些年代久遠的古董，可試著去盜墓．不過別忘了帶傢伙去．好比”鐵鏟”就是個不錯的工具．", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
