if UseItem(179) == true then goto label0 end;
    do return end;
::label0::
    Talk(32, "這．．．這是真跡！真是．．．真是唐朝．．．唐朝張旭的”率意帖”．．假不了，假不了的！", "talkname32", 0);
    Talk(0, "三莊主果然是行家．", "talkname0", 1);
    Talk(32, "少俠，可否再借老夫一看？", "talkname32", 0);
    Talk(0, "禿老頭，要看可以，先打贏我再說．", "talkname0", 1);
    Talk(32, "說什麼？我最痛恨人家叫我禿子，你這小子太不知死活了．", "talkname32", 0);
    Talk(0, "禿頭，禿頭下雨不愁，人家有傘，我有禿頭．", "talkname0", 1);
    Talk(32, "好小子，我瞧你是活得不耐煩了，看看老夫怎麼收拾你．", "talkname32", 0);
    if TryBattle(44) == true then goto label1 end;
        Dead();
        do return end;
::label1::
        LightScence();
        Talk(32, "小伙子，果然有兩下子，可是那”率意帖”我是要定了．", "talkname32", 0);
        Talk(0, "三莊主，別自不量力了．我看這梅莊之中也沒什麼能手了，真是害我白走一趟．", "talkname0", 1);
        Talk(32, "臭小子！別這麼囂張！", "talkname32", 0);
        Talk(0, "這樣好了，貴莊中只要有人能夠勝得了我，連同先前那幅”谿山行旅圖”和這”率意帖”一併送上．", "talkname0", 1);
        Talk(32, "小子，此話當真？", "talkname32", 0);
        Talk(0, "小爺我從來不說假話．", "talkname0", 1);
        Talk(32, "好！四弟，咱們去求二哥幫忙．", "talkname32", 0);
        jyx2_ReplaceSceneObject("", "NPC/danqingsheng2", "");--丹青生
        jyx2_ReplaceSceneObject("", "NPC/tubiweng", "");--tubiweng
        DarkScence();
        ModifyEvent(-2, 5, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(-2, 6, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        ModifyEvent(-2, 7, -2, -2, -1, -1, -1, 2908, 2908, 2908, -2, -2, -2);
        ModifyEvent(-2, 8, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
        SetScenceMap(-2, 1, 21, 34, 0);
        jyx2_ReplaceSceneObject("", "NPC/danqingsheng3", "1");--丹青生
        jyx2_ReplaceSceneObject("", "NPC/tubiweng2", "1");--tubiweng
        jyx2_ReplaceSceneObject("", "Bake/Static/Door/Door_028", "");--黑白子开门
        ModifyEvent(-2, 10, 1, 1, 254, -1, -1, 6054, 6054, 6054, -2, -2, -2);
        ModifyEvent(-2, 11, 1, 1, 254, -1, -1, 6050, 6050, 6050, -2, -2, -2);
        LightScence();
        AddRepute(2);
do return end;
