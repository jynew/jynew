Talk(70, "阿彌陀佛，施主請留步．", "talkname70", 0);
DarkScence();
jyx2_ReplaceSceneObject("", "NPC/xuanci", "");--玄慈追出山门
jyx2_ReplaceSceneObject("", "NPC/xuanci1", "1");--玄慈在山门
jyx2_ReplaceSceneObject("", "NPC/murongfu1", "1");--慕容复在山门
ModifyEvent(-2, 3, 1, 1, 515, -1, -1, 5372, 5372, 5372, -2, -2, -2);--by fanyu 启动脚本515，改变贴图 场景28-3
ModifyEvent(-2, 4, 1, 1, -1, -1, -1, 5420, 5420, 5420, -2, -2, -2);--by fanyu 改变贴图 场景28-4
ModifyEvent(-2, 5, 1, 1, -1, -1, -1, 5420, 5420, 5420, -2, -2, -2);--by fanyu 改变贴图 场景28-5
ModifyEvent(-2, 6, 1, 1, -1, -1, -1, 5420, 5420, 5420, -2, -2, -2);--by fanyu 改变贴图 场景28-6
LightScence();
Talk(51, "怎麼，方丈後悔了．", "talkname51", 1);
Talk(70, "老衲想通了，既造業因，便有業果．自己的名聲固然重要，但是我虧欠喬峰一家人實在太多了，我不能再讓你去害他．", "talkname70", 0);
Talk(51, "方丈這是做什麼，今天我去揭發喬峰，是為武林除害．", "talkname51", 1);
Talk(70, "是嗎？依老衲看，是為了你大燕復國的計畫吧．其實我早就該想到了，當初就是你父親慕容博施主，假傳音訊，說是契丹武士要大舉侵犯少林奪取武學典籍，才會釀成大錯．這一切，都是你父親要挑起漢遼武人相鬥，使你大燕從中取利．", "talkname70", 0);
Talk(51, "．．．．．", "talkname51", 1);
Talk(70, "快將信件留下．", "talkname70", 0);
Talk(51, "方丈不怕我將你的事也揭露出來．", "talkname51", 1);
Talk(70, "我個人事小，中原武林的和諧才重要，我不能讓你利用此信，引起武林的大風暴．", "talkname70", 0);
Talk(51, "好，你別怪我．這事傳出去後，看你少林寺的臉往那裡擺．一個堂堂的少林方丈和女人亂來，還生下了一名私生子．．．．．", "talkname51", 1);
Talk(0, "有這等事？", "talkname0", 1);
Talk(70, "阿彌陀佛！老衲是犯了佛門大戒，待將你們拿下，取回信件後，老衲自會自我懲處．", "talkname70", 0);
Talk(51, "看來免不了一戰了．", "talkname51", 1);
if TryBattle(81) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    ModifyEvent(-2, 0, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 移除人物 场景28-0
    ModifyEvent(-2, 1, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 移除人物 场景28-1
    ModifyEvent(-2, 2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 移除人物 场景28-2
    ModifyEvent(-2, 3, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 移除人物 场景28-3
    ModifyEvent(-2, 4, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 移除人物 场景28-4
    ModifyEvent(-2, 5, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 移除人物 场景28-5
    ModifyEvent(-2, 6, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 移除人物 场景28-6
    ModifyEvent(-2, 12, -2, -2, 578, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 玄慈回寺里 启动脚本578 场景28-12
    jyx2_ReplaceSceneObject("", "NPC/xuanci", "1");--玄慈回寺里
    jyx2_ReplaceSceneObject("", "NPC/xuanci1", "");--
    jyx2_ReplaceSceneObject("", "NPC/murongfu1", "");--慕容复归队
    ModifyEvent(51, 14, -2, -2, 527, 531, -1, -2, -2, -2, -2, -2, -2);--by fanyu  启动脚本527,531 场景51-14
    LightScence();
    Talk(51, "沒事了，我們走吧．", "talkname51", 1);

    AddRepute(8);
do return end;
