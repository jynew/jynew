if UseItem(178) == true then goto label0 end;
    do return end;
::label0::
    Talk(33, "這．．．這是．．．．我在前人筆記之中見過這記載．．．．上面說劉仲甫是當時國手，卻在驪山麓給一個鄉下老媼殺得大敗，登時嘔血數升，那局棋譜便稱”嘔血譜”．原想只道是個傳聞，怎料世上竟然真有這局嘔血譜？少俠，可否借老夫抄錄副本．", "talkname33", 0);
    Talk(0, "哈！哈！這”嘔血棋譜”是我費盡千辛萬苦才得來的，看一次五千萬兩黃金，看不看隨你．", "talkname0", 1);

    Talk(31, "二哥你瞧，這小子就是這德性，完全沒把我們梅莊放在眼裏，先前還說梅莊中沒人是他的對手，囂張極了．", "talkname31", 0);
    Talk(33, "少俠，別敬酒不吃吃罰酒，我黑白子想要的東西從來沒有得不到的，你還是乖乖地交出來吧．", "talkname33", 1);
    Talk(32, "二哥，別跟他多說廢話，咱們三人聯手，量他插翅也難飛．", "talkname32", 0);
    Talk(0, "枉費梅莊在江湖上的聲名如此響亮，想不到盡是一群倚多欺少之輩，可笑可笑．", "talkname0", 1);
    Talk(33, "三弟，四弟，咱們梅莊可別讓這個傢伙瞧扁了，就讓我來會一會，看他多大能耐．", "talkname33", 0);
    if TryBattle(45) == true then goto label1 end;
        Dead();
        do return end;
::label1::
        LightScence();
        Talk(0, "我就說嘛，你們這幾個老頭子根本就不夠看，我瞧啊，那什麼大莊主想必也沒什麼料．不過既然來了，就把他叫出和我比劃比劃．", "talkname0", 1);
        Talk(33, "臭小子！有種別跑！", "talkname33", 0);
        DarkScence();
        ModifyEvent(-2, 9, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(-2, 10, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(-2, 11, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        SetScenceMap(-2, 1, 37, 34, 0);
        jyx2_ReplaceSceneObject("", "NPC/danqingsheng3", "");--丹青生
jyx2_ReplaceSceneObject("", "NPC/danqingsheng4", "1");--丹青生
jyx2_ReplaceSceneObject("", "NPC/tubiweng2", "");--秃笔翁
jyx2_ReplaceSceneObject("", "NPC/tubiweng3", "1");--秃笔翁
jyx2_ReplaceSceneObject("", "NPC/heibaizi", "");--黑白子
jyx2_ReplaceSceneObject("", "NPC/heibaizi2", "1");--黑白子
        jyx2_ReplaceSceneObject("", "Bake/Static/Door/Door_026", "");--黄钟公开门
        ModifyEvent(-2, 16, 1, 1, -1, -1, -1, 6064, 6064, 6064, -2, -2, -2);
        ModifyEvent(-2, 17, 1, 1, -1, -1, -1, 6060, 6060, 6060, -2, -2, -2);
        ModifyEvent(-2, 18, 1, 1, -1, -1, -1, 6046, 6046, 6046, -2, -2, -2);
        LightScence();
        AddRepute(3);
do return end;
