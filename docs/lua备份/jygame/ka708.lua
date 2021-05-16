if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItem(186, -1);
    Talk(73, "除了中土之外，海外幾個小島上也住有一些武林人士．金先生除了留有一份他當時繪製的地圖及羅盤外，一艘他當時所使用的小船也停靠某個海岸邊，你可以利用這小船出海，到海外小島去找尋線索．對了，那份地圖是幾十年前所繪製的，所以有些方位可能有些誤差了．", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
