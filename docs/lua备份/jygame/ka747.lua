if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(73, "”天龍八部”一書是在喬峰手上．我希望你是用正當手段得到的，", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
