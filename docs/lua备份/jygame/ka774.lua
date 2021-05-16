if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(74, "沙漠中有座廢墟，藏有好東西．地點在（２１９，５２）", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
