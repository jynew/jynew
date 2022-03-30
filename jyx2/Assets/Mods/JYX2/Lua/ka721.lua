if UseItem(186) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(186, -1);
    Talk(73, "硝石是制造霹雳弹的的重要材料，通常在一些山洞中都可取得。", "talkname73", 0);
    Add3EventNum(-2, 0, 0, 1, -1)
do return end;
