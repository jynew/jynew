if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "除了中土之外，海外几个小岛上也住有一些武林人士。金先生除了留有一份他当时绘制的地图及罗盘外，一艘他当时所使用的小船也停靠某个海岸边，你可以利用这小船出海，到海外小岛去找寻线索。对了，那份地图是几十年前所绘制的，所以有些方位可能有些误差了。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
