Talk(70, "阿弥陀佛，施主请留步。", "talkname70", 0);
DarkScence();
ModifyEvent(-2, 3, 1, 1, 515, -1, -1, 5372, 5372, 5372, -2, -2, -2);--by fanyu 启动脚本515，改变贴图 场景28-3
jyx2_ReplaceSceneObject("", "NPC/方丈12", "");--玄慈追出山门
jyx2_ReplaceSceneObject("", "NPC/方丈3", "1");--玄慈在山门
jyx2_ReplaceSceneObject("", "NPC/慕容复1", "1");--慕容复在山门
SetRoleFace(2);
ModifyEvent(-2, 4, 1, 1, -1, -1, -1, 5420, 5420, 5420, -2, -2, -2);--by fanyu 改变贴图 场景28-4
ModifyEvent(-2, 5, 1, 1, -1, -1, -1, 5420, 5420, 5420, -2, -2, -2);--by fanyu 改变贴图 场景28-5
ModifyEvent(-2, 6, 1, 1, -1, -1, -1, 5420, 5420, 5420, -2, -2, -2);--by fanyu 改变贴图 场景28-6
jyx2_ReplaceSceneObject("", "NPC/少林弟子4", "1");
jyx2_ReplaceSceneObject("", "NPC/少林弟子5", "1");
jyx2_ReplaceSceneObject("", "NPC/少林弟子6", "1");
LightScence();
Talk(51, "怎么，方丈后悔了？", "talkname51", 1);
Talk(70, "老衲想通了，既造业因，便有业果。自己的名声固然重要，但是我亏欠乔峰一家人实在太多了，我不能再让你去害他。", "talkname70", 0);
Talk(51, "方丈这是做什么，今天我去揭发乔峰，是为武林除害。", "talkname51", 1);
Talk(70, "是吗？依老衲看，是为了你大燕复国的计划吧。其实我早就该想到了，当初就是你父亲慕容博施主，假传音讯，说是契丹武士要大举侵犯少林夺取武学典籍，才会酿成大错。这一切，都是你父亲要挑起汉辽武人相斗，使你大燕从中取利。", "talkname70", 0);
Talk(51, "……", "talkname51", 1);
Talk(70, "快将信件留下。", "talkname70", 0);
Talk(51, "方丈不怕我将你的事也揭露出来。", "talkname51", 1);
Talk(70, "我个人事小，中原武林的和谐才重要，我不能让你利用此信，引起武林的大风暴。", "talkname70", 0);
Talk(51, "好，你别怪我。这事传出去后，看你少林寺的脸往哪里摆。一个堂堂的少林方丈和女人乱来，还生下了一名私生子……", "talkname51", 1);
Talk(0, "有这等事？", "talkname0", 1);
Talk(70, "阿弥陀佛！老衲是犯了佛门大戒，待将你们拿下，取回信件后，老衲自会自我惩处。", "talkname70", 0);
Talk(51, "看来免不了一战了。", "talkname51", 1);
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
	jyx2_ReplaceSceneObject("", "NPC/方丈3", "");
	jyx2_ReplaceSceneObject("", "NPC/少林弟子4", "");
	jyx2_ReplaceSceneObject("", "NPC/少林弟子5", "");
	jyx2_ReplaceSceneObject("", "NPC/少林弟子6", "");
    ModifyEvent(-2, 12, -2, -2, 578, -1, -1, -2, -2, -2, -2, -2, -2);--by fanyu 玄慈回寺里 启动脚本578 场景28-12
	jyx2_ReplaceSceneObject("", "NPC/方丈12", "1");
	jyx2_ReplaceSceneObject("", "NPC/慕容复1", "");
    ModifyEvent(51, 14, -2, -2, 527, 531, -1, -2, -2, -2, -2, -2, -2);--by fanyu  启动脚本527,531 场景51-14
    LightScence();
    Talk(51, "没事了，我们走吧。", "talkname51", 1);

    AddRepute(8);
do return end;
