if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "医书上曾言，凡毒蛇出没之处，七步之内必有解除其毒性之药。其他毒物，无不如此，这是天地间万物生克的至理。所以如果你身中异毒时，或许解药就在该毒物的附近。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
