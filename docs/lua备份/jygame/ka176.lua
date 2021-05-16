Talk(23, "閣下硬闖我泰山派，不知是何用意．", "talkname23", 0);
Talk(0, "你的徒弟硬要我跟你拜師，我就來看看你夠不夠格當我師父．", "talkname0", 1);
Talk(23, "好個頑劣的惡徒，讓我來教訓教訓你．", "talkname23", 0);
if TryBattle(26) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    LightScence();
    Talk(0, "抱歉了，你似乎沒什麼像樣的東西可以教我．", "talkname0", 1);
    Talk(23, "哼！魔教的惡徒，要殺就殺，別在那囉唆．", "talkname23", 0);
    Talk(0, "好好的，幹麼殺你？你只是不夠格當我師父罷了", "talkname0", 1);
    Talk(23, "今日不殺我，我五嶽劍派同氣連枝，改日我們再上黑木崖向閣下及東方不敗討教．", "talkname23", 0);
    GetItem(68, 1);
    ModifyEvent(-2, -2, -2, -2, 177, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本177 场景29-01
    Add3EventNum(27, 0, 0, 0, 56)
    AddRepute(3);
do return end;
