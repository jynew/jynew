if UseItem(124) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(124, -1);
    Talk(0, "老前辈，我看这蜜蜂很难驯养哦！", "talkname0", 1);
    Talk(64, "没什么的，再过一阵子我就会让这百花谷中到处都是蜜蜂飞舞。", "talkname64", 0);
    Talk(0, "我这有罐玉蜂浆，你拿去试看看，会不会比较好用。", "talkname0", 1);
    ModifyEvent(-2, -2, -2, -2, -2, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 2, -2, -2, -1, -1, 408, -2, -2, -2, -2, -2, -2);--by fanyu|启动408脚本。场景20-编号2
    ModifyEvent(-2, 3, -2, -2, -1, -1, 408, -2, -2, -2, -2, -2, -2);--by fanyu|启动408脚本。场景20-编号3
do return end;
