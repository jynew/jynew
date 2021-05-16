if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(73, "醫書上曾言，凡毒蛇出沒之處，七步之內必有解除其毒性之藥．其他毒物，無不如此，這是天地間萬物生剋的至理．所以如果你身中異毒時，或許解藥就在該毒物的附近．", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
