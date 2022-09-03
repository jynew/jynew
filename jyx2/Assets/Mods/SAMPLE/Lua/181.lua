Talk(0, "这家伙有毒！");
if TryBattle(181) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    AddItem(181, 10);
    ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
do return end;
