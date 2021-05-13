if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(74, "神算子瑛姑住的黑龍潭，經過她巧妙的佈置後，常人難以進入．給你個提示：帶某個女人去就解決了．", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
