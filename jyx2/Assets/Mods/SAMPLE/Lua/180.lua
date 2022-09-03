Talk(0, "这家伙有毒！");
if TryBattle(180) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    AddItem(180, 10);
    AddItem(174, 100);
    ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
