if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(74, "“霹雳堂”啊！只不过改了名字嘛！", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
