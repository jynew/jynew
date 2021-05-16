Talk(0, "這位兄台，你家怎麼有這麼多漂亮姊姊．", "talkname0", 1);
Talk(61, "她們都是我的弟子．", "talkname61", 0);
Talk(0, "你這師父是教什麼武功，怎麼都收女弟子．難不成你這兒是”妹登風”嗎？", "talkname0", 1);
Talk(61, "什麼是”妹登風”？我這裡是白駝山．我是這裡的少主歐陽克，我的弟子都是從各地挑選出的美女，由我親自傳授”床上功夫”．", "talkname61", 0);
Talk(0, "＜這是限制級的遊戲嗎？＞你吃的消嗎？分我幾個好了．", "talkname0", 1);
Talk(61, "你是誰，到我白駝山撒野．要比床上功夫前先來比一比真武藝．", "talkname61", 0);
if TryBattle(69) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    LightScence();
    Talk(0, "怎樣，我的功夫比你強吧．", "talkname0", 1);
    Talk(61, "你知道我是誰嗎．", "talkname61", 0);
    Talk(0, "你不是說了嗎，你是白駝山的少主．", "talkname0", 1);
    Talk(61, "那你不知道我叔父是誰嗎？", "talkname61", 0);
    Talk(0, "是誰？", "talkname0", 1);
    Talk(61, "江湖上人稱”西毒”的歐陽鋒．", "talkname61", 0);
    Talk(0, "聽起來好像不好惹．", "talkname0", 1);
    Talk(61, "知道不好惹就對了．小子，我看你功夫還不錯，這樣子好了，我們可以找一些志同道合的人，使盡各種手段，打倒各大門派，稱霸這武林．到時我們就可和我叔父他們一般，闖出一番名號，讓武林中人聞之喪膽．", "talkname61", 0);
    Talk(0, "你說要不惜使用各種手段，那行為不是太卑鄙了嗎？", "talkname0", 1);
    Talk(61, "這年頭你還想正正當當的？那你要等到什麼時後才能稱霸武林．", "talkname61", 0);
    Talk(0, "我想看看．．．．", "talkname0", 1);
    AddRepute(1);
    ModifyEvent(-2, -2, -2, -2, 445, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 7, -2, -2, -1, -1, 473, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 8, -2, -2, -1, -1, 473, -2, -2, -2, -2, -2, -2);
    if AskJoin () == true then goto label1 end;
        Talk(0, "不行，我還是想當個大俠，不肖與你這個人為伍．", "talkname0", 1);
        Talk(61, "真是可惜，本來還想跟你研究研究”床上功夫”呢．", "talkname61", 0);
        do return end;
::label1::
        Talk(0, "好吧，那我們就一起稱霸武林吧．反正有句名言不是說”好人早死，壞人較長命”", "talkname0", 1);
        if JudgeFemaleInTeam() == true then goto label2 end;
            Talk(61, "不行，不行，我們同伴中沒有女的我會受不了，等你找到女的再說．", "talkname61", 0);
            do return end;
::label2::
            if TeamIsFull() == false then goto label3 end;
                Talk(61, "你的隊伍已滿，我無法加入．", "talkname61", 0);
                do return end;
::label3::
                Talk(61, "走吧，再去找一些邪惡的人來加入．", "talkname61", 0);
                DarkScence();
                jyx2_ReplaceSceneObject("", "NPC/ouyangke", "");--欧阳克加入队伍
                ModifyEvent(-2, 0, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
                ModifyEvent(-2, 1, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
                ModifyEvent(-2, 2, -2, -2, 472, -1, -1, -2, -2, -2, -2, -2, -2);
                ModifyEvent(-2, 3, -2, -2, 472, -1, -1, -2, -2, -2, -2, -2, -2);
                ModifyEvent(-2, 4, -2, -2, 472, -1, -1, -2, -2, -2, -2, -2, -2);
                ModifyEvent(-2, 5, -2, -2, 472, -1, -1, -2, -2, -2, -2, -2, -2);
                ModifyEvent(-2, 6, -2, -2, 472, -1, -1, -2, -2, -2, -2, -2, -2);
                ModifyEvent(-2, 7, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
                ModifyEvent(-2, 8, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
                LightScence();
                Join(61);
                AddEthics(-6);
do return end;
