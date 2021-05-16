Talk(0, "四位那麼好的閒情逸致聚在這兒，是不是準備打麻將，插花收不收啊？", "talkname0", 1);
Talk(33, "你這臭小子，看你做了什麼好事？還敢在這說風涼話．", "talkname33", 0);
Talk(34, "我早該知道天底下那有這麼好的事情，原來這一切都是你設計出來的詭計．", "talkname34", 0);
Talk(32, "你這小子，竟然利用我們所好，實在太可惡了．", "talkname32", 0);
Talk(0, "你們在那你一言我一句的，唱雙簧啊！", "talkname0", 1);
Talk(32, "小子，你真是太大膽了，竟敢把任老怪給放了，看我饒不饒你！", "talkname32", 0);
Talk(0, "喂！喂！你們在說什麼啊？我還沒說你們偷走我那四樣寶物，你們就惡人先告狀．真是豈有此理，什麼態度嘛．．．．況且我那有放走任前輩．．", "talkname0", 1);
Talk(33, "大哥，別跟這小子囉嗦了，此時再也顧不得什麼以老欺小的狗屁道義了．先將這小子捉上黑木崖，向東方教主請罪，否則咱們可就吃不完兜著走了．", "talkname33", 0);
Talk(34, "好，咱們一起上．", "talkname34", 0);
Talk(0, "誰怕誰！烏龜怕鐵鎚！四個一起上最好，省得我麻煩．", "talkname0", 1);
Talk(31, "小子你找死！", "talkname31", 0);
if TryBattle(47) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    LightScence();
    Talk(32, "小子，你可知你放走的是誰？", "talkname32", 0);
    Talk(0, "他不是你們請來的打手嗎？", "talkname0", 1);
    Talk(31, "你是真不知還是假不知，你放走了任老怪，就是與東方教主和我日月神教為敵．", "talkname31", 0);
    Talk(0, "你們是日月神教的？", "talkname0", 1);
    Talk(33, "不錯，我們是奉教主之命，在此看守任老怪的，你居然利用我們的弱點救他出去．", "talkname33", 0);
    Talk(34, "別說了，得趕緊回黑木崖向東方教主報告這件事．走！", "talkname34", 0);
    jyx2_ReplaceSceneObject("", "NPC/danqingsheng1", "");--丹青生
    jyx2_ReplaceSceneObject("", "NPC/tubiweng1", "");--秃笔翁
    jyx2_ReplaceSceneObject("", "NPC/heibaizi1", "");--黑白子
    yx2_ReplaceSceneObject("", "NPC/huangzhonggong1", "");--黄钟公
    DarkScence();
    ModifyEvent(-2, 20, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 21, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 22, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 23, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 24, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 3, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 5, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 6, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 7, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 9, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 12, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 13, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 14, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(-2, 15, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    LightScence();
    AddRepute(8);
do return end;
