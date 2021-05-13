if UseItem(177) == true then goto label0 end;
    do return end;
::label0::
    Talk(34, "”廣陵散”！！！這”廣陵散”絕響於人間已久，今日得睹古人名譜，實是不勝之喜．妙極！和平中正，卻又清絕幽絕．高量雅致，深藏玄機，便這麼神遊琴韻，片刻之間已然心懷大暢．", "talkname34", 0);
    Talk(0, "大莊主道號”黃鐘公”，自是琴中高手．看大莊主讀此琴譜時悠心自在，神情平和之狀，果是不求聲名利祿的世外高人，令小輩萬分欽羨．此譜雖然難得，卻也不是什麼值錢的東西，我留在身上也沒用，大莊主就拿去吧．", "talkname0", 1);
    Talk(34, "常言道：無功不受祿，你我素無淵源，焉可受你這等厚禮？聽我那三個兄弟說道，只須本莊有人武功勝過你，便可得那四樣東西，那老朽可不能白佔這個便宜．小兄弟，咱們便來比劃幾招如何？", "talkname34", 0);
    Talk(0, "好，前輩請．", "talkname0", 1);
    if TryBattle(46) == true then goto label1 end;
        Dead();
        do return end;
::label1::
        LightScence();
        Talk(34, "小兄弟武藝精深，令老朽衷心折服，那”廣陵散”老夫是無福消受了．", "talkname34", 0);
        Talk(33, "大哥，為了那四樣書畫，也顧不得那麼多了，咱們四兄弟一起上吧．", "talkname33", 0);
        Talk(34, "不可如此，咱們為了那些書畫以老欺小已是不該，更豈可以多勝少．", "talkname34", 0);
        Talk(33, "那咱們就請地牢那個老怪．．．．．", "talkname33", 0);
        Talk(34, "二弟！住嘴！", "talkname34", 0);
        Talk(0, "難道這梅莊中還另有高手？沒關係，我說過只要貴莊中有人可以打敗我，我這四件書畫，一定雙手奉上．", "talkname0", 1);
        Talk(33, "大哥，沒問題的，有我們在旁邊守著，那老怪物跑不掉的．", "talkname33", 0);
        Talk(34, "二弟你還說！少俠，敝莊中已無可勝你之人，你就請回吧．", "talkname34", 0);
jyx2_ReplaceSceneObject("", "NPC/danqingsheng4", "");--丹青生
jyx2_ReplaceSceneObject("", "NPC/danqingsheng", "1");--丹青生
jyx2_ReplaceSceneObject("", "NPC/tubiweng3", "");--秃笔翁
jyx2_ReplaceSceneObject("", "NPC/tubiweng", "1");--秃笔翁
jyx2_ReplaceSceneObject("", "NPC/heibaizi2", "");--黑白子
jyx2_ReplaceSceneObject("", "NPC/heibaizi", "1");--黑白子
        ModifyEvent(-2, -2, -2, -2, 265, -1, -1, -2, -2, -2, -2, -2, -2);
        ModifyEvent(-2, 16, -2, -2, 266, -1, -1, -2, -2, -2, -2, -2, -2);
        ModifyEvent(-2, 17, -2, -2, 267, -1, -1, -2, -2, -2, -2, -2, -2);
        ModifyEvent(-2, 18, -2, -2, 268, -1, -1, -2, -2, -2, -2, -2, -2);
        ModifyEvent(-2, 0, -2, -2, -1, -1, 269, -2, -2, -2, -2, -2, -2);
        ModifyEvent(-2, 1, -2, -2, -1, -1, 269, -2, -2, -2, -2, -2, -2);
        ModifyEvent(-2, 2, -2, -2, -1, -1, 269, -2, -2, -2, -2, -2, -2);
        AddRepute(3);
do return end;
