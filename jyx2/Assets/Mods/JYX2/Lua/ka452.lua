Talk(0, "小子，上次对你手下留情，你居然还敢再来，是不是活得不耐烦了！", "talkname0", 1);
if TryBattle(72) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    ModifyEvent(-2, -2, -2, -2, 451, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 16, -2, -2, 474, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 17, -2, -2, 474, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 18, -2, -2, 474, -1, -1, -2, -2, -2, -2, -2, -2);
    LightScence();
    Talk(0, "老伯，我看下一次的华山论剑，你还是别参加了，继续再苦练吧。", "talkname0", 1);
do return end;
