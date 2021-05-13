Talk(0, "喂！就是你要跟我較量較量嗎？是的話就快動手，我可沒那麼多時間和你窮磨菇．", "talkname0", 1);
Talk(26, "你是誰？到這裡來大小聲的幹什麼？", "talkname26", 0);
Talk(0, "你還問我是誰，我就是打敗”梅莊四友”的那個少年英雄．聽二莊主黑白子說，你因為崇拜我的武功了得，想跟我較量較量，不是嗎？", "talkname0", 1);
Talk(26, "哈！哈！看你年紀輕輕，竟如此狂妄，不錯！不錯！我喜歡．你說你打敗梅莊那四個窩囊廢，當真？", "talkname26", 0);
Talk(0, "當然是真的，懷疑啊！", "talkname0", 1);
Talk(0, "你還真奇怪，竟然把自己關在這不見天日的地洞，有病啊！還是受了什麼刺激？", "talkname0", 1);
Talk(26, "把自己關在這？我任我行會把自己關在這地牢之中？", "talkname26", 0);
Talk(0, "＜任我行？任我行？這名字  好像在那聽過．＞難道任前輩是被囚禁在這？", "talkname0", 1);
Talk(26, "說來慚愧，我堂堂日月神教教主竟會被困在這兒．", "talkname26", 0);
Talk(0, "＜這老頭是日月神教教主？  怎麼看都不像啊！＞", "talkname0", 1);
Talk(26, "要不是當年我潛心修習一門武功大法，將教中的所有大權交給東方不敗．不料那東方不敗狼子野心，表面上對我十分恭敬，什麼事都不敢違背，暗中卻培植一己勢力，假借諸般藉口，將所有忠於我的部屬或是撤革，或是處死，數年之間，我的親信竟然凋零殆盡．他見時機成熟，竟趁我練功入關之際，幹下叛逆篡位之事，並把我關在這西湖之底！", "talkname26", 0);
Talk(0, "竟有如此狼心狗肺的傢伙，不要讓我遇上，不然一定要給他好看．", "talkname0", 1);
Talk(26, "哈！哈！哈！就憑你．哈．．．！", "talkname26", 0);
Talk(0, "啊．．．．．", "talkname0", 1);
PlayAnimation(-1, 5974, 5992);
DarkScence();
AddItem(177, -1);
AddItem(178, -1);
AddItem(179, -1);
AddItem(180, -1);
ModifyEvent(-2, 1, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
jyx2_ReplaceSceneObject("", "NPC/renwoxing", "");--任我行逃走
LightScence();
PlayAnimation(-1, 6014, 6024);
Talk(0, "可惡的老頭，竟趁我不備偷襲．．．．啊！我的寶物！我上當了！一定是那四個卑鄙狗賊，明的打不過我，來陰的．真是太無恥下流了．", "talkname0", 1);
Talk(0, "這是什麼鬼地方？奇怪？方才那老傢伙突然一陣吼聲．．．．．喔！一定是那四個狗賊，明的打不過我，來暗的．真是太無恥下流了．咦？這門沒關，又不像是要把我關在這兒．嗯，真是可疑？看來可得好好的”瞧瞧”這個鬼地方！", "talkname0", 1);
ModifyEvent(55, 20, 1, 1, -1, -1, -1, 6060, 6060, 6060, -2, -2, -2);
ModifyEvent(55, 21, 1, 1, -1, -1, -1, 6050, 6050, 6050, -2, -2, -2);
ModifyEvent(55, 22, 1, 1, -1, -1, -1, 6062, 6062, 6062, -2, -2, -2);
ModifyEvent(55, 23, 1, 1, -1, -1, -1, 6074, 6074, 6074, -2, -2, -2);
ModifyEvent(55, 24, 0, 0, -1, -1, 275, -1, -1, -1, -2, -2, -2);
jyx2_ReplaceSceneObject("55", "NPC/danqingsheng1", "1");--丹青生
jyx2_ReplaceSceneObject("55", "NPC/tubiweng1", "1");--秃笔翁
jyx2_ReplaceSceneObject("55", "NPC/heibaizi1", "1");--黑白子
jyx2_ReplaceSceneObject("55", "NPC/huangzhonggong1", "1");--黄钟公
jyx2_ReplaceSceneObject("55", "NPC/danqingsheng", "");--丹青生
jyx2_ReplaceSceneObject("55", "NPC/tubiweng", "");--秃笔翁
jyx2_ReplaceSceneObject("55", "NPC/heibaizi", "");--黑白子
jyx2_ReplaceSceneObject("55", "NPC/huangzhonggong", "");--黄钟公
ModifyEvent(-2, 3, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
ModifyEvent(-2, 4, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
ModifyEvent(-2, 5, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
ModifyEvent(-2, 6, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
ModifyEvent(-2, 7, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
AddEthics(2);
do return end;
