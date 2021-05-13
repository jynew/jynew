Talk(0, "賢弟，這是怎麼一回事？那躺在地上的老人又是誰？", "talkname0", 1);
Talk(49, "我自己也迷糊了．我走進這房子後，屋內有個老人，他說我福緣深厚，破解了這盤棋局．接著硬化去了我原有的武功，並將他七十餘年的功力注入我體內．", "talkname49", 0);
Talk(0, "有這種事．", "talkname0", 1);
Talk(49, "他還叫我去殺了星宿老怪丁春秋．", "talkname49", 0);
jyx2_ReplaceSceneObject("", "NPC/suxinghe2", "1");--苏星河进屋
jyx2_ReplaceSceneObject("", "NPC/suxinghe", "");--苏星河进屋
Talk(0, "蘇前輩，這是怎麼回事？", "talkname0", 1);
Talk(52, "事情是這樣子的．本派乃逍遙派，師父收了我和丁春秋兩個弟子．我師父有個規矩，因他所學　甚雜，誰要做掌門，各種本　事都要比試，不但比武，還　得比琴棋書畫．但丁春秋於各種雜學一竅不通，眼見掌門人無望，竟忽施暗算將師父打落深谷，又將我打得重傷．", "talkname52", 0);
Talk(0, "這人真是可惡．", "talkname0", 1);
Talk(52, "後來師父趁機詐死，又設下了這個棋局，想藉此找出悟性高的人．立他為掌門，並傳他功力，將來好除去丁春秋這惡賊．今天，我們終於出現了能破解此珍瓏的人，師父在傳完他功力後也仙逝了．掌門師弟，我逍遙派的門戶就靠你清理了．", "talkname52", 0);
Talk(49, "我是誤打誤撞的，並沒有什麼悟性．更何況我是少林弟子，怎能改投別派．", "talkname49", 0);
Talk(0, "賢弟悟性沒有，”誤”性卻很高，我看你別當和尚了，做個掌門不是很好．", "talkname0", 1);
Talk(49, "可是．．．．．", "talkname49", 0);
Talk(0, "別可是了，你看，這位老前輩因為要傳你畢生功力而逝去，你還忍心拒絕人家嗎．況且，那丁春秋也是個無惡不作，罪無可赦的惡徒呀．", "talkname0", 1);
Talk(49, "．．．．．．．", "talkname49", 0);
Talk(0, "好，我看就這樣了．蘇前輩，我們會去找那星宿老怪，殺了他替你師父報仇", "talkname0", 1);
Talk(52, "老朽謝謝這位少俠的幫忙．掌門師弟，此去路上一切要小心，丁春秋那老賊行事卑鄙．．．．對了，我有一個徒弟醫術高明，人稱”閻王敵”的薛神醫，你可以去找他幫忙．見到他時只要出示掌門信物的”七寶指環”即可．", "talkname52", 0);
jyx2_ReplaceSceneObject("", "NPC/xiaoyaozi", "");--逍遥子1
DarkScence();
ModifyEvent(-2, 1, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
ModifyEvent(-2, 2, 1, 1, 581, -1, -1, 6522, 6522, 6522, -2, -2, -2);
ModifyEvent(-2, 3, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
ModifyEvent(-2, 0, 1, 1, 537, -1, -1, 6340, 6340, 6340, -2, -2, -2);
ModifyEvent(35, 3, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
NPCGetItem(49, 128, 1);
NPCGetItem(49, 56, 1);
NPCGetItem(49, 40, 1);
LightScence();
Talk(0, "賢弟，我們走了吧．", "talkname0", 1);
if TeamIsFull() == false then goto label0 end;
    Talk(49, "你的隊伍已滿，我無法加入．", "talkname49", 0);
    do return end;
::label0::
    DarkScence();
    jyx2_ReplaceSceneObject("", "NPC/xuzhu2", "");--虚竹加入
    ModifyEvent(-2, 2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    LightScence();
    AddMP(49, 300);
    AddHP(49, 200);
    AddAttack(49, 30);
    AddSpeed(49, 20);
    LearnMagic2(49, 15, 0);
    SetPersonMPPro(49,2);
    Join(49);
    ModifyEvent(28, 0, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(28, 1, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ModifyEvent(28, 2, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
    ChangeMMapMusic(3);
do return end;
