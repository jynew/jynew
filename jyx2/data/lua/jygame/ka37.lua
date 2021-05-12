if HaveItem(173) == false then goto label0 end;
    ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
    do return end;
::label0::
    Talk(0, "什么味道这样香浓，我头好昏，啊！难道这红树丛有毒 ．．．．．．", "talkname0", 1);
    PlayAnimation(-1, 5974, 5992);
    DarkScence();
    SetScencePosition2(30, 49);
    LightScence();
    PlayAnimation(-1, 6014, 6024);
    Talk(0, "咦！这是那里？莫非我已昏厥多时？此处非平常之地．", "talkname0", 1);
    ModifyEvent(-2, -2, -2, -2, -2, -2, 38, -2, -2, -2, -2, -2, -2);
do return end;
