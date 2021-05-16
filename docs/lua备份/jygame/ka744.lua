if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(73, "胡青牛夫婦知道紫衫龍王的下落．你救出王難姑後，記得去拜訪他們．", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
