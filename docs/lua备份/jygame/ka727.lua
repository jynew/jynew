if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(73, "武當派的張真人，對於提攜後進一向不遺餘力，你若有空，可常向他討教，肯定獲益良多．", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
