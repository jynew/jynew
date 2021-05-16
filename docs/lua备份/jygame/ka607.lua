Talk(97, "那裡來的小子，敢擅闖我神龍島．", "talkname97", 0);
Talk(0, "神龍島？聽起來好像頗棘手的，我還是下回有把握點再來．", "talkname0", 1);
Talk(97, "想走，我神龍島可不是你說來就來，說走就走的．", "talkname97", 0);
Talk(0, "別這樣嘛！我船隨便開開，就開到你們島上了．本想看看你們這邊有沒有我要找的書，但．．我看大概沒有．．．打擾了．．．", "talkname0", 1);
Talk(97, "果然是來偷書的．來人呀！快將他拿下．", "talkname97", 0);
if TryBattle(94) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    ModifyEvent(-2, 5, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 6, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 7, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 8, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 9, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 10, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    jyx2_ReplaceSceneObject("", "Gaswall/Wall1", "");--战斗结束，移除空气墙
    LightScence();
    Talk(0, "踏破鐵鞋無覓處，得來全不費功夫．這裡或許有我要找的書，進去找他們老大要．", "talkname0", 1);
    AddRepute(1);
do return end;
