Talk(0, "今天嵩山頂上似乎很熱鬧？", "talkname0", 1);
Talk(84, "今日是我五嶽劍派併派的大日子．閒雜人等，還請離去．", "talkname84", 0);
Talk(0, "這樣大的盛會，怎能少得了大爺我．快讓讓．", "talkname0", 1);
Talk(84, "閣下再不離去，休怪我們不客氣了．", "talkname84", 0);
Talk(0, "我正有此意．", "talkname0", 1);
if TryBattle(29) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    ModifyEvent(-2, 1, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 2, -2, -2, 205, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 3, -2, -2, 205, -1, -1, -2, -2, -2, -2, -2, -2);
    LightScence();
    AddRepute(2);
do return end;
