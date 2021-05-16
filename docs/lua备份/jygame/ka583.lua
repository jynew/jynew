PlayAnimation(9, 6420, 6448);
Talk(0, "哇！鱷魚！", "talkname0", 1);
if TryBattle(89) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 9, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    SetScenceMap(-2, 0, 34, 35, 358);
    LightScence();
    AddRepute(1);
do return end;
