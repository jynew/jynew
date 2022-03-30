ScenceFromTo(41, 31, 34, 31);
jyx2_CameraFollow("Level/NPC/miaorenfeng");
Talk(3, "你们是田归农请来的吧！神龙教何时跟田归农搭在一起了。", "talkname3", 0);
Talk(97, "还多亏了田兄，我们才知道《飞狐外传》一书在你这。识相的话就快将书交出来。", "talkname97", 0);
Talk(3, "田归农呢？他怎么不敢出来见我。", "talkname3", 0);
Talk(97, "我看你是见不到他了。田兄从毒手药王那弄来的断肠草粉末，药效也真够狠。现下你双眼已瞎，我看“打遍天下无敌手”的金面佛苗人凤，今日要上西天了！兄弟们，上！", "talkname97", 0);
ScenceFromTo(34, 31, 41, 31);
jyx2_CameraFollowPlayer();
Talk(0, "苗大侠，我帮你拿贼。", "talkname0", 1);
if TryBattle(3) == true then goto label0 end;
    Dead();
    do return end;
::label0::
	jyx2_ReplaceSceneObject("", "NPC/神龙弟子2", "");--战斗结束，移除人物
	jyx2_ReplaceSceneObject("", "NPC/神龙弟子3", "");--战斗结束，移除人物
	jyx2_ReplaceSceneObject("", "NPC/神龙弟子4", "");--战斗结束，移除人物
	jyx2_ReplaceSceneObject("", "NPC/神龙弟子5", "");--战斗结束，移除人物
	jyx2_ReplaceSceneObject("", "NPC/神龙弟子6", "");--战斗结束，移除人物
	jyx2_ReplaceSceneObject("", "NPC/神龙弟子7", "");--战斗结束，移除人物
    ModifyEvent(-2, 2, 0, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 战斗结束，移除人物 场景24-编号2
    ModifyEvent(-2, 3, 0, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 战斗结束，移除人物 场景24-编号3
    ModifyEvent(-2, 4, 0, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 战斗结束，移除人物 场景24-编号4
    ModifyEvent(-2, 5, 0, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 战斗结束，移除人物 场景24-编号5
    ModifyEvent(-2, 6, 0, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 战斗结束，移除人物 场景24-编号6
    ModifyEvent(-2, 7, 0, -2, -1, -1, -1, -1, -1, -1, -2, -2, -2);--by fanyu 战斗结束，移除人物 场景24-编号7
    SetScencePosition2(35, 31);
	jyx2_MovePlayer("afterBattle", "Level/Dynamic");
    LightScence();
    Talk(3, "小兄弟，你尊姓大名，与我可有渊源？", "talkname3", 0);
    Talk(0, "丈夫结交，但重义气，只需肝胆相照，何必提名道姓。", "talkname0", 1);
    Talk(3, "好，苗人凤独来独往，生平只有两个知交，一个是辽东大侠胡一刀，另一个便是你这位不知姓名没见过面的小兄弟。", "talkname3", 0);
    if InTeam(1) == false then goto label1 end;
        Talk(1, "你说什么。那你为何要杀死胡大侠。", "talkname1", 1);
        Talk(3, "这说来话长……", "talkname3", 0);
        Talk(0, "大哥，我们先想办法救苗大侠，这事以后再说。", "talkname0", 1);
::label1::
        Talk(0, "苗大侠，既然这药是毒手药王所配制，那我们去求毒手药王救治，或能解得。", "talkname0", 1);
        Talk(3, "要求毒手药王吗？那是徒劳往返，不用去了。", "talkname3", 0);
        Talk(0, "不，天下无难事！这位毒手药王住在哪里？", "talkname0", 1);
        Talk(3, "听说此人在洞庭湖畔隐居。", "talkname3", 0);
        Talk(0, "我这就去了！", "talkname0", 1);
        SetScenceMap(49, 1, 28, 37, 0);--by fanyu  场景49-编号1，坐标的贴图改变，门移除
        SetScenceMap(49, 1, 27, 37, 3692);--by fanyu  场景49-编号1，坐标的贴图改变，门移除
        SetScenceMap(49, 1, 27, 36, 3694);--by fanyu  场景49-编号1，坐标的贴图改变，门移除
		jyx2_FixMapObject("药王庄开门",1);
        ModifyEvent(-2, 9, -2, -2, -2, -2, 35, -2, -2, -2, -2, -2, -2);--by fanyu 启动35脚本 场景24-编号9
        ModifyEvent(-2, -2, -2, -2, -1, -1, -1, -2, -2, -2, -2, -2, -2);
        AddEthics(2);
        AddRepute(1);
        ChangeMMapMusic(3);
do return end;
