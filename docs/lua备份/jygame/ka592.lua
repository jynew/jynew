if UseItem(174) == true then goto label0 end;
    do return end;
::label0::
    if JudgeMoney(100) == true then goto label1 end;
        Talk(106, "兄弟，１００就１００，我們可是不二價的．", "talkname106", 0);
        do return end;
::label1::
        AddItem(174, -100);
        Talk(106, "好，從這裡一直往西南走，大概在瀾滄江的源頭就可以看到了．座標大約是在．．．．．．（１６８，４２６）附近．祝你玩的愉快．", "talkname106", 0);
        Talk(0, "就這麼簡單？", "talkname0", 1);
        Talk(106, "招牌上寫的清清楚楚，自助旅遊，當然是你自己去，難不成還我帶你去呀．", "talkname106", 0);
        Talk(0, "這樣就要１００兩，太離譜了吧．", "talkname0", 1);
        Talk(106, "你再吵，再吵我就將你的行為報告給全國的小二哥聯誼會，看你以後還有沒有小道消息可問．", "talkname106", 0);
        Talk(0, "這可不得了，萬萬不可．在下只是發發牢騷罷了，小二哥可別當真了．", "talkname0", 1);
        ModifyEvent(-2, 5, -2, -2, 571, -2, -2, -2, -2, -2, -2, -2, -2);
        ModifyEvent(-2, -2, -2, -2, 481, -2, -2, -2, -2, -2, -2, -2, -2);
do return end;
