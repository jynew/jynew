if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(74, "你知道吗？有很多古人死去时会将生平喜爱的东西拿去陪葬。所以如果你想找些年代久远的古董，可试着去盗墓。不过别忘了带家伙去。好比“铁铲”就是个不错的工具。", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
