ScenceFromTo(41, 29, 33, 29);
Talk(22, "我五嶽劍派同氣連枝，百餘年來攜手結盟，早便如同一家．兄弟忝為五派盟主，亦已多歷年所．只是近來武林中出了不少大事，兄弟與五嶽劍派的前輩師兄們商量，均覺若非連成一派，統一號令，來日大難，只怕不易抵擋．", "talkname22", 0);
Talk(20, "不知左盟主和那一派的前輩師兄們商量過了？怎地我莫某人不知其事？", "talkname20", 0);
Talk(22, "兄弟適才說到，武林中出了不少大事，五派非合而為一不可．其中一件大事，便是咱們五派中人，自相殘殺，不顧同盟義氣．莫大先生，我嵩山派弟子費師弟，在衡山城外喪命．有人親眼目睹，說是你莫大先生下的毒手，不知此事可真？", "talkname22", 0);
Talk(20, "並無其事！諒莫某這一點微末道行，怎殺得了那大嵩陽手？", "talkname20", 0);
Talk(22, "莫兄，這件事你也不用太過擔心，等我五派合併之後，莫兄和我也是師兄弟了．死者已矣，活著的人又何必再逞兇殺，多造殺孽？莫兄，你說是不是呢？", "talkname22", 0);
Talk(20, "哼！", "talkname20", 0);
Talk(22, "南嶽衡山派於併派之議，是無意見了．東嶽泰山派天門道兄，貴派意思如何？", "talkname22", 0);
Talk(23, "泰山派自祖師爺東靈道長創派以來，已三百餘年．貧道無德無能，不能發揚光大泰山一派，可是這三百多年的基業，說什麼也不能自貧道手中斷絕．併派之議，萬萬不能從命．", "talkname23", 0);
Talk(22, "道兄此言差矣．五派合併後我五嶽派聲勢壯大，其下各個弟子，那一個不沾到光？道兄何必為這區區泰山派掌門一職的私心，而阻擾了全派的大業．", "talkname22", 0);
Talk(23, "我這掌門人，做不做有什麼干係？只是泰山一派，恁說也不能在我手中給人併吞．", "talkname23", 0);
Talk(22, "道兄說了這麼多，心中卻就是為了放不下掌門人的名位．", "talkname22", 0);
Talk(23, "你真道我是如此私心？好，從此刻起，我這掌門人不做了．．．", "talkname23", 0);
Talk(22, "道兄是贊成此事了．．．", "talkname22", 0);
Talk(23, "你．．．．．．", "talkname23", 0);
Talk(0, "＜這老道性子太急了，一被激就著了人家的道＞", "talkname0", 1);
Talk(21, "左盟主已然身為五嶽劍派盟主，位望何等尊崇，何必定要併五派，由一人任掌門．", "talkname21", 0);
Talk(22, "師太此言差矣．左某只是提議五派合併．這掌門之位，當然還是要由我五派之人推舉出來．", "talkname22", 0);
ScenceFromTo(33, 29, 41, 29);
WalkFromTo(41, 29, 35, 29);
Talk(0, "我看別討論了，學武之人，就在武功下見真章．來來來，有誰要跟我打？", "talkname0", 1);
Talk(22, "不知閣下是誰？竟擅闖我嵩山派，你不知道我五嶽派在此聚會嗎？", "talkname22", 0);
Talk(19, "這位少俠前些時日來到我華山，我已讓他入我華山派了．", "talkname19", 0);
Talk(22, "原來是華山派的弟子，照如此說，岳先生也是贊成這”比劍奪帥”的方法了？", "talkname22", 0);
Talk(19, "比劍奪帥，原也是一法，但只可點到為止，以免傷了和氣．", "talkname19", 0);
Talk(23, "原來華山派是贊成併派的，我天門就先來領教你華山派的高招．", "talkname23", 0);
Talk(0, "這讓我來就可以了．", "talkname0", 1);
if TryBattle(30) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    LightScence();
    Talk(0, "承讓了，道長．", "talkname0", 1);
    Talk(21, "讓我定閒來領教領教華山派的高招．", "talkname21", 0);
    if TryBattle(31) == true then goto label1 end;
        Dead();
        do return end;
::label1::
        LightScence();
        Talk(0, "承讓了，師太．接下來不知是那位．", "talkname0", 1);
        Talk(20, "你連戰兩場，先下去休息吧．", "talkname20", 0);
        Talk(0, "沒關係，少年人體力充沛，莫掌門請出手吧．", "talkname0", 1);
        if TryBattle(32) == true then goto label2 end;
            Dead();
            do return end;
::label2::
            LightScence();
            Talk(22, "真所謂”長江後浪推前浪”華山派岳先生為了今天，想必已籌劃很久了吧．若左某也敗了，我嵩山派自當奉岳先生為掌門．", "talkname22", 0);
            if TryBattle(33) == true then goto label3 end;
                Dead();
                do return end;
::label3::
                LightScence();
                Talk(0, "終於都解決了，好累．左冷禪，看來你的陰謀是無法達成了．", "talkname0", 1);
                Talk(22, "我不該低估你的．岳不群．想不到你的心計比我還深，左某是輸你一籌．", "talkname22", 0);
                Talk(0, "胡說八道，人家岳先生才不想做什麼掌門呢．今天我來，其最主要的就是要阻止你併派的陰謀．", "talkname0", 1);
                Talk(22, "岳兄當真不做這五嶽派的掌門？", "talkname22", 0);
                Talk(19, "沒錯，我是跟這位少俠提到過，急於合併各派，實是難如登天，且會引起武林糾紛", "talkname19", 0);
                Talk(0, "你看，我說的沒錯吧．", "talkname0", 1);
                Talk(19, "但是各家門派如能擇地域相近，武功相似，又或相互交好，先行進行合併．則十年八年之內，門戶宗派便可減少一大半．慢慢來的話問題較小．咱們五嶽劍派今日合成五嶽派，就可為各家各派樹一典範，他日必能成為武林中千古豔稱的盛舉．", "talkname19", 0);
                Talk(0, "＜事情怎會變成這樣？好像跟我想的不太一樣！＞", "talkname0", 1);
                Talk(19, "嗯，五嶽派今日新創，百廢待舉，在下只能總領其事，各派的事務，還是由各派原任掌門主持．咱們五嶽派今日合併，若不能和衷同濟，那麼五派合併云云，也只是虛名而已．在下無德無能，暫且執掌本門門戶，種種興革，還須和眾位兄弟從長計議，在下不敢自專．現下天色已晚，各位都辛苦了，今天就到此為止，他日再到華山來共商大事．", "talkname19", 0);
                DarkScence();
                ModifyEvent(-2, 24, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
                ModifyEvent(-2, 25, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
                ModifyEvent(-2, 26, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
                ModifyEvent(-2, 27, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
                ModifyEvent(-2, 35, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
                ModifyEvent(-2, 36, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
                ModifyEvent(-2, 37, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
                ModifyEvent(-2, 38, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
                ModifyEvent(-2, 39, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
                ModifyEvent(-2, 40, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
                ModifyEvent(-2, 41, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
                ModifyEvent(-2, 42, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
                ModifyEvent(-2, 43, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
                ModifyEvent(-2, 44, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
                ModifyEvent(-2, 45, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
                ModifyEvent(-2, 46, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
                ModifyEvent(-2, 47, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
                ModifyEvent(-2, 48, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
                ModifyEvent(-2, 49, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
                ModifyEvent(-2, 50, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
                jyx2_ReplaceSceneObject("", "NPC/xuanci", "");--战斗结束，移除人物
                jyx2_ReplaceSceneObject("", "NPC/zuolengchan", "");--战斗结束，移除人物
                jyx2_ReplaceSceneObject("", "NPC/tangwenliang", "");--战斗结束，移除人物
                jyx2_ReplaceSceneObject("", "NPC/hengshanpai", "");--战斗结束，移除人物
                jyx2_ReplaceSceneObject("", "NPC/hetaichong", "");--战斗结束，移除人物
                  jyx2_ReplaceSceneObject("", "NPC/modaxiansheng", "");--战斗结束，移除人物
                jyx2_ReplaceSceneObject("", "NPC/yuebuqun", "");--战斗结束，移除人物
                jyx2_ReplaceSceneObject("", "NPC/taishanpai", "");--战斗结束，移除人物
                jyx2_ReplaceSceneObject("", "NPC/emeipai", "");--战斗结束，移除人物
                jyx2_ReplaceSceneObject("", "NPC/wudangpai", "");--战斗结束，移除人物
                  jyx2_ReplaceSceneObject("", "NPC/emeipai1", "");--战斗结束，移除人物
                jyx2_ReplaceSceneObject("", "NPC/wudangpai1", "");--战斗结束，移除人物
                jyx2_ReplaceSceneObject("", "NPC/hengshanpai1", "");--战斗结束，移除人物
                jyx2_ReplaceSceneObject("", "NPC/hengshanpai2", "");--战斗结束，移除人物
                 jyx2_ReplaceSceneObject("", "NPC/taishanpai1", "");--战斗结束，移除人物
                   jyx2_ReplaceSceneObject("", "NPC/kunlunpai1", "");--战斗结束，移除人物
                jyx2_ReplaceSceneObject("", "NPC/kongtongpai1", "");--战斗结束，移除人物
                 jyx2_ReplaceSceneObject("", "NPC/huashanpai", "");--战斗结束，移除人物
                ModifyEvent(-2, 2, -2, -2, 216, -1, -1, -2, -2, -2, -2, -2, -2);
                ModifyEvent(-2, 3, -2, -2, 216, -1, -1, -2, -2, -2, -2, -2, -2);
                ModifyEvent(-2, 14, -2, -2, 216, -1, -1, -2, -2, -2, -2, -2, -2);
                ModifyEvent(-2, 15, -2, -2, 216, -1, -1, -2, -2, -2, -2, -2, -2);
                ModifyEvent(-2, 16, -2, -2, 216, -1, -1, -2, -2, -2, -2, -2, -2);
                ModifyEvent(-2, 17, -2, -2, 216, -1, -1, -2, -2, -2, -2, -2, -2);
                ModifyEvent(-2, 18, -2, -2, 216, -1, -1, -2, -2, -2, -2, -2, -2);
                ModifyEvent(-2, 19, -2, -2, 216, -1, -1, -2, -2, -2, -2, -2, -2);
                ModifyEvent(-2, 20, -2, -2, 216, -1, -1, -2, -2, -2, -2, -2, -2);
                ModifyEvent(-2, 21, -2, -2, 216, -1, -1, -2, -2, -2, -2, -2, -2);
                ModifyEvent(-2, 22, -2, -2, 216, -1, -1, -2, -2, -2, -2, -2, -2);
                ModifyEvent(-2, 23, -2, -2, 216, -1, -1, -2, -2, -2, -2, -2, -2);
                ModifyEvent(-2, 29, -2, -2, 216, -1, -1, -2, -2, -2, -2, -2, -2);
                ModifyEvent(-2, 30, -2, -2, 216, -1, -1, -2, -2, -2, -2, -2, -2);
                ModifyEvent(-2, 31, -2, -2, 216, -1, -1, -2, -2, -2, -2, -2, -2);
                ModifyEvent(-2, 32, -2, -2, 216, -1, -1, -2, -2, -2, -2, -2, -2);
                ModifyEvent(-2, 33, -2, -2, 216, -1, -1, -2, -2, -2, -2, -2, -2);
                ModifyEvent(-2, 34, -2, -2, 216, -1, -1, -2, -2, -2, -2, -2, -2);
                ModifyEvent(-2, 28, -2, -2, 207, -1, -1, -2, -2, -2, -2, -2, -2);
                ModifyEvent(-2, 58, 0, 0, -1, -1, 218, -2, -2, -2, -2, -2, -2);
                ModifyEvent(-2, 59, 0, 0, -1, -1, 218, -2, -2, -2, -2, -2, -2);
                ModifyEvent(-2, 56, -2, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);
                ModifyEvent(-2, 57, -2, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);
                ModifyEvent(-2, 0, 0, 0, -1, -1, -1, -2, -2, -2, -2, -2, -2);
                LightScence();
                AddRepute(15);
do return end;
