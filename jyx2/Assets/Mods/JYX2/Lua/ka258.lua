if UseItem(178) == true then goto label0 end;
    do return end;
::label0::
    Talk(33, "这……这是……我在前人笔记之中见过这记载……上面说刘仲甫是当时国手，却在骊山麓给一个乡下老媪杀得大败，登时呕血数升，那局棋谱便称“呕血谱”。原想只道是个传闻，怎料世上竟然真有这局呕血谱？少侠，可否借老夫抄录副本。", "talkname33", 0);
    Talk(0, "哈！哈！这“呕血棋谱”是我费尽千辛万苦才得来的，看一次五千万两黄金，看不看随你。", "talkname0", 1);

    Talk(31, "二哥你瞧，这小子就是这德性，完全没把我们梅庄放在眼里，先前还说梅庄中没人是他的对手，嚣张极了。", "talkname31", 0);
    Talk(33, "少侠，别敬酒不吃吃罚酒，我黑白子想要的东西从来没有得不到的，你还是乖乖地交出来吧。", "talkname33", 1);
    Talk(32, "二哥，别跟他多说废话，咱们三人联手，量他插翅也难飞。", "talkname32", 0);
    Talk(0, "枉费梅庄在江湖上的声名如此响亮，想不到尽是一群倚多欺少之辈，可笑可笑。", "talkname0", 1);
    Talk(33, "三弟，四弟，咱们梅庄可别让这个家伙瞧扁了，就让我来会一会，看他多大能耐。", "talkname33", 0);
    if TryBattle(45) == true then goto label1 end;
        Dead();
        do return end;
::label1::
        LightScence();
        Talk(0, "我就说嘛，你们这几个老头子根本就不够看，我瞧啊，那什么大庄主想必也没什么料。不过既然来了，就把他叫出和我比划比划。", "talkname0", 1);
        Talk(33, "臭小子！有种别跑！", "talkname33", 0);
        DarkScence();
        ModifyEvent(-2, 9, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(-2, 10, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(-2, 11, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        SetScenceMap(-2, 1, 37, 34, 0);
        jyx2_ReplaceSceneObject("", "Dynamic/Door_026", "");--黄钟公开门
        ModifyEvent(-2, 16, 1, 1, -1, -1, -1, 6064, 6064, 6064, -2, -2, -2);
        ModifyEvent(-2, 17, 1, 1, -1, -1, -1, 6060, 6060, 6060, -2, -2, -2);
        ModifyEvent(-2, 18, 1, 1, -1, -1, -1, 6046, 6046, 6046, -2, -2, -2);
		jyx2_FixMapObject("梅庄求助黄钟公",1);
        LightScence();
        AddRepute(3);
do return end;
