if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(74, "一个练武之人最重要的是看他有没有“资质”。一个资质好的人，修练起武功来事半功倍，反之则事倍功半。所以你如果发现你资质不好时，我劝你也别在这武林混下去了，早死早投胎好了。要鉴定资质很简单，只要你比较同一本书在同一级的修练时，两个不同人所须经验点数的差异就可知晓了。所须经验点数较小者就是资质较高者。", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
