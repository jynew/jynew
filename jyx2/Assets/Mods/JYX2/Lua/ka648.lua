if UseItem(187) == true then goto label0 end;
    do return end;
::label0::
    AddItemWithoutHint(187, -1);
    ModifyEvent(-2, 0, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 0, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    Talk(0, "哈！这刀孔大小正适合放这把鸳刀。", "talkname0", 1);
    SetScenceMap(-2, 1, 23, 39, 0);
    SetScenceMap(-2, 1, 24, 39, 0);
    jyx2_FixMapObject("鸳鸯山洞开门",1);
do return end;
