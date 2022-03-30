if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(74, "我这有两个宝箱，内藏珍贵的宝物。不过可惜的是两个箱子的钥匙都不见了，你如果能找到钥匙并打开的话，里面的宝物就送给你。我记得那两只钥匙的颜色是“红”和“银”。", "talkname74", 0);
    Add3EventNum(-2, 1, 0, 1, -1)
do return end;
