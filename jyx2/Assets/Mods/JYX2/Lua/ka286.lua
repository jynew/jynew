Talk(0, "请问一下，你们镖局不做生意了吗？怎么没半个人影。小爷我有些珍贵的宝物还想找你们保呢！", "talkname0", 1);
Talk(36, "这位公子，非常抱歉，今天本镖局不做生意。", "talkname36", 0);
Talk(0, "开了镖局却不做生意，你这“福威镖局”的招牌是挂假的啊！叫你们总镖头出来。", "talkname0", 1);
Talk(36, "说不保镖就不保镖，你在这大呼小叫个什么劲。", "talkname36", 0);
if TryBattle(48) == true then goto label0 end;
    Dead();
    do return end;
::label0::
    LightScence();
    Talk(0, "我看福威镖局的武功也稀松平常，镖还是不要给你们保比较妥当。", "talkname0", 1);
    Talk(36, "哼！要不是我福威镖局被青城派的人大举入侵，家父被抓，家母被杀，其他的镖师死的死，逃的逃，否则……", "talkname36", 0);
    Talk(0, "这么惨？你们是走失了人家的镖，还是怎么样得罪了青城派。", "talkname0", 1);
    Talk(36, "听我父亲说，先祖林远图曾打败过青城派掌门长青子，所以今天他们是来报复的。", "talkname36", 0);
    Talk(0, "怎么如此可恶，小爷我看不过去了。", "talkname0", 1);
    Talk(36, "少侠武功高强，请少侠帮我救出家父，我林平之这辈子愿做牛做马服侍你。若少侠愿意帮忙，我福威镖局中的任何物品，少侠都可随意取用。这其中还包括了一本“松风剑谱”，是我从青城派那几个小贼身上偷来的。想研究看看他们的剑招上是不是有什么破绽。", "talkname36", 0);
    Talk(0, "说这么多做什么，我就帮你上青城派看看好了。", "talkname0", 1);
    ModifyEvent(-2, -2, -2, -2, 298, -1, -1, -2, -2, -2, -2, -2, -2);
    ModifyEvent(-2, 2, -2, -2, -1, -1, 299, -2, -2, -2, -2, -2, -2);
    if JudgeEventNum(3, -1) == true then goto label1 end;
        ModifyEvent(-2, 3, -2, -2, 948, -1, -1, -2, -2, -2, -2, -2, -2);
::label1::
        if JudgeEventNum(4, -1) == true then goto label2 end;
            ModifyEvent(-2, 4, -2, -2, 949, -1, -1, -2, -2, -2, -2, -2, -2);
::label2::
            ModifyEvent(36, 0, -2, -2, 293, -1, -1, -2, -2, -2, -2, -2, -2);
            ModifyEvent(36, 1, -2, -2, 293, -1, -1, -2, -2, -2, -2, -2, -2);
            ModifyEvent(36, 2, -2, -2, 293, -1, -1, -2, -2, -2, -2, -2, -2);
            ModifyEvent(36, 3, -2, -2, 293, -1, -1, -2, -2, -2, -2, -2, -2);
            ModifyEvent(36, 4, -2, -2, 295, -1, -1, -2, -2, -2, -2, -2, -2);
            ModifyEvent(1, 11, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            ModifyEvent(1, 12, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            ModifyEvent(1, 13, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
            ModifyEvent(1, 14, 0, 0, -1, -1, -1, -1, -1, -1, -2, -2, -2);
			jyx2_ReplaceSceneObject("1","NPC/青城弟子11","");
			jyx2_ReplaceSceneObject("1","NPC/青城弟子13","");
			jyx2_ReplaceSceneObject("1","NPC/青城弟子14","");
            AddRepute(1);
            ChangeMMapMusic(3);
do return end;
