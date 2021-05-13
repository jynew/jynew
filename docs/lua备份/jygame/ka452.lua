Talk(0, "小子，上次對你手下留情，你居然還敢再來，是不是活得不耐煩了！", "talkname0", 1);
if TryBattle(72) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    ModifyEvent(-2, -2, -2, -2, 451, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 16, -2, -2, 474, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 17, -2, -2, 474, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 18, -2, -2, 474, -1, -1, -2, -2, -2, -2, -2, -2);
    LightScence();
    Talk(0, "老伯，我看下一次的華山論劍，你還是別參加了，繼續再苦練吧．", "talkname0", 1);
do return end;
