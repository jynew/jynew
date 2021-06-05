if UseItem(152) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(152, -1);
    ModifyEvent(-2, -2, -2, -2, -1, -1, -1, 4664, 4664, 4664, -2, -2, -2);
    if Judge14BooksPlaced() == true then goto label1 end;
        do return end;
::label1::
        PlayWave(23);
        Talk(0, "咦！好像有什么声音．", "talkname0", 1);
        DarkScence();
        SetScenceMap(-2, 1, 18, 25, 0);
        SetScenceMap(-2, 1, 18, 26, 0);
        LightScence();
do return end;
