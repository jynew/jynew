if UseItem(127) == true then goto label0 end;
    do return end;
::label0::
    AddItem(127, -1);
    Talk(35, "好一只翡翠杯！得此美酒佳器，人生更有何憾．我令狐沖先乾為敬，謝謝兄弟贈酒之情．", "talkname35", 0);
    PlayAnimation(3, 5722, 5748);
    ModifyEvent(-2, -2, -2, -2, 243, -1, -1, 5722, 5748, 5722, -2, -2, -2);
    Talk(0, "＜令狐沖！他就是令狐沖＞我聽江湖上議論紛紛說令狐兄已遭華山派逐出師門，不知可有此事？", "talkname0", 1);
    Talk(35, "唉！令狐沖一生仗義直行，從不做違背良心之事，到頭來卻落至這個結果．這件事的始末也非三言兩語可道盡．唉．不談這個，咱們喝酒吧", "talkname35", 0);
    Talk(0, "不知令狐兄今後有何打算？", "talkname0", 1);
    Talk(35, "．．．．．．", "talkname35", 0);
    if AskJoin () == true then goto label1 end;
        Talk(0, "＜這個玩物喪志的傢伙，整天就只知道喝酒，跟他在一起真是浪費我找書的時間＞啊！令狐兄，我突然想起還有重要的事要辦，我先失陪了．", "talkname0", 1);
        do return end;
::label1::
        Talk(0, "我看不如這樣吧．令狐兄就和我一起同遊江湖共尋美酒，才不枉此生．", "talkname0", 1);
        if TeamIsFull() == false then goto label2 end;
            Talk(35, "你的隊伍已滿，我無法加入．", "talkname35", 0);
            do return end;
::label2::
            Talk(35, "這個提議甚好，咱們就一起喝盡人世間的佳釀美酒，走！對了，兄弟，告訴你一個好玩的地方，是我在華山時發現的．那地方甚為隱密，入口在華山的背面，有空我們可以去看看．", "talkname35", 0);
            ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -1, -2, -2);
            LightScence();
            Join(35);
            AddEthics(3);
do return end;
