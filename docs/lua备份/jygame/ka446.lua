Talk(61, "上次我是大意輸你，你還敢再來，這次要你躺著出去．", "talkname61", 0);
if TryBattle(69) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    LightScence();
    Talk(0, "怎樣，又大意了．", "talkname0", 1);
    Talk(61, "哼！", "talkname61", 0);
    ModifyEvent(-2, -2, -2, -2, 445, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 7, -2, -2, -1, -1, 473, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 8, -2, -2, -1, -1, 473, -2, -2, -2, -2, -2, -2);
do return end;
