Talk(52, "公子是否有破解這”珍瓏”的方法了？", "talkname52", 0);
if InTeam(49) == true then goto label0 end;
    Talk(0, "晚輩棋力不佳，否則我倒也想試一試．", "talkname0", 1);
    Talk(52, "可惜，可惜，難道世上無人能解開此”珍瓏”嗎？", "talkname52", 0);
    do return end;
::label0::
    jyx2_ReplaceSceneObject("", "NPC/xuzhu", "1");--虚竹
    Talk(0, "晚輩棋力不佳，不過我試試看好了．", "talkname0", 1);
    Talk(52, "公子請．", "talkname52", 0);
    DarkScence();
    LightScence();
    Talk(0, "嗯．．．前去無路，後有追兵，正也不是，邪也不是，那可難也！．．．＜咦，棋局上的白子黑子似乎都化做了各派高手，東一堆使劍，西一堆使拳，你圍住我，我圍住你，互相糾纏不清的廝殺．．．．．．．我方白色的人馬被黑色各派高手給圍住了，左衝右突，始終殺不出重圍．．．．＞難道我天命已盡，一切枉費心機了．我這樣努力的找尋”十四天書”，終究要化作一場夢！時也命也，夫復何言？我不如死了算了．", "talkname0", 1);
    Talk(49, "不可如此！＜大哥似乎入魔障了，怎麼辦？有了，我解不開這棋局，但搗亂一番，讓他心神一分，便有救了．．．．．．．＞我來解這棋局．嗯！就下在這裡好了．", "talkname49", 1);
    Talk(52, "胡鬧，胡鬧，你自填一氣，自己殺死一塊白棋，那有這等的下法．", "talkname52", 0);
    Talk(0, "咦！難道竟是這樣？前輩你看，白棋故意擠死了一大塊後，接下來就好下多了．", "talkname0", 1);
    Talk(52, "這．．這．．．這”珍瓏”竟然解了，原來關鍵在於第一著的怪棋．這局棋原本糾纏於得失勝敗之中，以至無可破解，小和尚這一著不著意於生死，更不著意於勝敗，反而勘破了生死，得到解脫．．．．．", "talkname52", 0);
    Talk(49, "小僧棋藝低劣，胡亂下子，志在救人．．", "talkname49", 1);
    Talk(0, "賢弟誤打誤撞，反而收其效果．", "talkname0", 1);
    Talk(52, "神僧天賦英才，可喜可賀．請入屋內．", "talkname52", 0);
    Talk(49, "這．．．．．", "talkname49", 1);
    Talk(0, "賢弟就進去吧．搞不好老前輩要發獎品給你呢．", "talkname0", 1);
    jyx2_ReplaceSceneObject("", "Gaswall/Wall1", "");--gaswall 取消
    jyx2_ReplaceSceneObject("", "NPC/xuzhu", "");--虚竹进屋
    jyx2_ReplaceSceneObject("", "NPC/xuzhu2", "1");--虚竹进屋
    jyx2_ReplaceSceneObject("", "NPC/xiaoyaozi", "1");--逍遥子1
    DarkScence();
    ModifyEvent(-2, 1, 1, 1, -1, -1, -1, 6486, 6486, 6486, -2, -2, -2);
    ModifyEvent(-2, 2, 1, 1, -1, -1, -1, 6450, 6450, 6450, -2, -2, -2);
    Leave(49);
    LightScence();
    ScenceFromTo(17, 28, 24, 19);
    Play2Amination(1, 6486, 6520, 2, 6450, 6484, 44);
    Play2Amination(1, 6486, 6520, 2, 6450, 6484, 44);
    Play2Amination(1, 6486, 6520, 2, 6450, 6484, 25);
    ScenceFromTo(24, 19, 17, 28);
    DarkScence();
    SetScenceMap(-2, 1, 22, 24, 1532);
    SetScenceMap(-2, 1, 22, 23, 1534);
    SetScenceMap(-2, 1, 23, 24, 0);
    SetScenceMap(-2, 1, 24, 24, 1536);
    SetScenceMap(-2, 1, 24, 23, 1538);
    ModifyEvent(-2, 0, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 1, 1, 1, -1, -1, -1, 6524, 6524, 6524, -2, -2, -2);
    ModifyEvent(-2, 2, 1, 1, 536, -1, -1, 6522, 6522, 6522, -2, -2, -2);
    ModifyEvent(-2, 3, 1, 1, -1, -1, -1, 6342, 6342, 6342, -2, -2, -2);
    LightScence();
    Talk(0, "奇怪，怎麼進去這麼久．．我也進去看看好了．", "talkname0", 1);
do return end;
