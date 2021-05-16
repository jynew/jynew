ModifyEvent(7, 6, 0, 0, -1, -1, -1, -1, -1, -1, 0, -2, -2);
AddEthics(5);
if InTeam(58) == false then goto label0 end;
    Talk(58, "龍兒！", "talkname58", 1);
    jyx2_ReplaceSceneObject("", "NPC/yangguo", "1");--杨过出现
    Talk(59, "過兒！", "talkname59", 0);
    Talk(58, "．．．．．", "talkname58", 1);
    Talk(59, "．．．．．", "talkname59", 0);
    Talk(58, "龍兒，你容貌一點也沒變，我卻老了．", "talkname58", 1);
    Talk(59, "不是老了，是我的過兒長大了．", "talkname59", 0);
    Talk(0, "不知龍姑娘怎會在這絕情谷底？", "talkname0", 1);
    Talk(58, "是啊，龍兒，你不是在絕情谷中留言，隨那南海神尼走了？", "talkname58", 1);
    Talk(59, "那時，我知道你為了我中毒難治，不願獨生．我想了又想，唯有自己先死，絕了你的念頭，才有希望化解你體內的情花之毒．但若我露了自盡的痕跡，只有更促你早死．我思量了一夜，決定用劍尖在斷腸崖前刻了那幾行字，故意定了一十六年之約，這才縱身躍入深谷．", "talkname59", 0);
    Talk(58, "你躍入這絕情谷底，便又怎樣？", "talkname58", 1);
    Talk(59, "我昏昏迷迷的跌進水潭，浮起來時給水流衝進冰窖，通到了這裡，自此便在此處過活．這裡並無禽鳥野獸，但潭中水產豐盛，谷底水果食之不盡．", "talkname59", 0);
    Talk(58, "真是老天有眼．", "talkname58", 1);
    Talk(0, "真是老天有眼，讓我們發現老頑童那蜜蜂上的刺字，才得以找到這條通往谷底的小路，讓你夫婦倆團圓．那蜜蜂上的字是龍姑娘刺上去的吧？", "talkname0", 1);
    Talk(59, "是的．", "talkname59", 0);
    Talk(0, "那不知”二午寺”，”一山惡”兩句話是什麼意思？", "talkname0", 1);
    Talk(59, "我在水底曾看到兩組數字，”１３２””２５４”，我就一起刻上去了．", "talkname59", 0);
    Talk(0, "”１３２”，”２５４”？我聽老頑童唸成”二午寺””一山惡”．唉！", "talkname0", 1);
    Talk(58, "楊某真是虧欠兄弟太多了，否則可能到現在，我夫婦倆還分隔兩地而無法相見．", "talkname58", 1);
    Talk(0, "不知楊兄現在有何打算．", "talkname0", 1);
    Talk(58, "我想先和龍兒回古墓中，兄弟將來若有什麼困難，儘管到古墓中找我夫婦倆．", "talkname58", 1);
    Talk(0, "楊兄慢走，願你夫婦倆別再分離．", "talkname0", 1);
    Talk(58, "那我夫婦倆先走了，祝兄弟一路順風．", "talkname58", 1);
    jyx2_ReplaceSceneObject("", "NPC/yangguo", "");--杨过离开
    jyx2_ReplaceSceneObject("", "NPC/xiaolongnv", "");--小龙女离开

    DarkScence();
    SetScenceMap(18, 1, 44, 31, 0);
    SetScenceMap(18, 1, 44, 30, 0);
    jyx2_ReplaceSceneObject("18", "Bake/Static/Door/Door_035", "");--古墓开门
    jyx2_ReplaceSceneObject("18", "Bake/Static/Door/Door_036", "");--
    jyx2_ReplaceSceneObject("18", "NPC/yangguo", "1");--杨过出现
    jyx2_ReplaceSceneObject("18", "NPC/xiaolongnv", "1");--小龙女出现
    ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(18, 0, 1, 1, 438, -1, -1, 6188, 6188, 6188, -2, -2, -2);
    ModifyEvent(18, 1, 1, 1, 440, -1, -1, 6068, 6068, 6068, -2, -2, -2);
    ModifyEvent(18, 2, -2, -2, 442, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(18, 3, -2, -2, 442, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(18, 4, -2, -2, 443, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(18, 5, -2, -2, 443, -1, -1, -2, -2, -2, -2, -2, -2);
    LearnMagic2(58, 24, 1);
    NPCGetItem(58, 61, 1);
    Leave(58);
    LightScence();
    Talk(0, "”問世間情是何物，直叫人生死相許”他們夫婦倆真是令人羨慕的神仙俠侶．”神仙俠侶”！．．．．．”神鵰俠侶”？對了，還有那頭老鵰，那本書該不會是在他倆身上吧．　　　　看來有空還要再前往古墓找他夫婦倆．", "talkname0", 1);
    do return end;
::label0::
    Talk(0, "＜哇！這姑娘真美，好像神  仙一般＞．姑娘不知為何一人獨自在這谷底．", "talkname0", 1);
    Talk(59, "你是怎麼進來的？", "talkname59", 0);
    Talk(0, "那裡有一條很隱密的小路通到這谷底．", "talkname0", 1);
    Talk(59, "在那裡？我要趕快出去找過兒．", "talkname59", 0);
    Talk(0, "過兒？莫非姑娘說的是楊過楊兄．請問姑娘芳名？　　　　　　　　　　　　", "talkname0", 1);
    Talk(59, "我是小龍女．你見過我過兒？", "talkname59", 0);
    Talk(0, "是的．我曾與楊兄有過一面之緣，楊兄也是非常想念龍姑娘．不知龍姑娘怎會在這絕情谷底？你不是在絕情谷留言，隨那南海神尼走了？", "talkname0", 1);
    Talk(59, "那時，我知道過兒為了我中毒難治，不願獨生．我想了又想，唯有自己先死，絕了過兒的念頭，才有希望化解過兒體內的情花之毒．但若我露了自盡的痕跡，只有更促使過兒早死．我思量了一夜，決定用劍尖在斷腸崖前刻了那幾行字，故意定了一十六年之約，這才縱身躍入深谷．", "talkname59", 0);
    Talk(0, "你躍入這絕情谷底，便又怎樣？", "talkname0", 1);
    Talk(59, "我昏昏迷迷的跌進水潭，浮起來時給水流衝進冰窖，通到了這裡，自此便在此處過活．這裡並無禽鳥野獸，但潭中水產豐盛，谷底水果食之不盡．", "talkname59", 0);
    Talk(0, "真是老天有眼，讓我發現了老頑童那蜜蜂上的刺字，才得以找到這條通往谷底的小路，讓你夫婦倆團圓．那蜜蜂上的字是龍姑娘刺上去的吧？", "talkname0", 1);
    Talk(59, "是的．", "talkname59", 0);
    Talk(0, "那不知”二午寺”，”一山惡”兩句話是什麼意思？", "talkname0", 1);
    Talk(59, "我在水底曾看到兩組數字，”１３２””２５４”，我就一起刻上去了．", "talkname59", 0);
    Talk(0, "”１３２”，”２５４”？我聽老頑童唸成”二午寺””一山惡”．唉！", "talkname0", 1);
    Talk(59, "過兒現在在那裡？", "talkname59", 0);
    Talk(0, "楊兄現正在神鵰穴中練功休養，那神鵰穴是在．．．．", "talkname0", 1);
    Talk(59, "我這就去找他．少俠將來有空，可到古墓中找我夫婦．", "talkname59", 0);
    jyx2_ReplaceSceneObject("", "NPC/xiaolongnv", "");--小龙女离开
    DarkScence();
    SetScenceMap(18, 1, 44, 31, 0);
    SetScenceMap(18, 1, 44, 30, 0);
    jyx2_ReplaceSceneObject("18", "Bake/Static/Door/Door_035", "");--古墓开门
    jyx2_ReplaceSceneObject("18", "Bake/Static/Door/Door_036", "");--
    jyx2_ReplaceSceneObject("18", "NPC/yangguo", "1");--杨过出现
    jyx2_ReplaceSceneObject("18", "NPC/xiaolongnv", "1");--小龙女出现
    ModifyEvent(-2, -2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(7, 6, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(18, 0, 1, 1, 438, -1, -1, 6188, 6188, 6188, -2, -2, -2);
    ModifyEvent(18, 1, 1, 1, 440, -1, -1, 6068, 6068, 6068, -2, -2, -2);
    ModifyEvent(18, 2, -2, -2, 442, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(18, 3, -2, -2, 442, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(18, 4, -2, -2, 443, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(18, 5, -2, -2, 443, -1, -1, -2, -2, -2, -2, -2, -2);
    LearnMagic2(58, 24, 1);
    NPCGetItem(58, 61, 1);
    LightScence();
    Talk(0, "”問世間情是何物，直叫人生死相許”他們夫婦倆真是令人羨慕的神仙俠侶．”神仙俠侶”！．．．．．”神鵰俠侶”？對了，還有那頭老鵰，那本書該不會是在他倆身上吧．　　　　看來有空還要再前往古墓找他夫婦倆．", "talkname0", 1);
do return end;
