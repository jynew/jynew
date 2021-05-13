Talk(83, "見性峰乃恆山派禁地，施主勿近．", "talkname83", 0);
if AskBattle() == true then goto label0 end;
    do return end;
::label0::
    Talk(0, "什麼恆山派禁地，土地權狀拿出來我瞧瞧．像妳們這樣據地稱王的人，我最痛恨了．", "talkname0", 1);
    Talk(83, "這位施主，再不離開，可別怪我們不客氣！", "talkname83", 0);
    Talk(0, "我從地理課本上知道，恆山風光明媚，鳥語花香，而見性峰更是如人間仙境．因此我特地上來觀光的，妳們這些臭道姑太不講道理了．老子我就偏要看看這見性峰到底長什麼樣！妳們能奈我何！", "talkname0", 1);
    Talk(83, "小子說什麼！我看你是嵩山派派來的奸細吧！快滾回去告訴你們掌門，恆山派絕不會答應併派的！", "talkname83", 0);
    Talk(0, "說什麼五四三的，聽嘸啦．小俠我都甘願冒著”一見尼姑，逢賭必輸”的大險來到妳們這尼姑庵中，怎可敗興而歸．", "talkname0", 1);
    Talk(83, "胡說八道的小子，先拿下再說．", "talkname83", 0);
    if TryBattle(23) == true then goto label1 end;
        Dead();
        do return end;
::label1::
        ModifyEvent(-2, 2, -2, -2, 169, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本169 场景31-2
        ModifyEvent(-2, 3, -2, -2, 169, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本169 场景31-3
        ModifyEvent(-2, 4, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 移出角色 场景31-4
        ModifyEvent(-2, 5, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 移出角色 场景31-5
        ModifyEvent(-2, 6, -2, -2, 169, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本169 场景31-6
        ModifyEvent(-2, 7, -2, -2, 169, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 启动脚本169 场景31-7
        jyx2_ReplaceSceneObject("","GasWalls/Wall1","");
        LightScence();
        Talk(0, "哼！愈是不讓我來，我就愈想探個究竟．", "talkname0", 1);
        AddRepute(1);
do return end;
