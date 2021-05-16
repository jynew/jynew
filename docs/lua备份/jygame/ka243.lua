Talk(35, "來，來！咱們再喝一杯．", "talkname35", 0);
if AskJoin () == true then goto label0 end;
    Talk(0, "＜這個玩物喪志的傢伙，整天就只知道喝酒，跟他在一起真是浪費我找書的時間＞啊！令狐兄，我突然想起還有重要的事要辦，我先失陪了．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "我看不如這樣吧．令狐兄就和我一起同遊江湖共尋美酒，才不枉此生．", "talkname0", 1);
    if TeamIsFull() == false then goto label1 end;
        Talk(35, "你的隊伍已滿，我無法加入．", "talkname35", 0);
        do return end;
::label1::
        Talk(35, "這個提議甚好，咱們就一起喝盡人世間的佳釀美酒，走！對了，兄弟，告訴你一個好玩的地方，是我在華山時發現的．那地方甚為隱密，入口在華山的背面，有空我們可以去看看．", "talkname35", 0);
        DarkScence();
        ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -1, -2, -2);
        LightScence();
        Join(35);
        AddEthics(3);
do return end;
